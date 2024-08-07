using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutpatientCreator : MonoBehaviour
{
    public static int numberOfOutpatient = 0; // ���� �ܷ� ȯ�� ��
    public List<Waypoint> spawnAreas = new List<Waypoint>(); // �ܷ� ȯ�ڰ� ������ ��ġ ����Ʈ
    public float infectionRate = 0.03f; // ���� Ȯ��
    public float spawnDelay = 1f; // ���� ��� �ð�
    private bool isWaiting = false; // ��� ���� �÷���

    // Start�� ó�� �������� ������Ʈ�Ǳ� ���� ȣ��˴ϴ�.
    void Start()
    {
        GameObject go = GameObject.Find("Gateways"); // "Gateways" ������Ʈ ã��
        if (go != null)
        {
            for (int i = 0; i < go.transform.childCount; i++) // �ڽ� ������Ʈ ��ȸ
            {
                Waypoint waypointRange = go.transform.GetChild(i).GetComponent<Waypoint>(); // Waypoint ������Ʈ ��������
                if (waypointRange != null)
                {
                    spawnAreas.Add(waypointRange); // Waypoint ����Ʈ�� �߰�
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
    }

    // Update�� �� �����Ӹ��� ȣ��˴ϴ�.
    void Update()
    {
        // ��� ���� �ƴϰ�, �ܷ� ȯ�� ���� �ִ�ġ���� ���� �� �ܷ� ȯ�� ����
        if (!isWaiting && numberOfOutpatient < ObjectPoolingManager.Instance.maxOfOutpatient)
        {
            StartCoroutine(SpawnOutpatient());
        }
    }

    // �ܷ� ȯ�� ���� �ڷ�ƾ
    IEnumerator SpawnOutpatient()
    {
        isWaiting = true; // ��� ���·� ����
        Vector3 spawnPosition = spawnAreas[Random.Range(0, spawnAreas.Count)].GetRandomPointInRange(); // ���� ���� ��ġ ����
        GameObject newOutpatient = ObjectPoolingManager.Instance.ActivateOutpatient(spawnPosition); // �ܷ� ȯ�� Ȱ��ȭ
        if (newOutpatient != null)
        {
            Person newOutPatientPerson = newOutpatient.GetComponent<Person>(); // Person ������Ʈ ��������
            if (newOutPatientPerson != null)
            {
                // ���� ���� ����
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
                newOutPatientPerson.role = Role.Outpatient; // ���� ����
                numberOfOutpatient++; // �ܷ� ȯ�� �� ����
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

        yield return new WaitForSeconds(spawnDelay); // ��� �ð�
        isWaiting = false; // ��� ���� ����
    }
}
