//****************************************************************************
// Description:
// Author: hiramtan@qq.com
//****************************************************************************
using HiIO;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace HiAssetBundle
{
    public class FileLocalMgr
    {
        public float progress { get; private set; }
        private Action handler;
        private byte[] fileInfoBytes;
        private string[] lines;
        private int index;
        private string fileOutPutPath;
        public void Init(Action param)
        {
            handler = param;
            string filePath = AssetBundleUtility.GetFileFolder() + "/" + AssetBundleUtility.fileName;
            if (File.Exists(filePath))
                handler();
            else
            {
                DeleteFileFolder();
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
                fileInfoBytes = param.bytes;
                lines = param.text.Split(new char[] { '\r', '\n' });
                GetFile();
            }
            else
                handler();
        }
        void GetFile()
        {
            progress = (float)index / lines.Length;
            if (lines.Length > index)
            {
                if (string.IsNullOrEmpty(lines[index])) { index++; GetFile(); }
                else
                {
                    string[] keyValue = lines[index].Split('|');
                    string fileName = keyValue[1].Trim();
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
        void DeleteFileFolder()
        {
            string tempD = AssetBundleUtility.GetFileFolder();
            //if (!Directory.Exists(tempD))
            //    return;
            //DirectoryInfo tempDInfo = new DirectoryInfo(tempD);
            //FileInfo[] tempFInfos = tempDInfo.GetFiles("*.*", SearchOption.AllDirectories);
            //for (int i = 0; i < tempFInfos.Length; i++)
            //    if (File.Exists(tempFInfos[i].Name))
            //        File.Delete(tempFInfos[i].Name);
            if (Directory.Exists(tempD))
                Directory.Delete(tempD, true);
        }
    }
}