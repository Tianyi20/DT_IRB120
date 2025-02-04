using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.IO;
using System.Text;

public class communication_with_matlab : MonoBehaviour
{

    // Use this for initialization
    TcpClient mySocket;
    String Host = "localhost";
    Int32 Port = 55000;

    void Start()
    {


        mySocket = new TcpClient(Host, Port);

        Debug.Log("socket is set up");
    }

    // Update is called once per frame
    void Update()
    {


        try
        {
            if (mySocket == null) return;

            //Debug.Log(robotIns.B1Angle);
            //通过socket传递TCP和joint的值

            Byte[] sendBytes = Encoding.UTF8.GetBytes("A" + GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[0] + "B"
                + GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[1] + "C" + GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[2] + "D" 
                + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[0] + "E" + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[1] + "F" 
                + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[2] + "G" + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[3] + "H"
                + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[4] + "I" + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[5] +"J"+"\n");

            mySocket.GetStream().Write(sendBytes, 0, sendBytes.Length);
        }
        catch (Exception e)
        {
            Debug.Log("Socket error: " + e);
        }
    }
}


