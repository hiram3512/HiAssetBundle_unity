//****************************************************************************
// Description:
// Author: hiramtan@qq.com
//****************************************************************************
using UnityEngine;
using System.Collections;
using System;

public class WWWLoader : MonoBehaviour
{
    private static WWWLoader instance;
    public static WWWLoader Instance
    {
        get
        {
            if (instance == null)
                instance = new GameObject("WWWLoader").AddComponent<WWWLoader>();
            return instance;
        }
    }
    public void Startload(string downloadUrl, Action<WWW> callBackHandler = null)
    {
        lock(this)
        {
            StartCoroutine(Load(downloadUrl, callBackHandler));
        }
    }
    private IEnumerator Load(string downloadUrl, Action<WWW> callBackHandler)
    {
        WWW www = new WWW(downloadUrl);
        while (!www.isDone)
            yield return www;
        if (callBackHandler != null)
            callBackHandler(www);
    }
}
