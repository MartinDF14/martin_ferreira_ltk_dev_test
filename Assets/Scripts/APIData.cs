using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class APIData 
{
    public int status;
    public string message;
    public string errors;
    public Data data;
}

[System.Serializable]
public class Data
{
    public string[] rows;
    public Definitions definitions;
}

[System.Serializable] 
public class Definitions
{
    public string[] across;
    public string[] down;
}