﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class UILayouter : EditorWindow
{

    private Object[] mSelectedObjects = null;
    private void OnEnable()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
        UpdateSelection();
        this.Repaint();
    }
    private void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }
    private void UpdateSelection()
    {
        mSelectedObjects = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);
    }
    Transform mRoot;
    Camera mCamera;
    Texture mReferenceTexture;
    void OnGUI()
    {
        mRoot = (Transform)EditorGUILayout.ObjectField("Select Root:", this.mRoot, typeof(Transform), true, GUILayout.ExpandWidth(true));
        if (mCamera==null)
        {
            Camera[] cameras = GameObject.FindObjectsOfType(typeof(Camera)) as Camera[];
            foreach (var c in cameras)
            {
                UICamera uic = c.GetComponent<UICamera>();
                if (uic!=null)
                {
                    mCamera = c;
                    break;
                }
            }
        }
        mCamera = (Camera)EditorGUILayout.ObjectField("Select Camera:", this.mCamera, typeof(Camera), true, GUILayout.ExpandWidth(true));
        //place reference background
        //GUILayout.BeginHorizontal();
        //mReferenceTexture = (Texture)EditorGUILayout.ObjectField("Select Reference:", this.mReferenceTexture, typeof(Texture), true, GUILayout.ExpandWidth(true));
        //if (GUILayout.Button("Create"))
        //{
        //    GameObject go = new GameObject("Reference_" + mReferenceTexture.name);
        //    go.transform.parent = mRoot;
        //    UITexture com = go.AddComponent<UITexture>();
        //    com.depth = -100;
        //    com.mainTexture = mReferenceTexture;
        //    com.MakePixelPerfect();
        //}
        //GUILayout.EndHorizontal();
        NGUIEditorTools.DrawSeparator();

        //create node


        //flip horizontal or vertical
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Init"))
            AdjustTransform(TransformAdjustType.Init);
        if (GUILayout.Button("Flip Horizontal"))
            AdjustTransform(TransformAdjustType.FlipHorizontal);
        if (GUILayout.Button("Flip Vertical"))
            AdjustTransform(TransformAdjustType.FlipVertical);
        GUILayout.EndHorizontal();
        //view control
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("100%"))
        {
            ResizeSceneView(1f);
        }
        if (GUILayout.Button("Recommend"))
        {
            ResizeSceneView(1.5f);
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            Toggle("◤", "ButtonLeft", UIAnchor.Side.TopLeft, true);
            Toggle("▲ ", "ButtonLeft", UIAnchor.Side.Top, true);
            Toggle("◥ ", "ButtonLeft", UIAnchor.Side.TopRight, true);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            Toggle("\u25C4 ", "ButtonLeft", UIAnchor.Side.Left, true);
            Toggle("● ", "ButtonLeft", UIAnchor.Side.Center, true);
            Toggle("\u25BA ", "ButtonLeft", UIAnchor.Side.Right, true);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            Toggle("◣ ", "ButtonLeft", UIAnchor.Side.BottomLeft, true);
            Toggle("▼ ", "ButtonLeft", UIAnchor.Side.Bottom, true);
            Toggle("◢ ", "ButtonLeft", UIAnchor.Side.BottomRight, true);
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        {
            GUILayout.BeginHorizontal();
            //if (GUILayout.Button("Config"))
            //{
            //    CreateAnchorNode(UIAnchor.Side.Left);
            //    CreateAnchorNode(UIAnchor.Side.Right);
            //    CreateAnchorNode(UIAnchor.Side.Top);
            //    CreateAnchorNode(UIAnchor.Side.Bottom);
            //    CreateAnchorNode(UIAnchor.Side.BottomLeft);
            //    CreateAnchorNode(UIAnchor.Side.BottomRight);
            //    CreateAnchorNode(UIAnchor.Side.Center);
            //    CreateAnchorNode(UIAnchor.Side.TopLeft);
            //    CreateAnchorNode(UIAnchor.Side.TopRight);
            //    //CreateAnchorNode(UIAnchor.Side.Left);
            //}
            //GUILayout.Space(50);
            //EditorGUILayout.IntField("Width:",100);
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    void CreateAnchorNode(UIAnchor.Side side)
    {
        string nodeName=side.ToString();
        Transform pre = mRoot.FindChild(nodeName);
        if (pre!=null)
            GameObject.DestroyImmediate(pre.gameObject);
        GameObject go = new GameObject(nodeName);
        go.transform.parent = mRoot;
        UIAnchor com = go.AddComponent<UIAnchor>();
        com.uiCamera = mCamera;
        com.side = side;
        //com.UpdatePosition();
        //go.SendMessage("Update",  SendMessageOptions.DontRequireReceiver);

    }
    void Toggle(string text, string style,UIAnchor.Side side, bool isHorizontal)
    {
        bool isActive = false;

        switch (side)
        {
            case UIAnchor.Side.TopLeft:
                isActive = anchorSide == UIAnchor.Side.TopLeft;
                break;
            case UIAnchor.Side.Top:
                isActive = anchorSide == UIAnchor.Side.Top;
                break;
            case UIAnchor.Side.TopRight:
                isActive = anchorSide == UIAnchor.Side.TopRight;
                break;
            case UIAnchor.Side.Left:
                isActive = anchorSide == UIAnchor.Side.Left;
                break;
            case UIAnchor.Side.Center:
                isActive = anchorSide == UIAnchor.Side.Center;
                break;
            case UIAnchor.Side.Right:
                isActive = anchorSide == UIAnchor.Side.Right;
                break;
            case UIAnchor.Side.BottomLeft:
                isActive = anchorSide == UIAnchor.Side.BottomLeft;
                break;
            case UIAnchor.Side.Bottom:
                isActive = anchorSide == UIAnchor.Side.Bottom;
                break;
            case UIAnchor.Side.BottomRight:
                isActive = anchorSide == UIAnchor.Side.BottomRight;
                break;
        }
        
        if (GUILayout.Toggle(isActive, text,"Button", GUILayout.Width(40)) != isActive)
        {
            anchorSide = side;
        }
    }

    UIAnchor.Side anchorSide = UIAnchor.Side.Center;
    Transform anchorNode
    {
        get
        {
            string nodeName = anchorSide.ToString();
            Transform cur = mRoot.FindChild(nodeName);
            if (cur==null)
            {
                GameObject go = new GameObject(nodeName);
                go.transform.parent = mRoot;
                UIAnchor com = go.AddComponent<UIAnchor>();
                com.uiCamera = mCamera;
                com.side = anchorSide;
//                com.UpdatePosition();
                //go.SendMessage("Update", SendMessageOptions.DontRequireReceiver);
                return go.transform;
            }
            else
                return cur;
        }
    }
    void ResizeSceneView(float multiply)
    {
        if (Selection.activeGameObject != null && Selection.activeGameObject.activeInHierarchy)
        {
            GameObject selected = Selection.activeGameObject;
            UIWidget widget = selected.GetComponent<UIWidget>();
            if (widget != null)
            {
                Handles.color = Color.red;
                Vector3[] worldPos = NGUIMath.CalculateWidgetCorners(widget);
                Vector2[] screenPos = new Vector2[4];
                for (int i = 0; i < 4; ++i)
                    screenPos[i] = HandleUtility.WorldToGUIPoint(worldPos[i]);
                Bounds b = new Bounds(screenPos[0], Vector3.zero);
                for (int i = 1; i < 4; ++i)
                    b.Encapsulate(screenPos[i]);
                float num = b.extents.magnitude * multiply;
                SceneView.currentDrawingSceneView.LookAt(selected.transform.position, Quaternion.LookRotation(new Vector3(0f, 0f, 1f)), num);
            }
        }
        SceneView.RepaintAll();
    }
    enum TransformAdjustType
    {
        Init,
        FlipHorizontal,
        FlipVertical
    }
    void AdjustTransform(TransformAdjustType type)
    {
        GameObject selectGo = Selection.activeGameObject;
        if (selectGo != null && selectGo.activeInHierarchy)
        {
            Vector3 eulerAngles = selectGo.transform.localEulerAngles;
            switch (type)
            {
                case TransformAdjustType.Init:
                    selectGo.transform.localEulerAngles = Vector3.zero;
                    break;
                case TransformAdjustType.FlipHorizontal:
                    eulerAngles.y += 180;
                    selectGo.transform.localEulerAngles = eulerAngles;
                    break;
                case TransformAdjustType.FlipVertical:
                    eulerAngles.x += 180;
                    selectGo.transform.localEulerAngles = eulerAngles;
                    break;
            }
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
        {
            List<Texture2D> draggedTextures = new List<Texture2D>();
            foreach (Object rObject in DragAndDrop.objectReferences)
            {
                if (rObject is Texture2D)
                {
                    draggedTextures.Add(rObject as Texture2D);
                }
            }

            if (draggedTextures.Count > 0)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (Event.current.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    // Drop
                    // Update editor selection
                    Selection.objects = this.CreateSpritesFromDragAndDrop(draggedTextures, ComputeDropPositionWorld(sceneView), Event.current.alt, Event.current.shift);
                }

                Event.current.Use();
            }
        }
    }
    // Compute the drop position world
    private Vector3 ComputeDropPositionWorld(SceneView sceneView)
    {
        // compute mouse position on the world y=0 plane
        float fOffsetY = 30.0f;
        Ray mouseRay = sceneView.camera.ScreenPointToRay(new Vector3(Event.current.mousePosition.x, Screen.height - Event.current.mousePosition.y - fOffsetY, 0.0f));

        float t = -mouseRay.origin.z / mouseRay.direction.z;
        Vector3 mouseWorldPos = mouseRay.origin + t * mouseRay.direction;
        mouseWorldPos.z = 0.0f;

        return mouseWorldPos;
    }
    private GameObject[] CreateSpritesFromDragAndDrop(List<Texture2D> texturesList, Vector3 position, bool alt, bool shift)
    {
        GameObject[] generatedSprites;

        int textureCount = texturesList.Count;
        generatedSprites = new GameObject[textureCount];

        for (int index = 0; index < textureCount; ++index)
        {
            generatedSprites[index] = this.CreateSprite(texturesList[index], position, shift);
        }

        return generatedSprites;
    }
    // spirte;sliced sprite;texture
    private GameObject CreateSprite(Texture2D texture, bool shift)
    {
        if (shift)
        {
            Debug.Log("create texture");
            GameObject go = new GameObject("Texture_" + texture.name);
            go.transform.parent = mRoot;
            UITexture com = go.AddComponent<UITexture>();
            com.mainTexture = texture;
            com.MakePixelPerfect();
            return go;
        }
        else
        {
            Debug.Log("create sprite");
            string path = AssetDatabase.GetAssetPath(texture);
            string atlasName = Path.GetFileName(Path.GetDirectoryName(path));
            UIAtlas atlas = UIAtlasCollection.instance.atlases.Find(p => p.name == atlasName);
            if (atlas != null)
            {
                GameObject go = new GameObject("Sprite_" + texture.name);
                go.transform.parent = anchorNode;
                UISprite com = go.AddComponent<UISprite>();
                com.atlas = atlas;
                com.spriteName = texture.name;
                string spriteName = texture.name;
                UIAtlas.Sprite spriteData = atlas.spriteList.Find(p => p.name == spriteName);
                if (spriteData == null)
                {
                    Debug.LogError(string.Format("atlas {0} does't have the sprite {1}", atlas.name, spriteName));
                    return null;
                }
                if (spriteData.inner.Equals(spriteData.outer))
                {
                    //Debug.Log("simple sprite");
                    com.MakePixelPerfect();
                }
                else
                {
                    //Debug.Log("sliced sprite");
                    com.type = UISprite.Type.Sliced;
                    if (com.transform.localScale.x < 100 && com.transform.localScale.y < 50)
                        com.transform.localScale = new Vector3(100, 50, 1);
                }
                return go;
            }
            else
            {
                Debug.LogError("can't find related atlas");
            }
        }
        return null;
    }

    // NEW
    private GameObject CreateSprite(Texture2D texture, Vector3 creationPosition, bool shift)
    {
        //string path = AssetDatabase.GetAssetPath(texture);
        //Debug.Log(path);
        ////check the filename is atlas
        //Debug.Log(Path.GetFileName(Path.GetDirectoryName(path)));


        // Create sprite game object
        GameObject spriteGameObject = CreateSprite(texture, shift);
        // Set creation position
        if (spriteGameObject != null)
        {
            spriteGameObject.transform.position = creationPosition;
        }

        return spriteGameObject;
    }
}
