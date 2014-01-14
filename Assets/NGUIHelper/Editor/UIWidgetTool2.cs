﻿using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 预添加：
///   
///   
/// </summary>
public class UIWidgetTool2 : EditorWindow
{

    static void SetActiveState(Transform t, bool state)
    {
        for (int i = 0; i < t.childCount; ++i)
        {
            Transform child = t.GetChild(i);
            if (state)
            {
                NGUITools.SetActiveSelf(child.gameObject, true);
                SetActiveState(child, true);
            }
            else
            {
                SetActiveState(child, false);
                NGUITools.SetActiveSelf(child.gameObject, false);
            }
            EditorUtility.SetDirty(child.gameObject);
        }
    }
    static void SetActiveState(UIWidget widget, bool state)
    {
        if (state)
        {
            NGUITools.SetActiveSelf(widget.gameObject, true);
            SetActiveState(widget.transform, true);
        }
        else
        {
            SetActiveState(widget.transform, false);
            NGUITools.SetActiveSelf(widget.gameObject, false);
        }
        EditorUtility.SetDirty(widget.gameObject);
    }

    Vector2 mScroll = Vector2.zero;
    Transform mSelectedFilterTransform;
    void FlatTransform(Transform tf)
    {
        if (tf.childCount>0)
        {
           Vector3 v= tf.transform.localPosition;
           v.z = 0;
           tf.transform.localPosition = v;
           for (int i = 0; i < tf.childCount;i++ )
           {
               FlatTransform(tf.GetChild(i));
           }
        }
    }
    void RoundToInt_Pos(Transform tf)
    {
        Vector3 v = tf.transform.localPosition;
        v.x = Mathf.RoundToInt(v.x);
        v.y = Mathf.RoundToInt(v.y);
        v.z = Mathf.RoundToInt(v.z);
        tf.transform.localPosition = v;
        for (int i = 0; i < tf.childCount; i++)
        {
            RoundToInt_Pos(tf.GetChild(i));
        }
    }
    [System.Serializable]
    class AtlasEntry
    {
        public bool enable = true;
        public UIAtlas atlas;
       public List<UISprite> sprites = new List<UISprite>();
    }
    [SerializeField]
    List<AtlasEntry> mAtlases = new List<AtlasEntry>();
    [System.Serializable]
    class FontEntry
    {
        public bool enable = true;
        public UIFont font;
        public List<UILabel> labels = new List<UILabel>();
    }
    [SerializeField]
    List<FontEntry> mFonts = new List<FontEntry>();

    void SelectTransform()
    {
        Transform tf = (Transform)EditorGUILayout.ObjectField("Select Transform:", this.mSelectedFilterTransform, typeof(Transform), true, GUILayout.ExpandWidth(true));
        if (tf!=null && tf!=mSelectedFilterTransform)
        {
            mSelectedFilterTransform = tf;
            mAtlases.Clear();
            mFonts.Clear();
            UILabel[] labels = mSelectedFilterTransform.GetComponentsInChildren<UILabel>(true);
            foreach (var label in labels)
            {
                UIFont font = label.font;
                FontEntry fe = mFonts.Find(p => p.font == font);
                if (fe == null)
                {
                    fe = new FontEntry();
                    fe.font = font;
                    mFonts.Add(fe);
                }
                fe.labels.Add(label);
            }
            UISprite[] sprites = mSelectedFilterTransform.GetComponentsInChildren<UISprite>(true);
            foreach (var sprite in sprites)
            {
                UIAtlas atlas = sprite.atlas;
                AtlasEntry ae = mAtlases.Find(p => p.atlas == atlas);
                if (ae==null)
                {
                    ae = new AtlasEntry();
                    ae.atlas = atlas;
                    mAtlases.Add(ae);
                }
                ae.sprites.Add(sprite);
            }

            Debug.Log(mFonts.Count + " " + mAtlases.Count);
        }
        //else
        //{
        //    mAtlases.Clear();
        //    mFonts.Clear();
        //}
        GUILayout.BeginHorizontal();
            if (GUILayout.Button("Flat Z Depth",GUILayout.Width(150)))
            {
                if (mSelectedFilterTransform!=null)
                    FlatTransform(mSelectedFilterTransform);
            }
            if (GUILayout.Button("Pos RoundToInt", GUILayout.Width(150)))
            {
                if (mSelectedFilterTransform != null)
                    RoundToInt_Pos(mSelectedFilterTransform);
            }
        GUILayout.EndHorizontal();
    }
    bool mShowSpecific = false;
    float mTempDepth;
    void OnGUI()
    {
        SelectTransform();
        if (mSelectedFilterTransform==null)
        {
            mAtlases.Clear();
            mFonts.Clear();
            return;
        }
        NGUIEditorTools.DrawSeparator();
        mShowSpecific = EditorGUILayout.Toggle("Show Specific", mShowSpecific);
        mScroll = GUILayout.BeginScrollView(mScroll);
        foreach (var fe in mFonts)
        {
            DrawFontEntryRow(fe);

            if (mShowSpecific)
            {
                foreach (var label in fe.labels)
                    DrawWidgetRow(label);
            }
        }
        NGUIEditorTools.DrawSeparator();
        foreach ( var ae in mAtlases)
        {
            DrawAtlasRow(ae);

            if (mShowSpecific)
            {
                foreach (var sprite in ae.sprites)
                    DrawWidgetRow(sprite);
            }
        }
        GUILayout.EndScrollView();
    }
    void DrawFontEntryRow(FontEntry fe)
    {
        GUILayout.BeginHorizontal();
        {
            bool show= EditorGUILayout.Toggle(fe.enable, GUILayout.Width(20f));
            if (show!=fe.enable)
            {
                fe.enable = show;
                foreach (var label in fe.labels)
                    NGUITools.SetActive(label.gameObject, show);

            }

            GUILayout.Label(fe.font.name, GUILayout.Width(150));
            GUILayout.Label(string.Format("Num:{0}", fe.labels.Count), GUILayout.Width(50));
            GUILayout.Space(10);
            float sum = 0;
            foreach (var label in fe.labels)
            {
                sum += label.transform.localPosition.z;
            }
            float meanDepth = sum / fe.labels.Count;
            GUILayout.Label(string.Format("Mean Depth:{0}", meanDepth), GUILayout.Width(150));
            GUILayout.Space(10);
            mTempDepth = EditorGUILayout.FloatField(mTempDepth, GUILayout.Width(50));
            if (GUILayout.Button("Apply", GUILayout.Width(50)))
            {
                foreach (var label in fe.labels)
                {
                    Vector3 v = label.transform.localPosition;
                    v.z = mTempDepth;
                    label.transform.localPosition = v;
                }
            }
        }
        GUILayout.EndHorizontal();
    }



    void DrawAtlasRow(AtlasEntry ae)
    {
        GUILayout.BeginHorizontal();
        {
            bool show = EditorGUILayout.Toggle(ae.enable, GUILayout.Width(20f));
            if (show != ae.enable)
            {
                ae.enable = show;
                foreach (var sprite in ae.sprites)
                    NGUITools.SetActive(sprite.gameObject, show);
            }

            GUILayout.Label(ae.atlas.name, GUILayout.Width(150));
            GUILayout.Label(string.Format("Num:{0}", ae.sprites.Count), GUILayout.Width(50));
            GUILayout.Space(10);
            float sum = 0;
            foreach (var sprite in ae.sprites)
            {
                if (sprite!=null)
                    sum += sprite.transform.localPosition.z;
            }
            float meanDepth = sum / ae.sprites.Count;
            GUILayout.Label(string.Format("Mean Depth:{0}", meanDepth), GUILayout.Width(150));
            GUILayout.Space(10);
            mTempDepth = EditorGUILayout.FloatField(mTempDepth, GUILayout.Width(50));
            if (GUILayout.Button("Apply", GUILayout.Width(50)))
            {
                foreach (var sprite in ae.sprites)
                {
                    Vector3 v = sprite.transform.localPosition;
                    v.z = mTempDepth;
                    sprite.transform.localPosition = v;
                }
            }
        }
        GUILayout.EndHorizontal();
    }
    
    void DrawWidgetRow(UIWidget widget)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(50f);
        if (Selection.activeGameObject == widget.gameObject)
            GUI.contentColor = Color.blue;
        else
        {
            if (widget.enabled)
            {
                GUI.contentColor = Color.black;
            }
            else
                GUI.contentColor = new Color32(70, 70, 70, 255);
        }
        if (GUILayout.Button(widget.name, EditorStyles.whiteLabel, GUILayout.MinWidth(80f)))
        {
            Selection.activeGameObject = widget.gameObject;
        }
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Depth:", GUILayout.Width(50f));
        GUILayout.Space(0);
        widget.depth = EditorGUILayout.IntField(widget.depth, GUILayout.Width(50f));
        GUILayout.Space(30);
        EditorGUILayout.LabelField("Z Depth:", GUILayout.Width(60f));

        Vector3 v = widget.transform.localPosition;
        float z = EditorGUILayout.FloatField(v.z, GUILayout.Width(50f));
        if (z != v.z)
        {
            v.z = z;
            widget.transform.localPosition = v;
        }
        GUILayout.EndHorizontal();
    }

    int mDepth;
    float mZDepth;
    public bool showAtlasDepthEditor = true;

}