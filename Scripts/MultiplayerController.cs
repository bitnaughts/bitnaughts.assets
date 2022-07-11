using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

public class MultiplayerController : MonoBehaviour
{
    void Start() {
        InvokeRepeating("Ping", .01f, 1);
    }
    void Ping()
    {
        // Debug.Log("Hello");
        StartCoroutine(GetRequest());
    }
    IEnumerator GetRequest()
    {
        string url = "https://bitnaughts.azurewebsites.net";
        string name = "Test";
        string data = GameObject.Find("Example").GetComponent<StructureController>().ToString();
        #if UNITY_EDITOR
            url = "http://localhost:7071";
        #endif
        using (UnityWebRequest webRequest = UnityWebRequest.Get($"{url}/api/ping?name={name}&data={data}&cursor=0"))
        {
            webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");
            yield return webRequest.SendWebRequest();
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                case UnityWebRequest.Result.ProtocolError:
                    Debug.Log(webRequest.error.ToString());
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(webRequest.downloadHandler.text.ToString());
                    break;
            }
        }
    }
}