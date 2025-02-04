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
File Name: abb_data_processing.cs
****************************************************************************/

// ------------------------------------------------------------------------------------------------------------------------//
// ----------------------------------------------------- LIBRARIES --------------------------------------------------------//
// ------------------------------------------------------------------------------------------------------------------------//

// -------------------- System -------------------- //
using System;
using System.Net;
using System.IO;
using System.Xml;
using System.Net.Http;
using System.Threading;
// -------------------- Unity -------------------- //
using UnityEngine;


// -------------------- Class {Global Variable} -> Main Control -------------------- //
public static class GlobalVariables_Main_Control
{
    // -------------------- Bool -------------------- //
    public static bool abb_irb_dt_enable_rws_xml, abb_irb_dt_enable_rws_json, abb_irb_dt_enable_rws_iosystem;
    public static bool connect, disconnect;
    // -------------------- String -------------------- //
    public static string[] abb_irb_rws_xml_config = new string[2];
    public static string[] abb_irb_rws_json_config = new string[2];
    public static string[] abb_irb_rws_iosystem = new string[2];

}


// -------------------- Class {Global Variable} -> Robot Web Services(RWS) Client -------------------- //
public static class GlobalVariables_RWS_client
    {
    // -------------------- Float -------------------- //
    public static string[] robotBaseRotLink_irb_joint = { "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0" };
    // -------------------- String -------------------- //
    public static string[] robotBaseRotLink_irb_cartes = new string[7] { "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0" };
    // DO 口初始化，分别代表DO_1:电磁铁 ； DO_2 ：传送带电机 ; DO_3 ： 原形工件码垛；DO_4：正方形工件码垛 ； DO_5 ：长方形工件码垛
    public static string[] robotiosystemDo = new string[6] { "0", "0", "0", "0", "0", "0" };

    public static int resultCountPerSecond_DO_thread = 0;
    public static int resultCountPerSecond_TCP_thread = 0;
    public static int resultCountPerSecond_Joint_thread = 0;

}

public static class Inverse_kinmatic_IRB120
{
    // -------------------- Float -------------------- //
    public static string[] TCP_send_need = { "0.0", "0.0", "0.0"};
    public static string[] robotBaseRotLink_irb_joint_inverse_solution = { "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0" };
}


public class abb_data_processing : MonoBehaviour
    {

        

        // -------------------- Thread -------------------- //
        private Thread rws_read_Thread_xml, rws_read_Thread_json, rws_read_Thread_iosystem;
        // -------------------- Vector3 -------------------- //
        Vector3 set_position_ABB_IRB120;
        // -------------------- CookieContainer -------------------- //
        static CookieContainer c_cookie = new CookieContainer();
        // -------------------- Network Credential -------------------- //
        static NetworkCredential n_credential = new NetworkCredential("Default User", "robotics");
        // -------------------- Stream -------------------- //
        Stream xml_irb_joint;
        // -------------------- XmlNode -------------------- //
        XmlNode joint1, joint2, joint3, joint4, joint5, joint6;
        // -------------------- Int -------------------- //
        private int main_abb_state = 0;

        // ------------------------------------------------------------------------------------------------------------------------//
        // ------------------------------------------------ INITIALIZATION {START} ------------------------------------------------//
        // ------------------------------------------------------------------------------------------------------------------------//
        void Start()
        {
            // ------------------------ Initialization { IRB Digital Twin {Control Robot} - RWS{Robot Web Services) JSON } 线程1用JSON格式来读取六个关节j1 j2 j3 j4 j5 j6------------------------//
            // Robot IP Address
            GlobalVariables_Main_Control.abb_irb_rws_xml_config[0] = "192.168.125.1";
            // Robot JSON Target
            GlobalVariables_Main_Control.abb_irb_rws_xml_config[1] = "/rw/rapid/tasks/T_ROB1/motion?resource=jointtarget&json=1";
            // Control -> Start {Read RWS data (XML)}
            GlobalVariables_Main_Control.abb_irb_dt_enable_rws_xml = true;
            // ------------------------ Initialization { IRB Digital Twin {Control Robot} - RWS{Robot Web Services) JSON }线程2用JSON格式来读取TCP坐标和 q1 q2 q3 q4 - ------------------------//
            // Robot IP Address
            GlobalVariables_Main_Control.abb_irb_rws_json_config[0] = "192.168.125.1";
            // Robot JSON Target
            GlobalVariables_Main_Control.abb_irb_rws_json_config[1] = "/rw/rapid/tasks/T_ROB1/motion?resource=robtarget&json=1";
            // ------------------------ Initialization { IRB Digital Twin {Control Robot} - RWS{Robot Web Services) JSON }线程3用JSON格式来读取DO_1到DO_5口的信息 - ------------------------/
            // Control -> Start {Read RWS data (JSON)}
            GlobalVariables_Main_Control.abb_irb_dt_enable_rws_json = true;
            GlobalVariables_Main_Control.abb_irb_rws_iosystem[0] = "192.168.125.1";
            // Robot JSON Target
            GlobalVariables_Main_Control.abb_irb_rws_iosystem[1] = "/rw/iosystem/signals?json=1";
            GlobalVariables_Main_Control.abb_irb_dt_enable_rws_iosystem = true;

        }

        // ------------------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------------ MAIN FUNCTION {Cyclic} ------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------------------ //
        private void Update()
        {            //这一串代码两个switch的意思是执行一次读取，然后检查和机器人的通信有没有切断，没有切断就继续读数据，有切断就abort程序
            switch (main_abb_state)
            {
                case 0:
                    {
                        // ------------------------ Wait State {Disconnect State} ------------------------//
                        if (GlobalVariables_Main_Control.connect == true)
                        {
                            // Control -> Start {Read RWS XML data}
                            GlobalVariables_Main_Control.abb_irb_dt_enable_rws_xml = true;
                            // Control -> Start {Read RWS JSON data}
                            GlobalVariables_Main_Control.abb_irb_dt_enable_rws_json = true;

                            // ABB IRB 120 Control -> Start {RWS JSON}
                            // ------------------------ Threading Block 1 { RWS Read Data {JSON} } ------------------------//
                            rws_read_Thread_xml = new Thread(() => RWS_Service_read_json_thread_Joint("http://" + GlobalVariables_Main_Control.abb_irb_rws_xml_config[0], GlobalVariables_Main_Control.abb_irb_rws_xml_config[1]));
                            rws_read_Thread_xml.IsBackground = true;
                            rws_read_Thread_xml.Start();

                            // ABB IRB 120 Control -> Start {RWS JSON}
                            // ------------------------ Threading Block 2 { RWS Read Data {JSON} } ------------------------//
                            rws_read_Thread_json = new Thread(() => RWS_Service_read_json_thread_function_TCP("http://" + GlobalVariables_Main_Control.abb_irb_rws_json_config[0], GlobalVariables_Main_Control.abb_irb_rws_json_config[1]));
                            rws_read_Thread_json.IsBackground = true;
                            rws_read_Thread_json.Start();

                            // ABB IRB 120 Control -> Start {RWS JSON}
                            // ------------------------ Threading Block 3{ RWS Read Data {JSON} } ------------------------//
                            rws_read_Thread_iosystem = new Thread(() => { RWS_Service_read_json_thread_function_DO_system("http://" + GlobalVariables_Main_Control.abb_irb_rws_iosystem[0], GlobalVariables_Main_Control.abb_irb_rws_iosystem[1]); });
                            rws_read_Thread_iosystem.IsBackground = true;
                            Debug.Log($"Thread hash : {rws_read_Thread_iosystem.GetHashCode()}");
                            rws_read_Thread_iosystem.Start();
                            // go to connect state
                            main_abb_state = 1;
                        }
                    }
                    break;
                case 1:
                    {
                        // ------------------------ Data Processing State {Connect State} ------------------------//
                        if (GlobalVariables_Main_Control.disconnect == true)
                        {
                            // Control -> Stop {Read RWS XML data}
                            GlobalVariables_Main_Control.abb_irb_dt_enable_rws_xml = true;
                            // Control -> Stop {Read RWS JSON data}
                            GlobalVariables_Main_Control.abb_irb_dt_enable_rws_json = true;
                            GlobalVariables_Main_Control.abb_irb_dt_enable_rws_iosystem = true;
                            // Abort threading block {RWS XML -> read data}
                            if (rws_read_Thread_xml.IsAlive == true)
                            {
                                rws_read_Thread_xml.Abort();
                            }
                            // Abort threading block {RWS JSON -> read data}
                            if (rws_read_Thread_json.IsAlive == true)
                            {
                                rws_read_Thread_json.Abort();
                            }
                            if (rws_read_Thread_iosystem.IsAlive == true)
                            {
                                rws_read_Thread_iosystem.Abort();
                            }
                            if (rws_read_Thread_xml.IsAlive == false && rws_read_Thread_json.IsAlive == false && rws_read_Thread_iosystem.IsAlive == false)
                            {
                                // go to initialization state {wait state -> disconnect state}
                                main_abb_state = 0;
                            }
                        }
                    }
                    break;
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------//
        // -------------------------------------------------------- FUNCTIONS -----------------------------------------------------//
        // ------------------------------------------------------------------------------------------------------------------------//

        // -------------------- Abort Threading Blocks -------------------- //
        void OnApplicationQuit()
        {
            try
            {
                // Stop - threading while
                // XML
                GlobalVariables_Main_Control.abb_irb_dt_enable_rws_xml = false;
                // JSON
                GlobalVariables_Main_Control.abb_irb_dt_enable_rws_json = false;
                GlobalVariables_Main_Control.abb_irb_dt_enable_rws_iosystem = false;

                // Abort threading block {RWS XML -> read data}
                if (rws_read_Thread_xml.IsAlive == true)
                {
                    rws_read_Thread_xml.Abort();
                }
                // Abort threading block {RWS JSON -> read data}
                if (rws_read_Thread_json.IsAlive == true)
                {
                    rws_read_Thread_json.Abort();
                }
                if (rws_read_Thread_iosystem.IsAlive == true)
                {
                    rws_read_Thread_iosystem.Abort();
                }
            }
            catch (Exception e)
            {
                // Destroy all
                Destroy(this);
            }
            finally
            {
                // Destroy all
                Destroy(this);
            }
        }

        // ------------------------ Threading Block { RWS - Robot Web Services (READ) -> XML } ------------------------//
        async void RWS_Service_read_json_thread_function_DO_system(string ip_adr, string json_target)
        {
            var handler = new HttpClientHandler { Credentials = new NetworkCredential("Default User", "robotics") };
            // disable the proxy, the controller is connected on same subnet as the PC 
            handler.Proxy = null;
            handler.UseProxy = false;

            // Send a request continue when complete
            using (HttpClient client = new HttpClient(handler))
            {
                // while - reading {data Joint, Cartesian}
                while (GlobalVariables_Main_Control.abb_irb_dt_enable_rws_json)
                {

                    using (HttpResponseMessage response = await client.GetAsync(ip_adr + json_target))
                    {
                        using (HttpContent content = response.Content)
                        {
                            try
                            {
                                // Check that response was successful or throw exception
                                response.EnsureSuccessStatusCode();
                                // Get HTTP response from completed task.
                                string result = await content.ReadAsStringAsync();


                             GlobalVariables_RWS_client.resultCountPerSecond_DO_thread++;

                            // Deserialize the returned json string
                            dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);




                                // Display controller name, version and version name
                                var service = obj._embedded._state[18];//18:DO_05
                                var service1 = obj._embedded._state[19];//19:DO_04
                                var service2 = obj._embedded._state[20];//20:DO_03
                                var service3 = obj._embedded._state[21];//21:DO_02
                                var service4 = obj._embedded._state[22];//22:DO_01
                                var service5 = obj._embedded._state[37];//22:DI_02

                                //Debug.Log(Time.frameCount);
                                //Debug.Log($"Current State:  {GlobalVariables_RWS_client.robotiosystemDo[0]} {GlobalVariables_RWS_client.robotiosystemDo[1]} " +
                                //    $"{GlobalVariables_RWS_client.robotiosystemDo[2]} " +
                                //    $"{GlobalVariables_RWS_client.robotiosystemDo[3]} " +
                                //    $"{GlobalVariables_RWS_client.robotiosystemDo[4]} " +
                                //    $"{GlobalVariables_RWS_client.robotiosystemDo[5]}" +
                                //    $"");
                                //Debug.Log($"Incoming State:  {service4.lvalue} {service3.lvalue} {service2.lvalue} {service1.lvalue} {service.lvalue} {service5.lvalue} ");


                                //Debug.Log("=====");


                                GlobalVariables_RWS_client.robotiosystemDo[0] = Convert.ToString(service4.lvalue);

                                GlobalVariables_RWS_client.robotiosystemDo[1] = Convert.ToString(service3.lvalue);

                                GlobalVariables_RWS_client.robotiosystemDo[2] = Convert.ToString(service2.lvalue);

                                GlobalVariables_RWS_client.robotiosystemDo[3] = Convert.ToString(service1.lvalue);

                                GlobalVariables_RWS_client.robotiosystemDo[4] = Convert.ToString(service.lvalue);

                                GlobalVariables_RWS_client.robotiosystemDo[5] = Convert.ToString(service5.lvalue);//DI_02

                                // Debug.Log(GlobalVariables_RWS_client.robotiosystemDo[1]);
                                // Thread Sleep {200 ms}
                                /*Thread.Sleep(200);*/
                            }
                            catch (Exception e)
                            {
                                Debug.Log(e.Message);
                            }
                            finally
                            {
                                content.Dispose();
                            }
                        }
                    }
                }
            }
        }
        async void RWS_Service_read_json_thread_Joint(string ip_adr, string json_target)
        {
            var handler = new HttpClientHandler { Credentials = new NetworkCredential("Default User", "robotics") };
            // disable the proxy, the controller is connected on same subnet as the PC 
            handler.Proxy = null;
            handler.UseProxy = false;

            // Send a request continue when complete
            using (HttpClient client = new HttpClient(handler))
            {
                // while - reading {data Joint, Cartesian}
                while (GlobalVariables_Main_Control.abb_irb_dt_enable_rws_json)
                {
                    using (HttpResponseMessage response = await client.GetAsync(ip_adr + json_target))
                    {
                        using (HttpContent content = response.Content)
                        {
                            try
                            {
                                // Check that response was successful or throw exception
                                response.EnsureSuccessStatusCode();


                            GlobalVariables_RWS_client.resultCountPerSecond_Joint_thread++;



                            // Get HTTP response from completed task.
                            string result = await content.ReadAsStringAsync();
                                // Deserialize the returned json string
                                dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);

                            // Display controller name, version and version name
                            var service = obj._embedded._state[0];

                                //-> Read RWS JSON joint angle
                                GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[0] = Convert.ToString(service.j1);
                                GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[1] = Convert.ToString(service.j2);
                                GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[2] = Convert.ToString(service.j3);
                                GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[3] = Convert.ToString(service.j4);
                                GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[4] = Convert.ToString(service.j5);
                                GlobalVariables_RWS_client.robotBaseRotLink_irb_joint[5] = Convert.ToString(service.j6);

                                // Thread Sleep {200 ms}
                                /*Thread.Sleep(200);*/
                            }
                            catch (Exception e)
                            {
                                Debug.Log(e.Message);
                            }
                            finally
                            {
                                content.Dispose();
                            }
                        }
                    }
                }
            }
        }

        // ------------------------ Threading Block { RWS - Robot Web Services (READ) -> JSON } ------------------------//
        async void RWS_Service_read_json_thread_function_TCP(string ip_adr, string json_target)
        {
            var handler = new HttpClientHandler { Credentials = new NetworkCredential("Default User", "robotics") };
            // disable the proxy, the controller is connected on same subnet as the PC 
            handler.Proxy = null;
            handler.UseProxy = false;

            // Send a request continue when complete
            using (HttpClient client = new HttpClient(handler))
            {
                // while - reading {data Joint, Cartesian}
                while (GlobalVariables_Main_Control.abb_irb_dt_enable_rws_json)
                {
                    using (HttpResponseMessage response = await client.GetAsync(ip_adr + json_target))
                    {
                        using (HttpContent content = response.Content)
                        {
                            try
                            {
                                // Check that response was successful or throw exception
                                response.EnsureSuccessStatusCode();
                                // Get HTTP response from completed task.
                                string result = await content.ReadAsStringAsync();
                                // Deserialize the returned json string
                                dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);

                            GlobalVariables_RWS_client.resultCountPerSecond_TCP_thread++;





                            // Display controller name, version and version name
                            var service = obj._embedded._state[0];

                                // TCP {X, Y, Z} -> Read RWS JSON
                                GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[0] = Convert.ToString(service.x);
                                GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[1] = Convert.ToString(service.y);
                                GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[2] = Convert.ToString(service.z);
                                // Quaternion {q1 .. q4} -> Read RWS JSON
                                GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[3] = Convert.ToString(service.q1);
                                GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[4] = Convert.ToString(service.q2);
                                GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[5] = Convert.ToString(service.q3);
                                GlobalVariables_RWS_client.robotBaseRotLink_irb_cartes[6] = Convert.ToString(service.q4);

                                // Thread Sleep {200 ms}
                                /*Thread.Sleep(200);*/
                            }
                            catch (Exception e)
                            {
                                //Debug.Log(e.Message);
                            }
                            finally
                            {
                                content.Dispose();
                            }
                        }
                    }
                }
            }
        }
    }

