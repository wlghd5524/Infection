using UnityEngine;
using UnityEditor;
using System.IO;

public class PrefabModify : MonoBehaviour
{
    [MenuItem("Tools/Transfer SkinnedMeshRenderer In Folder")]
    public static void TransferSkinnedMeshRendererInFolder()
    {
        string folderPath = "Assets/Resources/Prefabs/Nurse"; // ������ ���� ��θ� �����ϼ���.

        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (prefab != null)
            {
                bool isModified = false;

                // Prefab�� �ν��Ͻ��� �����Ͽ� �۾��� ����
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

                // �θ� ������Ʈ�� ���� ��� �ڽĵ��� Ȯ��
                foreach (Transform child in instance.transform)
                {
                    SkinnedMeshRenderer skinnedMeshRenderer = child.GetComponent<SkinnedMeshRenderer>();

                    if (skinnedMeshRenderer != null)
                    {
                        // �θ� ������Ʈ�� SkinnedMeshRenderer �߰�
                        SkinnedMeshRenderer parentSkinnedMeshRenderer = instance.AddComponent<SkinnedMeshRenderer>();

                        // SkinnedMeshRenderer�� �Ӽ� ����
                        parentSkinnedMeshRenderer.sharedMesh = skinnedMeshRenderer.sharedMesh;
                        parentSkinnedMeshRenderer.materials = skinnedMeshRenderer.sharedMaterials;
                        parentSkinnedMeshRenderer.bones = skinnedMeshRenderer.bones;
                        parentSkinnedMeshRenderer.rootBone = skinnedMeshRenderer.rootBone;

                        // �ڽ� ������Ʈ ����
                        DestroyImmediate(child.gameObject);

                        isModified = true;
                    }
                }

                if (isModified)
                {
                    // Prefab ����
                    PrefabUtility.SaveAsPrefabAsset(instance, assetPath);
                    Debug.Log($"Modified and saved prefab: {assetPath}");
                }

                // �ν��Ͻ� ����
                DestroyImmediate(instance);
            }
        }

        Debug.Log("SkinnedMeshRenderer transfer completed in folder.");
    }
}
