using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[Serializable]
public class APIManager : MonoBehaviour
{
    const string API_URL = "https://ltk-crossword-prototype.com/api/?rows=%1&cols=%2";

    RequestManager _requestManager;
    Crosswords _crosswords;
    int _width, _height;

    bool _useFakeJson = false;

    public MockedResponse testing;

    void Awake()
    {
        _requestManager = GetComponent<RequestManager>();
        _crosswords = FindObjectOfType<Crosswords>();
    }

    public void SendAPIRequest(string width, string height)
    {
        _width = int.Parse(width);
        _height = int.Parse(height);
        _requestManager.StartCoroutine(_requestManager.SendRequest(_requestManager.CreateRequest(API_URL, width, height), ResolveAPIResponse));
    }

    void ResolveAPIResponse(UnityWebRequest www, bool error)
    {
        _crosswords.CreateCrossword(error && !_useFakeJson ? new MockedResponse().Create(_width, _height) : GetDataFromResponse(www));
    }

    APIData GetDataFromResponse(UnityWebRequest www)
    {
        //I copied the json you provided as an example and saved it into a json file at Resources
        //Then loaded it as it would be the API response

        //In here we should change the loaded json string for the www string that comes from the API
        var jsonSampleFile = Resources.Load<TextAsset>("Sample");

        var API_data = JsonUtility.FromJson<APIData>(jsonSampleFile.text);

        return API_data;
    }

    public void UseFakeJson(bool value)
    {
        _useFakeJson = value;
    }
}
