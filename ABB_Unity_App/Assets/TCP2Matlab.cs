using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using QFramework;
using UnityEngine;

public class TCP2Matlab : MonoBehaviour, ICanRegisterEvent
{
    [SerializeField] main_ui_control main;

    TcpClient send2Matlab;
    String Host = "localhost";
    Int32 send2Port = 55000;

    TcpClient receiveSocket;
    Int32 receivePort = 65000;

    void Awake() 
    {
        this.RegisterEvent<SendTCP2MatlabEvent>(OnSendTCP2Matlab).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    void Start() 
    {
        send2Matlab = new TcpClient(Host, send2Port);
        receiveSocket = new TcpClient(Host, receivePort);
    }

    void OnSendTCP2Matlab(SendTCP2MatlabEvent e) 
    {

        //    Byte[] sendBytes = Encoding.UTF8.GetBytes("A" + GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[0] + "B"
        //+ GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[1] + "C" + GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[2] + "D"
        //+ GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[0] + "E" + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[1] + "F"
        //+ GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[2] + "G" + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[3] + "H"
        //+ GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[4] + "I" + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[5] + "J" + "\n");

        //send2Matlab.GetStream().Write(sendBytes, 0, sendBytes.Length);

        string contentStr = e.content;
        
        Debug.Log($"[{nameof(TCP2Matlab)}] Sending:");
        Debug.Log(contentStr);

    //    Byte[] sendBytes = Encoding.UTF8.GetBytes("A" + GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[0] + "B"
    //+ GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[1] + "C" + GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[2] + "D"
    //+ GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[0] + "E" + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[1] + "F"
    //+ GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[2] + "G" + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[3] + "H"
    //+ GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[4] + "I" + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[5] + "J" + "\n");

        Byte[] sendBytes = Encoding.UTF8.GetBytes(contentStr + "\n");



        if (send2Matlab != null)
        {
            Debug.Log($"[{nameof(TCP2Matlab)}] I am sending: {sendBytes} ");
            send2Matlab.GetStream().Write(sendBytes, 0, sendBytes.Length);


                if (receiveSocket == null) return;

                NetworkStream stream;

                float time = Time.time;

                while (true)
                {
                    if(Time.time - time > 2)
                    {
                    return;
                    }
                    

                    stream = receiveSocket.GetStream();
                    if (stream != null) break;
                    
                }

                byte[] receiveBuffer = new byte[10000];
                int bytesRead = stream.Read(receiveBuffer, 0, receiveBuffer.Length);
                string receivedData = Encoding.UTF8.GetString(receiveBuffer, 0, bytesRead);

            Debug.Log($"[{nameof(TCP2Matlab)}] Receive¡¡{receivedData}");

                // Parse received data (assuming the joint angles are floats)
                string[] jointAngles = receivedData.Split(new[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G' }, StringSplitOptions.RemoveEmptyEntries);

                float[] receivedJoints = new float[6];

                for (int i = 0; i < 6; i++)
                {
                    receivedJoints[i] = float.Parse(jointAngles[i]);
                }

                main.SendNewTargetCommand(receivedJoints);



        }
        else
        {
            Debug.LogError($"[{nameof(TCP2Matlab)}] {nameof(send2Matlab)} is null");
        }
    }

    public IArchitecture GetArchitecture()
    {
        return Abb.Interface;
    }
}


public class SendTCP2MatlabEvent 
{
    public string content;

    public SendTCP2MatlabEvent(string contextStr) 
    {
        this.content = contextStr;
    }
}



public class SendTCP2MatlabCommand : AbstractCommand
{
    string content;

    public SendTCP2MatlabCommand(string str) 
    {
        this.content = str;
    }

    protected override void OnExecute()
    {
        if (content != null && content != "")
        {
            this.SendEvent(new SendTCP2MatlabEvent(content));
        }
        else 
        {
            Debug.LogError($"[{nameof(TCP2Matlab)}] You are sending void string content");
        }
    }
}


public class SendTCP_X_Command: AbstractCommand
{
    string contentStr;

    public SendTCP_X_Command(float delta) 
    {
        float temp = GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[0].ToFloat() + delta;
        contentStr = "A" + temp + "B"
    + GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[1] + "C" + GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[2] + "D"
    + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[0] + "E" + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[1] + "F"
    + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[2] + "G" + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[3] + "H"
    + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[4] + "I" + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[5] + "J";

    }


    protected override void OnExecute()
    {
        this.SendCommand(new SendTCP2MatlabCommand(contentStr));
    }
}

public class SendTCP_Y_Command : AbstractCommand
{
    string contentStr;

    public SendTCP_Y_Command(float delta)
    {
        float temp = GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[1].ToFloat() + delta;
        contentStr = "A" + GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[0] + "B"
    + temp.ToString() + "C" + GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[2] + "D"
    + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[0] + "E" + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[1] + "F"
    + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[2] + "G" + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[3] + "H"
    + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[4] + "I" + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[5] + "J";

    }

    protected override void OnExecute()
    {
        this.SendCommand(new SendTCP2MatlabCommand(contentStr));
    }
}

public class SendTCP_Z_Command : AbstractCommand
{
    string contentStr;

    public SendTCP_Z_Command(float delta)
    {
        float temp = GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[2].ToFloat() + delta;
        contentStr = "A" + GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[0] + "B"
    + GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[1] + "C" + temp.ToString() + "D"
    + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[0] + "E" + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[1] + "F"
    + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[2] + "G" + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[3] + "H"
    + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[4] + "I" + GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[5] + "J";

    }

    protected override void OnExecute()
    {
        this.SendCommand(new SendTCP2MatlabCommand(contentStr));
    }
}