﻿// ------------------------------------------------------------------------------------------------------------------------//
// ----------------------------------------------------- LIBRARIES --------------------------------------------------------//
// ------------------------------------------------------------------------------------------------------------------------//

// -------------------- Unity -------------------- //
using UnityEngine;

public class irb120_link5 : MonoBehaviour
{
    void FixedUpdate()
    {
        transform.localEulerAngles = new Vector3(0f, (-1) * float.Parse(GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[4]), 0f);

    }
    void OnApplicationQuit()
    {
        Destroy(this);
    }
}
