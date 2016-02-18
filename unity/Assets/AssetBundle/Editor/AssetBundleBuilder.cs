using System.IO;
using UnityEditor;
using UnityEngine;

namespace HiAssetBundle
{
    public class AssetBundleBuider
    {
        private static BuildAssetBundleOptions options = BuildAssetBundleOptions.UncompressedAssetBundle;

        [MenuItem("AssetBundles/Build", false, 0)]
        public static void Build()
        {
            string fileFolder = AssetBundleUtility.GetFileOutPutFolder();
            if (Directory.Exists(fileFolder))
                Directory.Delete(fileFolder, true);
            Directory.CreateDirectory(fileFolder);
            CopyUpdateFiles();
            BuildAssetBundles();
            GenerateFileInfo();
        }

        [MenuItem("AssetBundles/Clean User's Data", false, 1)]
        public static void Clean()
        {
            string path = AssetBundleUtility.GetFileFolder();
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }

        [MenuItem("AssetBundles/Simulate Server", false, 2)]
        public static void Simulate()
        {
            Build();
            if (!Directory.Exists(Application.streamingAssetsPath))
                Directory.CreateDirectory(Application.streamingAssetsPath);
            string directory = Application.streamingAssetsPath + "/" + AssetBundleUtility.fileFolderName;
            if (Directory.Exists(directory))
                Directory.Delete(directory, true);
            Directory.Move(AssetBundleUtility.GetFileOutPutFolder(), directory);

        }
        private static void CopyUpdateFiles()
        {
            string luaPath = Application.dataPath + "/" + AssetBundleUtility.updateFolderName;
            if (!Directory.Exists(luaPath))
                return;
            string outPath = AssetBundleUtility.GetFileOutPutFolder() + "/" + AssetBundleUtility.updateFolderName;
            if (Directory.Exists(outPath))
                Directory.Delete(outPath, true);
            Directory.CreateDirectory(outPath);
            DirectoryInfo directoryInfo = new DirectoryInfo(luaPath);
            FileInfo[] fileInfos = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (FileInfo paramFileInfo in fileInfos)
            {
                string info = paramFileInfo.FullName;
                if (info.EndsWith(".meta"))
                    continue;
                info = info.Replace("\\", "/");
                string newfile = info.Replace(luaPath, string.Empty);
                string newpath = outPath + newfile;
                string path = Path.GetDirectoryName(newpath);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                if (File.Exists(newpath))
                    File.Delete(newpath);
                File.Copy(info, newpath, true);
            }
        }
        private static void BuildAssetBundles()
        {
            string outPath = AssetBundleUtility.GetFileOutPutFolder() + "/" +
                AssetBundleUtility.GetPlatformName(EditorUserBuildSettings.activeBuildTarget);
            if (Directory.Exists(outPath))
                Directory.Delete(outPath, true);
            Directory.CreateDirectory(outPath);
            BuildPipeline.BuildAssetBundles(outPath, options, EditorUserBuildSettings.activeBuildTarget);
        }
        private static void GenerateFileInfo()
        {
            string fileFolder = AssetBundleUtility.GetFileOutPutFolder();
            DirectoryInfo directoryInfo = new DirectoryInfo(fileFolder);
            string fileInfoPath = fileFolder + "/" + AssetBundleUtility.fileName;
            if (File.Exists(fileInfoPath))
                File.Delete(fileInfoPath);
            FileInfo[] fileInfos = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
            FileStream stream = new FileStream(fileInfoPath, FileMode.CreateNew);
            StreamWriter writer = new StreamWriter(stream);
            foreach (FileInfo paramFileInfo in fileInfos)
            {
                string fileInfo = paramFileInfo.FullName;
                string md5 = AssetBundleUtility.GetMd5(fileInfo);
                fileInfo = fileInfo.Replace("\\", "/");
                fileInfo = fileInfo.Replace(fileFolder + "/", string.Empty);
                writer.WriteLine(fileInfo + "|" + md5);
            }
            writer.Close();
            stream.Close();
            AssetDatabase.Refresh();
        }
    }
}