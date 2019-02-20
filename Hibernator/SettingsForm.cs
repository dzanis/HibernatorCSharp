using System;
using System.Windows.Forms;

namespace Hibernator
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;          
            this.ControlBox = false;// Убираем кнопки свернуть, развернуть, закрыть

        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            checkBox_TimerInvert.Checked = Settings.timerinvert;
            checkBox_Logging.Checked = Settings.logging;

        }

        private void checkBox_TimerInvert_CheckedChanged(object sender, EventArgs e)
        {
            Settings.timerinvert = checkBox_TimerInvert.Checked;
        }

        private void checkBox_Logging_CheckedChanged(object sender, EventArgs e)
        {
            Settings.logging = checkBox_Logging.Checked;
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            Settings.save();
            this.Dispose();
        }
    }

   

}
