using System.IO;
using UnityEditor;
using UnityEngine;

public class FolderSetup : EditorWindow
{
    // A simple struct to represent a folder with its nested children
    [System.Serializable]
    public struct NestedFolder
    {
        public string parentName;
        public string[] childrenNames;
    }

    private string baseFolderName = "NewModule";
    private NestedFolder[] nestedSubFolders = new NestedFolder[]
    {
        new NestedFolder { parentName = "Scripts", childrenNames = new string[] { "Player", "Gameplay", "UI", "Enemy", "Extra" } },
        new NestedFolder { parentName = "Prefabs", childrenNames = new string[] { "Characters", "Environment", "Weapons" } },
        new NestedFolder { parentName = "Animation", childrenNames = new string[] { "Player", "Enemy" } },
        new NestedFolder { parentName = "Audio", childrenNames = new string[] { "Music", "SFX" } },
        new NestedFolder { parentName = "Imported Assets", childrenNames = new string[] { "Characters", "Decor", "Environments", "Weapons" } },
        new NestedFolder { parentName = "Palettes", childrenNames = new string[] {} },
        new NestedFolder { parentName = "Tiles", childrenNames = new string[] {} },
        new NestedFolder { parentName = "UI", childrenNames = new string[] {} },
          new NestedFolder { parentName = "Level Scenes", childrenNames = new string[] {} }
    };

    private Vector2 scrollPosition;

    [MenuItem("Tools/Folder Creator")]
    public static void ShowWindow()
    {
        GetWindow<FolderSetup>("Folder Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create Project Folders", EditorStyles.boldLabel);

        // Input field for the base folder name
        baseFolderName = EditorGUILayout.TextField("Base Folder Name:", baseFolderName);
        EditorGUILayout.Space();

        GUILayout.Label("Subfolder structure:", EditorStyles.boldLabel);

        // Use a scroll view for the nested folder display
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < nestedSubFolders.Length; i++)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            // Input field for the parent folder name
            nestedSubFolders[i].parentName = EditorGUILayout.TextField("Parent Folder:", nestedSubFolders[i].parentName);

            // Display nested folders with a header
            EditorGUILayout.LabelField("Nested Folders:", EditorStyles.miniBoldLabel);
            EditorGUI.indentLevel++;

            // Loop through and display text fields for children
            for (int j = 0; j < nestedSubFolders[i].childrenNames.Length; j++)
            {
                nestedSubFolders[i].childrenNames[j] = EditorGUILayout.TextField(nestedSubFolders[i].childrenNames[j]);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Create Folders"))
        {
            CreateFolders();
        }
    }

    private void CreateFolders()
    {
        string basePath = "Assets/" + baseFolderName;

        // Create the base folder
        if (!AssetDatabase.IsValidFolder(basePath))
        {
            AssetDatabase.CreateFolder("Assets", baseFolderName);
        }

        // Create the nested folder structure
        foreach (NestedFolder nestedFolder in nestedSubFolders)
        {
            if (!string.IsNullOrEmpty(nestedFolder.parentName))
            {
                string parentFolderPath = Path.Combine(basePath, nestedFolder.parentName);

                // Create the parent subfolder
                if (!AssetDatabase.IsValidFolder(parentFolderPath))
                {
                    AssetDatabase.CreateFolder(basePath, nestedFolder.parentName);
                }

                // Create nested children folders
                foreach (string childFolder in nestedFolder.childrenNames)
                {
                    if (!string.IsNullOrEmpty(childFolder))
                    {
                        string childFolderPath = Path.Combine(parentFolderPath, childFolder);
                        if (!AssetDatabase.IsValidFolder(childFolderPath))
                        {
                            AssetDatabase.CreateFolder(parentFolderPath, childFolder);
                        }
                    }
                }
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("Folders created successfully!");
    }


}
