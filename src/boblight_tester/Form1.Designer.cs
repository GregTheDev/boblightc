namespace boblight_tester
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
            this.txtServerIp = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtServerPort = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.btnSendHello = new System.Windows.Forms.Button();
            this.btnSendPing = new System.Windows.Forms.Button();
            this.btnSendGetVersion = new System.Windows.Forms.Button();
            this.btnGetLights = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtServerIp
            // 
            this.txtServerIp.Location = new System.Drawing.Point(15, 25);
            this.txtServerIp.Name = "txtServerIp";
            this.txtServerIp.Size = new System.Drawing.Size(100, 22);
            this.txtServerIp.TabIndex = 0;
            this.txtServerIp.Text = "127.0.0.1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Server IP";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(118, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Server Port";
            // 
            // txtServerPort
            // 
            this.txtServerPort.Location = new System.Drawing.Point(121, 25);
            this.txtServerPort.Name = "txtServerPort";
            this.txtServerPort.Size = new System.Drawing.Size(100, 22);
            this.txtServerPort.TabIndex = 3;
            this.txtServerPort.Text = "19333";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(227, 25);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 4;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(308, 25);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(75, 23);
            this.btnDisconnect.TabIndex = 5;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(213, 86);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(575, 352);
            this.txtLog.TabIndex = 6;
            // 
            // btnSendHello
            // 
            this.btnSendHello.Location = new System.Drawing.Point(15, 84);
            this.btnSendHello.Name = "btnSendHello";
            this.btnSendHello.Size = new System.Drawing.Size(129, 23);
            this.btnSendHello.TabIndex = 7;
            this.btnSendHello.Text = "Send \"hello\"";
            this.btnSendHello.UseVisualStyleBackColor = true;
            this.btnSendHello.Click += new System.EventHandler(this.btnSendHello_Click);
            // 
            // btnSendPing
            // 
            this.btnSendPing.Location = new System.Drawing.Point(15, 113);
            this.btnSendPing.Name = "btnSendPing";
            this.btnSendPing.Size = new System.Drawing.Size(129, 23);
            this.btnSendPing.TabIndex = 8;
            this.btnSendPing.Text = "Send \"ping\"";
            this.btnSendPing.UseVisualStyleBackColor = true;
            this.btnSendPing.Click += new System.EventHandler(this.btnSendPing_Click);
            // 
            // btnSendGetVersion
            // 
            this.btnSendGetVersion.Location = new System.Drawing.Point(15, 142);
            this.btnSendGetVersion.Name = "btnSendGetVersion";
            this.btnSendGetVersion.Size = new System.Drawing.Size(129, 23);
            this.btnSendGetVersion.TabIndex = 9;
            this.btnSendGetVersion.Text = "Send \"get version\"";
            this.btnSendGetVersion.UseVisualStyleBackColor = true;
            this.btnSendGetVersion.Click += new System.EventHandler(this.btnSendGetVersion_Click);
            // 
            // btnGetLights
            // 
            this.btnGetLights.Location = new System.Drawing.Point(15, 171);
            this.btnGetLights.Name = "btnGetLights";
            this.btnGetLights.Size = new System.Drawing.Size(129, 23);
            this.btnGetLights.TabIndex = 10;
            this.btnGetLights.Text = "Send \"get lights\"";
            this.btnGetLights.UseVisualStyleBackColor = true;
            this.btnGetLights.Click += new System.EventHandler(this.btnGetLights_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnGetLights);
            this.Controls.Add(this.btnSendGetVersion);
            this.Controls.Add(this.btnSendPing);
            this.Controls.Add(this.btnSendHello);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.txtServerPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtServerIp);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "Form1";
            this.Text = "Boblight Tester";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtServerIp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtServerPort;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnSendHello;
        private System.Windows.Forms.Button btnSendPing;
        private System.Windows.Forms.Button btnSendGetVersion;
        private System.Windows.Forms.Button btnGetLights;
    }
}

