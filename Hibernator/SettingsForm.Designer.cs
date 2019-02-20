namespace Hibernator
{
    partial class SettingsForm
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
            this.checkBox_TimerInvert = new System.Windows.Forms.CheckBox();
            this.checkBox_Logging = new System.Windows.Forms.CheckBox();
            this.button_OK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkBox_TimerInvert
            // 
            this.checkBox_TimerInvert.AutoSize = true;
            this.checkBox_TimerInvert.Location = new System.Drawing.Point(13, 13);
            this.checkBox_TimerInvert.Name = "checkBox_TimerInvert";
            this.checkBox_TimerInvert.Size = new System.Drawing.Size(79, 17);
            this.checkBox_TimerInvert.TabIndex = 0;
            this.checkBox_TimerInvert.Text = "TimerInvert";
            this.checkBox_TimerInvert.UseVisualStyleBackColor = true;
            this.checkBox_TimerInvert.CheckedChanged += new System.EventHandler(this.checkBox_TimerInvert_CheckedChanged);
            // 
            // checkBox_Logging
            // 
            this.checkBox_Logging.AutoSize = true;
            this.checkBox_Logging.Location = new System.Drawing.Point(111, 13);
            this.checkBox_Logging.Name = "checkBox_Logging";
            this.checkBox_Logging.Size = new System.Drawing.Size(64, 17);
            this.checkBox_Logging.TabIndex = 1;
            this.checkBox_Logging.Text = "Logging";
            this.checkBox_Logging.UseVisualStyleBackColor = true;
            this.checkBox_Logging.CheckedChanged += new System.EventHandler(this.checkBox_Logging_CheckedChanged);
            // 
            // button_OK
            // 
            this.button_OK.Location = new System.Drawing.Point(197, 56);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(75, 23);
            this.button_OK.TabIndex = 2;
            this.button_OK.Text = "OK";
            this.button_OK.UseVisualStyleBackColor = true;
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 91);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.checkBox_Logging);
            this.Controls.Add(this.checkBox_TimerInvert);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "SettingsForm";
            this.ShowInTaskbar = false;
            this.Text = "Settings";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox_TimerInvert;
        private System.Windows.Forms.CheckBox checkBox_Logging;
        private System.Windows.Forms.Button button_OK;
    }
}