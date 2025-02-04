using System.Collections;
using System.Collections.Generic;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TCP_Panel : MonoBehaviour,ICanSendCommand
{
    [SerializeField] float tick = 1f;

    [SerializeField] TMP_InputField input_Scaler;

    [SerializeField] TCP_TupleUnit[] TCPUnits;

    private void Awake()
    {
        input_Scaler.text = 1.ToString();

        TCPUnits[0].btn_Plus.onClick.AddListener(() =>
        {
            this.SendCommand(new SendTCP_X_Command(GetParsedScaler()*tick));
        });
        TCPUnits[0].btn_Minus.onClick.AddListener(() =>
        {
            this.SendCommand(new SendTCP_X_Command(- GetParsedScaler() * tick));
        });
        TCPUnits[1].btn_Plus.onClick.AddListener(() =>
        {
            this.SendCommand(new SendTCP_Y_Command(GetParsedScaler() * tick));
        });
        TCPUnits[1].btn_Minus.onClick.AddListener(() =>
        {
            this.SendCommand(new SendTCP_Y_Command(- GetParsedScaler() * tick));
        });
        TCPUnits[2].btn_Plus.onClick.AddListener(() =>
        {
            this.SendCommand(new SendTCP_Z_Command(GetParsedScaler() * tick));
        });
        TCPUnits[2].btn_Minus.onClick.AddListener(() =>
        {
            this.SendCommand(new SendTCP_Z_Command(- GetParsedScaler() * tick));
        });
    }

    public float GetParsedScaler()
    {
        return input_Scaler.text.ToFloat();
    }

    public IArchitecture GetArchitecture()
    {
        return Abb.Interface;
    }
}
