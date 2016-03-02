using UnityEngine;
using System.Collections;
using System;

public class WWWLoader : MonoBehaviour
{
    public static WWWLoader instance;

    void Awake()
    {
        instance = this;
    }
    public void StartDownload(string downloadUrl, Action<WWW> callBack = null)
    {
        StartCoroutine(DownLoad(downloadUrl, callBack));
    }
    private IEnumerator DownLoad(string downloadUrl, Action<WWW> callBack)
    {
        WWW www = new WWW(downloadUrl);
        yield return www;
        if (callBack != null)
            callBack(www);
    }
}
