﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Reflection;
using Object = UnityEngine.Object;

public class NGUIHelperUtility
{

    public static string GetGameObjectPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }
    public static GameObject InstantiatePrefab(string assetPath)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
        GameObject go = EditorUtility.InstantiatePrefab(prefab) as GameObject;
        return go;
    }


    #region get prefabs recursive
    public static List<string> GetPrefabsRecursive(string path)
    {
        //List<string> paths = new List<string>();
        //if (string.IsNullOrEmpty(path))
        //{
        //    Debug.Log("path is null");
        //    return paths;
        //}
        //DirectoryInfo dirInfo = new DirectoryInfo(path);
        //if (dirInfo != null)
        //{
        //    GetPrefabsRecursive(dirInfo, paths);
        //}
        return GetAssetsRecursive(path, "*.prefab");
    }
    //static void GetPrefabsRecursive(FileSystemInfo info, List<string> paths)
    //{
    //    DirectoryInfo dir = info as DirectoryInfo;
    //    if (dir == null) return;//不是目录 
    //    FileSystemInfo[] files = dir.GetFileSystemInfos();
    //    FileInfo[] prefabs = dir.GetFiles("*.prefab");
    //    foreach (FileInfo f in prefabs)
    //    {
    //        string fullPath = f.ToString();
    //        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("Assets");
    //        System.Text.RegularExpressions.Match match = regex.Match(fullPath);
    //        string path = fullPath.Substring(match.Index);
    //        paths.Add(path);
    //    }
    //    for (int i = 0; i < files.Length; i++)
    //    {
    //        FileInfo file = files[i] as FileInfo;
    //        if (file == null)//对于子目录，进行递归调用 
    //            GetPrefabsRecursive(files[i], paths);
    //    }
    //}
    #endregion

    public static List<string> GetAnimationClipsRecursive(string path)
    {
        return GetAssetsRecursive(path, "*.anim");
    }

    public static List<string> GetAssetsRecursive(string path,string searchPattern)
    {
        List<string> paths = new List<string>();
        if (string.IsNullOrEmpty(path))
        {
            Debug.Log("path is null");
            return paths;
        }
        DirectoryInfo dirInfo = new DirectoryInfo(path);
        if (dirInfo != null)
        {
            GetAssetsRecursive(dirInfo, paths, searchPattern);
        }
        return paths;
    }
    static void GetAssetsRecursive(FileSystemInfo info, List<string> paths, string searchPattern)
    {
        DirectoryInfo dir = info as DirectoryInfo;
        if (dir == null) return;//不是目录 
        FileSystemInfo[] files = dir.GetFileSystemInfos();
        FileInfo[] prefabs = dir.GetFiles(searchPattern);
        foreach (FileInfo f in prefabs)
        {
            string fullPath = f.ToString();
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("Assets");
            System.Text.RegularExpressions.Match match = regex.Match(fullPath);
            string path = fullPath.Substring(match.Index);
            paths.Add(path);
        }
        for (int i = 0; i < files.Length; i++)
        {
            FileInfo file = files[i] as FileInfo;
            if (file == null)//对于子目录，进行递归调用 
                GetAssetsRecursive(files[i], paths,searchPattern);
        }
    }

    /// <summary>          
    /// Copy文件夹          
    /// </summary>          
    /// <param name="sPath">源文件夹路径</param>          
    /// <param name="dPath">目的文件夹路径</param>          
    /// <returns>完成状态：success-完成；其他-报错</returns>          
    public static string CopyFolder(string sPath, string dPath)
    {
        string flag = "success";
        try
        {
            if (!Directory.Exists(dPath))
                Directory.CreateDirectory(dPath);
            DirectoryInfo sDir = new DirectoryInfo(sPath);
            FileInfo[] fileArray = sDir.GetFiles();
            foreach (FileInfo file in fileArray)
                file.CopyTo(dPath + "/" + file.Name, true);
            DirectoryInfo dDir = new DirectoryInfo(dPath);
            DirectoryInfo[] subDirArray = sDir.GetDirectories();
            foreach (DirectoryInfo subDir in subDirArray)
            {
                CopyFolder(subDir.FullName, dPath + "/" + subDir.Name);
            }
        }
        catch (Exception ex)
        {
            flag = ex.ToString();
        }
        return flag;
    }



    public static bool IsDirectory(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }
        // get the file attributes for file or directory
        FileAttributes attr = File.GetAttributes(path);
        //detect whether its a directory or file
        if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            return true;
        else
            return false;
    }

    public static bool IsScript(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }
        FileAttributes attr = File.GetAttributes(path);
        FileInfo fi = new FileInfo(path);
        Debug.Log(fi.Extension);
        if (fi.Extension == ".cs" || fi.Extension == ".js"||fi.Extension==".boo")
            return true;
        else
            return false;
    }





    public static Object GetObject(string name)
    {
        int assetID = EditorPrefs.GetInt(name, -1);
        return (assetID != -1) ? EditorUtility.InstanceIDToObject(assetID) : null;
    }


#region GetType
  public static  List<Type> GetTypeList()
    {
        List<Type> types = new List<Type>();
        AppDomain domain = AppDomain.CurrentDomain;
        Type ComponentType = typeof(Component);
        //mTypes.Clear();
        foreach (Assembly asm in domain.GetAssemblies())
        {
            Assembly currentAssembly = null;
            //	add UnityEngine.dll component types
            if (asm.FullName == "UnityEngine")
                currentAssembly = asm;
            //	check only for temporary assemblies (i.e. d6a5e78fb39c28ds27a1ec4f9g1 )
            if (ContainsNumbers(asm.FullName))
                currentAssembly = asm;
            if (currentAssembly != null)
            {
                foreach (Type t in currentAssembly.GetExportedTypes())
                {
                    if (ComponentType.IsAssignableFrom(t))
                    {
                        types.Add(t);
                    }
                }
            }
        }
        return types;
    }
  static  bool ContainsNumbers(String text)
    {
        int i = 0;
        foreach (char c in text)
        {
            if (int.TryParse(c.ToString(), out i))
                return true;
        }
        return false;
    }
#endregion


  /// <summary>
  //	This makes it easy to create, name and place unique new ScriptableObject asset files.
  /// </summary>
  public static void CreateAsset<T>() where T : ScriptableObject
  {
      T asset = ScriptableObject.CreateInstance<T>();

      string path = AssetDatabase.GetAssetPath(Selection.activeObject);
      if (path == "")
      {
          path = "Assets";
      }
      else if (Path.GetExtension(path) != "")
      {
          path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
      }

      string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

      AssetDatabase.CreateAsset(asset, assetPathAndName);

      AssetDatabase.SaveAssets();
      EditorUtility.FocusProjectWindow();
      Selection.activeObject = asset;
  }
}
