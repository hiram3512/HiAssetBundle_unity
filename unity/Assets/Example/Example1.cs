//****************************************************************************
// Description:
// Author: hiramtan@qq.com
//****************************************************************************
using UnityEngine;
using System.Collections;
using HiAssetBundle;


public class Example1 : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        CopyFileFromAppToPersistentFolder();
    }
    void CopyFileFromAppToPersistentFolder()
    {
        FileLocalMgr mgr = new FileLocalMgr();
        mgr.Init(CopyFinish);
    }
    void CopyFinish()
    {

    }
    void CheckHowManyFileNeedUpdate()
    {
        FileUpdateMgr mgr = new FileUpdateMgr();
        mgr.Init_OnlyForTest(CheckFinish);
        // mgr.Init("url",CheckFinish);
    }
    void CheckFinish(float param)
    {
        Debug.Log(string.Format("you should download: {0}M", param));
        FileUpdateMgr mgr = new FileUpdateMgr();
        mgr.StartUpdate(DownloadFinish);
    }
    void DownloadFinish()
    {
        Debug.Log("finish");
    }
}
