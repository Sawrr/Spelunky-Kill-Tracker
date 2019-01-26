using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using AchievementsTracker.Properties;
using System.Drawing;
using System.Web.Script.Serialization;

namespace KillTracker
{
    static public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Global exception handler
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Console.WriteLine(e.ExceptionObject.ToString());
            };

            // Initial setup
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new TrayApplicationContext());
        }

        public class TrayApplicationContext : ApplicationContext
        {
            private MainForm form;
            private Tracker tracker;

            public TrayApplicationContext()
            {
                // Create forms
                form = new MainForm();
                form.Show();

                // Create tracker thread
                tracker = new Tracker(form);
                Thread trackerThread = new Thread(() => tracker.Main());
                trackerThread.IsBackground = true;
                trackerThread.Start();

                // Set main form to terminate application on close
                form.FormClosing += (s, e) =>
                {
                    Exit(s, e);
                };
            }

            void Exit(object sender, EventArgs e)
            {
                Application.Exit();
            }
        }
    }
}
