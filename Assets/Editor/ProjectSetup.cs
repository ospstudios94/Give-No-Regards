using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class ProjectSetup : EditorWindow
{
    // Lists for bulk creation
    private List<string> tagsToCreate = new List<string> { "" };
    private List<string> layersToCreate = new List<string> { "" };
    private List<string> sortingLayersToCreate = new List<string> { "" };
    private List<string> sceneNames = new List<string> { "NewLevel" };
    private Vector2 scrollPos;
    private string folderPath = "Assets/Project/Level Scenes";

    [MenuItem("Tools/Project Setup Manager")]
    public static void ShowWindow()
    {
        GetWindow<ProjectSetup>("Project Setup Manager");
    }

    void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
        // --- TAGS SECTION ---
        DrawListSection("Tags", tagsToCreate);
        //if (GUILayout.Button("Apply All Tags"))
        //{
        //    foreach (var t in tagsToCreate) if (!string.IsNullOrEmpty(t)) AddTag(t);
        //}

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // --- LAYERS SECTION ---
        DrawListSection("Layers", layersToCreate);
        //if (GUILayout.Button("Apply All Layers"))
        //{
        //    foreach (var l in layersToCreate) if (!string.IsNullOrEmpty(l)) AddLayer(l);
        //}

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // --- SORTING LAYERS SECTION ---
        DrawListSection("Sorting Layers", sortingLayersToCreate);
        //if (GUILayout.Button("Apply All Sorting Layers"))
        //{
        //    foreach (var sl in sortingLayersToCreate) if (!string.IsNullOrEmpty(sl)) AddSortingLayer(sl);
        //}

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // --- SCENES SECTION ---
        GUILayout.Label("Scene Settings", EditorStyles.boldLabel);
        folderPath = EditorGUILayout.TextField("Save Folder", folderPath);

        DrawListSection("Level Names", sceneNames, false);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        //if (GUILayout.Button("Build Scenes", GUILayout.Height(40)))
        //{
        //    CreateScenes();
        //}

        if (GUILayout.Button("Full Project Initialize", GUILayout.Height(40)))
        {
            foreach (var t in tagsToCreate) if (!string.IsNullOrEmpty(t)) AddTag(t);
            foreach (var l in layersToCreate) if (!string.IsNullOrEmpty(l)) AddLayer(l);
            foreach (var sl in sortingLayersToCreate) if (!string.IsNullOrEmpty(sl)) AddSortingLayer(sl);
            CreateScenes();
            Debug.Log("Full project initialization complete.");
        }
        EditorGUILayout.EndScrollView();
    }

    // Helper to draw the +/- list UI for any category
    private void DrawListSection(string title, List<string> list, bool showHeader = true)
    {
        if (showHeader) GUILayout.Label(title, EditorStyles.boldLabel);

        for (int i = 0; i < list.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            list[i] = EditorGUILayout.TextField($"{title} {i}", list[i]);
            if (GUILayout.Button("-", GUILayout.Width(25))) { list.RemoveAt(i); }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button($"Add {title} Slot")) list.Add("");
    }

    // --- LOGIC METHODS (Updated for safety) ---

    public static void AddTag(string tagName)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            if (tagsProp.GetArrayElementAtIndex(i).stringValue.Equals(tagName)) return;
        }

        tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
        tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tagName;
        tagManager.ApplyModifiedProperties();
    }

    public static void AddLayer(string layerName)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layersProp = tagManager.FindProperty("layers");

        for (int i = 8; i < layersProp.arraySize; i++)
        {
            SerializedProperty sp = layersProp.GetArrayElementAtIndex(i);
            if (sp.stringValue.Equals(layerName)) return;
            if (string.IsNullOrEmpty(sp.stringValue))
            {
                sp.stringValue = layerName;
                tagManager.ApplyModifiedProperties();
                return;
            }
        }
    }

    public static void AddSortingLayer(string layerName)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty sortingLayersProp = tagManager.FindProperty("m_SortingLayers");

        for (int i = 0; i < sortingLayersProp.arraySize; i++)
        {
            if (sortingLayersProp.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue == layerName) return;
        }

        sortingLayersProp.InsertArrayElementAtIndex(sortingLayersProp.arraySize);
        SerializedProperty newEntry = sortingLayersProp.GetArrayElementAtIndex(sortingLayersProp.arraySize - 1);
        newEntry.FindPropertyRelative("name").stringValue = layerName;
        newEntry.FindPropertyRelative("uniqueID").intValue = layerName.GetHashCode();
        tagManager.ApplyModifiedProperties();
    }

    private void CreateScenes()
    {
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
        List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

        for (int i = 0; i < sceneNames.Count; i++)
        {
            string name = sceneNames[i];
            if (string.IsNullOrEmpty(name)) continue;

            string path = $"{folderPath}/{name}.unity";
        //       Scene scene;
        // if (File.Exists(path)) {
        //     scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
        // } else {
        //     scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        // }
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Setup Camera
            GameObject camGo = new GameObject("Main Camera");
            camGo.AddComponent<Camera>();
            camGo.tag = "MainCamera";
            camGo.transform.position = new Vector3(0, 0, -10);

            // Setup 2D Light (Requires URP package)
            
            GameObject light2D = new GameObject("Global Light 2D");
            Light2D li = light2D.AddComponent<Light2D>();
            li.lightType = Light2D.LightType.Global;

            EditorSceneManager.SaveScene(newScene, path);
            EditorSceneManager.CloseScene(newScene, true);

            if (!buildScenes.Any(s => s.path == path))
            {
                buildScenes.Add(new EditorBuildSettingsScene(path, true));
            }
        }

        EditorBuildSettings.scenes = buildScenes.ToArray();
        AssetDatabase.Refresh();
        Debug.Log("Project Setup: Scenes, Tags, and Layers processed.");
    }

}
