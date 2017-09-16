namespace WindowsFormsApplication1
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
            this.buttonDiscovery = new System.Windows.Forms.Button();
            this.listViewLog = new System.Windows.Forms.ListView();
            this.columnOrder = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLog = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnTick = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonClear = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.buttonGetPower = new System.Windows.Forms.Button();
            this.buttonGetColor = new System.Windows.Forms.Button();
            this.buttonSetPower = new System.Windows.Forms.Button();
            this.comboBoxPower = new System.Windows.Forms.ComboBox();
            this.buttonSetColor = new System.Windows.Forms.Button();
            this.textBoxColor = new System.Windows.Forms.TextBox();
            this.TextBox_AdapterIP = new System.Windows.Forms.TextBox();
            this.textBox_AdapterPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonDiscovery
            // 
            this.buttonDiscovery.Location = new System.Drawing.Point(228, 13);
            this.buttonDiscovery.Name = "buttonDiscovery";
            this.buttonDiscovery.Size = new System.Drawing.Size(104, 46);
            this.buttonDiscovery.TabIndex = 0;
            this.buttonDiscovery.Text = "Discover";
            this.buttonDiscovery.UseVisualStyleBackColor = true;
            this.buttonDiscovery.Click += new System.EventHandler(this.buttonDiscovery_Click);
            // 
            // listViewLog
            // 
            this.listViewLog.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnOrder,
            this.columnTime,
            this.columnLog,
            this.columnTick});
            this.listViewLog.FullRowSelect = true;
            this.listViewLog.Location = new System.Drawing.Point(388, 79);
            this.listViewLog.MultiSelect = false;
            this.listViewLog.Name = "listViewLog";
            this.listViewLog.Size = new System.Drawing.Size(861, 756);
            this.listViewLog.TabIndex = 1;
            this.listViewLog.UseCompatibleStateImageBehavior = false;
            this.listViewLog.View = System.Windows.Forms.View.Details;
            // 
            // columnOrder
            // 
            this.columnOrder.Text = "Order";
            // 
            // columnTime
            // 
            this.columnTime.Text = "Time";
            this.columnTime.Width = 135;
            // 
            // columnLog
            // 
            this.columnLog.Text = "Log";
            this.columnLog.Width = 519;
            // 
            // columnTick
            // 
            this.columnTick.Text = "Total time";
            this.columnTick.Width = 118;
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(1162, 50);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(75, 23);
            this.buttonClear.TabIndex = 2;
            this.buttonClear.Text = "Clear Log";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(0, 79);
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox1.Size = new System.Drawing.Size(360, 745);
            this.listBox1.TabIndex = 3;
            // 
            // buttonGetPower
            // 
            this.buttonGetPower.Location = new System.Drawing.Point(388, 10);
            this.buttonGetPower.Name = "buttonGetPower";
            this.buttonGetPower.Size = new System.Drawing.Size(164, 23);
            this.buttonGetPower.TabIndex = 4;
            this.buttonGetPower.Text = "Get Power";
            this.buttonGetPower.UseVisualStyleBackColor = true;
            this.buttonGetPower.Click += new System.EventHandler(this.buttonGetPower_Click);
            // 
            // buttonGetColor
            // 
            this.buttonGetColor.Location = new System.Drawing.Point(602, 11);
            this.buttonGetColor.Name = "buttonGetColor";
            this.buttonGetColor.Size = new System.Drawing.Size(181, 23);
            this.buttonGetColor.TabIndex = 5;
            this.buttonGetColor.Text = "Get Color";
            this.buttonGetColor.UseVisualStyleBackColor = true;
            this.buttonGetColor.Click += new System.EventHandler(this.buttonGetColor_Click);
            // 
            // buttonSetPower
            // 
            this.buttonSetPower.Location = new System.Drawing.Point(477, 42);
            this.buttonSetPower.Name = "buttonSetPower";
            this.buttonSetPower.Size = new System.Drawing.Size(75, 23);
            this.buttonSetPower.TabIndex = 6;
            this.buttonSetPower.Text = "Set Power";
            this.buttonSetPower.UseVisualStyleBackColor = true;
            this.buttonSetPower.Click += new System.EventHandler(this.buttonSetPower_Click);
            // 
            // comboBoxPower
            // 
            this.comboBoxPower.FormattingEnabled = true;
            this.comboBoxPower.Items.AddRange(new object[] {
            "On",
            "Off"});
            this.comboBoxPower.Location = new System.Drawing.Point(388, 42);
            this.comboBoxPower.Name = "comboBoxPower";
            this.comboBoxPower.Size = new System.Drawing.Size(83, 21);
            this.comboBoxPower.TabIndex = 7;
            // 
            // buttonSetColor
            // 
            this.buttonSetColor.Location = new System.Drawing.Point(708, 42);
            this.buttonSetColor.Name = "buttonSetColor";
            this.buttonSetColor.Size = new System.Drawing.Size(75, 23);
            this.buttonSetColor.TabIndex = 8;
            this.buttonSetColor.Text = "Set Color";
            this.buttonSetColor.UseVisualStyleBackColor = true;
            this.buttonSetColor.Click += new System.EventHandler(this.buttonSetColor_Click);
            // 
            // textBoxColor
            // 
            this.textBoxColor.Location = new System.Drawing.Point(602, 43);
            this.textBoxColor.Name = "textBoxColor";
            this.textBoxColor.Size = new System.Drawing.Size(100, 20);
            this.textBoxColor.TabIndex = 9;
            // 
            // TextBox_AdapterIP
            // 
            this.TextBox_AdapterIP.Location = new System.Drawing.Point(110, 13);
            this.TextBox_AdapterIP.Name = "TextBox_AdapterIP";
            this.TextBox_AdapterIP.Size = new System.Drawing.Size(112, 20);
            this.TextBox_AdapterIP.TabIndex = 10;
            this.TextBox_AdapterIP.Text = "192.168.X.XXX";
            // 
            // textBox_AdapterPort
            // 
            this.textBox_AdapterPort.Location = new System.Drawing.Point(110, 39);
            this.textBox_AdapterPort.Name = "textBox_AdapterPort";
            this.textBox_AdapterPort.Size = new System.Drawing.Size(112, 20);
            this.textBox_AdapterPort.TabIndex = 11;
            this.textBox_AdapterPort.Text = "8080";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "SORIS Adapter IP:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "SORIS Adapter Port:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1249, 836);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_AdapterPort);
            this.Controls.Add(this.TextBox_AdapterIP);
            this.Controls.Add(this.textBoxColor);
            this.Controls.Add(this.buttonSetColor);
            this.Controls.Add(this.comboBoxPower);
            this.Controls.Add(this.buttonSetPower);
            this.Controls.Add(this.buttonGetColor);
            this.Controls.Add(this.buttonGetPower);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.listViewLog);
            this.Controls.Add(this.buttonDiscovery);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonDiscovery;
        private System.Windows.Forms.ListView listViewLog;
        private System.Windows.Forms.ColumnHeader columnOrder;
        private System.Windows.Forms.ColumnHeader columnTime;
        private System.Windows.Forms.ColumnHeader columnLog;
        private System.Windows.Forms.ColumnHeader columnTick;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button buttonGetPower;
        private System.Windows.Forms.Button buttonGetColor;
        private System.Windows.Forms.Button buttonSetPower;
        private System.Windows.Forms.ComboBox comboBoxPower;
        private System.Windows.Forms.Button buttonSetColor;
        private System.Windows.Forms.TextBox textBoxColor;
        private System.Windows.Forms.TextBox TextBox_AdapterIP;
        private System.Windows.Forms.TextBox textBox_AdapterPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

