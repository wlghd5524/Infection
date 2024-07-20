using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public class DoctorCreator : MonoBehaviour
{
    public static DoctorCreator Instance;
    public int numberOfDoctor = 0;
    private List<GameObject> rootObjects = new List<GameObject>();
    private int[] doctorCount = { 3, 3 };

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        for (int i = 0; i < doctorCount.Length; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                ObjectPoolingManager.Instance.ActivateDoctor(GameObject.Find("Doctor " + ((i * 5) + j)));
            }

        }
    }

    public void ChangeDoctor(GameObject endDoctor)
    {
        string name = endDoctor.name;
        int num = name[name.Length - 1] - '0';

        for (int i = 0; i < 5; i++)
        {
            GameObject newDoctor = GameObject.Find("Doctor " + doctorCount[num / 5]++ % 5);
            if (newDoctor == null)
            {
                Debug.LogError("새로운 닥터를 찾을 수 없습니다.");
            }
            if (!newDoctor.GetComponent<DoctorController>().isResting)
            {
                continue;
            }
            ObjectPoolingManager.Instance.DeactivateDoctor(endDoctor);
            ObjectPoolingManager.Instance.ActivateDoctor(newDoctor);
            break;
        }
    }
}
