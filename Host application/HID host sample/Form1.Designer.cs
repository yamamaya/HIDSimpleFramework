namespace HID_host_sample {
    partial class Form1 {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose( bool disposing ) {
            if ( disposing && ( components != null ) ) {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent() {
            this.buttonOpen = new System.Windows.Forms.Button();
            this.buttonLEDOn = new System.Windows.Forms.Button();
            this.buttonLEDOff = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxSwitch = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxADC = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonOpen
            // 
            this.buttonOpen.Location = new System.Drawing.Point(13, 13);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(75, 23);
            this.buttonOpen.TabIndex = 0;
            this.buttonOpen.Text = "Open";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // buttonLEDOn
            // 
            this.buttonLEDOn.Enabled = false;
            this.buttonLEDOn.Location = new System.Drawing.Point(94, 13);
            this.buttonLEDOn.Name = "buttonLEDOn";
            this.buttonLEDOn.Size = new System.Drawing.Size(75, 23);
            this.buttonLEDOn.TabIndex = 1;
            this.buttonLEDOn.Text = "LED ON";
            this.buttonLEDOn.UseVisualStyleBackColor = true;
            this.buttonLEDOn.Click += new System.EventHandler(this.buttonLEDOn_Click);
            // 
            // buttonLEDOff
            // 
            this.buttonLEDOff.Enabled = false;
            this.buttonLEDOff.Location = new System.Drawing.Point(175, 13);
            this.buttonLEDOff.Name = "buttonLEDOff";
            this.buttonLEDOff.Size = new System.Drawing.Size(75, 23);
            this.buttonLEDOff.TabIndex = 2;
            this.buttonLEDOff.Text = "LED OFF";
            this.buttonLEDOff.UseVisualStyleBackColor = true;
            this.buttonLEDOff.Click += new System.EventHandler(this.buttonLEDOff_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "Switch";
            // 
            // textBoxSwitch
            // 
            this.textBoxSwitch.Location = new System.Drawing.Point(54, 43);
            this.textBoxSwitch.Name = "textBoxSwitch";
            this.textBoxSwitch.ReadOnly = true;
            this.textBoxSwitch.Size = new System.Drawing.Size(47, 19);
            this.textBoxSwitch.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(119, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "ADC";
            // 
            // textBoxADC
            // 
            this.textBoxADC.Location = new System.Drawing.Point(154, 43);
            this.textBoxADC.Name = "textBoxADC";
            this.textBoxADC.ReadOnly = true;
            this.textBoxADC.Size = new System.Drawing.Size(47, 19);
            this.textBoxADC.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(262, 75);
            this.Controls.Add(this.textBoxADC);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxSwitch);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonLEDOff);
            this.Controls.Add(this.buttonLEDOn);
            this.Controls.Add(this.buttonOpen);
            this.Name = "Form1";
            this.Text = "HID host sample";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.Button buttonLEDOn;
        private System.Windows.Forms.Button buttonLEDOff;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxSwitch;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxADC;
    }
}

