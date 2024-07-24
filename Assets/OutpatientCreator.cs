using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutpatientCreator : MonoBehaviour
{
    public static int numberOfOutpatient = 0; // 현재 외래 환자 수
    public List<Waypoint> spawnAreas = new List<Waypoint>(); // 외래 환자가 생성될 위치 리스트
    public float infectionRate = 0.03f; // 감염 확률
    public float spawnDelay = 1f; // 생성 대기 시간
    private bool isWaiting = false; // 대기 상태 플래그

    // Start는 처음 프레임이 업데이트되기 전에 호출됩니다.
    void Start()
    {
        GameObject go = GameObject.Find("Gateways"); // "Gateways" 오브젝트 찾기
        if (go != null)
        {
            for (int i = 0; i < go.transform.childCount; i++) // 자식 오브젝트 순회
            {
                Waypoint waypointRange = go.transform.GetChild(i).GetComponent<Waypoint>(); // Waypoint 컴포넌트 가져오기
                if (waypointRange != null)
                {
                    spawnAreas.Add(waypointRange); // Waypoint 리스트에 추가
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
    }

    // Update는 매 프레임마다 호출됩니다.
    void Update()
    {
        // 대기 중이 아니고, 외래 환자 수가 최대치보다 적을 때 외래 환자 생성
        if (!isWaiting && numberOfOutpatient < ObjectPoolingManager.Instance.maxOfOutpatient)
        {
            StartCoroutine(SpawnOutpatient());
        }
    }

    // 외래 환자 생성 코루틴
    IEnumerator SpawnOutpatient()
    {
        isWaiting = true; // 대기 상태로 설정
        Vector3 spawnPosition = spawnAreas[Random.Range(0, spawnAreas.Count)].GetRandomPointInRange(); // 랜덤 생성 위치 설정
        GameObject newOutpatient = ObjectPoolingManager.Instance.ActivateOutpatient(spawnPosition); // 외래 환자 활성화
        if (newOutpatient != null)
        {
            Person newOutPatientPerson = newOutpatient.GetComponent<Person>(); // Person 컴포넌트 가져오기
            if (newOutPatientPerson != null)
            {
                // 감염 상태 설정
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
                newOutPatientPerson.role = Role.Outpatient; // 역할 설정
                numberOfOutpatient++; // 외래 환자 수 증가
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

        yield return new WaitForSeconds(spawnDelay); // 대기 시간
        isWaiting = false; // 대기 상태 해제
    }
}
