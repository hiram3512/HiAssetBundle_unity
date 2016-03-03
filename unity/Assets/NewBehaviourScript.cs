//****************************************************************************
// Description:
// Author: hiramtan@qq.com
//****************************************************************************
using UnityEngine;
using System.Collections;
using HiAssetBundle;

public class NewBehaviourScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        FileLocalMgr mgr = new FileLocalMgr();
        mgr.Init(Test);
    }
    void Test()
    {
        Debug.Log("finish");
    }
    // Update is called once per frame
    void Update()
    {

    }
}
