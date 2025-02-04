using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public static class ABB_IRB120_Joint_range
{
    // 定义每个关节的最小和最大工作范围，单位为度
    public static float Joint1Min = -165f;  // 关节1的最小工作范围
    public static float Joint1Max = 165f;   // 关节1的最大工作范围

    public static float Joint2Min = -110f;  // 关节2的最小工作范围
    public static float Joint2Max = 110f;   // 关节2的最大工作范围

    public static float Joint3Min = -90f;   // 关节3的最小工作范围
    public static float Joint3Max = 70f;    // 关节3的最大工作范围

    public static float Joint4Min = -160f;  // 关节4的最小工作范围
    public static float Joint4Max = 160f;   // 关节4的最大工作范围

    public static float Joint5Min = -120f;  // 关节5的最小工作范围
    public static float Joint5Max = 120f;   // 关节5的最大工作范围

    public static float Joint6Min = -400f;  // 关节6的最小工作范围
    public static float Joint6Max = 400f;   // 关节6的最大工作范围
}


public class SixAxis : MonoBehaviour
{
    [SerializeField] JointUnit[] jointUnits;

    float[] jointsValue = new float[6];

    main_ui_control main;

    private void Awake()
    {
        main = GetComponentInParent<main_ui_control>();

        for (int i = 0; i < jointUnits.Length; i++) 
        {
            jointUnits[i].index = i;
            jointUnits[i].SetLabel($"J{i+1}");
            jointUnits[i].JointValueChanged.AddListener(OnJointValueChanged);   
        }


    }

    void OnJointValueChanged() 
    {
        for (int i = 0; i < jointUnits.Length; i++)
        {
            jointsValue[i] = jointUnits[i].value.Value;
        }

        main.SendNewTargetCommand(jointsValue);
    }
}
