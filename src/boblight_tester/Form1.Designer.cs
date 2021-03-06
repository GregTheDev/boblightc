﻿namespace boblight_tester
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
            this.btnSendSetPriority = new System.Windows.Forms.Button();
            this.txtPriority = new System.Windows.Forms.TextBox();
            this.btnSetLightRgb = new System.Windows.Forms.Button();
            this.txtSetRgbLightName = new System.Windows.Forms.TextBox();
            this.txtSetRgbRgb = new System.Windows.Forms.TextBox();
            this.txtSetSpeedSpeed = new System.Windows.Forms.TextBox();
            this.txtSetSpeedLightName = new System.Windows.Forms.TextBox();
            this.btnSetLightSpeed = new System.Windows.Forms.Button();
            this.txtSetInterpolation = new System.Windows.Forms.TextBox();
            this.txtSetInterpolationName = new System.Windows.Forms.TextBox();
            this.btnSetLIghtInterpolation = new System.Windows.Forms.Button();
            this.txtSetUse = new System.Windows.Forms.TextBox();
            this.txtSetUseLightName = new System.Windows.Forms.TextBox();
            this.btnSetUse = new System.Windows.Forms.Button();
            this.txtSetSingleChangeValue = new System.Windows.Forms.TextBox();
            this.txtSetSingleChangeLightName = new System.Windows.Forms.TextBox();
            this.btnSetSingleChange = new System.Windows.Forms.Button();
            this.btnSendSync = new System.Windows.Forms.Button();
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
            this.txtLog.Location = new System.Drawing.Point(460, 86);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(328, 352);
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
            // btnSendSetPriority
            // 
            this.btnSendSetPriority.Location = new System.Drawing.Point(15, 200);
            this.btnSendSetPriority.Name = "btnSendSetPriority";
            this.btnSendSetPriority.Size = new System.Drawing.Size(129, 23);
            this.btnSendSetPriority.TabIndex = 11;
            this.btnSendSetPriority.Text = "Send \"set priority\"";
            this.btnSendSetPriority.UseVisualStyleBackColor = true;
            this.btnSendSetPriority.Click += new System.EventHandler(this.btnSendSetPriority_Click);
            // 
            // txtPriority
            // 
            this.txtPriority.Location = new System.Drawing.Point(150, 201);
            this.txtPriority.Name = "txtPriority";
            this.txtPriority.Size = new System.Drawing.Size(71, 22);
            this.txtPriority.TabIndex = 12;
            this.txtPriority.Text = "1";
            this.txtPriority.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnSetLightRgb
            // 
            this.btnSetLightRgb.Location = new System.Drawing.Point(15, 229);
            this.btnSetLightRgb.Name = "btnSetLightRgb";
            this.btnSetLightRgb.Size = new System.Drawing.Size(129, 23);
            this.btnSetLightRgb.TabIndex = 13;
            this.btnSetLightRgb.Text = "Send \"set light rgb\"";
            this.btnSetLightRgb.UseVisualStyleBackColor = true;
            this.btnSetLightRgb.Click += new System.EventHandler(this.btnSetLightRgb_Click);
            // 
            // txtSetRgbLightName
            // 
            this.txtSetRgbLightName.Location = new System.Drawing.Point(150, 231);
            this.txtSetRgbLightName.Name = "txtSetRgbLightName";
            this.txtSetRgbLightName.Size = new System.Drawing.Size(71, 22);
            this.txtSetRgbLightName.TabIndex = 14;
            this.txtSetRgbLightName.Text = "start3";
            this.txtSetRgbLightName.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtSetRgbRgb
            // 
            this.txtSetRgbRgb.Location = new System.Drawing.Point(231, 231);
            this.txtSetRgbRgb.Name = "txtSetRgbRgb";
            this.txtSetRgbRgb.Size = new System.Drawing.Size(71, 22);
            this.txtSetRgbRgb.TabIndex = 15;
            this.txtSetRgbRgb.Text = "0.1|0.2|0.3";
            this.txtSetRgbRgb.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtSetSpeedSpeed
            // 
            this.txtSetSpeedSpeed.Location = new System.Drawing.Point(231, 260);
            this.txtSetSpeedSpeed.Name = "txtSetSpeedSpeed";
            this.txtSetSpeedSpeed.Size = new System.Drawing.Size(71, 22);
            this.txtSetSpeedSpeed.TabIndex = 18;
            this.txtSetSpeedSpeed.Text = "1.23";
            this.txtSetSpeedSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtSetSpeedLightName
            // 
            this.txtSetSpeedLightName.Location = new System.Drawing.Point(150, 260);
            this.txtSetSpeedLightName.Name = "txtSetSpeedLightName";
            this.txtSetSpeedLightName.Size = new System.Drawing.Size(71, 22);
            this.txtSetSpeedLightName.TabIndex = 17;
            this.txtSetSpeedLightName.Text = "start3";
            this.txtSetSpeedLightName.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnSetLightSpeed
            // 
            this.btnSetLightSpeed.Location = new System.Drawing.Point(15, 258);
            this.btnSetLightSpeed.Name = "btnSetLightSpeed";
            this.btnSetLightSpeed.Size = new System.Drawing.Size(129, 23);
            this.btnSetLightSpeed.TabIndex = 16;
            this.btnSetLightSpeed.Text = "Send \"set light speed\"";
            this.btnSetLightSpeed.UseVisualStyleBackColor = true;
            this.btnSetLightSpeed.Click += new System.EventHandler(this.btnSetLightSpeed_Click);
            // 
            // txtSetInterpolation
            // 
            this.txtSetInterpolation.Location = new System.Drawing.Point(282, 288);
            this.txtSetInterpolation.Name = "txtSetInterpolation";
            this.txtSetInterpolation.Size = new System.Drawing.Size(71, 22);
            this.txtSetInterpolation.TabIndex = 21;
            this.txtSetInterpolation.Text = "false";
            this.txtSetInterpolation.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtSetInterpolationName
            // 
            this.txtSetInterpolationName.Location = new System.Drawing.Point(201, 288);
            this.txtSetInterpolationName.Name = "txtSetInterpolationName";
            this.txtSetInterpolationName.Size = new System.Drawing.Size(71, 22);
            this.txtSetInterpolationName.TabIndex = 20;
            this.txtSetInterpolationName.Text = "start3";
            this.txtSetInterpolationName.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnSetLIghtInterpolation
            // 
            this.btnSetLIghtInterpolation.Location = new System.Drawing.Point(15, 287);
            this.btnSetLIghtInterpolation.Name = "btnSetLIghtInterpolation";
            this.btnSetLIghtInterpolation.Size = new System.Drawing.Size(165, 23);
            this.btnSetLIghtInterpolation.TabIndex = 19;
            this.btnSetLIghtInterpolation.Text = "Send \"set light interpolation\"";
            this.btnSetLIghtInterpolation.UseVisualStyleBackColor = true;
            this.btnSetLIghtInterpolation.Click += new System.EventHandler(this.btnSetLIghtInterpolation_Click);
            // 
            // txtSetUse
            // 
            this.txtSetUse.Location = new System.Drawing.Point(282, 317);
            this.txtSetUse.Name = "txtSetUse";
            this.txtSetUse.Size = new System.Drawing.Size(71, 22);
            this.txtSetUse.TabIndex = 24;
            this.txtSetUse.Text = "false";
            this.txtSetUse.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtSetUseLightName
            // 
            this.txtSetUseLightName.Location = new System.Drawing.Point(201, 317);
            this.txtSetUseLightName.Name = "txtSetUseLightName";
            this.txtSetUseLightName.Size = new System.Drawing.Size(71, 22);
            this.txtSetUseLightName.TabIndex = 23;
            this.txtSetUseLightName.Text = "start3";
            this.txtSetUseLightName.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnSetUse
            // 
            this.btnSetUse.Location = new System.Drawing.Point(15, 316);
            this.btnSetUse.Name = "btnSetUse";
            this.btnSetUse.Size = new System.Drawing.Size(165, 23);
            this.btnSetUse.TabIndex = 22;
            this.btnSetUse.Text = "Send \"set light use\"";
            this.btnSetUse.UseVisualStyleBackColor = true;
            this.btnSetUse.Click += new System.EventHandler(this.btnSetUse_Click);
            // 
            // txtSetSingleChangeValue
            // 
            this.txtSetSingleChangeValue.Location = new System.Drawing.Point(282, 346);
            this.txtSetSingleChangeValue.Name = "txtSetSingleChangeValue";
            this.txtSetSingleChangeValue.Size = new System.Drawing.Size(71, 22);
            this.txtSetSingleChangeValue.TabIndex = 27;
            this.txtSetSingleChangeValue.Text = "12.3";
            this.txtSetSingleChangeValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtSetSingleChangeLightName
            // 
            this.txtSetSingleChangeLightName.Location = new System.Drawing.Point(201, 346);
            this.txtSetSingleChangeLightName.Name = "txtSetSingleChangeLightName";
            this.txtSetSingleChangeLightName.Size = new System.Drawing.Size(71, 22);
            this.txtSetSingleChangeLightName.TabIndex = 26;
            this.txtSetSingleChangeLightName.Text = "start3";
            this.txtSetSingleChangeLightName.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnSetSingleChange
            // 
            this.btnSetSingleChange.Location = new System.Drawing.Point(15, 345);
            this.btnSetSingleChange.Name = "btnSetSingleChange";
            this.btnSetSingleChange.Size = new System.Drawing.Size(178, 23);
            this.btnSetSingleChange.TabIndex = 25;
            this.btnSetSingleChange.Text = "Send \"set light singlechange\"";
            this.btnSetSingleChange.UseVisualStyleBackColor = true;
            this.btnSetSingleChange.Click += new System.EventHandler(this.btnSetSingleChange_Click);
            // 
            // btnSendSync
            // 
            this.btnSendSync.Location = new System.Drawing.Point(15, 374);
            this.btnSendSync.Name = "btnSendSync";
            this.btnSendSync.Size = new System.Drawing.Size(178, 23);
            this.btnSendSync.TabIndex = 28;
            this.btnSendSync.Text = "Send \"sync\"";
            this.btnSendSync.UseVisualStyleBackColor = true;
            this.btnSendSync.Click += new System.EventHandler(this.btnSendSync_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnSendSync);
            this.Controls.Add(this.txtSetSingleChangeValue);
            this.Controls.Add(this.txtSetSingleChangeLightName);
            this.Controls.Add(this.btnSetSingleChange);
            this.Controls.Add(this.txtSetUse);
            this.Controls.Add(this.txtSetUseLightName);
            this.Controls.Add(this.btnSetUse);
            this.Controls.Add(this.txtSetInterpolation);
            this.Controls.Add(this.txtSetInterpolationName);
            this.Controls.Add(this.btnSetLIghtInterpolation);
            this.Controls.Add(this.txtSetSpeedSpeed);
            this.Controls.Add(this.txtSetSpeedLightName);
            this.Controls.Add(this.btnSetLightSpeed);
            this.Controls.Add(this.txtSetRgbRgb);
            this.Controls.Add(this.txtSetRgbLightName);
            this.Controls.Add(this.btnSetLightRgb);
            this.Controls.Add(this.txtPriority);
            this.Controls.Add(this.btnSendSetPriority);
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
        private System.Windows.Forms.Button btnSendSetPriority;
        private System.Windows.Forms.TextBox txtPriority;
        private System.Windows.Forms.Button btnSetLightRgb;
        private System.Windows.Forms.TextBox txtSetRgbLightName;
        private System.Windows.Forms.TextBox txtSetRgbRgb;
        private System.Windows.Forms.TextBox txtSetSpeedSpeed;
        private System.Windows.Forms.TextBox txtSetSpeedLightName;
        private System.Windows.Forms.Button btnSetLightSpeed;
        private System.Windows.Forms.TextBox txtSetInterpolation;
        private System.Windows.Forms.TextBox txtSetInterpolationName;
        private System.Windows.Forms.Button btnSetLIghtInterpolation;
        private System.Windows.Forms.TextBox txtSetUse;
        private System.Windows.Forms.TextBox txtSetUseLightName;
        private System.Windows.Forms.Button btnSetUse;
        private System.Windows.Forms.TextBox txtSetSingleChangeValue;
        private System.Windows.Forms.TextBox txtSetSingleChangeLightName;
        private System.Windows.Forms.Button btnSetSingleChange;
        private System.Windows.Forms.Button btnSendSync;
    }
}

