using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HiAssetBundle
{
    public class AssetBundleMgr
    {
        private static Dictionary<string, LoadedAssetBundle> loadedAssetBundles = new Dictionary<string, LoadedAssetBundle>();
        private static Dictionary<string, string[]> assetBundleDependencies = new Dictionary<string, string[]>();

        public static void Init()
        {
            SetDependenceInfo();
        }
        private static void SetDependenceInfo()
        {
            string path = AssetBundleUtility.GetManifestPath();
            if (!File.Exists(path))
                return;
            AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
            AssetBundleManifest assetBundleManifest = assetBundle.LoadAsset("assetbundlemanifest") as AssetBundleManifest;
            string[] assetBundleNames = assetBundleManifest.GetAllAssetBundles();
            foreach (string paramName in assetBundleNames)
            {
                string[] dependence = assetBundleManifest.GetAllDependencies(paramName);
                assetBundleDependencies.Add(paramName, dependence);
            }
            assetBundle.Unload(true);
        }

        public static AssetBundle GetAssetBundle(string paramName)
        {
            if (assetBundleDependencies.ContainsKey(paramName))
            {
                string[] dependenceNames = assetBundleDependencies[paramName];
                foreach (string paramdependenceName in dependenceNames)
                {
                    if (loadedAssetBundles.ContainsKey(paramdependenceName))
                        loadedAssetBundles[paramdependenceName].dependenceCount++;
                    else
                    {
                        string path = AssetBundleUtility.GetAssetBundleFolder() + "/" + paramdependenceName;
                        AssetBundle ab = AssetBundle.LoadFromFile(path);
                        LoadedAssetBundle loadedAssetBundle = new LoadedAssetBundle(ab);
                        loadedAssetBundles.Add(paramdependenceName, loadedAssetBundle);
                    }
                }
            }

            if (loadedAssetBundles.ContainsKey(paramName))
            {
                loadedAssetBundles[paramName].dependenceCount++;
                return loadedAssetBundles[paramName].assetBundle;
            }
            else
            {
                string assetBundlePath = AssetBundleUtility.GetAssetBundleFolder() + "/" + paramName;
                AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
                LoadedAssetBundle loadedAssetBundle = new LoadedAssetBundle(assetBundle);
                loadedAssetBundles.Add(paramName, loadedAssetBundle);
                return loadedAssetBundles[paramName].assetBundle;
            }

        }

        public static void UnLoadAssetBundle(string paramName)
        {
            if (assetBundleDependencies.ContainsKey(paramName))
            {
                string[] dependenceNames = assetBundleDependencies[paramName];
                foreach (string paramdependenceName in dependenceNames)
                {
                    if (loadedAssetBundles.ContainsKey(paramdependenceName))
                    {
                        loadedAssetBundles[paramdependenceName].dependenceCount--;
                        if (loadedAssetBundles[paramdependenceName].dependenceCount == 0)
                        {
                            loadedAssetBundles[paramdependenceName].Unload();
                            loadedAssetBundles.Remove(paramdependenceName);
                        }

                    }
                }
            }
            if (loadedAssetBundles.ContainsKey(paramName))
            {
                loadedAssetBundles[paramName].dependenceCount--;
                if (loadedAssetBundles[paramName].dependenceCount == 0)
                {
                    loadedAssetBundles[paramName].Unload();
                    loadedAssetBundles.Remove(paramName);
                }

            }
        }

        private class LoadedAssetBundle
        {
            public AssetBundle assetBundle;
            public int dependenceCount;

            public LoadedAssetBundle(AssetBundle paramAssetBundle)
            {
                assetBundle = paramAssetBundle;
                dependenceCount = 1;
            }
            public void Unload()
            {
                assetBundle.Unload(false);
            }
        }
    }
}
