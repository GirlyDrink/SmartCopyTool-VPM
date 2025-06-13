using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class SmartCopyTool : EditorWindow
{
    private string sourceFolderPath = "";
    private string destinationFolderName = "";
    private DefaultAsset sourceFolder;
    private Vector2 scrollPosition;
    private List<ReferenceChange> referenceChanges = new List<ReferenceChange>();
    private bool showPreview = false;
    private float[] columnWidths = { 200f, 120f, 200f, 200f, 80f }; // Initial widths
    private int maxPreviewRows = 100; // Default row limit
    private int displayedRows = 100; // Current displayed rows
    private Dictionary<string, GUIContent> contentCache = new Dictionary<string, GUIContent>(); // Cache for GUIContent

    private class ReferenceChange
    {
        public string AssetPath { get; set; }
        public string PropertyName { get; set; }
        public string OldReferencePath { get; set; }
        public string NewReferencePath { get; set; }
        public bool IsValid { get; set; }
    }

    [MenuItem("Tools/GirlyDrink's Tools/Smart Copy Tool")]
    public static void ShowWindow()
    {
        GetWindow<SmartCopyTool>("Smart Copy Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Smart Copy Tool", EditorStyles.boldLabel);
        GUILayout.Label("Copy a folder and update all asset references in all components.", EditorStyles.helpBox);

        // Source folder selection
        sourceFolder = (DefaultAsset)EditorGUILayout.ObjectField(
            "Source Folder", sourceFolder, typeof(DefaultAsset), false);

        if (sourceFolder != null)
        {
            sourceFolderPath = AssetDatabase.GetAssetPath(sourceFolder);
        }
        else
        {
            sourceFolderPath = "";
        }

        // Destination folder name input
        destinationFolderName = EditorGUILayout.TextField("New Folder Name", destinationFolderName);

        // Preview button
        EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(sourceFolderPath) || string.IsNullOrEmpty(destinationFolderName));
        if (GUILayout.Button("Preview Changes"))
        {
            showPreview = true;
            referenceChanges.Clear();
            contentCache.Clear();
            displayedRows = maxPreviewRows;
            CollectReferenceChanges();
        }
        EditorGUI.EndDisabledGroup();

        // Display preview
        if (showPreview && referenceChanges.Count > 0)
        {
            GUILayout.Label("Preview of Reference Changes:", EditorStyles.boldLabel);

            // Row limit input
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Max Rows:", GUILayout.Width(80));
            maxPreviewRows = EditorGUILayout.IntField(maxPreviewRows, GUILayout.Width(60));
            maxPreviewRows = Mathf.Max(1, maxPreviewRows); // Minimum 1 row
            EditorGUILayout.EndHorizontal();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));

            // Table header with resizable columns
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            string[] headers = { "Asset", "Property", "Old Reference", "New Reference", "Status" };
            for (int i = 0; i < columnWidths.Length; i++)
            {
                Rect headerRect = GUILayoutUtility.GetRect(columnWidths[i], 20, GUILayout.Width(columnWidths[i]));
                GUI.Label(headerRect, headers[i], EditorStyles.boldLabel);

                // Resize handle
                Rect resizeRect = new Rect(headerRect.xMax - 2, headerRect.y, 4, headerRect.height);
                EditorGUIUtility.AddCursorRect(resizeRect, MouseCursor.ResizeHorizontal);
                if (Event.current.type == EventType.MouseDrag && resizeRect.Contains(Event.current.mousePosition))
                {
                    columnWidths[i] += Event.current.delta.x;
                    columnWidths[i] = Mathf.Max(50f, columnWidths[i]);
                    Repaint();
                }
            }
            EditorGUILayout.EndHorizontal();

            // Table rows
            int rowCount = Mathf.Min(referenceChanges.Count, displayedRows);
            for (int i = 0; i < rowCount; i++)
            {
                var change = referenceChanges[i];
                EditorGUILayout.BeginHorizontal();
                DrawTableCell(change.AssetPath, columnWidths[0]);
                DrawTableCell(change.PropertyName, columnWidths[1]);
                DrawTableCell(change.OldReferencePath, columnWidths[2]);
                DrawTableCell(change.NewReferencePath, columnWidths[3], !change.IsValid);
                DrawTableCell(change.IsValid ? "Valid" : "Missing", columnWidths[4], !change.IsValid);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            // Show More button
            if (rowCount < referenceChanges.Count)
            {
                if (GUILayout.Button($"Show More ({referenceChanges.Count - rowCount} remaining)"))
                {
                    displayedRows = Mathf.Min(displayedRows + maxPreviewRows, referenceChanges.Count);
                }
            }

            // Confirm and Cancel buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Confirm and Copy"))
            {
                PerformSmartCopy();
                showPreview = false;
                referenceChanges.Clear();
                contentCache.Clear();
            }
            if (GUILayout.Button("Cancel"))
            {
                showPreview = false;
                referenceChanges.Clear();
                contentCache.Clear();
            }
            EditorGUILayout.EndHorizontal();
        }
        else if (showPreview)
        {
            GUILayout.Label("No reference changes found.", EditorStyles.helpBox);
            if (GUILayout.Button("Cancel"))
            {
                showPreview = false;
                referenceChanges.Clear();
                contentCache.Clear();
            }
        }
    }

    private void DrawTableCell(string text, float width, bool isError = false)
    {
        if (!contentCache.TryGetValue(text, out GUIContent content))
        {
            content = new GUIContent(TruncateText(text, width), text);
            contentCache[text] = content;
        }
        Rect cellRect = GUILayoutUtility.GetRect(width, 20, GUILayout.Width(width));
        GUI.Label(cellRect, content, isError ? EditorStyles.boldLabel : EditorStyles.label);
    }

    private string TruncateText(string text, float width)
    {
        if (string.IsNullOrEmpty(text)) return text;
        GUIStyle style = EditorStyles.label;
        float textWidth = style.CalcSize(new GUIContent(text)).x;
        if (textWidth <= width - 10) return text;

        int len = text.Length;
        while (len > 0 && style.CalcSize(new GUIContent(text.Substring(0, len) + "...")).x > width - 10)
            len--;
        return len > 0 ? text.Substring(0, len) + "..." : "...";
    }

    private void CollectReferenceChanges()
    {
        if (!AssetDatabase.IsValidFolder(sourceFolderPath))
        {
            Debug.LogError("Selected source is not a valid folder.");
            showPreview = false;
            return;
        }

        string sourceFolderName = Path.GetFileName(sourceFolderPath);
        string parentFolder = Path.GetDirectoryName(sourceFolderPath);
        string destinationFolderPath = Path.Combine(parentFolder, destinationFolderName);

        // Get all assets in the source folder
        string[] assetGuids = AssetDatabase.FindAssets("", new[] { sourceFolderPath });

        foreach (string guid in assetGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            if (asset == null) continue;

            if (asset is GameObject go)
            {
                CollectGameObjectReferenceChanges(go, sourceFolderPath, destinationFolderPath);
            }
            else
            {
                CollectSerializedObjectReferenceChanges(asset, sourceFolderPath, destinationFolderPath);
            }
        }
    }

    private void CollectGameObjectReferenceChanges(GameObject go, string sourceFolderPath, string destinationFolderPath)
    {
        // Process the GameObject itself
        SerializedObject serializedObject = new SerializedObject(go);
        CollectSerializedPropertyChanges(serializedObject, sourceFolderPath, destinationFolderPath);

        // Process all components
        Component[] components = go.GetComponents<Component>();
        foreach (Component component in components)
        {
            if (component == null) continue;
            SerializedObject serializedComponent = new SerializedObject(component);
            CollectSerializedPropertyChanges(serializedComponent, sourceFolderPath, destinationFolderPath);
        }

        // Recursively process all children
        foreach (Transform child in go.transform)
        {
            CollectGameObjectReferenceChanges(child.gameObject, sourceFolderPath, destinationFolderPath);
        }
    }

    private void CollectSerializedObjectReferenceChanges(Object asset, string sourceFolderPath, string destinationFolderPath)
    {
        SerializedObject serializedObject = new SerializedObject(asset);
        CollectSerializedPropertyChanges(serializedObject, sourceFolderPath, destinationFolderPath);
    }

    private void CollectSerializedPropertyChanges(SerializedObject serializedObject, string sourceFolderPath, string destinationFolderPath)
    {
        SerializedProperty properties = serializedObject.GetIterator();
        while (properties.NextVisible(true))
        {
            if (properties.propertyType == SerializedPropertyType.ObjectReference && properties.objectReferenceValue != null)
            {
                string referencedAssetPath = AssetDatabase.GetAssetPath(properties.objectReferenceValue);
                if (!string.IsNullOrEmpty(referencedAssetPath) && referencedAssetPath.StartsWith(sourceFolderPath))
                {
                    string relativePath = referencedAssetPath.Substring(sourceFolderPath.Length + 1);
                    string newAssetPath = Path.Combine(destinationFolderPath, relativePath);
                    bool isValid = File.Exists(Path.Combine(Application.dataPath, newAssetPath.Substring(7))); // Remove "Assets/" prefix

                    referenceChanges.Add(new ReferenceChange
                    {
                        AssetPath = AssetDatabase.GetAssetPath(serializedObject.targetObject),
                        PropertyName = properties.propertyPath,
                        OldReferencePath = referencedAssetPath,
                        NewReferencePath = newAssetPath,
                        IsValid = isValid
                    });
                }
            }
        }
    }

    private void PerformSmartCopy()
    {
        if (!AssetDatabase.IsValidFolder(sourceFolderPath))
        {
            Debug.LogError("Selected source is not a valid folder.");
            return;
        }

        // Determine destination path
        string sourceFolderName = Path.GetFileName(sourceFolderPath);
        string parentFolder = Path.GetDirectoryName(sourceFolderPath);
        string destinationFolderPath = Path.Combine(parentFolder, destinationFolderName);

        // Ensure destination folder is unique
        if (AssetDatabase.IsValidFolder(destinationFolderPath))
        {
            Debug.LogError($"Destination folder '{destinationFolderPath}' already exists. Please choose a different name.");
            return;
        }

        // Copy the folder
        if (!AssetDatabase.CopyAsset(sourceFolderPath, destinationFolderPath))
        {
            Debug.LogError($"Failed to copy folder from '{sourceFolderPath}' to '{destinationFolderPath}'.");
            return;
        }

        AssetDatabase.Refresh();

        // Update asset references
        UpdateAssetReferences(sourceFolderPath, destinationFolderPath);

        Debug.Log($"Successfully copied '{sourceFolderPath}' to '{destinationFolderPath}' and updated all asset references.");
    }

    private void UpdateAssetReferences(string sourceFolderPath, string destinationFolderPath)
    {
        // Get all assets in the destination folder
        string[] assetGuids = AssetDatabase.FindAssets("", new[] { destinationFolderPath });
        List<Object> modifiedAssets = new List<Object>();

        foreach (string guid in assetGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            if (asset == null) continue;

            bool modified = false;

            // Handle GameObjects (prefabs) and their hierarchies
            if (asset is GameObject go)
            {
                modified |= UpdateGameObjectReferences(go, sourceFolderPath, destinationFolderPath);
            }
            // Handle other assets (materials, animations, etc.)
            else
            {
                modified |= UpdateSerializedObjectReferences(asset, sourceFolderPath, destinationFolderPath);
            }

            if (modified && !modifiedAssets.Contains(asset))
            {
                EditorUtility.SetDirty(asset);
                modifiedAssets.Add(asset);
            }
        }

        AssetDatabase.SaveAssets();
    }

    private bool UpdateGameObjectReferences(GameObject go, string sourceFolderPath, string destinationFolderPath)
    {
        bool modified = false;

        // Process the GameObject itself
        SerializedObject serializedObject = new SerializedObject(go);
        modified |= UpdateSerializedProperties(serializedObject, sourceFolderPath, destinationFolderPath);

        // Process all components
        Component[] components = go.GetComponents<Component>();
        foreach (Component component in components)
        {
            if (component == null) continue;
            SerializedObject serializedComponent = new SerializedObject(component);
            modified |= UpdateSerializedProperties(serializedComponent, sourceFolderPath, destinationFolderPath);
        }

        // Recursively process all children
        foreach (Transform child in go.transform)
        {
            modified |= UpdateGameObjectReferences(child.gameObject, sourceFolderPath, destinationFolderPath);
        }

        return modified;
    }

    private bool UpdateSerializedObjectReferences(Object asset, string sourceFolderPath, string destinationFolderPath)
    {
        SerializedObject serializedObject = new SerializedObject(asset);
        return UpdateSerializedProperties(serializedObject, sourceFolderPath, destinationFolderPath);
    }

    private bool UpdateSerializedProperties(SerializedObject serializedObject, string sourceFolderPath, string destinationFolderPath)
    {
        bool modified = false;
        SerializedProperty properties = serializedObject.GetIterator();

        while (properties.NextVisible(true))
        {
            if (properties.propertyType == SerializedPropertyType.ObjectReference && properties.objectReferenceValue != null)
            {
                string referencedAssetPath = AssetDatabase.GetAssetPath(properties.objectReferenceValue);
                if (!string.IsNullOrEmpty(referencedAssetPath) && referencedAssetPath.StartsWith(sourceFolderPath))
                {
                    string relativePath = referencedAssetPath.Substring(sourceFolderPath.Length + 1);
                    string newAssetPath = Path.Combine(destinationFolderPath, relativePath);
                    Object newAsset = AssetDatabase.LoadAssetAtPath<Object>(newAssetPath);

                    if (newAsset != null)
                    {
                        properties.objectReferenceValue = newAsset;
                        modified = true;
                    }
                    else
                    {
                        Debug.LogWarning($"Could not find asset at '{newAssetPath}' for object '{serializedObject.targetObject.name}'.");
                    }
                }
            }
        }

        if (modified)
        {
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        return modified;
    }
}