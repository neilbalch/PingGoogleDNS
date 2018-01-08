using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net.NetworkInformation;
using System.Net;
using System.Drawing;

namespace PingGoogleDNS
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            PingGoogleDNS pinger = new PingGoogleDNS();
            pinger.StartPinger();
        }

        class PingGoogleDNS
        {
            #region Global Thread and Icon Objects
            NotifyIcon pingIcon;
            // Load Icon files
            Icon connectedIcon = new Icon("connected.ico");
            Icon disconnectedIcon = new Icon("disconnected.ico");
            // Create ping sender thread
            Thread pingWorker;
            #endregion

            #region StartPinger
            public void StartPinger() {
                // Create tray icon and make it visible
                pingIcon = new NotifyIcon();
                pingIcon.Icon = disconnectedIcon;
                pingIcon.Visible = true;

                // Create Menu Items and add them to a context meny on the tray icon
                MenuItem quit = new MenuItem("Quit / Exit");
                MenuItem name = new MenuItem("Pings google DNS (8.8.8.8) every second and reports status.");
                ContextMenu contextMenu = new ContextMenu();
                contextMenu.MenuItems.Add(quit);
                contextMenu.MenuItems.Add(name);
                pingIcon.ContextMenu = contextMenu;

                // Make Quit button close application
                quit.Click += Quit_Click;

                // Start the Ping Sender thread
                pingWorker = new Thread(new ThreadStart(PingSenderThread));
                pingWorker.Start();
            }
            #endregion

            #region Quit Button Handler
            private void Quit_Click(object sender, EventArgs e)
            {
                // Get rid of the tray icon
                pingIcon.Dispose();
                // Kill the Ping Sender thread
                pingWorker.Abort();
                // Exit Apllication
                Environment.Exit(1);
            }
            #endregion

            #region Ping Sender Thread
            public void PingSenderThread()
            {
                // Create the ping sender
                Ping pingSender = new Ping();

                // Form the IPAddress object for the Google DNS server (8.8.8.8)
                IPAddress iPAddress;
                IPAddress.TryParse("8.8.8.8", out iPAddress);

                // Ping the server and get a reply
                PingReply reply;
                try
                {
                    while (true)
                    {
                        try
                        {
                            reply = pingSender.Send(iPAddress);

                            if (reply.Status == IPStatus.Success)
                            {
                                // Change the tray icon to connected
                                pingIcon.Icon = connectedIcon;
                                // Make tray icon alt text say success
                                pingIcon.Text = "Ping to 8.8.8.8 success!";
                            }
                        }
                        catch (PingException pe)
                        {
                            // Change the Tray icon to disconnected
                            pingIcon.Icon = disconnectedIcon;
                            // Make tray icon alt text say failure
                            pingIcon.Text = "Ping to 8.8.8.8 failure.";
                        }

                        // Sleep for 1000ms (1 second)
                        Thread.Sleep(1000);
                    }
                }
                catch (ThreadAbortException)
                {
                    // No need to do anything, just catch the ThreadAbortException.
                }
            }
            #endregion
        }
    }
}
