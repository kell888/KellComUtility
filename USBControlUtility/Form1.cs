using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using KellComUtility;

namespace USBControlUtility
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        byte sHead, rHead, sCmd, gCmd;
        byte[] tail, setK1, setK2, setK3, setK4, getK1, getK2, getK3, getK4, resetK1, resetK2, resetK3, resetK4, setAll, resetAll, getAll;

        private void 清空ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtGet.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sHead = Convert.ToByte("0x3A", 16);
            rHead = Convert.ToByte("0xA3", 16);
            tail = ComUtility.StrHexToBin("0x0D 0x0A");
            setK1 = ComUtility.StrHexToBin("01 01 FF");
            setK2 = ComUtility.StrHexToBin("01 02 FF");
            setK3 = ComUtility.StrHexToBin("01 03 FF");
            setK4 = ComUtility.StrHexToBin("01 04 FF");
            setAll = ComUtility.StrHexToBin("01 00 FF");
            resetK1 = ComUtility.StrHexToBin("01 01 00");
            resetK2 = ComUtility.StrHexToBin("01 02 00");
            resetK3 = ComUtility.StrHexToBin("01 03 00");
            resetK4 = ComUtility.StrHexToBin("01 04 00");
            resetAll = ComUtility.StrHexToBin("01 00 00");
            getK1 = ComUtility.StrHexToBin("01 01");
            getK2 = ComUtility.StrHexToBin("01 02");
            getK3 = ComUtility.StrHexToBin("01 03");
            getK4 = ComUtility.StrHexToBin("01 04");
            getAll = ComUtility.StrHexToBin("01 00");
            sCmd = Convert.ToByte("0x88", 16);
            gCmd = Convert.ToByte("0x99", 16);
            //列出本机所有串口
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length > 0)
            {
                comboBoxPortSelect.Items.AddRange(ports);
                //默认选中第一个
                comboBoxPortSelect.SelectedIndex = 0;
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (btnOpen.Text == "打开串口")
            {
                OpenOrClosePort(true);
            }
            else
            {
                OpenOrClosePort(false);
            }
        }

        private bool OpenOrClosePort(bool open)
        {
            if (open)
            {
                //打开串口
                if (sp.IsOpen)
                {
                    sp.Close();
                }
                try
                {
                    sp.Open();
                    btnOpen.Text = "关闭串口";//按钮打开
                    txtGet.AppendText("串口打开成功！" + Environment.NewLine);//状态栏
                    return true;
                }
                catch (Exception e)
                {
                    btnOpen.Text = "打开串口";
                    MessageBox.Show("没有发现串口或串口已被占用！" + e.Message);
                    txtGet.AppendText("串口打开失败！" + Environment.NewLine);
                }
            }
            else
            {
                //关闭串口
                try
                {
                    sp.Close();
                    btnOpen.Text = "打开串口";//按钮关闭
                    txtGet.AppendText("串口关闭成功！" + Environment.NewLine);//状态栏
                    return true;
                }
                catch (Exception e)
                {
                    btnOpen.Text = "关闭串口";
                    MessageBox.Show(e.Message);
                    txtGet.AppendText("串口关闭失败！" + Environment.NewLine);
                }
            }
            return false;
        }

        private void comboBoxPortSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            int current = comboBoxPortSelect.SelectedIndex;
            if (current > -1)
            {
                sp.PortName = comboBoxPortSelect.Text;
                if (sp.IsOpen)
                {
                    btnOpen.Text = "关闭串口";
                }
                else
                {
                    btnOpen.Text = "打开串口";
                }
            }
        }

        public bool Send(int k, bool set, bool read = false)
        {
            bool flag = false;
            byte[] realData = null;
            switch (k)
            {
                case 1:
                    if (read)
                        realData = getK1;
                    else
                    {
                        if (set)
                            realData = setK1;
                        else
                            realData = resetK1;
                    }
                    break;
                case 2:
                    if (read)
                        realData = getK2;
                    else
                    {
                        if (set)
                            realData = setK2;
                        else
                            realData = resetK2;
                    }
                    break;
                case 3:
                    if (read)
                        realData = getK3;
                    else
                    {
                        if (set)
                            realData = setK3;
                        else
                            realData = resetK3;
                    }
                    break;
                case 4:
                    if (read)
                        realData = getK4;
                    else
                    {
                        if (set)
                            realData = setK4;
                        else
                            realData = resetK4;
                    }
                    break;
                case 0:
                    if (read)
                        realData = getAll;
                    else
                    {
                        if (set)
                            realData = setAll;
                        else
                            realData = resetAll;
                    }
                    break;
            }
            byte cmd = sCmd;
            if (read) cmd = gCmd;
            int dataIndex = 2;
            int tailLen = tail.Length;//帧尾
            int needLen = tailLen + dataIndex;
            int length = (byte)realData.Length;
            byte[] package = new byte[length + needLen];
            package[0] = sHead;//帧头
            package[1] = cmd;//命令号
            for (int i = 0; i < realData.Length; i++)
            {
                package[dataIndex + i] = realData[i];
            }
            for (int i = 0; i < tailLen; i++)
            {
                package[dataIndex + i + length] = tail[i];
            }
            if (!sp.IsOpen)
            {
                MessageBox.Show("串口没有打开，请打开串口！");
                return false;
            }
            txtGet.AppendText(DateTime.Now.ToLongTimeString() + " 发送:[" + ComUtility.GetHex(package) + "]");
            try
            {
                sp.Write(package, 0, package.Length);
                flag = true;
            }
            catch (Exception e)
            {
                throw e;
            }
            txtGet.AppendText(" 完毕。" + Environment.NewLine);
            return flag;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                Send(1, btnSend.Text.StartsWith("打开"));
            }
            catch (Exception ex)
            { txtGet.AppendText("命令执行出错：" + ex.Message + Environment.NewLine); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Send(2, button1.Text.StartsWith("打开"));
            }
            catch (Exception ex)
            { txtGet.AppendText("命令执行出错：" + ex.Message + Environment.NewLine); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Send(3, button2.Text.StartsWith("打开"));
            }
            catch (Exception ex)
            { txtGet.AppendText("命令执行出错：" + ex.Message + Environment.NewLine); }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                Send(4, button3.Text.StartsWith("打开"));
            }
            catch (Exception ex)
            { txtGet.AppendText("命令执行出错：" + ex.Message + Environment.NewLine); }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                Send(0, true);
            }
            catch (Exception ex)
            { txtGet.AppendText("命令执行出错：" + ex.Message + Environment.NewLine); }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                Send(0, false);
            }
            catch (Exception ex)
            { txtGet.AppendText("命令执行出错：" + ex.Message + Environment.NewLine); }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                Send(0, false, true);
            }
            catch (Exception ex)
            { txtGet.AppendText("命令执行出错：" + ex.Message + Environment.NewLine); }
        }

        private void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            System.Threading.Thread.Sleep(50);
            StringBuilder builder = new StringBuilder();
            SerialPort sp = sender as SerialPort;
            byte[] buf = new byte[sp.BytesToRead];
            sp.Read(buf, 0, buf.Length);
            builder.Append(ComUtility.GetHex(buf).ToUpper());
            string content = builder.ToString();
            this.Invoke((EventHandler)(delegate
            {
                txtGet.AppendText(DateTime.Now.ToLongTimeString() + " 接收: [" + content + "] -- ");
            }));
            if (buf.Length == 3 && buf[0] == rHead && buf[1] == tail[0] && buf[2] == tail[1])
            {
                this.Invoke((EventHandler)(delegate
                {
                    txtGet.AppendText("命令执行失败" + Environment.NewLine);
                }));
            }
            else
            {
                if (buf[0] == rHead)
                {
                    bool k1, k2, k3, k4;
                    if ((buf[4] == tail[0]) && (buf[5] == tail[1]))
                    {
                        int k = (int)buf[2];
                        switch (k)
                        {
                            case 1:
                                k1 = buf[3] == 255;
                                if (k1)
                                {
                                    this.Invoke((EventHandler)(delegate
                                    {
                                        btnSend.Text = "关闭K1";
                                        panel1.BackgroundImage = USBControlUtility.Properties.Resources.on;
                                    }));
                                }
                                else
                                {
                                    this.Invoke((EventHandler)(delegate
                                    {
                                        btnSend.Text = "打开K1";
                                        panel1.BackgroundImage = USBControlUtility.Properties.Resources.off;
                                    }));
                                }
                                break;
                            case 2:
                                k2 = buf[3] == 255;
                                if (k2)
                                {
                                    this.Invoke((EventHandler)(delegate
                                    {
                                        button1.Text = "关闭K2";
                                        panel2.BackgroundImage = USBControlUtility.Properties.Resources.on;
                                    }));
                                }
                                else
                                {
                                    this.Invoke((EventHandler)(delegate
                                    {
                                        button1.Text = "打开K2";
                                        panel2.BackgroundImage = USBControlUtility.Properties.Resources.off;
                                    }));
                                }
                                break;
                            case 3:
                                k3 = buf[3] == 255;
                                if (k3)
                                {
                                    this.Invoke((EventHandler)(delegate
                                    {
                                        button2.Text = "关闭K3";
                                        panel3.BackgroundImage = USBControlUtility.Properties.Resources.on;
                                    }));
                                }
                                else
                                {
                                    this.Invoke((EventHandler)(delegate
                                    {
                                        button2.Text = "打开K3";
                                        panel3.BackgroundImage = USBControlUtility.Properties.Resources.off;
                                    }));
                                }
                                break;
                            case 4:
                                k4 = buf[3] == 255;
                                if (k4)
                                {
                                    this.Invoke((EventHandler)(delegate
                                    {
                                        button3.Text = "关闭K4";
                                        panel4.BackgroundImage = USBControlUtility.Properties.Resources.on;
                                    }));
                                }
                                else
                                {
                                    this.Invoke((EventHandler)(delegate
                                    {
                                        button3.Text = "打开K4";
                                        panel4.BackgroundImage = USBControlUtility.Properties.Resources.off;
                                    }));
                                }
                                break;
                        }
                        string s = buf[3] == 0 ? ":关" : ":开";
                        this.Invoke((EventHandler)(delegate
                        {
                            txtGet.AppendText("K" + k + s + Environment.NewLine);
                        }));
                    }
                    else if (buf[6] == tail[0] && buf[7] == tail[1])
                    {
                        k1 = buf[2] == 255;
                        k2 = buf[3] == 255;
                        k3 = buf[4] == 255;
                        k4 = buf[5] == 255;
                        this.Invoke((EventHandler)(delegate
                        {
                            if (k1)
                            {
                                btnSend.Text = "关闭K1";
                                panel1.BackgroundImage = USBControlUtility.Properties.Resources.on;
                            }
                            else
                            {
                                btnSend.Text = "打开K1";
                                panel1.BackgroundImage = USBControlUtility.Properties.Resources.off;
                            }
                            if (k2)
                            {
                                button1.Text = "关闭K2";
                                panel2.BackgroundImage = USBControlUtility.Properties.Resources.on;
                            }
                            else
                            {
                                button1.Text = "打开K2";
                                panel2.BackgroundImage = USBControlUtility.Properties.Resources.off;
                            }
                            if (k3)
                            {
                                button2.Text = "关闭K3";
                                panel3.BackgroundImage = USBControlUtility.Properties.Resources.on;
                            }
                            else
                            {
                                button2.Text = "打开K3";
                                panel3.BackgroundImage = USBControlUtility.Properties.Resources.off;
                            }
                            if (k4)
                            {
                                button3.Text = "关闭K4";
                                panel4.BackgroundImage = USBControlUtility.Properties.Resources.on;
                            }
                            else
                            {
                                button3.Text = "打开K4";
                                panel4.BackgroundImage = USBControlUtility.Properties.Resources.off;
                            }
                        }));
                        StringBuilder ss = new StringBuilder();
                        for (int i = 0; i < 4; i++)
                        {
                            ss.Append("K" + Convert.ToString(i + 1) + ((buf[2 + i] == 0) ? ":关" : ":开") + ",");
                        }
                        this.Invoke((EventHandler)(delegate
                        {
                            txtGet.AppendText(ss + Environment.NewLine);
                        }));
                    }
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sp.IsOpen)
            {
                try
                {
                    Send(0, false);
                }
                catch { }
            }
        }
    }
}
