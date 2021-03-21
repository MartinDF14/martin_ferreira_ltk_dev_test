using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonActions : MonoBehaviour
{
    public Text width, height;
    public GameObject log;

    APIManager _APIManager;
    
    void Awake()
    {
        _APIManager = FindObjectOfType<APIManager>();    
    }

    public void SendAPIRequest()
    {
        int i_width, i_height;
        string s_width, s_height;
        bool b_width, b_height;

        s_width = width?.text;
        s_height = height?.text;

        b_width = int.TryParse(s_width, out i_width);
        b_height = int.TryParse(s_height, out i_height);

        if (!b_width || !b_height || i_width < 9 || i_width > 13 || i_height < 9 || i_height > 13)
            log.SetActive(true);
        else
        {
            log.SetActive(false);
            _APIManager?.SendAPIRequest(s_width, s_height);
        }

    }

    public void ChangeAPIResponseType(bool useFakeJson)
    {
        _APIManager?.UseFakeJson(useFakeJson);
    }
}
