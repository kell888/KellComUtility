using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace KellComUtility
{
    /// <summary>
    /// ComNum为short类型的串口参数设置控件
    /// </summary>
    [DefaultEvent("ValueChanged")]
    public partial class CommPortSetting : UserControl
    {
        public CommPortSetting()
        {
            InitializeComponent();
            LoadLocalComs();
            LoadStopBits();
            LoadParities();
            baudRate.SelectedItem = 9600;
            dataBits.SelectedItem = 8;
        }

        public event EventHandler ValueChanged;
        public event EventHandler ComChanged;

        short comNum;

        public short ComNum
        {
            get
            {
                return comNum;
            }
            set
            {
                if (value > 0)
                    com.SelectedItem = "COM" + value.ToString();
            }
        }

        int baudRat = 9600;

        public int BaudRate
        {
            get { return baudRat; }
            set { baudRate.SelectedItem = value.ToString(); }
        }

        int dataBit = 8;

        public int DataBits
        {
            get { return dataBit; }
            set
            {
                dataBit = value;
                dataBits.SelectedItem = dataBit.ToString();
            }
        }

        StopBits stopBit = StopBits.One;

        public StopBits StopBits
        {
            get
            {
                return stopBit;
            }
            set
            {
                stopBit = value;
                stopBits.SelectedItem = stopBit.ToString();
            }
        }

        Parity parit = Parity.None;

        public Parity Parity
        {
            get
            {
                return parit;
            }
            set
            {
                parit = value;
                parity.SelectedItem = parit.ToString();
            }
        }

        private void CommPortSetting_Load(object sender, EventArgs e)
        {
        }

        private void LoadParities()
        {
            parity.Items.Clear();
            string[] s = Enum.GetNames(typeof(Parity));
            foreach (string e in s)
            {
                parity.Items.Add(e);
            }
            parity.SelectedItem = "None";
        }

        private void LoadStopBits()
        {
            stopBits.Items.Clear();
            string[] s = Enum.GetNames(typeof(StopBits));
            foreach (string e in s)
            {
                stopBits.Items.Add(e);
            }
            stopBits.SelectedItem = "One";
        }

        private void LoadLocalComs()
        {
            string[] coms = SerialPort.GetPortNames();
            com.Items.Clear();
            foreach (string c in coms)
            {
                com.Items.Add(c.ToUpper());
            }
            if (com.Items.Count > 0)
                com.SelectedIndex = 0;
        }

        //private void baudRate_TextChanged(object sender, EventArgs e)
        //{
        //    ComboBox cb = sender as ComboBox;
        //    if (cb != null)
        //    {
        //        string s = cb.Text.Trim();
        //        uint ret;
        //        if (!uint.TryParse(s, out ret))
        //        {
        //            MessageBox.Show(cb.Name + "必须是正整数！");
        //            cb.Focus();
        //            cb.SelectAll();
        //        }
        //        else
        //        {
        //            if (cb.Name == "baudRate")
        //            {
        //                baudRat = int.Parse(baudRate.Text.Trim());
        //            }
        //            else if (cb.Name == "dataBits")
        //            {
        //                dataBit = int.Parse(dataBits.Text.Trim());
        //            }
        //        }
        //    }
        //}

        private void parity_SelectedIndexChanged(object sender, EventArgs e)
        {
            parit = (Parity)Enum.Parse(typeof(Parity), parity.Text);
            if (ValueChanged != null)
                ValueChanged(this, EventArgs.Empty);
        }

        private void stopBits_SelectedIndexChanged(object sender, EventArgs e)
        {
            stopBit = (StopBits)Enum.Parse(typeof(StopBits), stopBits.Text);
            if (ValueChanged != null)
                ValueChanged(this, EventArgs.Empty);
        }

        private void com_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (com.Text.Length > 3)
                comNum = short.Parse(com.Text.Substring(3));
            else
                comNum = 0;
            if (ComChanged != null)
                ComChanged(this, EventArgs.Empty);
        }

        private void baudRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            //如果有了TextChanged事件处理，此处便无需多余的处理
            baudRat = int.Parse(baudRate.Text);
            if (ValueChanged != null)
                ValueChanged(this, EventArgs.Empty);
        }

        private void dataBits_SelectedIndexChanged(object sender, EventArgs e)
        {
            //如果有了TextChanged事件处理，此处便无需多余的处理
            dataBit = int.Parse(dataBits.Text);
            if (ValueChanged != null)
                ValueChanged(this, EventArgs.Empty);
        }
    }
}
