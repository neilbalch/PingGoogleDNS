using System;
using System.Drawing;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;

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
            Application.Run(); // https://stackoverflow.com/q/69421384/3339274
        }

        class PingGoogleDNS
        {
            #region Global Thread and Icon Objects
            NotifyIcon pingIcon;
            // Load Icon files
            //Icon connectedIcon = new Icon("connected.ico");
            //Icon disconnectedIcon = new Icon("disconnected.ico");
            Icon goodIcon = new Icon("good_connection.ico");
            Icon weakIcon = new Icon("weak_connection.ico");
            Icon noIcon = new Icon("no_connection.ico");
            // Create ping sender thread
            Thread pingWorker;
            MenuItem roundTripTime;
            #endregion

            #region StartPinger
            public void StartPinger()
            {
                // Create tray icon and make it visible
                pingIcon = new NotifyIcon();
                pingIcon.Icon = noIcon /*disconnectedIcon*/;
                pingIcon.Visible = true;

                // Create Menu Items and add them to a context meny on the tray icon
                MenuItem quit = new MenuItem("Quit / Exit");
                MenuItem name = new MenuItem("Pings Google DNS (8.8.8.8)");
                roundTripTime = new MenuItem("Latest ping: N/A ms");
                ContextMenu contextMenu = new ContextMenu();
                contextMenu.MenuItems.Add(quit);
                contextMenu.MenuItems.Add(name);
                contextMenu.MenuItems.Add(roundTripTime);
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
                            // Send ping with timeout of 2000 ms (2s)
                            reply = pingSender.Send(iPAddress, 2000 /* ms */);

                            if (reply.Status == IPStatus.Success)
                            {
                                // Change the tray icon to connected
                                if (reply.RoundtripTime < 500 /* ms */)
                                    pingIcon.Icon = goodIcon /*connectedIcon*/;
                                else
                                    pingIcon.Icon = weakIcon;

                                // Make tray icon alt text say success
                                pingIcon.Text = "8.8.8.8 ping success! (" + reply.RoundtripTime + " ms)";
                                // Make conext menu item say success
                                roundTripTime.Text = "Latest ping: " + reply.RoundtripTime + " ms";

                                // Sleep for 1000ms (1 second)
                                Thread.Sleep(1000);
                            } else
                            {
                                // Change the Tray icon to disconnected
                                pingIcon.Icon = noIcon /*disconnectedIcon*/;
                                // Make tray icon alt text say timeout
                                pingIcon.Text = "8.8.8.8 ping timeout.";
                                // Make context menu item say timeout
                                roundTripTime.Text = "Latest ping: N/A ms (> 2000 ms)";
                            }
                        }
                        catch (PingException pe)
                        {
                            // Change the Tray icon to disconnected
                            pingIcon.Icon = noIcon /*disconnectedIcon*/;
                            // Make tray icon alt text say failure
                            pingIcon.Text = "8.8.8.8 ping failure.";
                            // Make context menu item say failure
                            roundTripTime.Text = "Latest ping: N/A ms (exception)";
                        }
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
