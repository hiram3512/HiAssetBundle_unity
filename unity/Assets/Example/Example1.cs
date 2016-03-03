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
        FileLocalMgr mgr = new FileLocalMgr();
        mgr.Init(FinishLocalFile);
    }
    void FinishLocalFile()
    {
        FileUpdateMgr mgr = new FileUpdateMgr();
        mgr.SimulateServer_OnlyForTest(FinishUpdate);
    }
    void FinishUpdate()
    {
        Debug.Log("finish");
    }
    // Update is called once per frame
    void Update()
    {

    }
}
