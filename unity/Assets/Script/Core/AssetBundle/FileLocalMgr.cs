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
        handler = param;
        string filePath = AssetBundleUtility.GetFileFolder() + "/" + AssetBundleUtility.fileName;
        if (File.Exists(filePath))
            handler();
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
    private Action handler;
    private byte[] fileInfoBytes;
    private string[] liens;
    private int index;
    private string fileOutPutPath;
    void GetFileInfoFinish(WWW param)
    {
        if (param.text.Length > 0)
        {
            fileInfoBytes = param.bytes;
            liens = param.text.Split(new char[] { '\r', '\n' });
            GetFile();
        }
        else
            handler();
    }
    void GetFile()
    {
        if (liens.Length > index)
        {
            if (string.IsNullOrEmpty(liens[index])) { index++; GetFile(); }
            else
            {
                string[] keyValue = liens[index].Split('|');
                string fileName = keyValue[0].Trim();
                string filePath = AssetBundleUtility.fileFolderName + "/" + fileName;
                fileOutPutPath = AssetBundleUtility.GetFileFolder() + "/" + fileName;
                IOManager.Instance.ReadFileFromStreamingAssetsPath(filePath, GetFileFinish);
            }
        }
        else
        {
            string path = AssetBundleUtility.fileFolderName + "/" + AssetBundleUtility.fileName;
            IOManager.Instance.WriteFileToPersistentDataPath(path, fileInfoBytes);
            handler();
        }
    }
    void GetFileFinish(WWW param)
    {
        index++;
        IOManager.Instance.WriteFile(fileOutPutPath, param.bytes);
        GetFile();
    }
    public IEnumerator DelateFileFolder()
    {
        string directory = AssetBundleUtility.GetFileFolder();
        if (Directory.Exists(directory))
            Directory.Delete(directory, true);
        yield return new WaitForSeconds(0.2f);
    }
}