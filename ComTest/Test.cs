using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using KellComUtility;

namespace ComTest
{
    public partial class Test : Form
    {
        public Test()
        {
            InitializeComponent();
        }

        List<SerialPort> serialports;
        int current = -1;
        StringBuilder builder = new StringBuilder();

        private void FormLoad(object sender, EventArgs e)
        {
            txtCmd.SelectedIndex = 0;
            serialports = new List<SerialPort>();
            //列出本机所有串口
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length > 0)
            {
                Array.Sort(ports);
                foreach (string port in ports)
                {
                    SerialPort sp = new SerialPort(port);
                    sp.ReadTimeout = (int)numericUpDown1.Value;
                    //添加事件注册
                    sp.DataReceived += comm_DataReceived;
                    serialports.Add(sp);
                }
                comboBoxPortSelect.Items.AddRange(ports);
                //默认选中第一个
                comboBoxPortSelect.SelectedIndex = 0;
            }
        }

        private bool OpenOrClosePort(bool open)
        {
            SerialPort sp = serialports[current];
            if (open)
            {
                //打开本计算机的串口
                if (sp.IsOpen)
                {
                    sp.Close();
                }
                try
                {
                    sp.Open();
                    btnOpen.Text = "关闭串口";//按钮打开
                    lblToolStripStatus.Text = "串口打开成功！";//状态栏
                    return true;
                }
                catch (Exception e)
                {
                    btnOpen.Text = "打开串口";
                    MessageBox.Show("没有发现串口或串口已被占用！" + e.Message);
                    lblToolStripStatus.Text = "串口打开失败！";
                }
            }
            else
            {
                //关闭本计算机的串口
                try
                {
                    sp.Close();
                    btnOpen.Text = "打开串口";//按钮关闭
                    lblToolStripStatus.Text = "串口关闭成功！";//状态栏
                    return true;
                }
                catch (Exception e)
                {
                    btnOpen.Text = "关闭串口";
                    MessageBox.Show(e.Message);
                    lblToolStripStatus.Text = "串口关闭失败！";
                }
            }
            return false;
        }

        //存放待发送的一包数据（包括帧头，命令号，帧长，帧数据，校验，帧尾等）
        public void Send(string data, byte length, byte head, byte[] tail, byte cmd, bool parity = false, bool frameLen = false)
        {
            data = data.Trim();
            string[] datas = data.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (frameLen)
            {
                if (datas.Length != length)
                {
                    MessageBox.Show("数据长度不匹配，请重新输入数据！");
                    textBox1.SelectAll();
                    textBox1.Focus();
                    return;
                }
            }
            if (current > -1)
            {
                int dataIndex=2;
                if (frameLen) dataIndex++;
                int tailLen = tail.Length;//帧尾
                int needLen = tailLen + dataIndex;
                if (parity) needLen = tailLen + dataIndex + 2;//假设是两位数的校验码（满足CRC16）
                //存放帧数据（length个字节）
                byte[] realData = ComUtility.StrHexToBin(data);
                if (length == 0)
                    length = (byte)realData.Length;
                byte[] package = new byte[length + needLen];
                package[0] = head;//帧头
                package[1] = cmd;//命令号
                byte frameLength = Convert.ToByte(ComUtility.GetHex(length, true), 16);//帧长
                if (frameLen) package[2] = frameLength;
                if (parity)
                {
                    byte[] tmp1 = new byte[] { head, cmd, frameLength };
                    byte[] tmp = ComUtility.MergeData(tmp1, realData);
                    object o = ComUtility.Parity(ComUtility.Convert(tmp), ComUtility.GetDefaultIndexs(tmp.Length));
                    if (o != null && o is ushort)
                    {
                        ushort pari = (ushort)o;
                        byte pari1, pari2;
                        ComUtility.GetBytes(pari, out pari1, out pari2);
                        package[dataIndex + length] = pari1;//校验位（高字节）
                        package[dataIndex + 1 + length] = pari2;//校验位（低字节）
                        for (int i = 0; i < tailLen; i++)
                        {
                            package[dataIndex + 2 + i + length] = tail[i];
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < realData.Length; i++)
                    {
                        package[dataIndex + i] = realData[i];
                    }
                    for (int i = 0; i < tailLen; i++)
                    {
                        package[dataIndex + i + length] = tail[i];
                    }
                }

                SerialPort sp = serialports[current];
                if (!sp.IsOpen)
                {
                    MessageBox.Show("串口没有打开，请打开串口！");
                    return;
                }
                txtGet.AppendText(DateTime.Now.ToLongTimeString() + " 发送:" + ComUtility.GetHex(package));
                sp.Write(package, 0, package.Length);//向串口发送一个数据包
                txtGet.AppendText(" 完毕。" + Environment.NewLine);
            }
            else
            {
                MessageBox.Show("没有指定任何串口，请选定一个串口！");
            }
        }

        private void comm_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string rHead = string.Empty;
            string tail = string.Empty;
            this.Invoke((EventHandler)(delegate
            {
                rHead = txtRHead.Text.Trim();
                tail = txtTail.Text.Trim();
            }));
            byte start = Convert.ToByte(rHead, 16);
            System.Threading.Thread.Sleep(50);
            SerialPort sp = sender as SerialPort;
            byte[] buf = new byte[sp.BytesToRead];
            sp.Read(buf, 0, buf.Length);
            builder.Append(ComUtility.GetHex(buf).ToUpper());
            //if (!builder.ToString().StartsWith(ComUtility.Format(rHead, false).ToUpper()))
            //{
            //    this.Invoke((EventHandler)(delegate
            //    {
            //        txtGet.AppendText("尚未读到合法收帧头，继续读...." + Environment.NewLine);
            //    }));
            //    builder.Remove(0, builder.Length);//Clear
            //    return;
            //}
            //if (!builder.ToString().EndsWith(ComUtility.Format(tail, false).ToUpper()))
            //{
            //    this.Invoke((EventHandler)(delegate
            //    {
            //        txtGet.AppendText("尚未读到合法帧尾，继续读...."+ Environment.NewLine);
            //    }));
            //    return;
            //}
            string content = builder.ToString();
            //因为要访问ui资源，所以需要使用invoke方式同步ui。
            this.Invoke((EventHandler)(delegate
            {
                //直接按ASCII规则转换成字符串
                //builder.Append(Encoding.ASCII.GetString(buf)); 
                txtGet.AppendText(DateTime.Now.ToLongTimeString() + " 接收: [" + content + "] -- ");
            }));
            //判断返回正确与否
            //if (content == rHead + tail)
            //{
            //    this.Invoke((EventHandler)(delegate
            //    {
            //        lblToolStripStatus.Text = "执行命令失败！";
            //    }));
            //}
            //else if (content.Length > (rHead + tail).Length && content.StartsWith(ComUtility.Format(rHead, false)) && content.EndsWith(ComUtility.Format(tail, false)))
            //{
            //    this.Invoke((EventHandler)(delegate
            //    {
            //        lblToolStripStatus.Text = "执行命令成功！";
            //    }));
            //}
            //else
            //{
            //    this.Invoke((EventHandler)(delegate
            //    {
            //        lblToolStripStatus.Text = "执行命令返回未知状态！";
            //    }));
            //}
            byte[] t = ComUtility.StrHexToBin(tail);
            if (buf.Length == 3 && buf[0] == start && buf[1] == t[0] && buf[2] == t[1])
            {
                this.Invoke((EventHandler)(delegate
                {
                    txtGet.AppendText("命令执行错误" + Environment.NewLine);
                }));
            }
            else
            {
                if (buf[0] == start)
                {
                    if ((buf[4] == t[0]) && (buf[5] == t[1]))
                    {
                        int k = (int)buf[2];
                        string s = buf[3] == 0 ? ":关" : ":开";
                        this.Invoke((EventHandler)(delegate
                        {
                            txtGet.AppendText("开关" + k + s + Environment.NewLine);
                        }));
                    }
                    else if (buf[6] == t[0] && buf[7] == t[1])
                    {
                        StringBuilder ss = new StringBuilder();
                        for (int i = 0; i < 4; i++)
                        {
                            ss.Append("开关" + Convert.ToString(i + 1) + ((buf[2 + i] == 0) ? ":关" : ":开") + " ");
                        }
                        this.Invoke((EventHandler)(delegate
                        {
                            txtGet.AppendText("全部开关" + ":" + ss + Environment.NewLine);
                        }));
                    }
                }
            }
            if (checkBox2.Checked)
            {
                //判断校验是否正确
                byte[] data = ComUtility.StrHexToBin(content);
                //object o = ComUtility.Parity(ComUtility.Convert(data), ComUtility.GetDefaultIndexs(data.Length));
                //if (o != null && o is ushort)
                //{
                //    ushort rxParity = (ushort)o;
                if (ComUtility.PassParity(data, data.Length))
                {
                    this.Invoke((EventHandler)(delegate
                    {
                        lblToolStripStatus.Text += " 校验错误！";
                    }));
                }
                //}
            }
            builder.Remove(0, builder.Length);//Clear
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

        private void comboBoxPortSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            current = comboBoxPortSelect.SelectedIndex;
            if (current > -1 && current < serialports.Count)
            {
                SerialPort sp = serialports[current];
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

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            foreach (SerialPort sp in serialports)
            {
                sp.ReadTimeout = (int)numericUpDown1.Value;
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                byte head = Convert.ToByte(txtSHead.Text.Trim(), 16);
                byte[] tail = ComUtility.StrHexToBin(txtTail.Text.Trim());
                byte cmd = Convert.ToByte(txtCmd.Text.Trim(), 16);
                Send(textBox1.Text, 0, head, tail, cmd);//(byte)numericUpDown2.Value
            }
            catch (Exception ex)
            {
                MessageBox.Show("发送失败：" + ex.Message);
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            foreach (SerialPort sp in serialports)
            {
                SetParam(sp);
            }
        }

        private void SetParam(SerialPort sp)
        {
            if (sp != null && int.Parse(sp.PortName.Substring(3)) == serialPortSetting1.ComNum)
            {
                sp.BaudRate = serialPortSetting1.BaudRate;
                sp.DataBits = serialPortSetting1.DataBits;
                sp.Parity = serialPortSetting1.Parity;
                sp.StopBits = serialPortSetting1.StopBits;
            }
        }

        private void GetParam(SerialPort sp)
        {
            if (sp != null && int.Parse(sp.PortName.Substring(3)) == serialPortSetting1.ComNum)
            {
                serialPortSetting1.BaudRate = sp.BaudRate;
                serialPortSetting1.DataBits = sp.DataBits;
                serialPortSetting1.Parity = sp.Parity;
                serialPortSetting1.StopBits = sp.StopBits;
            }
        }

        private void serialPortSetting1_ComChanged(object sender, EventArgs e)
        {
            foreach (SerialPort sp in serialports)
            {
                GetParam(sp);
            }
        }

        private void 清空ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtGet.Clear();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            foreach (SerialPort sp in serialports)
            {
                sp.DtrEnable = checkBox1.Checked;
            }
        }

        private void txtCmd_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (txtCmd.SelectedIndex == 0)
                numericUpDown2.Value = 3;
            else if (txtCmd.SelectedIndex == 1)
                numericUpDown2.Value = 2;
        }
    }
}
