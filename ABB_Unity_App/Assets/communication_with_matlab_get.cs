using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.IO;
using System.Text;



public class communication_with_matlab_get : MonoBehaviour
{
    // Start is called before the first frame update

    // Use this for initialization
    TcpClient mySocket;
    String Host = "localhost";
    Int32 Port = 65000;

    void Start()
    {
        try
        {
            mySocket = new TcpClient(Host, Port);

            Debug.Log("socket is set up");
        }
        catch (Exception e)
        {
            Debug.Log("Socket setup error: " + e);
        }

    }


    // ...
    void Update()
    {
      

        try
        {
            if (mySocket == null) return;

            // Receive joint angles from MATLAB
            NetworkStream stream = mySocket.GetStream();

            if (stream == null) return;
            byte[] receiveBuffer = new byte[10000];
            int bytesRead = stream.Read(receiveBuffer, 0, receiveBuffer.Length);
            string receivedData = Encoding.UTF8.GetString(receiveBuffer, 0, bytesRead);

            // Parse received data (assuming the joint angles are floats)
            string[] jointAngles = receivedData.Split(new[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G' }, StringSplitOptions.RemoveEmptyEntries);

            // Update the joint angles in your RobotScript or manipulate your robot as needed
            Inverse_kinmatic_IRB120.robotBaseRotLink_irb_joint_inverse_solution[0] = jointAngles[0];
            Inverse_kinmatic_IRB120.robotBaseRotLink_irb_joint_inverse_solution[1] = jointAngles[1];
            Inverse_kinmatic_IRB120.robotBaseRotLink_irb_joint_inverse_solution[2] = jointAngles[2];
            Inverse_kinmatic_IRB120.robotBaseRotLink_irb_joint_inverse_solution[3] = jointAngles[3];
            Inverse_kinmatic_IRB120.robotBaseRotLink_irb_joint_inverse_solution[4] = jointAngles[4];
            Inverse_kinmatic_IRB120.robotBaseRotLink_irb_joint_inverse_solution[5] = jointAngles[5];

            Debug.Log(Inverse_kinmatic_IRB120.robotBaseRotLink_irb_joint_inverse_solution[0]);

        }
        catch (Exception e)
        {
            Debug.Log("Socket error: " + e);
        }
    }
    // ...

}
