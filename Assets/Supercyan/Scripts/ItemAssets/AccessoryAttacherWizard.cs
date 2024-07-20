#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class AccessoryAttacherWizard : ScriptableWizard
{
    [SerializeField] private AccessoryWearLogic m_accessoryWearer = default;
    [SerializeField] private AccessoryObject m_accessoryObject = default;

    [SerializeField] private bool m_keepAccessoryPrefabConnection = false;

    [MenuItem("Tools/Supercyan/Accessory Attacher")]
    private static void CreateWizard()
    {
        DisplayWizard<AccessoryAttacherWizard>("Accessory Attacher");
    }

    private void OnWizardCreate()
    {
        AttachAccessory(m_accessoryObject, m_accessoryWearer, m_keepAccessoryPrefabConnection);
    }

    private static void AttachAccessory(AccessoryObject accessoryObject, AccessoryWearLogic accessoryWearer, bool keepAccessoryPrefabConnection)
    {
        if (EditorApplication.isPlaying)
        {
            if (keepAccessoryPrefabConnection == true)
            {
                keepAccessoryPrefabConnection = false;
                Debug.LogError("Accessory Attacher: keepAccessoryPrefabConnection is not supported in playmode");
            }
        }

        AccessoryLogic accessory = Instantiate(accessoryObject.Accessory, keepAccessoryPrefabConnection);
        accessory.transform.parent = accessoryWearer.transform;
        accessoryWearer.Attach(accessory);
    }

    private static T Instantiate<T>(T component, bool keepPrefabConnection) where T : Component
    {
        if (component == null) { return null; }
        GameObject instance = Instantiate(component.gameObject, keepPrefabConnection);
        return instance.GetComponent<T>();
    }

    private static GameObject Instantiate(GameObject original, bool keepPrefabConnection)
    {
        if (keepPrefabConnection == true)
        {
            return PrefabUtility.InstantiatePrefab(original) as GameObject;
        }
        else
        {
            return Instantiate(original);
        }
    }

    private void OnWizardUpdate()
    {
        helpString = "Used to attach accessories to characters in scene.";
        isValid = m_accessoryWearer != null && m_accessoryObject != null;
    }
}

#endif
