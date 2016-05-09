using System.IO;
using UnityEditor;
using UnityEngine;

namespace HiAssetBundle
{
    public class AssetBundleBuider
    {
        private static BuildAssetBundleOptions options = BuildAssetBundleOptions.UncompressedAssetBundle;

        [MenuItem("AssetBundles/Build AssetBundle(windows)", false, 0)]
        public static void BuildWindows()
        {
            Build(BuildTarget.StandaloneWindows);
        }
        [MenuItem("AssetBundles/Build AssetBundle(ios)", false, 1)]
        public static void BuildIos()
        {
            Build(BuildTarget.iOS);
        }
        [MenuItem("AssetBundles/Build AssetBundle(android)", false, 2)]
        public static void BuildAndroid()
        {
            Build(BuildTarget.Android);
        }
        [MenuItem("AssetBundles/Clean User's Data", false, 10)]
        public static void Clean()
        {
            string path = AssetBundleUtility.GetFileFolder();
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            Debug.Log("clean finish");
        }
        private static void Build(BuildTarget param)
        {
            string fileFolder = Application.streamingAssetsPath + "/" + AssetBundleUtility.fileFolderName;
            if (Directory.Exists(fileFolder))
                Directory.Delete(fileFolder, true);
            string assetBundleFolder = fileFolder + "/" + AssetBundleUtility.GetPlatformName(param);
            if (!Directory.Exists(assetBundleFolder))
                Directory.CreateDirectory(assetBundleFolder);
            BuildPipeline.BuildAssetBundles(assetBundleFolder, options, param);
            GenerateFileInfo(fileFolder);
            Debug.Log("Finish build assetbundle");
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
                if (fileInfo.EndsWith(".meta"))
                    continue;
                long length = paramFileInfo.Length;
                string md5 = AssetBundleUtility.GetMd5(fileInfo);
                fileInfo = fileInfo.Replace("\\", "/");
                fileInfo = fileInfo.Replace(fileFolder + "/", string.Empty);
                writer.WriteLine(md5 + "|" + fileInfo + "|" + length);
            }
            EditorUtility.ClearProgressBar();
            writer.Close();
            stream.Close();
        }

        [MenuItem("AssetBundles/Rename sources a new name", false, 11)]
        private static void RenameSources()
        {
            string tempSourcesPath = Application.dataPath + "/Art";
            string tempSplit = "#";//use this sign to split directory

            DirectoryInfo tempDInfo = new DirectoryInfo(tempSourcesPath);
            FileInfo[] tempFInfo = tempDInfo.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (var param in tempFInfo)
            {
                string tempPath = param.ToString();
                if (tempPath.EndsWith(".meta"))
                    continue;
                if (tempPath.Contains(tempSplit))//already name it, donnt need to rename it again
                    continue;
                tempPath = tempPath.Replace(@"\", "/");
                tempPath = tempPath.Substring(tempPath.IndexOf("Assets"));
                int tempIndex = tempPath.IndexOf("/") + 1;
                string tempName = tempPath.Substring(tempIndex);
                tempIndex = tempName.LastIndexOf(".");
                tempName = tempName.Substring(0, tempIndex);
                tempName = tempName.Replace("/", tempSplit);
                string tempError = AssetDatabase.RenameAsset(tempPath, tempName);
                if (!string.IsNullOrEmpty(tempError))
                    Debug.Log(tempError);
            }
            AssetDatabase.Refresh();
        }

        [MenuItem("AssetBundles/Set prefab a assetbundle name", false, 12)]
        private static void SetPrefabAAssetBundleName()
        {
            string tempPrefabPath = Application.dataPath + "/Example";
            DirectoryInfo tempDInfo = new DirectoryInfo(tempPrefabPath);
            FileInfo[] tempFInfo = tempDInfo.GetFiles("*.prefab", SearchOption.AllDirectories);
            foreach (var VARIABLE in tempFInfo)
            {
                string tempPath = VARIABLE.ToString();
                tempPath = tempPath.Replace(@"\", " / ");
                tempPath = tempPath.Replace(" ", "");
                tempPath = tempPath.Substring(tempPath.IndexOf("Assets"));
                AssetImporter tempAImporter = AssetImporter.GetAtPath(tempPath);
                int tempIndex = tempPath.IndexOf("/") + 1;
                tempPath = tempPath.Substring(tempIndex);
                tempIndex = tempPath.LastIndexOf("/");
                tempPath = tempPath.Substring(0, tempIndex);
                tempPath = tempPath.Replace("/", "");
                tempPath = tempPath.ToLower();
                if (tempAImporter != null)
                    tempAImporter.assetBundleName = tempPath;
                else
                    Debug.LogError("this prefab cann't set name: +" + tempPath);
            }
        }
    }
}