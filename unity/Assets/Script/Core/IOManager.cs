//****************************************************************************
// Description: 文件常用操作
// Author: hiramtan@qq.com
//****************************************************************************
using System;
using System.IO;
using UnityEngine;
using System.Collections;
namespace HiIO
{
    public class IOManager
    {
        private static IOManager instance;
        public static IOManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new IOManager();
                return instance;
            }
        }
        public bool IsFolderExist(string param)
        {
            return Directory.Exists(param);
        }

        public void CreateFolder(string param)
        {
            Directory.CreateDirectory(param);
        }

        public void DeleteFolder(string param)
        {
            Directory.Delete(param, true);//第二个参数：删除子目录
        }

        public bool IsFileExist(string param)
        {
            return File.Exists(param);
        }
        Action<WWW> finishLoadFromStreamingAssetsPathHandler;
        public void ReadFileFromStreamingAssetsPath(string paramPath, Action<WWW> paramHandler)
        {
            finishLoadFromStreamingAssetsPathHandler = paramHandler;
            paramPath = GetStreamingAssetsPath() + "/" + paramPath;
            WWWLoader.Instance.Startload(paramPath, FinishLoadFromStreamingAssetsPath);
        }
        private void FinishLoadFromStreamingAssetsPath(WWW paramWWW)
        {
            finishLoadFromStreamingAssetsPathHandler(paramWWW);
        }

        public byte[] ReadFileFromPersistentDataPath(string param)
        {
            param = Application.persistentDataPath + "/" + param;
            return ReadFile(param);
        }
        public byte[] ReadFile(string param)
        {
            try
            {
                using (FileStream fs = new FileStream(param, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, (int)fs.Length);
                    return bytes;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
            return null;
        }
        public void WriteFileToPersistentDataPath(string paramPath, byte[] paramBytes)
        {
            paramPath = Application.persistentDataPath + "/" + paramPath;
            WriteFile(paramPath, paramBytes);
        }
        public void WriteFile(string paramPath, byte[] paramBytes)
        {
            try
            {
                string directory = Path.GetDirectoryName(paramPath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                using (FileStream fs = new FileStream(paramPath, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(paramBytes, 0, paramBytes.Length);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
        public void DeleteFile(string param)
        {
            File.Delete(param);
        }
        private string GetStreamingAssetsPath()
        {
# if UNITY_EDITOR || UNITY_IPHONE
            string path = "file://" + Application.streamingAssetsPath;
#else
            string path = Application.streamingAssetsPath;
#endif
            return path;
        }
    }
}