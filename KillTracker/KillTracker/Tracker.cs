using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KillTracker
{
    class Tracker
    {
        private const int PROCESS_WM_READ = 0x0010;

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        // Lock for resetting run manager
        private readonly object _runManagerLock = new Object();

        private MainForm ui;
        private Process spelunky;
        private bool running;

        public Tracker(MainForm form)
        {
            ui = form;
        }

        internal void UpdateKills(int kills)
        {
            ui.SetKills(kills);
        }

        public void Main()
        {
            running = false;

            // Listen for Spelunky Process
            int processHandle = 0;
            int baseAddress = 0;
            try
            {
                spelunky = SpelunkyProcessListener.listenForSpelunkyProcess();
                Console.WriteLine("Spelunky process detected");
                processHandle = (int)OpenProcess(PROCESS_WM_READ, false, spelunky.Id);
                baseAddress = spelunky.MainModule.BaseAddress.ToInt32();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error encountered listening for or opening the process");
                Console.WriteLine(e.ToString());
            }

            // Listen for process terminating
            spelunky.EnableRaisingEvents = true;
            spelunky.Exited += new EventHandler((s, e) =>
            {
                Console.WriteLine("Spelunky process exited");

                // Now start over
                Main();
            });

            // Create game manager
            GameManager gameManager = new GameManager(this, new MemoryReader(processHandle, baseAddress));

            // main game loop
            running = true;
            long time;
            while (running)
            {
                time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                long wakeUpTime = time + 16;

                // Acquire lock to prevent reset during updates
                lock(_runManagerLock)
                {
                    gameManager.update();
                }

                long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                long sleepTime = wakeUpTime - currentTime;
                if (sleepTime > 0)
                {
                    Thread.Sleep((int)sleepTime);
                }
                else
                {
                    Console.WriteLine("This tick took longer than 16 ms");
                }
            }
        }
    }
}
