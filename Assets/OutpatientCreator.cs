using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutpatientCreator : MonoBehaviour
{
    public static int numberOfOutpatient = 0;
    public List<Waypoint> spawnAreas = new List<Waypoint>();
    public float infectionRate = 0.03f;
    public float spawnDelay = 1f; // 대기 시간을 설정 가능하게 만듦
    private bool isWaiting = false;

    // Start는 처음 프레임이 업데이트되기 전에 호출됩니다.
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
                    Debug.LogError("Gateways 자식 오브젝트에 Waypoint 컴포넌트가 없습니다.");
                }
            }
        }
        else
        {
            Debug.LogError("Gateways 게임 오브젝트를 찾을 수 없습니다.");
        }
        InvokeRepeating("spawnOutpatient", 0.1f, spawnDelay);
    }

    // Update는 매 프레임마다 호출됩니다.
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
                Debug.LogError("새 외래 환자에 Person 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.LogError("새 외래 환자를 활성화하는 데 실패했습니다.");
        }
        
        yield return new WaitForSeconds(spawnDelay);
        isWaiting = false;

    }
}
