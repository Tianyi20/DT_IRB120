/****************************************************************************
MIT License
Copyright(c) 2020 Roman Parak
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*****************************************************************************
Author   : Roman Parak
Email    : Roman.Parak @outlook.com
Github   : https://github.com/rparak
File Name: main_ui_control.cs
****************************************************************************/

// ------------------------------------------------------------------------------------------------------------------------ //
// ----------------------------------------------------- LIBRARIES -------------------------------------------------------- //
// ------------------------------------------------------------------------------------------------------------------------ //

// -------------------- System -------------------- //
using System;
using System.Text;
using System.Net;
using System.Xml;
using System.Globalization;
using System.Text;
// -------------------- Unity -------------------- //
using UnityEngine;
using UnityEngine.UI;
// --------------------- TM ---------------------- //
using TMPro;
using System.IO;
using System.Threading;
using ABB_RWS_Data_Processing_XML;
using UnityEngine.Networking;
using System.Collections;
using System.Net.Http;
using UnityEditor.PackageManager;
using System.Threading.Tasks;
using static UnityEngine.Networking.UnityWebRequest;

public class main_ui_control : MonoBehaviour
{
    // -------------------- GameObject -------------------- //
    public GameObject camera_obj;
    // -------------------- Image -------------------- //
    public Image connection_panel_img, diagnostic_panel_img;
    public Image connection_info_img;
    // -------------------- TMP_InputField -------------------- //
    public TMP_InputField ip_address_txt;
    // -------------------- Float -------------------- //
    private float ex_param = 100f;
    // -------------------- TextMeshProUGUI -------------------- //
    public TextMeshProUGUI position_x_txt, position_y_txt, position_z_txt;
    public TextMeshProUGUI position_q1_txt, position_q2_txt, position_q3_txt, position_q4_txt;
    public TextMeshProUGUI position_j1_txt, position_j2_txt, position_j3_txt;
    public TextMeshProUGUI position_j4_txt, position_j5_txt, position_j6_txt;
    public TextMeshProUGUI connectionInfo_txt;

    public TextMeshProUGUI Refreshing_rate_DO;
    public TextMeshProUGUI Refreshing_rate_TCP;
    public TextMeshProUGUI Refreshing_rate_Joint;


    public TextMeshProUGUI Grab_magnet;
    public TextMeshProUGUI Conveyer;
    public TextMeshProUGUI Stacking_event;
    public TextMeshProUGUI UltraRed;

    public TextMeshProUGUI TCP_X;
    public TextMeshProUGUI TCP_Y;
    public TextMeshProUGUI TCP_Z;




    private HttpClient client;
    private HttpClientHandler handler;
    private CookieContainer cookies;
    // ------------------------------------------------------------------------------------------------------------------------ //
    // ------------------------------------------------ INITIALIZATION {START} ------------------------------------------------ //
    // ------------------------------------------------------------------------------------------------------------------------ //
    void Start()
    {
        // Connection information {image} -> Connect/Disconnect
        connection_info_img.GetComponent<Image>().color = new Color32(255, 0, 48, 50);
        // Connection information {text} -> Connect/Disconnect
        connectionInfo_txt.text = "Disconnect";

        // Panel Initialization -> Connection/Diagnostic Panel
        connection_panel_img.transform.localPosition = new Vector3(1215f + (ex_param), 0f, 0f);
        diagnostic_panel_img.transform.localPosition = new Vector3(780f + (ex_param), 0f, 0f);

        // Position {Cartesian} -> X..Z
        position_x_txt.text = "0.00";
        position_y_txt.text = "0.00";
        position_z_txt.text = "0.00";
        // Position {Rotation} -> Quaternion(1..4)
        position_q1_txt.text = "0.00000000";
        position_q2_txt.text = "0.00000000";
        position_q3_txt.text = "0.00000000";
        position_q4_txt.text = "0.00000000";
        // Position Joint -> 1 - 6
        position_j1_txt.text = "0.00";
        position_j2_txt.text = "0.00";
        position_j3_txt.text = "0.00";
        position_j4_txt.text = "0.00";
        position_j5_txt.text = "0.00";
        position_j6_txt.text = "0.00";

        // Robot IP Address
        ip_address_txt.text = "192.168.125.1";

        cookies = new CookieContainer();
        handler = new HttpClientHandler()
        {
            Credentials = new NetworkCredential("Default User", "robotics"),
            Proxy = null,
            UseCookies = true,
            CookieContainer = cookies
        };

        client = new HttpClient(handler);
    }

    // ------------------------------------------------------------------------------------------------------------------------ //
    // ------------------------------------------------ MAIN FUNCTION {Cyclic} ------------------------------------------------ //
    // ------------------------------------------------------------------------------------------------------------------------ //
    void Update()
    {
        // Robot IP Address (Read) -> XML thread function
        GlobalVariables_Main_Control.abb_irb_rws_xml_config[0]  = ip_address_txt.text;
        // Robot IP Address (Write) -> JSON thraed function
        GlobalVariables_Main_Control.abb_irb_rws_json_config[0] = ip_address_txt.text;


        // ------------------------ Connection Information ------------------------//
        // If the button (connect/disconnect) is pressed, change the color and text
        if (GlobalVariables_Main_Control.connect == true)
        {
            // green color
            connection_info_img.GetComponent<Image>().color = new Color32(135, 255, 0, 50);
            connectionInfo_txt.text = "Connect";
        }
        else if(GlobalVariables_Main_Control.disconnect == true)
        {
            // red color
            connection_info_img.GetComponent<Image>().color = new Color32(255, 0, 48, 50);
            connectionInfo_txt.text = "Disconnect";
        }

        // ------------------------ Cyclic read parameters {diagnostic panel} ------------------------ //
        // Position {Cartesian} -> X..Z
        position_x_txt.text = GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[0];
        TCP_X.text = GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[0];
        position_y_txt.text = GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[1];
        TCP_Y.text = GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[1];
        position_z_txt.text = GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[2];
        TCP_Z.text = GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[2];
        // Position {Rotation} -> Quaternion(q1..q4)
        position_q1_txt.text = GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[3];
        position_q2_txt.text = GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[4];
        position_q3_txt.text = GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[5];
        position_q4_txt.text = GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[6];
        // Position Joint -> 1 - 6
        position_j1_txt.text = GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[0].ToString();
        position_j2_txt.text = GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[1].ToString();
        position_j3_txt.text = GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[2].ToString();
        position_j4_txt.text = GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[3].ToString();
        position_j5_txt.text = GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[4].ToString();
        position_j6_txt.text = GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[5].ToString();

        if(!(Count_D0.Second_DO_thread == 0|| Count_D0.Second_TCP_thread == 00 || Count_D0.Second_Joint_thread == 00))
        {

            //Refreshing Rate 
            Refreshing_rate_DO.text = (1000 / Count_D0.Second_DO_thread).ToString();
            Refreshing_rate_TCP.text = (1000 / Count_D0.Second_TCP_thread).ToString();
            Refreshing_rate_Joint.text = (1000 / Count_D0.Second_Joint_thread).ToString();
        }

        if (GlobalVariables_RWS_client.robotiosystemDo[0] == "0")
        {
            Grab_magnet.text = "Off";
        }
        else
        {
            Grab_magnet.text = "On";
        }
        ///////////////传送带DO GUI
        if(GlobalVariables_RWS_client.robotiosystemDo[1] == "0")
        {
            Conveyer.text = "Off";
        }
        else
        {
            Conveyer.text = "On";
        }
        ////////////////////当前码垛状态 GUI
        if (GlobalVariables_RWS_client.robotiosystemDo[2] == "1")
        {
            Stacking_event.text = "Sphere Stacking";

        }
        else if(GlobalVariables_RWS_client.robotiosystemDo[3] == "1") 
        {
            Stacking_event.text = "Cube Stacking";
        }
        else if (GlobalVariables_RWS_client.robotiosystemDo[4] == "1")
        {
            Stacking_event.text = "Rectangle Stacking";
        }
        else
        {
            Stacking_event.text = "None";
        }

        //////////// 当前的红外感情GUI
        if(GlobalVariables_RWS_client.robotiosystemDo[5] == "1")
        {
            UltraRed.text = "Active";
        }
        else
        {
            UltraRed.text = "None";
        }



    }

    // ------------------------------------------------------------------------------------------------------------------------//
    // -------------------------------------------------------- FUNCTIONS -----------------------------------------------------//
    // ------------------------------------------------------------------------------------------------------------------------//

    // -------------------- Destroy Blocks -------------------- //
    void OnApplicationQuit()
    {
        // Destroy all
        Destroy(this);
    }

    private void OnDestroy()
    {
        client?.Dispose();
        handler?.Dispose();
    }

    // -------------------- Connection Panel -> Visible On -------------------- //
    public void TaskOnClick_ConnectionBTN()
    {
        // visible on
        connection_panel_img.transform.localPosition = new Vector3(0f, 0f, 0f);
        // visible off
        diagnostic_panel_img.transform.localPosition = new Vector3(780f + (ex_param), 0f, 0f);
    }

    // -------------------- Connection Panel -> Visible off -------------------- //
    public void TaskOnClick_EndConnectionBTN()
    {
        connection_panel_img.transform.localPosition = new Vector3(1215f + (ex_param), 0f, 0f);
    }

    // -------------------- Diagnostic Panel -> Visible On -------------------- //
    public void TaskOnClick_DiagnosticBTN()
    {
        // visible on
        diagnostic_panel_img.transform.localPosition = new Vector3(0f, 0f, 0f);
        // visible off
        connection_panel_img.transform.localPosition = new Vector3(780f + (ex_param), 0f, 0f);
    }

    // -------------------- Diagnostic Panel -> Visible Off -------------------- //
    public void TaskOnClick_EndDiagnosticBTN()
    {
        diagnostic_panel_img.transform.localPosition = new Vector3(1215f + (ex_param), 0f, 0f);
    }

    // -------------------- Camera Position -> Right -------------------- //
    public async void TaskOnClick_CamViewRBTN() //////////////急停stop
    {

        string post_data = "";
        var result = await ControlDataAsync(ABB_Data.ip_address, "execution?action=stop", post_data);
        
    }

    public async void SendNewTargetCommand(float[] joints)
    {
        if (joints == null || joints.Length != 6)
        {
            return;
        }

        string post_data = "";
        var result = await ControlDataAsync(ABB_Data.ip_address, "execution?action=stop", post_data);




        result = await ControlDataAsync(ABB_Data.ip_address, "execution?action=resetpp", post_data);



        ABB_Data.J_Orientation = "value=[" +
                                     $"[{joints[0]},{joints[1]},{joints[2]},{joints[3]},{joints[4]},{joints[5]}],[0,0,0,0,0,0]" +

                                     "]";
        result = await ControlDataAsync(ABB_Data.ip_address, "symbol/data/RAPID/T_ROB1/J_Orientation_Target?action=set", ABB_Data.J_Orientation);



        post_data = "regain=continue&execmode=continue&cycle=forever&condition=none&stopatbp=disabled&alltaskbytsp=false";
        result = await ControlDataAsync(ABB_Data.ip_address, "execution?action=start", post_data);
    }


    // -------------------- Camera Position -> Left -------------------- //
    public async void TaskOnClick_CamViewLBTN()/////////重新开始一切工作
    {
        


        string post_data = "";
        var result = await ControlDataAsync(ABB_Data.ip_address, "execution?action=stop", post_data);

        

 
         result =  await ControlDataAsync(ABB_Data.ip_address, "execution?action=resetpp", post_data);

        

        ABB_Data.J_Orientation = "value=[" +
                                     "[0.000440569,0.00771831,-0.00510078,0,89.9999,0],[0,0,0,0,0,0]" +

                                     "]";
        result = await ControlDataAsync(ABB_Data.ip_address, "symbol/data/RAPID/T_ROB1/J_Orientation_Target?action=set", ABB_Data.J_Orientation);
        


        post_data = "regain=continue&execmode=continue&cycle=forever&condition=none&stopatbp=disabled&alltaskbytsp=false";
         result = await ControlDataAsync(ABB_Data.ip_address, "execution?action=start", post_data);
        

        //post_data = "stopmode=stop&usetsp=normal";
        //await ControlDataAsync(ABB_Data.ip_address, "execution?action=stop", post_data);
    }

    // -------------------- Camera Position -> Home (in front) -------------------- //
    public void TaskOnClick_CamViewHBTN()
    {
        
        camera_obj.transform.localPosition = new Vector3(-1.5f, 2.2f, -3.5f);
        camera_obj.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
    }

    // -------------------- Camera Position -> Top -------------------- //
    public async void TaskOnClick_CamViewTBTN() /////////////////继续工作
    {
        string post_data = "";
        post_data = "regain=continue&execmode=continue&cycle=forever&condition=none&stopatbp=disabled&alltaskbytsp=false";
        var result = await ControlDataAsync(ABB_Data.ip_address, "execution?action=start", post_data);
  
    }

    // -------------------- Connect Button -> is pressed -------------------- //
    public void TaskOnClick_ConnectBTN()
    {
        GlobalVariables_Main_Control.connect    = true;
        GlobalVariables_Main_Control.disconnect = false;
    }

    // -------------------- Disconnect Button -> is pressed -------------------- //
    public void TaskOnClick_DisconnectBTN()
    {
        GlobalVariables_Main_Control.connect    = false;
        GlobalVariables_Main_Control.disconnect = true;
    }




    public async Task<String> ControlDataAsync(string host, string target, string value)
    {
        var url = new Uri($"http://{host}/rw/rapid/{target}");

        var content = new StringContent(value, Encoding.ASCII, "application/x-www-form-urlencoded");
        
        HttpResponseMessage response = await client.PostAsync(url, content);

        Debug.Log(response.StatusCode);

        // Ensure the request was successful
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

}
