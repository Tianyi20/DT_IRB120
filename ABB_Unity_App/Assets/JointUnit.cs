using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class JointUnit : MonoBehaviour
{
    public int index;

    public UnityEvent JointValueChanged;

    bool isPCControl = false;

    private IEnumerator delayCoroutine;

    [SerializeField] TMP_Text text_CurrentValue;
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text label;

    [SerializeField] TMP_Text text_Min;
    [SerializeField] TMP_Text text_Max;


    float currentReportValue = 0f;

    [SerializeField] float minValue = -90f;
    [SerializeField] float maxValue = 90f;

    public BindableProperty<float> value = new BindableProperty<float>(0f);

    private void Awake()
    {
        currentReportValue = slider.value;

        text_Min.text = minValue.ToString();
        text_Max.text = maxValue.ToString();

        //Receive incoming value and show on slider
        value.Register(v =>
        {

            // Map the incoming value from [-90, 90] to [0, 1]
            float mappedValue = Mathf.InverseLerp(minValue, maxValue, v);
            //slider.value = v;
            slider.SetValueWithoutNotify(mappedValue);
        
        });



        slider.onValueChanged.AddListener(v =>
        {
            isPCControl = true;

            // Map the slider value from [0, 1] to [-90, 90]
            float mappedValue = Mathf.Lerp(minValue, maxValue, v);

            // Set the mapped value to the robot
            value.SetValueWithoutEvent(mappedValue);

            // Display the mapped value in the text_CurrentValue
            text_CurrentValue.text = $"{mappedValue:00}бу";

            // Reset the delayCoroutine if it's already running
            if (delayCoroutine != null)
            {
                StopCoroutine(delayCoroutine);
            }

            // Start a new coroutine to delay setting isPCControl to false
            delayCoroutine = DelayedSetPCControlFalse();
            StartCoroutine(delayCoroutine);
        });

        InvokeRepeating(nameof(ReportValue), 0f, 1f);
        InvokeRepeating(nameof(CheckValue),0f,0.1f);
    }

    private void ReportValue() 
    {
        if (!isPCControl) return;

        if (slider.value != currentReportValue)
        {
            Debug.Log($"[{nameof(JointUnit)}] Report as {slider.value} {currentReportValue}");

            JointValueChanged?.Invoke();
            currentReportValue = slider.value;
        }
    }

    private void CheckValue() 
    {
        if (isPCControl) return;



        var v = GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[index].ToFloat();
        //Debug.Log($"[{nameof(JointUnit)}] {v} ");

        text_CurrentValue.text  = $"{v:00}бу";

        value.Value = v;
    }

    public void SetLabel(string str)
    {
        label.text = str;
    }

    private IEnumerator DelayedSetPCControlFalse()
    {
        yield return new WaitForSeconds(0.5f);
        isPCControl = false;
        ReportValue();
    }

    // Additional method to stop the coroutine if needed
    private void StopDelayCoroutine()
    {
        if (delayCoroutine != null)
        {
            StopCoroutine(delayCoroutine);
        }
    }
}
