using System;
using System.Collections;
using System.Collections.Generic;
using ABB_RWS_Data_Processing_XML;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

public class ABB_Stream
{
    // Initialization of Class variables
    //  Thread
    private Thread robot_thread = null;
    private bool exit_thread = false;
    // Robot Web Services (RWS): XML Communication
    private CookieContainer c_cookie = new CookieContainer();
    private NetworkCredential n_credential = new NetworkCredential("Default User", "robotics");

    // Control state
    private int main_state = 0;

    public void ABB_Stream_Thread()
    {
        try
        {
            // Initialization timer
            var t = new Stopwatch();

            while (exit_thread == false)
            {
                // t_{0}: Timer start.
                t.Start();

                switch (main_state)
                {
                    case 0:
                        {
                            // State: Reset PP to main

                            // Create data to send
                            string post_data = "";

                            // Control data: Sending data to the robot controller
                            Stream result = Util.Control_Data(ABB_Data.ip_address, "execution?action=resetpp", post_data);

                            main_state = 1;
                        }
                        break;

                    case 1:
                        {
                            // State: Set Joint Targets

                            // Control data: Sending data to the robot controller
                            Stream result = Util.Control_Data(ABB_Data.ip_address, "symbol/data/RAPID/T_ROB1/J_Orientation_Target?action=set", ABB_Data.J_Orientation);

                            main_state = 2;
                        }
                        break;

                    case 2:
                        {
                            // State: Start Rapid

                            // Create data to send
                            string post_data = "regain=continue&execmode=continue&cycle=forever&condition=none&stopatbp=disabled&alltaskbytsp=false";

                            // Control data: Sending data to the robot controller
                            Stream result = Util.Control_Data(ABB_Data.ip_address, "execution?action=start", post_data);

                            main_state = 3;
                        }
                        break;



                    case 3:
                        {
                            // State: Stop

                            // Create data to send
                            string post_data = "stopmode=stop&usetsp=normal";

                            // Control data: Sending data to the robot controller
                            Util.Control_Data(ABB_Data.ip_address, "execution?action=stop", post_data);

                            main_state = 5;
                        }
                        break;

                    case 5:
                        {
                            // State: Empty
                        }
                        break;
                }

                Console.WriteLine("Current State: {0}", main_state);

                // t_{1}: Timer stop.
                t.Stop();

                // Recalculate the time: t = t_{1} - t_{0} -> Elapsed Time in milliseconds
                if (t.ElapsedMilliseconds < ABB_Data.time_step)
                {
                    Thread.Sleep(ABB_Data.time_step - (int)t.ElapsedMilliseconds);
                }

                // Reset (Restart) timer.
                t.Restart();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Communication Problem: {0}", e);
        }
    }

    public void Start()
    {
        exit_thread = false;
        // Start a thread to stream ABB Robot
        robot_thread = new Thread(new ThreadStart(ABB_Stream_Thread));
        robot_thread.IsBackground = true;
        robot_thread.Start();
    }
    public void Stop()
    {
        exit_thread = true;
        // Stop a thread
        Thread.Sleep(100);
    }
    public void Destroy()
    {
        // Stop a thread (Robot Web Services communication)
        Stop();
        Thread.Sleep(100);
    }
}
