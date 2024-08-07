using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NurseCreator : MonoBehaviour
{
    public static NurseCreator Instance; // NurseCreator의 싱글톤 인스턴스
    public int numberOfNurse = 0; // 현재 간호사 수

    // Start는 첫 프레임 업데이트 전에 호출됩니다.
    void Start()
    {
        Instance = this; // 싱글톤 인스턴스 설정
        for (int i = 0; i < ObjectPoolingManager.Instance.maxOfNurse; i++) // 최대 간호사 수만큼 루프 실행
        {
            if (i % 20 < 10) // 조건에 따라 특정 간호사만 활성화
            {
                GameObject newNurse = GameObject.Find("Nurse " + i); // 간호사 객체 찾기
                ObjectPoolingManager.Instance.ActivateNurse(newNurse); // 간호사 활성화
                Transform ward = GameObject.Find("NurseWaypoints").transform.Find("Ward (" + i / 20 + ")"); // 병동 찾기

                switch (i % 10) // 간호사의 종류에 따라 웨이포인트 설정
                {
                    case 0:
                        newNurse.GetComponent<NurseController>().waypoints.Add(ward.transform.Find("PatientRoom").GetComponent<Waypoint>()); // 환자실 웨이포인트 추가
                        break;
                    case 1:
                    case 2:
                    case 3:
                        for (int j = 0; j < 5; j++)
                        {
                            newNurse.GetComponent<NurseController>().waypoints.Add(ward.transform.Find("Doctor'sOffice (" + j + ")").GetComponent<Waypoint>()); // 의사실 웨이포인트 추가
                        }
                        break;
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                        newNurse.GetComponent<NurseController>().waypoints.Add(ward.transform.Find("Counter").GetComponent<Waypoint>()); // 카운터 웨이포인트 추가
                        break;
                    case 8:
                    case 9:
                        newNurse.GetComponent<NurseController>().waypoints.Add(GameObject.Find("OutPatientWaypoints").transform.Find("Ward (" + i / 20 + ")").transform.Find("CounterWaypoint (0)").GetComponent<Waypoint>()); // 외래환자 카운터 웨이포인트 추가
                        newNurse.GetComponent<NurseController>().waypoints.Add(GameObject.Find("OutPatientWaypoints").transform.Find("Ward (" + i / 20 + ")").transform.Find("SofaWaypoint (0)").GetComponent<Waypoint>()); // 외래환자 소파 웨이포인트 추가
                        break;
                }
            }
        }
    }

    // Update는 매 프레임 호출됩니다.
    void Update()
    {

    }
}
