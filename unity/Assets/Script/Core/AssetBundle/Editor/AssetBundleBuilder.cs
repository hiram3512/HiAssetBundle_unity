using System.IO;
using UnityEditor;
using UnityEngine;

namespace HiAssetBundle
{
    public class AssetBundleBuider
    {
        private static BuildAssetBundleOptions options = BuildAssetBundleOptions.UncompressedAssetBundle;

        [MenuItem("AssetBundles/Build AssetBundle", false, 0)]
        public static void Build()
        {
            string fileFolder = AssetBundleUtility.GetFileOutPutFolder_OnlyForEditor();
            if (Directory.Exists(fileFolder))
                Directory.Delete(fileFolder, true);
            Directory.CreateDirectory(fileFolder);
            CopyUpdateFiles();
            BuildAssetBundles();
            GenerateFileInfo();
            Debug.Log("build finish");
        }

        [MenuItem("AssetBundles/Clean User's Data", false, 3)]
        public static void Clean()
        {
            string path = AssetBundleUtility.GetFileFolder();
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            Debug.Log("clean finish");
        }
        private static void CopyUpdateFiles()
        {
            string updatePath = Application.dataPath + "/" + AssetBundleUtility.updateFolderName;
            if (!Directory.Exists(updatePath))
                return;
            DirectoryInfo directoryInfo = new DirectoryInfo(updatePath);
            FileInfo[] fileInfos = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
            string outPath = AssetBundleUtility.GetFileOutPutFolder_OnlyForEditor() + "/" + AssetBundleUtility.updateFolderName;
            if (Directory.Exists(outPath))
                Directory.Delete(outPath, true);
            Directory.CreateDirectory(outPath);
            float total, processed = 0;
            total = fileInfos.Length;
            foreach (FileInfo paramFileInfo in fileInfos)
            {
                processed++;
                EditorUtility.DisplayProgressBar("CopyFile", "Progress", processed / total);
                string info = paramFileInfo.FullName;
                if (info.EndsWith(".meta"))
                    continue;
                info = info.Replace("\\", "/");
                string newfile = info.Replace(updatePath, string.Empty);
                string newpath = outPath + newfile;
                newpath = newpath.Replace(" ", string.Empty);
                string path = Path.GetDirectoryName(newpath);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                if (File.Exists(newpath))
                    File.Delete(newpath);
                File.Copy(info, newpath, true);
            }
            EditorUtility.ClearProgressBar();
        }
        private static void BuildAssetBundles()
        {
            string outPath = AssetBundleUtility.GetFileOutPutFolder_OnlyForEditor() + "/" +
                AssetBundleUtility.GetPlatformName(EditorUserBuildSettings.activeBuildTarget);
            if (Directory.Exists(outPath))
                Directory.Delete(outPath, true);
            Directory.CreateDirectory(outPath);
            BuildPipeline.BuildAssetBundles(outPath, options, EditorUserBuildSettings.activeBuildTarget);
        }
        private static void GenerateFileInfo()
        {
            string fileFolder = AssetBundleUtility.GetFileOutPutFolder_OnlyForEditor();
            DirectoryInfo directoryInfo = new DirectoryInfo(fileFolder);
            string fileInfoPath = fileFolder + "/" + AssetBundleUtility.fileName;
            if (File.Exists(fileInfoPath))
                File.Delete(fileInfoPath);
            FileInfo[] fileInfos = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
            FileStream stream = new FileStream(fileInfoPath, FileMode.CreateNew);
            StreamWriter writer = new StreamWriter(stream);
            float total, processed = 0;
            total = fileInfos.Length;
            foreach (FileInfo paramFileInfo in fileInfos)
            {
                processed++;
                EditorUtility.DisplayProgressBar("GenerateFile", "Progress", processed / total);
                string fileInfo = paramFileInfo.FullName;
                string md5 = AssetBundleUtility.GetMd5(fileInfo);
                fileInfo = fileInfo.Replace("\\", "/");
                fileInfo = fileInfo.Replace(fileFolder + "/", string.Empty);
                writer.WriteLine(fileInfo + "|" + md5);
            }
            EditorUtility.ClearProgressBar();
            writer.Close();
            stream.Close();
        }
        [MenuItem("AssetBundles/Copy to StreamingAsset", false, 3)]
        public static void CopyFileToStreamingAsset()
        {
            Build();
            if (!Directory.Exists(Application.streamingAssetsPath))
                Directory.CreateDirectory(Application.streamingAssetsPath);
            string directory = Application.streamingAssetsPath + "/" + AssetBundleUtility.fileFolderName;
            if (Directory.Exists(directory))
                Directory.Delete(directory, true);
            Directory.Move(AssetBundleUtility.GetFileOutPutFolder_OnlyForEditor(), directory);
            AssetDatabase.Refresh();
            Debug.Log("simulate finish");
        }
    }
}