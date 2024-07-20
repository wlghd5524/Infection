using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class PrefabModify : EditorWindow
{
    private GameObject objectToAdd;
    private string folderPath = "Prefabs"; // Resources 폴더 내부의 상대 경로

    [MenuItem("Tools/Add Object to Prefabs in Folder")]
    static void Init()
    {
        PrefabModify window = (PrefabModify)EditorWindow.GetWindow(typeof(PrefabModify));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Add Object to Prefabs in Resources Folder", EditorStyles.boldLabel);

        objectToAdd = (GameObject)EditorGUILayout.ObjectField("Object to Add", objectToAdd, typeof(GameObject), true);

        EditorGUILayout.Space();

        folderPath = EditorGUILayout.TextField("Folder Path (in Resources)", folderPath);

        EditorGUILayout.Space();

        if (GUILayout.Button("Add Object to Prefabs"))
        {
            AddObjectToPrefabsInFolder();
        }
    }

    void AddObjectToPrefabsInFolder()
    {
        if (objectToAdd == null)
        {
            Debug.LogError("Object to add is not assigned!");
            return;
        }

        string fullPath = Path.Combine(Application.dataPath, "Resources", folderPath);
        if (!Directory.Exists(fullPath))
        {
            Debug.LogError($"Folder not found: {fullPath}");
            return;
        }

        string[] prefabPaths = Directory.GetFiles(fullPath, "*.prefab", SearchOption.AllDirectories);

        foreach (string prefabPath in prefabPaths)
        {
            string assetPath = "Assets" + prefabPath.Substring(Application.dataPath.Length);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (prefab == null)
            {
                Debug.LogError($"Failed to load prefab at path: {assetPath}");
                continue;
            }

            GameObject prefabInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            GameObject newObject = Instantiate(objectToAdd, prefabInstance.transform);
            newObject.name = objectToAdd.name;

            PrefabUtility.SaveAsPrefabAsset(prefabInstance, assetPath);
            DestroyImmediate(prefabInstance);

            Debug.Log($"Object added to prefab: {assetPath}");
        }

        AssetDatabase.Refresh();
        Debug.Log($"Object added to all prefabs in the specified folder successfully!");
    }
}