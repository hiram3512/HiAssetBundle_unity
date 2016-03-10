using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
namespace HiAssetBundle
{
    public class FileUpdateMgr : IDisposable
    {
        public bool needUpdate { get { return updateList.Count > 0; } }
        public float length { get; private set; }
        public float progress { get; private set; }
        private string url = "";
        private Action<float> checkFinishHandler;
        private Action finishHandler;
        private Dictionary<string, UpdateFileInfo> newDic = new Dictionary<string, UpdateFileInfo>();
        private List<string> updateList = new List<string>();
        private float totalCount;
        private byte[] fileInfoBytes;
        private bool disposed;
        public void Init(string paramUrl, Action<float> paramCallBack)
        {
            url = paramUrl;
            checkFinishHandler = paramCallBack;
            SetNewDic();
        }
        #region only for test, will simulate update file from streamingAsset folder to user's data folder
        public void Init_OnlyForTest(Action<float> paramCallBack)
        {
#if UNITY_EDITOR || UNITY_IPHONE
            string downloadUrl = "file://" + Application.streamingAssetsPath + "/" + AssetBundleUtility.fileFolderName;
#else
            string downloadUrl = Application.streamingAssetsPath + "/" + AssetBundleUtility.fileFolderName;
#endif
            Init(downloadUrl, paramCallBack);
        }
        #endregion

        private void SetNewDic()
        {
            string fileUrl = url + "/" + AssetBundleUtility.fileName;
            fileUrl = fileUrl.Replace(" ", string.Empty);
            WWWLoader.Instance.Startload(fileUrl, FinishDownloadFileInfo);
        }
        private void FinishDownloadFileInfo(WWW paramWWW)
        {
            newDic.Clear();
            fileInfoBytes = paramWWW.bytes;
            string text = paramWWW.text;
            string[] lines = text.Split(new char[] { '\r', '\n' });
            foreach (string paramLine in lines)
            {
                if (string.IsNullOrEmpty(paramLine))
                    continue;
                string[] keyValue = paramLine.Split('|');
                newDic.Add(keyValue[0].Trim(), new UpdateFileInfo(keyValue[1].Trim(), float.Parse(keyValue[2].Trim())));
            }
            GetUpdateDic();
        }
        private void GetUpdateDic()
        {
            foreach (KeyValuePair<string, UpdateFileInfo> param in newDic)
            {
                string path = AssetBundleUtility.GetFileFolder() + "/" + param.Key;
                if (File.Exists(path))
                {
                    string md5 = AssetBundleUtility.GetMd5(path);
                    UpdateFileInfo info = newDic.Where(z => z.Value.md5 == md5).FirstOrDefault().Value;
                    if (info == null)
                    {
                        updateList.Add(param.Key);
                        length += param.Value.length;
                        File.Delete(path);
                    }

                }
                else
                {
                    updateList.Add(param.Key);
                    length += param.Value.length;
                }
            }
            checkFinishHandler(length);
        }
        public void StartUpdate(Action param)
        {
            finishHandler = param;
            CleanOldFile();
            if (needUpdate)
            {
                totalCount = updateList.Count;
                DownloadFile();
            }
            else
                Finish();
        }

        private void CleanOldFile()
        {
            string fileFolder = AssetBundleUtility.GetFileFolder();
            if (!Directory.Exists(fileFolder))
                return;
            DirectoryInfo directoryInfo = new DirectoryInfo(fileFolder);
            FileInfo[] fileInfos = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (FileInfo paramInfo in fileInfos)
            {
                string filePath = paramInfo.FullName.Replace("\\", "/");
                filePath = filePath.Replace(AssetBundleUtility.GetFileFolder() + "/", string.Empty);
                if (!newDic.ContainsKey(filePath))
                {
                    if (File.Exists(paramInfo.FullName))
                        File.Delete(paramInfo.FullName);
                }
            }
        }
        private void DownloadFile()
        {
            if (needUpdate)
            {
                string downloadUrl = url + "/" + updateList[0];
                WWWLoader.Instance.Startload(downloadUrl, FinishDownloadFile);
            }
            else
                Finish();
        }
        private void FinishDownloadFile(WWW paramWWW)
        {
            string filePath = AssetBundleUtility.GetFileFolder() + "/" + updateList[0];
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            File.WriteAllBytes(filePath, paramWWW.bytes);
            updateList.RemoveAt(0);
            DownloadFile();
			if(updateList!=null)
				progress =(totalCount - updateList.Count) / totalCount;
        }
        private void Finish()//when finish downloading whole files
        {
            string filePath = AssetBundleUtility.GetFileFolder() + "/" + AssetBundleUtility.fileName;
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            File.WriteAllBytes(filePath, fileInfoBytes);
            AssetBundleMgr.Init();
            finishHandler();
            Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~FileUpdateMgr()
        {
            Dispose(false);
        }
        void Dispose(bool paramDisposing)
        {
            if (disposed)
                return;
            if (paramDisposing)
            {
                newDic = null;
                updateList = null;
                fileInfoBytes = null;
                checkFinishHandler = null;
                finishHandler = null;
            }
            disposed = true;
        }
        private class UpdateFileInfo
        {
            public string md5;
            public float length;
            public UpdateFileInfo(string paramMd5, float paramLength)
            {
                md5 = paramMd5;
                length = paramLength;
            }
        }
    }
}