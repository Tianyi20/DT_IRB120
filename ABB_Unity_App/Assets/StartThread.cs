
using System;
using System.Collections;
using System.Collections.Generic;
using ABB_RWS_Data_Processing_XML;
using UnityEngine;

public class StartThread : MonoBehaviour
{
    private void Awake()
    {
        // Initialization {Robot Web Services ABB}
        //  Stream Data:
        ABB_Data.ip_address = "192.168.125.1";
        //  The target of reading the data: jointtarget / robtarget
        ABB_Data.xml_target = "robtarget";
        //  Communication speed (ms)
        ABB_Data.time_step = 12;
        //  Joint Targets
        ABB_Data.J_Orientation = "value=[" +
                                 "[0,0,0,0,0,90],[0,0,0,0,0,0]" +

                                 "]";

        // Start Stream {ABB Robot Web Services - XML}
        ABB_Stream ABB_Stream_Robot_XML = new ABB_Stream();
        ABB_Stream_Robot_XML.Start();

        //Console.WriteLine("[INFO] Stop (y):");
        // Stop communication
        //string stop_rs = Convert.ToString(Console.ReadLine());

        //if (stop_rs == "y")
        //{
        //    // Destroy ABB {Stream}
        //    ABB_Stream_Robot_XML.Destroy();

        //    // Application quit
        //    Environment.Exit(0);
        //}
    }
}
