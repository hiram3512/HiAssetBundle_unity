using UnityEngine;
using System.Collections;
using System;

namespace HiAssetBundle
{
    public class WWWDownloadMgr : MonoBehaviour
    {
        public static WWWDownloadMgr instance;
        // Use this for initialization
        void Awake()
        {
            instance = this;
        }
        public void StartDownload(string downloadUrl, Action<WWW> callBack)
        {
            StartCoroutine(DownLoad(downloadUrl, callBack));
        }
        public IEnumerator DownLoad(string downloadUrl, Action<WWW> callBack)
        {
            WWW www = new WWW(downloadUrl);
            yield return www;
            callBack(www);
        }
    }
}


