using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using System.Globalization;
using System.Text;
using System.IO;
using System;
using System.Diagnostics.Contracts;
using UnityEngine.Networking;
using UnityEngine;
using QFramework;

public class Util: MonoSingleton<Util>
{

    private static NetworkCredential n_credential = new NetworkCredential("Default User", "robotics");
    private static CookieContainer c_cookie = new CookieContainer();

    public IEnumerator Control_Data_Coroutine(string host, string target, string value) 
    {
        Debug.Log($"{nameof(Control_Data_Coroutine)}");

        var url = "http://" + host + "/rw/rapid/" + target;
        WWWForm form = new WWWForm();
        ASCIIEncoding encoding = new ASCIIEncoding();
        byte[] value_byte = encoding.GetBytes(value);

        form.AddBinaryData("value", value_byte);
        var www = UnityWebRequest.Post(url,form);

        Debug.Log($"{nameof(Control_Data_Coroutine)} Send POST HTTP: {www.uri} {www.uploadedBytes} ");

        yield return www.SendWebRequest();

        if (www.isHttpError || www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
        }
    }

    public static Stream Control_Data(string host, string target, string value)
    {

        // http:// + ip address + xml address + target
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri("http://" + host + "/rw/rapid/" + target));

        // Login: Default User; Password: robotics
        request.Credentials = n_credential;
        // don't use proxy, it's aussumed that the RC/VC is reachable without going via proxy 
        request.Proxy = null;
        request.Method = "POST";
        request.PreAuthenticate = true;
        // re-use http session between requests 
        request.CookieContainer = c_cookie;

        // Create data to send (Byte)
        ASCIIEncoding encoding = new ASCIIEncoding();
        byte[] value_byte = encoding.GetBytes(value);

        // set the length of the post data
        request.ContentLength = value_byte.Length;

        // Use form data when sending update etc to controller
        request.ContentType = "application/x-www-form-urlencoded";

        using (var stream = request.GetRequestStream())
        {
            stream.Write(value_byte, 0, value_byte.Length);
            stream.Close();
        }

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        return response.GetResponseStream();
    }
}
