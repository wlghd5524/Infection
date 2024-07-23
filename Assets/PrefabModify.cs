using UnityEngine;
using UnityEditor;
using System.IO;

public class PrefabModify : MonoBehaviour
{
    [MenuItem("Tools/Transfer SkinnedMeshRenderer In Folder")]
    public static void TransferSkinnedMeshRendererInFolder()
    {
        string folderPath = "Assets/Resources/Prefabs/Nurse"; // 변경할 폴더 경로를 지정하세요.

        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (prefab != null)
            {
                bool isModified = false;

                // Prefab의 인스턴스를 생성하여 작업을 수행
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

                // 부모 오브젝트가 가진 모든 자식들을 확인
                foreach (Transform child in instance.transform)
                {
                    SkinnedMeshRenderer skinnedMeshRenderer = child.GetComponent<SkinnedMeshRenderer>();

                    if (skinnedMeshRenderer != null)
                    {
                        // 부모 오브젝트에 SkinnedMeshRenderer 추가
                        SkinnedMeshRenderer parentSkinnedMeshRenderer = instance.AddComponent<SkinnedMeshRenderer>();

                        // SkinnedMeshRenderer의 속성 복사
                        parentSkinnedMeshRenderer.sharedMesh = skinnedMeshRenderer.sharedMesh;
                        parentSkinnedMeshRenderer.materials = skinnedMeshRenderer.sharedMaterials;
                        parentSkinnedMeshRenderer.bones = skinnedMeshRenderer.bones;
                        parentSkinnedMeshRenderer.rootBone = skinnedMeshRenderer.rootBone;

                        // 자식 오브젝트 삭제
                        DestroyImmediate(child.gameObject);

                        isModified = true;
                    }
                }

                if (isModified)
                {
                    // Prefab 저장
                    PrefabUtility.SaveAsPrefabAsset(instance, assetPath);
                    Debug.Log($"Modified and saved prefab: {assetPath}");
                }

                // 인스턴스 삭제
                DestroyImmediate(instance);
            }
        }

        Debug.Log("SkinnedMeshRenderer transfer completed in folder.");
    }
}
