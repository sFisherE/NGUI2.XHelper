﻿//using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;

using Object= UnityEngine.Object;
public class ScriptSpider : EditorWindow
{
    [MenuItem("NGUIHelper/Script Spider")]
    static void Init()
    {
        ScriptSpider window = (ScriptSpider)EditorWindow.GetWindow(typeof(ScriptSpider), false, "Script Spider");
    }
#region ForType
    public ScriptSpider()
    {
        FillTypeList();
    }
    private List<Type> mTypes = new List<Type>();
    void FillTypeList()
    {
        AppDomain domain = AppDomain.CurrentDomain;
        Type ComponentType = typeof(Component);
        mTypes.Clear();
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
                        mTypes.Add(t);
                    }
                }
            }
        }
    }
    bool ContainsNumbers(String text)
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
    void OnSelectScript(Object obj)
    {
        mMonoBehaviour = obj as MonoBehaviour;
        Repaint();
    }
    MonoBehaviour mMonoBehaviour;
    UnityEngine.Object mScript;
    void DrawSelectAtlas()
    {
        //ComponentSelector.Draw<Object>("Select", mMonoFile, OnSelectMonoBehaviour);
        mScript = EditorGUILayout.ObjectField("Select Script:", mScript, typeof(Object), false) as Object;
        //判断是不是一个类
    }


    string mPath;
    UnityEngine.Object mFolder;
    void OnSelectFolder(UnityEngine.Object obj)
     {
         mFolder = obj;
         if (mFolder!=null)
        {
            mPath = AssetDatabase.GetAssetPath(mFolder);

            mPrefabNames.Clear();
            mPrefabNames = NGUIHelperUtility.GetAllPrefabs(mPath);
        }
     }
    void DrawSelectPath()
    {
        GUILayout.BeginHorizontal();
        {
            mFolder = EditorGUILayout.ObjectField("Select Path:", mFolder, typeof(Object), false) as Object;
            if (mFolder != null)
            {
                mPath = AssetDatabase.GetAssetPath(mFolder);
                mPrefabNames.Clear();
                mPrefabNames = NGUIHelperUtility.GetAllPrefabs(mPath);
            }
            GUILayout.Label(mPath);
        }
        GUILayout.EndHorizontal();
    }
    List<string> mPrefabNames = new List<string>();

    Vector2 mScroll2 = Vector2.zero;
    Vector2 mScroll = Vector2.zero;
    GUIStyle mStyle = new GUIStyle();
    bool mShowRelatedSprites = false;
    void OnGUI()
    {
        DrawSelectPath();
        DrawSelectAtlas();
        NGUIEditorTools.DrawSeparator();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Detect", GUILayout.Width(200)))
        {
        }
        GUILayout.EndHorizontal();
    }

    /// <summary>
    ///   实时更新
    /// </summary>
    //void OnInspectorUpdate()
    //{
    //    Repaint();
    //} 
}