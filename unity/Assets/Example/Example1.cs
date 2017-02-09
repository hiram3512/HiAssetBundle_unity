//****************************************************************************
// Description:资源维护
// 1.将跟包资源复制到沙盒目录下(可以扩展压缩/解压缩,比如跟包资源是压缩包)
// 2.检查本地需要更新的文件列表并提示玩家需要更新xx兆资源
// 3.开始更新资源服务器上的最新资源
// 4.更新完成后执行回调方法(可以扩展后续逻辑,比如更新完成后开始登陆)
//
//ps.所有示例逻辑会把streamingAssetsPath当做服务器资源地址,模拟从服务器下载过程.真正需要从服务器下载时使用注释的代码.
//
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
