namespace KellComUtility
{
    partial class CommPortSetting
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.com = new System.Windows.Forms.ComboBox();
            this.baudRate = new System.Windows.Forms.ComboBox();
            this.dataBits = new System.Windows.Forms.ComboBox();
            this.stopBits = new System.Windows.Forms.ComboBox();
            this.parity = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // com
            // 
            this.com.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.com.FormattingEnabled = true;
            this.com.Location = new System.Drawing.Point(3, 2);
            this.com.Name = "com";
            this.com.Size = new System.Drawing.Size(53, 20);
            this.com.TabIndex = 0;
            this.com.SelectedIndexChanged += new System.EventHandler(this.com_SelectedIndexChanged);
            // 
            // baudRate
            // 
            this.baudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.baudRate.FormattingEnabled = true;
            this.baudRate.Items.AddRange(new object[] {
            "110",
            "300",
            "600",
            "1200",
            "2400",
            "4800",
            "9600",
            "19200",
            "38400",
            "57600",
            "115200",
            "128000",
            "256000"});
            this.baudRate.Location = new System.Drawing.Point(57, 2);
            this.baudRate.Name = "baudRate";
            this.baudRate.Size = new System.Drawing.Size(58, 20);
            this.baudRate.TabIndex = 1;
            this.baudRate.SelectedIndexChanged += new System.EventHandler(this.baudRate_SelectedIndexChanged);
            // 
            // dataBits
            // 
            this.dataBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dataBits.FormattingEnabled = true;
            this.dataBits.Items.AddRange(new object[] {
            "4",
            "5",
            "6",
            "7",
            "8"});
            this.dataBits.Location = new System.Drawing.Point(116, 2);
            this.dataBits.Name = "dataBits";
            this.dataBits.Size = new System.Drawing.Size(31, 20);
            this.dataBits.TabIndex = 2;
            this.dataBits.SelectedIndexChanged += new System.EventHandler(this.dataBits_SelectedIndexChanged);
            // 
            // stopBits
            // 
            this.stopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.stopBits.FormattingEnabled = true;
            this.stopBits.Items.AddRange(new object[] {
            "1",
            "1.5",
            "2"});
            this.stopBits.Location = new System.Drawing.Point(148, 2);
            this.stopBits.Name = "stopBits";
            this.stopBits.Size = new System.Drawing.Size(96, 20);
            this.stopBits.TabIndex = 3;
            this.stopBits.SelectedIndexChanged += new System.EventHandler(this.stopBits_SelectedIndexChanged);
            // 
            // parity
            // 
            this.parity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.parity.FormattingEnabled = true;
            this.parity.Items.AddRange(new object[] {
            "None"});
            this.parity.Location = new System.Drawing.Point(245, 2);
            this.parity.Name = "parity";
            this.parity.Size = new System.Drawing.Size(54, 20);
            this.parity.TabIndex = 4;
            this.parity.SelectedIndexChanged += new System.EventHandler(this.parity_SelectedIndexChanged);
            // 
            // CommPortSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.parity);
            this.Controls.Add(this.stopBits);
            this.Controls.Add(this.dataBits);
            this.Controls.Add(this.baudRate);
            this.Controls.Add(this.com);
            this.Name = "CommPortSetting";
            this.Size = new System.Drawing.Size(301, 24);
            this.Load += new System.EventHandler(this.CommPortSetting_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox com;
        private System.Windows.Forms.ComboBox baudRate;
        private System.Windows.Forms.ComboBox dataBits;
        private System.Windows.Forms.ComboBox stopBits;
        private System.Windows.Forms.ComboBox parity;
    }
}
