using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using TMPro;
using UnityEngine;



public static class Count_D0

{
    public static int Second_DO_thread = 0;
    public static int Second_Joint_thread = 0;
    public static int Second_TCP_thread = 0;

}


public class EventMonitor : MonoBehaviour,ICanSendEvent
{
    string[] currentDO = new string[6] { "0", "0", "0", "0", "0", "0" };
    void Start() 
    {
        InvokeRepeating(nameof(ResetPackageCount_DO),0f,1f);
        InvokeRepeating(nameof(ResetPackageCount_TCP), 0f, 1f);
        InvokeRepeating(nameof(ResetPackageCount_Joint), 0f, 1f);

    }

    void ResetPackageCount_DO() 
    {
        Debug.Log($"[{nameof(EventMonitor)}] {nameof(GlobalVariables_RWS_client.resultCountPerSecond_DO_thread)}is {GlobalVariables_RWS_client.resultCountPerSecond_DO_thread}");
        Count_D0.Second_DO_thread = GlobalVariables_RWS_client.resultCountPerSecond_DO_thread;
        GlobalVariables_RWS_client.resultCountPerSecond_DO_thread = 0;
    }

    void ResetPackageCount_TCP()
    {
        Debug.Log($"[{nameof(EventMonitor)}] {nameof(GlobalVariables_RWS_client.resultCountPerSecond_TCP_thread)}is {GlobalVariables_RWS_client.resultCountPerSecond_TCP_thread}");
        Count_D0.Second_TCP_thread = GlobalVariables_RWS_client.resultCountPerSecond_TCP_thread;
        GlobalVariables_RWS_client.resultCountPerSecond_TCP_thread = 0;
    }

    void ResetPackageCount_Joint()
    {
        Debug.Log($"[{nameof(EventMonitor)}] {nameof(GlobalVariables_RWS_client.resultCountPerSecond_Joint_thread)}is {GlobalVariables_RWS_client.resultCountPerSecond_Joint_thread}");
        Count_D0.Second_Joint_thread = GlobalVariables_RWS_client.resultCountPerSecond_Joint_thread;
        GlobalVariables_RWS_client.resultCountPerSecond_Joint_thread = 0;
    }




    void Update()
    {
        //Debug.Log($"[{nameof(EventMonitor)}] {nameof(GlobalVariables_RWS_client.resultCountPerSecond_DO_thread)}is {GlobalVariables_RWS_client.resultCountPerSecond_DO_thread}");

        string[] incoming = { GlobalVariables_RWS_client.robotiosystemDo[0],
            GlobalVariables_RWS_client.robotiosystemDo[1],
            GlobalVariables_RWS_client.robotiosystemDo[2],
            GlobalVariables_RWS_client.robotiosystemDo[3],
            GlobalVariables_RWS_client.robotiosystemDo[4],
            GlobalVariables_RWS_client.robotiosystemDo[5]
        };

        //Debug.Log($"Monitor cur: {currentDO[0]}{currentDO[1]}{currentDO[2]}{currentDO[3]}{currentDO[4]}{currentDO[5]}");
        //Debug.Log($"Monitor  incoming {incoming[0]}{incoming[1]}{incoming[2]}{incoming[3]}{incoming[4]}{incoming[5]}");
        if (incoming[0] != currentDO[0])
        {
            //Debug.Log($"{incoming[0]} {currentDO[0]} {nameof(MagEvent)} IsOn: {incoming[0] == "1"}");
            this.SendEvent(new MagEvent { IsOn = (incoming[0] == "1") });
        }

        if (incoming[1] != currentDO[1])
        {
            //Debug.Log($"{incoming[1]} {currentDO[1]} {nameof(ConveyerEvent)} IsOn: {incoming[1] == "1"}");
            this.SendEvent(new ConveyerEvent { IsOn = (incoming[1] == "1") });
        }

        if (incoming[2] != currentDO[2])
        {
            //Debug.Log($"{incoming[2]} {currentDO[2]} {nameof(StackingEvent)} IsOn: {incoming[2] == "1"}");
            this.SendEvent(new StackingEvent { IsOn = (incoming[2] == "1"), shapeType = ShapeType.Sphere });
        }

        if (incoming[3] != currentDO[3])
        {

            //Debug.Log($"{incoming[3]} {currentDO[3]} {nameof(StackingEvent)} IsOn: {incoming[3] == "1"}");
            this.SendEvent(new StackingEvent { IsOn = (incoming[3] == "1"), shapeType = ShapeType.Cube });
        }

        if (incoming[4] != currentDO[4])
        {

            //Debug.Log($"{incoming[4]} {currentDO[4]} {nameof(StackingEvent)} IsOn: {incoming[4] == "1"}");
            this.SendEvent(new StackingEvent { IsOn = (incoming[4] == "1"), shapeType = ShapeType.Rectangle });
        }

        if (incoming[5] != currentDO[5])
        {

            //Debug.Log($"{incoming[5]} {currentDO[5]} {nameof(UltraRedEvent)} IsOn: {incoming[5] == "1"}");
            this.SendEvent(new UltraRedEvent { IsOn = (incoming[5] == "1") });
        }

        currentDO = incoming;
    }



    public IArchitecture GetArchitecture()
    {
        return Abb.Interface;
    }
}
