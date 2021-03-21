using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Collections;
using System;

public class RequestManager : MonoBehaviour
{
    public IEnumerator SendRequest(string url, Action<UnityWebRequest, bool> callback = null)
    {
        UnityWebRequest www = new UnityWebRequest(url);

        yield return www.SendWebRequest();

        bool error = !string.IsNullOrEmpty(www.error);

        if (!error)
            print($"Response: {www.error}");

        callback?.Invoke(www, error);
    }

    public string CreateRequest(string req, string val1)
    {
        return req.Replace("%1", val1);
    }
    public string CreateRequest(string req, string val1, string val2)
    {
        return req.Replace("%1", val1).Replace("%2", val2);
    }
    public string CreateRequest(string req, string val1, string val2, string val3)
    {
        return req.Replace("%1", val1).Replace("%2", val2).Replace("%3", val3);
    }
    public string CreateRequest(string req, Queue<string> values)
    {
        int index = 1;
        while (values.Count > 0)
        {
            string val = values.Dequeue();
            req = req.Replace($"%{index}", val);
            index++;
        }
        return req;
    }
}