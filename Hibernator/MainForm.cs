using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading; //Именно это пространство имен поддерживает многопоточность
using Microsoft.Win32;
using System.IO;

namespace Hibernator
{

    public partial class MainForm : Form
    {
        
        public byte minutesOff = 30;//через сколько минут выключить, от 1 до 99 минут
        public bool timerinvert = false;//сколько минут нет активности или сколько осталось до гибернации    

        string textInfo = "При бездействии пользователя,\n гибернизация начнётся через {0} мин \nСвернуть для фоновой работы. \nВ трее индикация бездействия в минутах\n";
        Hibernator hibernator;

        public MainForm()
        {
            InitializeComponent();
            Text = "Hibernator 2019.02.20";
            this.StartPosition = FormStartPosition.CenterScreen;
            SystemEvents.PowerModeChanged += OnPowerChange;
            Application.ApplicationExit += new EventHandler(ApplicationExit);
        
            if(!Settings.load())
            {   // если самый первый старт приложения
                Settings.timerinvert = timerinvert;
                Settings.minutesOff = minutesOff;
                Settings.save();
            }
            else
            {   // если не первый старт приложения то запуск в свёрнутом виде
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
            }

            // load settings
            minutesOff = Settings.minutesOff;
            timerinvert = Settings.timerinvert;
            Log.Write("MainForm");

        }

        private void MainForm_Load(object sender, EventArgs e)
        {        
            checkBox1.Checked = timerinvert;
            trackBar1.Value = minutesOff;
            label1.Text = String.Format("minutesOff {0}", minutesOff);
            label2.Text = String.Format(textInfo, minutesOff);           
            hibernator = new Hibernator(this);
            hibernator.Start();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
            {
                if (MessageBox.Show("Do you really want to exit?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != DialogResult.Yes)
                {
                e.Cancel = true;
                }       
            }

        private void MainForm_Resize(object sender, EventArgs e)
            {           
                if (this.WindowState == FormWindowState.Minimized)
                {
                    Hide();
                    setIconNumber(0);
                    notifyIcon.Visible = true;
                }
            }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            timerinvert = checkBox1.Checked;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            minutesOff = (byte)trackBar1.Value;
            label1.Text = String.Format("minutesOff {0}", minutesOff);
            label2.Text = String.Format(textInfo, minutesOff);
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

     
        private void setIconNumber(int number)
        {
            number = !timerinvert ? number : minutesOff - number;
            // Create a bitmap and draw text on it
            Bitmap bitmap = new Bitmap(16, 16);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.Transparent);
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            string str = number > 9 ? "" + number : " " + number;          
            graphics.DrawString(str, new Font("System", 16, FontStyle.Regular, GraphicsUnit.Pixel), Brushes.White, -4, -2);
            // Convert the bitmap with text to an Icon
            Icon icon = Icon.FromHandle(bitmap.GetHicon());
            notifyIcon.Icon = icon;
        }

        static int prevlastInputTime = 0;
        //обновление иконки в трее
        public void notyfyiconUpdate(int lastInputTime)
        {
            // lastInputTime - последняя активность пользователя в минутах
            // такая логика обновляет иконку не чаще одного раза в минуту
            if ((lastInputTime == 0 && prevlastInputTime > 0) ||
                (lastInputTime - prevlastInputTime >= 1))
            {
                setIconNumber(lastInputTime);
                Console.WriteLine("lastInputTime == " + lastInputTime);
            }
            prevlastInputTime = lastInputTime;
        }


        void ApplicationExit(object sender, EventArgs e)
        {
            // Сохраняем переменные. 
            Settings.timerinvert = timerinvert;
            Settings.minutesOff = minutesOff;
            Settings.save();
            // останавливаю поток гибернатора
            hibernator.Stop();
            Log.Write("ApplicationExit");
        }


        // https://stackoverflow.com/questions/18206183/event-to-detect-system-wake-up-from-sleep-in-c-sharp
        private void OnPowerChange(object s, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    Log.Write("PowerModes.Resume");
                    //MessageBox.Show("PowerModes.Resume");
                    hibernator.Start();                   
                    break;
                case PowerModes.Suspend:
                    Log.Write("PowerModes.Suspend");
                    //MessageBox.Show("PowerModes.Suspend");
                    hibernator.Stop();// не уверен нужно ли останавливать поток перед гибернацией                   
                    break;
            }
        }

    }


    public class Hibernator
    {

        private Thread myThread;
        private MainForm main;
        private bool run;
        public bool autoHibernate;//будет true при автоматическом входе в гибернацию

        public Hibernator(MainForm main)
        {
            this.main = main;
        }

        public void Start()
        {
            run = true;         
            myThread = new Thread(thread_func); //Создаем новый объект потока (Thread)
            myThread.IsBackground = true;// что-бы поток закрывался вместе с приложением
            myThread.Start(); //запускаем поток
        }

        /// остановить поток, например при закрытии приложения
        public void Stop()
        {
            run = false;
            myThread.Abort();
            myThread.Join();
            myThread = null;
        }


        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetWindowText(IntPtr hWnd, string lpString);

        [DllImport("user32.dll")]
        public static extern int SendMessageW(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);


        /// ожидание отмены пред гибернизацией с обратным отсчётом на кнопке
        public void message_thread_func()
        {
            Log.Write("message_thread_func");
            const int WM_COMMAND = 0x0111;
            const int IDYES = 6;
            const int IDNO = 7;
            const int BN_CLICKED = 0;

            IntPtr hwndMsgBox = (IntPtr)null;
            while(hwndMsgBox == (IntPtr)null)// ожидание до вызова окна сообщения
            {
                hwndMsgBox = FindWindow(null, "HibernateConfirm");
                Thread.Sleep(100);
            }
                
            IntPtr hwndButton = FindWindowEx(hwndMsgBox, (IntPtr)0, "Button", null);
            const int SW_HIDE = 0;
            ShowWindow(hwndButton, SW_HIDE);

            if (hwndMsgBox != null)
            {
                // длбавляем обратный отсчёт на титле
                for (int i = 30; i > 0; i--) // отсчёт 30 секунд
                {
                    if ( ((GetLastInputTime() / 60) < main.minutesOff) &&  // если была активность мышки или клавы
                        !autoHibernate) // при выходе из гибернации отмена только нажатием на кнопку 
                        SendMessageW(hwndMsgBox, WM_COMMAND, (IntPtr)(IDNO | (BN_CLICKED << 16)), hwndButton); //то симулируем нажатие "Нет"
                    string title_text = "HibernateConfirm " + i;
                    SetWindowText(hwndMsgBox, title_text);
                    Thread.Sleep(1000);
                }

                // симулируем нажатие "Да"               
                SendMessageW(hwndMsgBox, WM_COMMAND, (IntPtr)(IDYES | (BN_CLICKED << 16)), hwndButton);
            }

            Log.Write("message_thread_func  end");

        }

        public void thread_func()
        {
            Log.Write("thread_func");           
            while (run)
            {
                int lastInputTime = GetLastInputTime() / 60;// convert sec to min
                main.notyfyiconUpdate(lastInputTime);//обновление иконки в трее
                // form1.PowerModesResume нужен что-бы запустить таймер  выключения при выходе из гибернации
                if (lastInputTime >= main.minutesOff || autoHibernate)
                {
                    Log.Write("lastInputTime >= main.minutesOff");
                    Thread thread = new Thread(message_thread_func);
                    thread.IsBackground = true;
                    thread.Start();
                    Form fullWindow = CreateFullscreenWindow();// белое окно на весь экран
                    if (MessageBox.Show("Move mouse or press any key \nto interrupt the hibernation ",
                        "HibernateConfirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2,
                        MessageBoxOptions.ServiceNotification) != DialogResult.Yes)
                    {
                        Log.Write("HibernateConfirm != DialogResult.Yes");
                        thread.Abort();
                        thread = null;
                        autoHibernate = false;
                        fullWindow.Dispose();
                        continue;
                    }                
                    Log.Write("HibernateConfirm == DialogResult.Yes");
                    autoHibernate = true;//если уход автоматически в гибернацию
                    fullWindow.Dispose();
                    thread.Abort();
                    thread = null;
                    run = false;
                    Thread.Sleep(1000);
                    System.Diagnostics.Process.Start("CMD.exe", "/C shutdown -h");
                    //System.Diagnostics.Process.Start("CMD.exe", "/C "); // test

                }
                Thread.Sleep(1000);

            }
            Log.Write("thread_func end");
        }

        private Form CreateFullscreenWindow()
        {
            Form f = new Form();
            f.FormBorderStyle = FormBorderStyle.None;
            f.WindowState = FormWindowState.Maximized;
            f.TopMost = true;
            f.Show();
            return f;
        }

        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        internal struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }
        ///возврашяет сколько прошло секунд после последнего нажимания клавиш или движение мышки
        static int GetLastInputTime()
        {
            int idleTime = 0;
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            int envTicks = Environment.TickCount;
            if (GetLastInputInfo(ref lastInputInfo))
            {
                int lastInputTick = (int)lastInputInfo.dwTime;
                idleTime = envTicks - lastInputTick;
            }
            else
            {
                //throw new Exception(GetLastError().ToString());
            }
            return ((idleTime > 0) ? (idleTime / 1000) : idleTime);// millisec to sec
        }

    }

    public class Log
    {
        private static object sync = new object();
        public static void Write(string msg)
        {

            string filename = string.Format("log_{0:dd.MM.yyy}.txt", DateTime.Now);
            lock (sync)
            {
                File.AppendAllText(filename, string.Format("{0:HH:mm:ss.ms} : {1}\n", DateTime.Now, msg));
            }

        }
    }

    public class Settings
    {
        public static byte minutesOff;//через сколько минут выключить, от 1 до 99 минут
        public static bool timerinvert;//сколько минут нет активности или сколько осталось до гибернации

        public static void save()
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open("settings.dat", FileMode.OpenOrCreate)))
            {
                writer.Write(minutesOff);
                writer.Write(timerinvert);
            }
        }

        /// если файл успешно прочитан то вернёт true 
        public static bool load()
        {           
            if(File.Exists("settings.dat"))
            {
                using (BinaryReader reader = new BinaryReader(File.Open("settings.dat", FileMode.Open)))
                {
                    minutesOff = reader.ReadByte();
                    timerinvert = reader.ReadBoolean();
                }                
                return true;
            }
            return false;
        }
    }

}
