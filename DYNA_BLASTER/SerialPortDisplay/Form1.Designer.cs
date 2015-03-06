namespace SerialPortExample
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.gpCmds = new System.Windows.Forms.GroupBox();
            this.btnSendStrCmd = new System.Windows.Forms.Button();
            this.txStrCmd = new System.Windows.Forms.TextBox();
            this.gbSET = new System.Windows.Forms.GroupBox();
            this.nudID = new System.Windows.Forms.NumericUpDown();
            this.btnTorqueLed = new System.Windows.Forms.Button();
            this.btnSetID1 = new System.Windows.Forms.Button();
            this.btnPos185 = new System.Windows.Forms.Button();
            this.btnSetPos180 = new System.Windows.Forms.Button();
            this.gbGET = new System.Windows.Forms.GroupBox();
            this.btnCurTemp = new System.Windows.Forms.Button();
            this.btnGetInfo = new System.Windows.Forms.Button();
            this.gpSent = new System.Windows.Forms.GroupBox();
            this.txSent = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tslConnected = new System.Windows.Forms.ToolStripStatusLabel();
            this.gbRec = new System.Windows.Forms.GroupBox();
            this.txReceived = new System.Windows.Forms.TextBox();
            this.gbLog = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnLogClear = new System.Windows.Forms.Button();
            this.txLog = new System.Windows.Forms.TextBox();
            this.SPI = new System.IO.Ports.SerialPort(this.components);
            this.timLOG = new System.Windows.Forms.Timer(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.gbPortcontrol = new System.Windows.Forms.GroupBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnRescanPorts = new System.Windows.Forms.Button();
            this.cbPorts = new System.Windows.Forms.ComboBox();
            this.lbSelectPort = new System.Windows.Forms.Label();
            this.txBaud = new System.Windows.Forms.TextBox();
            this.lsBaud = new System.Windows.Forms.ListBox();
            this.txId = new System.Windows.Forms.TextBox();
            this.btnIdPlus = new System.Windows.Forms.Button();
            this.btnIdMinus = new System.Windows.Forms.Button();
            this.txReadBuff = new System.Windows.Forms.TextBox();
            this.txCurCmd = new System.Windows.Forms.TextBox();
            this.timSim = new System.Windows.Forms.Timer(this.components);
            this.gpCmds.SuspendLayout();
            this.gbSET.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudID)).BeginInit();
            this.gbGET.SuspendLayout();
            this.gpSent.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.gbRec.SuspendLayout();
            this.gbLog.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.gbPortcontrol.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpCmds
            // 
            this.gpCmds.Controls.Add(this.btnSendStrCmd);
            this.gpCmds.Controls.Add(this.txStrCmd);
            this.gpCmds.Controls.Add(this.gbSET);
            this.gpCmds.Controls.Add(this.gbGET);
            this.gpCmds.Location = new System.Drawing.Point(6, 188);
            this.gpCmds.Name = "gpCmds";
            this.gpCmds.Size = new System.Drawing.Size(280, 186);
            this.gpCmds.TabIndex = 6;
            this.gpCmds.TabStop = false;
            this.gpCmds.Text = "Basic commands";
            // 
            // btnSendStrCmd
            // 
            this.btnSendStrCmd.Location = new System.Drawing.Point(237, 114);
            this.btnSendStrCmd.Name = "btnSendStrCmd";
            this.btnSendStrCmd.Size = new System.Drawing.Size(37, 38);
            this.btnSendStrCmd.TabIndex = 7;
            this.btnSendStrCmd.Text = "Send";
            this.btnSendStrCmd.UseVisualStyleBackColor = true;
            this.btnSendStrCmd.Click += new System.EventHandler(this.btnSendStrCmd_Click);
            // 
            // txStrCmd
            // 
            this.txStrCmd.Location = new System.Drawing.Point(6, 158);
            this.txStrCmd.Name = "txStrCmd";
            this.txStrCmd.Size = new System.Drawing.Size(268, 20);
            this.txStrCmd.TabIndex = 6;
            // 
            // gbSET
            // 
            this.gbSET.Controls.Add(this.nudID);
            this.gbSET.Controls.Add(this.btnTorqueLed);
            this.gbSET.Controls.Add(this.btnSetID1);
            this.gbSET.Controls.Add(this.btnPos185);
            this.gbSET.Controls.Add(this.btnSetPos180);
            this.gbSET.Location = new System.Drawing.Point(123, 19);
            this.gbSET.Name = "gbSET";
            this.gbSET.Size = new System.Drawing.Size(111, 133);
            this.gbSET.TabIndex = 3;
            this.gbSET.TabStop = false;
            this.gbSET.Text = "SET";
            // 
            // nudID
            // 
            this.nudID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudID.Dock = System.Windows.Forms.DockStyle.Top;
            this.nudID.Hexadecimal = true;
            this.nudID.Location = new System.Drawing.Point(3, 96);
            this.nudID.Maximum = new decimal(new int[] {
            253,
            0,
            0,
            0});
            this.nudID.Name = "nudID";
            this.nudID.Size = new System.Drawing.Size(105, 20);
            this.nudID.TabIndex = 77;
            this.nudID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudID.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnTorqueLed
            // 
            this.btnTorqueLed.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnTorqueLed.Location = new System.Drawing.Point(3, 76);
            this.btnTorqueLed.Name = "btnTorqueLed";
            this.btnTorqueLed.Size = new System.Drawing.Size(105, 20);
            this.btnTorqueLed.TabIndex = 76;
            this.btnTorqueLed.Text = "Torque, led";
            this.btnTorqueLed.UseVisualStyleBackColor = true;
            this.btnTorqueLed.Click += new System.EventHandler(this.btnTorqueLed_Click);
            // 
            // btnSetID1
            // 
            this.btnSetID1.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSetID1.Location = new System.Drawing.Point(3, 56);
            this.btnSetID1.Name = "btnSetID1";
            this.btnSetID1.Size = new System.Drawing.Size(105, 20);
            this.btnSetID1.TabIndex = 75;
            this.btnSetID1.Text = "All ID=1";
            this.btnSetID1.UseVisualStyleBackColor = true;
            this.btnSetID1.Click += new System.EventHandler(this.btnSetID1_Click);
            // 
            // btnPos185
            // 
            this.btnPos185.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnPos185.Location = new System.Drawing.Point(3, 36);
            this.btnPos185.Name = "btnPos185";
            this.btnPos185.Size = new System.Drawing.Size(105, 20);
            this.btnPos185.TabIndex = 74;
            this.btnPos185.Text = "position 185°";
            this.btnPos185.UseVisualStyleBackColor = true;
            this.btnPos185.Click += new System.EventHandler(this.btnPos185_Click);
            // 
            // btnSetPos180
            // 
            this.btnSetPos180.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSetPos180.Location = new System.Drawing.Point(3, 16);
            this.btnSetPos180.Name = "btnSetPos180";
            this.btnSetPos180.Size = new System.Drawing.Size(105, 20);
            this.btnSetPos180.TabIndex = 2;
            this.btnSetPos180.Text = "position 180°";
            this.btnSetPos180.UseVisualStyleBackColor = true;
            this.btnSetPos180.Click += new System.EventHandler(this.btnSetPos_Click);
            // 
            // gbGET
            // 
            this.gbGET.Controls.Add(this.btnCurTemp);
            this.gbGET.Controls.Add(this.btnGetInfo);
            this.gbGET.Location = new System.Drawing.Point(6, 19);
            this.gbGET.Name = "gbGET";
            this.gbGET.Size = new System.Drawing.Size(111, 133);
            this.gbGET.TabIndex = 2;
            this.gbGET.TabStop = false;
            this.gbGET.Text = "GET";
            // 
            // btnCurTemp
            // 
            this.btnCurTemp.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCurTemp.Location = new System.Drawing.Point(3, 36);
            this.btnCurTemp.Name = "btnCurTemp";
            this.btnCurTemp.Size = new System.Drawing.Size(105, 20);
            this.btnCurTemp.TabIndex = 2;
            this.btnCurTemp.Text = "cur Temp";
            this.btnCurTemp.UseVisualStyleBackColor = true;
            this.btnCurTemp.Click += new System.EventHandler(this.btnCurTemp_Click);
            // 
            // btnGetInfo
            // 
            this.btnGetInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnGetInfo.Location = new System.Drawing.Point(3, 16);
            this.btnGetInfo.Name = "btnGetInfo";
            this.btnGetInfo.Size = new System.Drawing.Size(105, 20);
            this.btnGetInfo.TabIndex = 1;
            this.btnGetInfo.Text = "info";
            this.btnGetInfo.UseVisualStyleBackColor = true;
            this.btnGetInfo.Click += new System.EventHandler(this.btnGetInfo_Click);
            // 
            // gpSent
            // 
            this.gpSent.Controls.Add(this.txSent);
            this.gpSent.Location = new System.Drawing.Point(292, 3);
            this.gpSent.Name = "gpSent";
            this.gpSent.Size = new System.Drawing.Size(310, 179);
            this.gpSent.TabIndex = 7;
            this.gpSent.TabStop = false;
            this.gpSent.Text = "Sent";
            // 
            // txSent
            // 
            this.txSent.BackColor = System.Drawing.Color.White;
            this.txSent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txSent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txSent.Enabled = false;
            this.txSent.Location = new System.Drawing.Point(3, 16);
            this.txSent.Multiline = true;
            this.txSent.Name = "txSent";
            this.txSent.ReadOnly = true;
            this.txSent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txSent.Size = new System.Drawing.Size(304, 160);
            this.txSent.TabIndex = 9;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslConnected});
            this.statusStrip1.Location = new System.Drawing.Point(0, 390);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(977, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            this.statusStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.statusStrip1_ItemClicked);
            // 
            // tslConnected
            // 
            this.tslConnected.BackColor = System.Drawing.Color.LimeGreen;
            this.tslConnected.Name = "tslConnected";
            this.tslConnected.Size = new System.Drawing.Size(70, 17);
            this.tslConnected.Text = "disconnected";
            // 
            // gbRec
            // 
            this.gbRec.Controls.Add(this.txReceived);
            this.gbRec.Location = new System.Drawing.Point(292, 188);
            this.gbRec.Name = "gbRec";
            this.gbRec.Size = new System.Drawing.Size(310, 186);
            this.gbRec.TabIndex = 10;
            this.gbRec.TabStop = false;
            this.gbRec.Text = "Received";
            // 
            // txReceived
            // 
            this.txReceived.BackColor = System.Drawing.Color.White;
            this.txReceived.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txReceived.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txReceived.Location = new System.Drawing.Point(3, 16);
            this.txReceived.Multiline = true;
            this.txReceived.Name = "txReceived";
            this.txReceived.ReadOnly = true;
            this.txReceived.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txReceived.Size = new System.Drawing.Size(304, 167);
            this.txReceived.TabIndex = 9;
            // 
            // gbLog
            // 
            this.gbLog.Controls.Add(this.button1);
            this.gbLog.Controls.Add(this.btnLogClear);
            this.gbLog.Controls.Add(this.txLog);
            this.gbLog.Location = new System.Drawing.Point(608, 3);
            this.gbLog.Name = "gbLog";
            this.gbLog.Size = new System.Drawing.Size(357, 260);
            this.gbLog.TabIndex = 11;
            this.gbLog.TabStop = false;
            this.gbLog.Text = "Log";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(301, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(22, 20);
            this.button1.TabIndex = 15;
            this.button1.Text = "×";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // btnLogClear
            // 
            this.btnLogClear.Font = new System.Drawing.Font("Liberation Sans Narrow", 5.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnLogClear.Location = new System.Drawing.Point(140, 0);
            this.btnLogClear.Name = "btnLogClear";
            this.btnLogClear.Size = new System.Drawing.Size(22, 20);
            this.btnLogClear.TabIndex = 14;
            this.btnLogClear.Text = "del";
            this.btnLogClear.UseVisualStyleBackColor = true;
            this.btnLogClear.Click += new System.EventHandler(this.btnLogClear_Click);
            // 
            // txLog
            // 
            this.txLog.BackColor = System.Drawing.Color.White;
            this.txLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txLog.Location = new System.Drawing.Point(3, 16);
            this.txLog.Multiline = true;
            this.txLog.Name = "txLog";
            this.txLog.ReadOnly = true;
            this.txLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txLog.Size = new System.Drawing.Size(351, 241);
            this.txLog.TabIndex = 9;
            this.txLog.WordWrap = false;
            // 
            // SPI
            // 
            this.SPI.PortName = "COM4";
            // 
            // timLOG
            // 
            this.timLOG.Enabled = true;
            this.timLOG.Tick += new System.EventHandler(this.timLOG_Tick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.gbPortcontrol);
            this.groupBox1.Location = new System.Drawing.Point(0, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(133, 179);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // gbPortcontrol
            // 
            this.gbPortcontrol.AutoSize = true;
            this.gbPortcontrol.Controls.Add(this.btnClose);
            this.gbPortcontrol.Controls.Add(this.btnOpen);
            this.gbPortcontrol.Controls.Add(this.btnRescanPorts);
            this.gbPortcontrol.Controls.Add(this.cbPorts);
            this.gbPortcontrol.Controls.Add(this.lbSelectPort);
            this.gbPortcontrol.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbPortcontrol.Location = new System.Drawing.Point(3, 16);
            this.gbPortcontrol.Name = "gbPortcontrol";
            this.gbPortcontrol.Size = new System.Drawing.Size(127, 122);
            this.gbPortcontrol.TabIndex = 66;
            this.gbPortcontrol.TabStop = false;
            this.gbPortcontrol.Text = "Port Control";
            // 
            // btnClose
            // 
            this.btnClose.AutoSize = true;
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnClose.Location = new System.Drawing.Point(3, 96);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(121, 23);
            this.btnClose.TabIndex = 59;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.AutoSize = true;
            this.btnOpen.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnOpen.Location = new System.Drawing.Point(3, 73);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(121, 23);
            this.btnOpen.TabIndex = 58;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnRescanPorts
            // 
            this.btnRescanPorts.AutoSize = true;
            this.btnRescanPorts.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnRescanPorts.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnRescanPorts.Location = new System.Drawing.Point(3, 50);
            this.btnRescanPorts.Name = "btnRescanPorts";
            this.btnRescanPorts.Size = new System.Drawing.Size(121, 23);
            this.btnRescanPorts.TabIndex = 57;
            this.btnRescanPorts.Text = "&Rescan ports";
            this.btnRescanPorts.UseVisualStyleBackColor = true;
            this.btnRescanPorts.Click += new System.EventHandler(this.btnRescanPorts_Click);
            // 
            // cbPorts
            // 
            this.cbPorts.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbPorts.FormattingEnabled = true;
            this.cbPorts.Location = new System.Drawing.Point(3, 29);
            this.cbPorts.Name = "cbPorts";
            this.cbPorts.Size = new System.Drawing.Size(121, 21);
            this.cbPorts.TabIndex = 36;
            // 
            // lbSelectPort
            // 
            this.lbSelectPort.AutoSize = true;
            this.lbSelectPort.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbSelectPort.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbSelectPort.Location = new System.Drawing.Point(3, 16);
            this.lbSelectPort.Name = "lbSelectPort";
            this.lbSelectPort.Size = new System.Drawing.Size(61, 13);
            this.lbSelectPort.TabIndex = 37;
            this.lbSelectPort.Text = "Select port:";
            // 
            // txBaud
            // 
            this.txBaud.Location = new System.Drawing.Point(139, 49);
            this.txBaud.Name = "txBaud";
            this.txBaud.Size = new System.Drawing.Size(100, 20);
            this.txBaud.TabIndex = 13;
            this.txBaud.Text = "57600";
            // 
            // lsBaud
            // 
            this.lsBaud.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lsBaud.FormattingEnabled = true;
            this.lsBaud.Location = new System.Drawing.Point(139, 76);
            this.lsBaud.Name = "lsBaud";
            this.lsBaud.Size = new System.Drawing.Size(141, 41);
            this.lsBaud.TabIndex = 14;
            this.lsBaud.SelectedIndexChanged += new System.EventHandler(this.lsBaud_SelectedIndexChanged);
            // 
            // txId
            // 
            this.txId.Location = new System.Drawing.Point(156, 4);
            this.txId.Name = "txId";
            this.txId.Size = new System.Drawing.Size(40, 20);
            this.txId.TabIndex = 67;
            this.txId.Text = "1";
            // 
            // btnIdPlus
            // 
            this.btnIdPlus.Location = new System.Drawing.Point(202, 4);
            this.btnIdPlus.Name = "btnIdPlus";
            this.btnIdPlus.Size = new System.Drawing.Size(37, 19);
            this.btnIdPlus.TabIndex = 68;
            this.btnIdPlus.Text = "+";
            this.btnIdPlus.UseVisualStyleBackColor = true;
            this.btnIdPlus.Click += new System.EventHandler(this.btnIdPlus_Click);
            // 
            // btnIdMinus
            // 
            this.btnIdMinus.Location = new System.Drawing.Point(239, 3);
            this.btnIdMinus.Name = "btnIdMinus";
            this.btnIdMinus.Size = new System.Drawing.Size(37, 19);
            this.btnIdMinus.TabIndex = 69;
            this.btnIdMinus.Text = "-";
            this.btnIdMinus.UseVisualStyleBackColor = true;
            this.btnIdMinus.Click += new System.EventHandler(this.btnIdMinus_Click);
            // 
            // txReadBuff
            // 
            this.txReadBuff.Location = new System.Drawing.Point(611, 280);
            this.txReadBuff.Multiline = true;
            this.txReadBuff.Name = "txReadBuff";
            this.txReadBuff.Size = new System.Drawing.Size(351, 42);
            this.txReadBuff.TabIndex = 70;
            // 
            // txCurCmd
            // 
            this.txCurCmd.Location = new System.Drawing.Point(611, 328);
            this.txCurCmd.Multiline = true;
            this.txCurCmd.Name = "txCurCmd";
            this.txCurCmd.Size = new System.Drawing.Size(351, 42);
            this.txCurCmd.TabIndex = 71;
            // 
            // timSim
            // 
            this.timSim.Enabled = true;
            this.timSim.Tick += new System.EventHandler(this.timSim_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(977, 412);
            this.Controls.Add(this.txCurCmd);
            this.Controls.Add(this.txReadBuff);
            this.Controls.Add(this.lsBaud);
            this.Controls.Add(this.btnIdMinus);
            this.Controls.Add(this.txBaud);
            this.Controls.Add(this.btnIdPlus);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txId);
            this.Controls.Add(this.gbLog);
            this.Controls.Add(this.gpSent);
            this.Controls.Add(this.gbRec);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.gpCmds);
            this.Name = "Form1";
            this.Text = "Serial Port Sample";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.gpCmds.ResumeLayout(false);
            this.gpCmds.PerformLayout();
            this.gbSET.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudID)).EndInit();
            this.gbGET.ResumeLayout(false);
            this.gpSent.ResumeLayout(false);
            this.gpSent.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.gbRec.ResumeLayout(false);
            this.gbRec.PerformLayout();
            this.gbLog.ResumeLayout(false);
            this.gbLog.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbPortcontrol.ResumeLayout(false);
            this.gbPortcontrol.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gpCmds;
        private System.Windows.Forms.GroupBox gpSent;
        private System.Windows.Forms.TextBox txSent;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tslConnected;
        private System.Windows.Forms.GroupBox gbSET;
        private System.Windows.Forms.Button btnSetPos180;
        private System.Windows.Forms.GroupBox gbGET;
        private System.Windows.Forms.Button btnGetInfo;
        private System.Windows.Forms.GroupBox gbRec;
        private System.Windows.Forms.TextBox txReceived;
        private System.Windows.Forms.GroupBox gbLog;
        private System.Windows.Forms.TextBox txLog;
        private System.IO.Ports.SerialPort SPI;
        private System.Windows.Forms.Timer timLOG;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox gbPortcontrol;
        private System.Windows.Forms.Button btnRescanPorts;
        private System.Windows.Forms.ComboBox cbPorts;
        private System.Windows.Forms.Label lbSelectPort;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.TextBox txBaud;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnLogClear;
        private System.Windows.Forms.Button btnSendStrCmd;
        private System.Windows.Forms.TextBox txStrCmd;
        private System.Windows.Forms.ListBox lsBaud;
        private System.Windows.Forms.Button btnIdMinus;
        private System.Windows.Forms.Button btnIdPlus;
        private System.Windows.Forms.TextBox txId;
        private System.Windows.Forms.TextBox txReadBuff;
        private System.Windows.Forms.TextBox txCurCmd;
        private System.Windows.Forms.Button btnCurTemp;
        private System.Windows.Forms.NumericUpDown nudID;
        private System.Windows.Forms.Button btnTorqueLed;
        private System.Windows.Forms.Button btnSetID1;
        private System.Windows.Forms.Button btnPos185;
        private System.Windows.Forms.Timer timSim;


    }
}

