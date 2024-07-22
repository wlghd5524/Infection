using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutpatientCreator : MonoBehaviour
{
    public static int numberOfOutpatient = 0;
    public List<Waypoint> spawnAreas = new List<Waypoint>();
    public float infectionRate = 0.03f;
    public float spawnDelay = 1f; // ��� �ð��� ���� �����ϰ� ����
    private bool isWaiting = false;

    // Start�� ó�� �������� ������Ʈ�Ǳ� ���� ȣ��˴ϴ�.
    void Start()
    {
        GameObject go = GameObject.Find("Gateways");
        if (go != null)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Waypoint waypointRange = go.transform.GetChild(i).GetComponent<Waypoint>();
                if (waypointRange != null)
                {
                    spawnAreas.Add(waypointRange);
                }
                else
                {
                    Debug.LogError("Gateways �ڽ� ������Ʈ�� Waypoint ������Ʈ�� �����ϴ�.");
                }
            }
        }
        else
        {
            Debug.LogError("Gateways ���� ������Ʈ�� ã�� �� �����ϴ�.");
        }
        InvokeRepeating("spawnOutpatient", 0.1f, spawnDelay);
    }

    // Update�� �� �����Ӹ��� ȣ��˴ϴ�.
    void Update()
    {
        if (!isWaiting && numberOfOutpatient < ObjectPoolingManager.Instance.maxOfOutpatient)
        {
            StartCoroutine(SpawnOutpatient());
        }
    } 

    IEnumerator SpawnOutpatient()
    {
        isWaiting = true;
        Vector3 spawnPosition = spawnAreas[Random.Range(0, spawnAreas.Count)].GetRandomPointInRange();
        GameObject newOutpatient = ObjectPoolingManager.Instance.ActivateOutpatient(spawnPosition);
        if (newOutpatient != null)
        {
            Person newOutPatientPerson = newOutpatient.GetComponent<Person>();
            if (newOutPatientPerson != null)
            {
                if (Random.value < infectionRate)
                {
                    if (StageManager.Instance.stage == 1)
                    {
                        newOutPatientPerson.status = InfectionState.Stage1;
                    }
                    else if (StageManager.Instance.stage == 2)
                    {
                        newOutPatientPerson.status = InfectionState.Stage2;
                    }
                }
                newOutPatientPerson.role = Role.Outpatient;
                numberOfOutpatient++;
            }
            else
            {
                Debug.LogError("�� �ܷ� ȯ�ڿ� Person ������Ʈ�� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogError("�� �ܷ� ȯ�ڸ� Ȱ��ȭ�ϴ� �� �����߽��ϴ�.");
        }
        
        yield return new WaitForSeconds(spawnDelay);
        isWaiting = false;

    }
}
