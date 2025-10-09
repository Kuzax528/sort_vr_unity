using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ProjectOrganizer
{
    static readonly string[] standardFolders = new[]
    {
        "Assets/Scenes",
        "Assets/Prefabs",
        "Assets/Materials",
        "Assets/Textures",
        "Assets/Scripts",
        "Assets/Models",
        "Assets/Audio",
        "Assets/Animations",
        "Assets/Shaders",
        "Assets/Fonts",
        "Assets/Editor",
        "Assets/Documentation"
    };

    [MenuItem("Tools/Project Organizer/Create Standard Folders")]
    public static void CreateStandardFolders()
    {
        foreach (var folder in standardFolders)
        {
            if (!AssetDatabase.IsValidFolder(folder))
            {
                string parent = Path.GetDirectoryName(folder).Replace("\\", "/");
                string newFolderName = Path.GetFileName(folder);
                if (parent == "") parent = "Assets";
                AssetDatabase.CreateFolder(parent, newFolderName);
            }
        }
        AssetDatabase.Refresh();
        Debug.Log("Project Organizer: standard folders created/verified.");
    }

    [MenuItem("Tools/Project Organizer/Organize Assets (Dry Run)")]
    public static void OrganizeAssetsDryRun() => OrganizeAssets(false);

    [MenuItem("Tools/Project Organizer/Organize Assets (Apply)")]
    public static void OrganizeAssetsApply() => OrganizeAssets(true);

    static void OrganizeAssets(bool apply)
    {
        if (apply)
        {
            if (!EditorUtility.DisplayDialog("Organize Assets", "This will move assets into folders (GUIDs preserved). Make a backup or commit first. Continue?", "Yes", "Cancel"))
                return;
        }

        CreateStandardFolders();

        string[] guids = AssetDatabase.FindAssets("", new[] { "Assets" });
        int total = guids.Length;
        int count = 0;

        var moves = new List<(string from, string to)>();

        foreach (var guid in guids)
        {
            count++;
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // skip folders and editor folder
            if (string.IsNullOrEmpty(path) || AssetDatabase.IsValidFolder(path) || path.StartsWith("Assets/Editor") ||
                path.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
                continue;

            EditorUtility.DisplayProgressBar("Project Organizer", $"Scanning {path}", (float)count / total);

            string ext = Path.GetExtension(path).ToLowerInvariant();
            string fileName = Path.GetFileName(path);
            string destFolder = null;

            // extension / type based mapping
            if (ext == ".unity") destFolder = "Assets/Scenes";
            else if (ext == ".prefab") destFolder = "Assets/Prefabs";
            else if (ext == ".mat") destFolder = "Assets/Materials";
            else if (ext == ".controller" || ext == ".anim") destFolder = "Assets/Animations";
            else if (ext == ".shader") destFolder = "Assets/Shaders";
            else if (ext == ".ttf" || ext == ".otf") destFolder = "Assets/Fonts";
            else if (ext == ".cs") destFolder = "Assets/Scripts";
            else if (ext == ".fbx" || ext == ".obj" || ext == ".dae") destFolder = "Assets/Models";
            else if (ext == ".wav" || ext == ".mp3" || ext == ".ogg" || ext == ".aif") destFolder = "Assets/Audio";
            else if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".tga" || ext == ".psd" || ext == ".exr") destFolder = "Assets/Textures";
            else if (ext == ".asmdef") destFolder = "Assets/Scripts";
            else if (ext == ".txt" || ext == ".md") destFolder = "Assets/Documentation";

            // If no mapping, skip (you can add more rules here)
            if (string.IsNullOrEmpty(destFolder)) continue;

            // skip if already in correct folder
            if (path.StartsWith(destFolder + "/", StringComparison.OrdinalIgnoreCase)) continue;

            string targetPath = $"{destFolder}/{fileName}";
            targetPath = AssetDatabase.GenerateUniqueAssetPath(targetPath);
            moves.Add((path, targetPath));
        }

        EditorUtility.ClearProgressBar();

        if (moves.Count == 0)
        {
            Debug.Log("Project Organizer: nothing to move.");
            return;
        }

        // Dry run: log planned moves
        if (!apply)
        {
            Debug.Log($"Project Organizer: Dry Run - {moves.Count} moves planned.");
            foreach (var m in moves)
                Debug.Log($"Would move: {m.from} -> {m.to}");
            return;
        }

        // Apply moves
        int moved = 0;
        foreach (var m in moves)
        {
            string err = AssetDatabase.MoveAsset(m.from, m.to);
            if (!string.IsNullOrEmpty(err))
            {
                Debug.LogError($"Failed to move {m.from} -> {m.to} : {err}");
            }
            else
            {
                moved++;
                Debug.Log($"Moved: {m.from} -> {m.to}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Project Organizer: moved {moved}/{moves.Count} assets.");
    }
}