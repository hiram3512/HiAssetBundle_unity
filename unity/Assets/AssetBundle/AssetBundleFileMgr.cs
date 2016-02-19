using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace HiAssetBundle
{
    public class AssetBundleFileMgr : IDisposable
    {
        public bool needUpdate { get { return updateList.Count > 0; } }
        public float overallProgress { get; private set; }
        private string url = "";
        private Action finishCallBack;
        private Dictionary<string, string> newDic = new Dictionary<string, string>();
        private List<string> updateList = new List<string>();
        private GameObject downloaderObj;
        private float totalCount;
        private bool disposed;
        public void Init(string paramUrl, Action paramCallBack)
        {
            url = paramUrl;
            finishCallBack = paramCallBack;
            downloaderObj = new GameObject("WWWDownloadMgr");
            downloaderObj.AddComponent<WWWDownloadMgr>();
            SetNewDic();
        }
        #region only for test, will simulate update file from streamingAsset folder to user's data folder
        public void SimulateServer_OnlyForTest(Action paramCallBack)
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
            WWWDownloadMgr.instance.StartDownload(fileUrl, FinishDownloadFileInfo);
        }
        private void FinishDownloadFileInfo(WWW paramWWW)
        {
            newDic.Clear();
            string text = paramWWW.text;
            string[] lines = text.Split(new char[] { '\r','\n'});
            foreach (string paramLine in lines)
            {
                if (string.IsNullOrEmpty(paramLine))
                    continue;
                string[] keyValue = paramLine.Split('|');
                keyValue[0] = keyValue[0].Replace(" ", string.Empty);
                keyValue[1] = keyValue[1].Replace(" ", string.Empty);
                newDic.Add(keyValue[0], keyValue[1]);
            }
            GetUpdateDic();
        }
        private void GetUpdateDic()
        {
            foreach (KeyValuePair<string, string> param in newDic)
            {
                string path = AssetBundleUtility.GetFileFolder() + "/" + param.Key;
                if (File.Exists(path))
                {
                    string md5 = AssetBundleUtility.GetMd5(path);
                    if (!newDic.ContainsValue(md5))
                    {
                        updateList.Add(param.Key);
                        File.Delete(path);
                    }
                }
                else
                    updateList.Add(param.Key);
            }
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
                WWWDownloadMgr.instance.StartDownload(downloadUrl, FinishDownloadFile);
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
            overallProgress = 100 * (totalCount - updateList.Count) / totalCount;
        }
        private void Finish()//when finish downloading whole files
        {
            AssetBundleMgr.Init();
            finishCallBack();
            Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~AssetBundleFileMgr()
        {
            Dispose(false);
        }
        void Dispose(bool paramDisposing)
        {
            if (disposed)
                return;
            if (paramDisposing)
            {
                finishCallBack = null;
                MonoBehaviour.Destroy(downloaderObj);
            }
            disposed = true;
        }
    }
}