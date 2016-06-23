//*********************************************************************
// Description:
// Author: hiramtan@live.com
//*********************************************************************
using UnityEngine;
using System.Collections;
using HiAssetBundle;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class ResourceMgr
{

    private static ResourceMgr instance;
    public static ResourceMgr Instance
    {
        get
        {
            if (instance == null)
                instance = new ResourceMgr();
            return instance;
        }
    }
    /// <summary>
    /// Important: paramName must include extension, for example "tank.prefab"
    /// </summary>
    /// <param name="paramFolder"></param>
    /// <param name="paramName"></param>
    /// <returns></returns>
    public T GetObj<T>(string param) where T:UnityEngine.Object
    {
        int tempIndex = param.LastIndexOf("#");
        string paramFolder = param.Substring(0, tempIndex);
        paramFolder = paramFolder.Replace("#", "/");
        string paramName = param.Substring(tempIndex + 1);
#if UNITY_EDITOR
        var temp = "Assets/" + paramFolder + "/" + paramName;
        return AssetDatabase.LoadAssetAtPath<T>(temp);
#endif
        paramFolder = paramFolder.Replace("/", "");
        paramFolder = paramFolder.ToLower();
        paramName = paramName.Remove(paramName.LastIndexOf("."));
        paramName = paramName.ToLower();
        AssetBundle tempAB = AssetBundleMgr.GetAssetBundle(paramFolder);
        if (tempAB != null && tempAB.Contains(paramName))
            return tempAB.LoadAsset<T>(paramName);
        Debug.LogError("the things you want to load is null");
        return null;
    }

    public GameObject GetGameObject(string param)
    {
        Object temp = GetObj<Object>(param);
        if (temp != null)
            return (GameObject)MonoBehaviour.Instantiate(temp);
        return null;
    }
}
