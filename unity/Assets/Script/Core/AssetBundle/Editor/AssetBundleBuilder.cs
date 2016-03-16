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
            string fileFolder = Application.streamingAssetsPath + "/" + AssetBundleUtility.fileFolderName;
            if (Directory.Exists(fileFolder))
                Directory.Delete(fileFolder, true);
            string assetBundleFolder = fileFolder + "/" + AssetBundleUtility.GetPlatformName(EditorUserBuildSettings.activeBuildTarget);
            if (!Directory.Exists(assetBundleFolder))
                Directory.CreateDirectory(assetBundleFolder);
            BuildPipeline.BuildAssetBundles(assetBundleFolder, options, EditorUserBuildSettings.activeBuildTarget);
            GenerateFileInfo(fileFolder);
            Debug.Log("Finish build assetbundle");
        }

        [MenuItem("AssetBundles/Clean User's Data", false, 1)]
        public static void Clean()
        {
            string path = AssetBundleUtility.GetFileFolder();
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            Debug.Log("clean finish");
        }
        private static void GenerateFileInfo(string param)
        {
            string fileFolder = param;
            DirectoryInfo directoryInfo = new DirectoryInfo(fileFolder);
            string fileInfoPath = fileFolder + "/" + AssetBundleUtility.fileName;
            FileInfo[] fileInfos = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
            FileStream stream = new FileStream(fileInfoPath, FileMode.CreateNew);
            StreamWriter writer = new StreamWriter(stream);
            float total, processed = 0;
            total = fileInfos.Length;
            foreach (FileInfo paramFileInfo in fileInfos)
            {
                processed++;
                EditorUtility.DisplayProgressBar(AssetBundleUtility.fileName, "Progress", processed / total);
                string fileInfo = paramFileInfo.FullName;
                long length = paramFileInfo.Length;
                string md5 = AssetBundleUtility.GetMd5(fileInfo);
                fileInfo = fileInfo.Replace("\\", "/");
                fileInfo = fileInfo.Replace(fileFolder + "/", string.Empty);
                writer.WriteLine(fileInfo + "|" + md5 + "|" + length);
            }
            EditorUtility.ClearProgressBar();
            writer.Close();
            stream.Close();
        }
    }
}