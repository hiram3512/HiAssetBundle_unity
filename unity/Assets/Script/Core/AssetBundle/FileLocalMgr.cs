//****************************************************************************
// Description:
// Author: hiramtan@qq.com
//****************************************************************************
using UnityEngine;
using System.Collections;
using HiAssetBundle;
using System.IO;
using System;
using HiIO;
public class FileLocalMgr
{
    public void Init(Action param)
    {
        string filePath = AssetBundleUtility.GetFileFolder() + "/" + AssetBundleUtility.fileName;
        if (File.Exists(filePath))
        {
            param();
        }
        else
        {
            DelateFileFolder();
            GetFileInfo();
        }
    }
    void GetFileInfo()
    {
        string filePath = AssetBundleUtility.fileFolderName + "/" + AssetBundleUtility.fileName;
        IOManager.Instance.ReadFileFromStreamingAssetsPath(filePath, GetFileInfoFinish);
    }
    void GetFileInfoFinish(WWW param)
    {
        if (param.text.Length > 0)
        {
            GetFile(param.text);
        }
    }
    private string fileOutPutPath;
    void GetFile(string param)
    {
        string[] lines = param.Split(new char[] { '\r', '\n' });
        foreach (string paramLine in lines)
        {
            if (string.IsNullOrEmpty(paramLine))
                continue;
            string[] keyValue = paramLine.Split('|');
            string fileName = keyValue[0].Trim();
            fileOutPutPath = AssetBundleUtility.GetFileFolder() + "/" + fileName;
            IOManager.Instance.ReadFileFromStreamingAssetsPath(fileName, GetFileFinish);
        }
    }
    void GetFileFinish(WWW param)
    {

    }
    public void DelateFileFolder()
    {
        string directory = AssetBundleUtility.GetFileFolder();
        if (Directory.Exists(directory))
            Directory.Delete(directory, true);
    }

}