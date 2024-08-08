using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NPCClickManager 클래스는 NPC(여기서는 간호사)를 검색하고, 마스크를 착용하거나 격리하는 등의 작업을 수행합니다.
public class NPCClickManager : MonoBehaviour
{
    // 박스캐스트의 거리 설정
    public float boxCastDistance = 100f;  // 박스캐스트 거리
    // 박스캐스트의 크기 설정
    public Vector3 boxCastSize = new Vector3(100f, 1f, 100f); // 박스캐스트 크기
    // NPC의 태그를 설정 (여기서는 'Nurse')
    public string npcTag = "Nurse"; // Nurse 태그

    // 간호사를 검색하는 메서드, origin 위치에서 가장 가까운 간호사를 찾습니다.
    public GameObject SearchNurse(Vector3 origin)
    {
        Transform closestNurse = null;
        float closestDistance = Mathf.Infinity;
        // 'Nurse' 태그를 가진 모든 게임 오브젝트를 찾습니다.
        GameObject[] nurses = GameObject.FindGameObjectsWithTag(npcTag);

        foreach (GameObject nurse in nurses)
        {
            // 각 간호사의 NurseController 컴포넌트를 가져옵니다.
            NurseController nurseController = nurse.GetComponent<NurseController>();
            // 간호사가 근무 중이고 의사 사무실에서 대기 중이면 건너뜁니다.
            if (nurseController.isWorking || nurseController.isWaitingAtDoctorOffice || nurseController.isRest)
            {
                continue;
            }
            // 간호사가 같은 층에 있는지 확인합니다.
            if (Mathf.Abs(origin.y - nurse.transform.position.y) <= 1.0f)
            {
                // origin과 간호사 사이의 거리를 계산합니다.
                float distance = Vector3.Distance(origin, nurse.transform.position);
                // 가장 가까운 간호사를 업데이트합니다.
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNurse = nurse.transform;
                }
            }
        }
        // 가장 가까운 간호사가 있으면 로그에 이름을 출력합니다.
        if (closestNurse != null)
        {
            Person person = closestNurse.GetComponent<Person>();
            if (person != null)
            {
                Debug.Log("Closest Nurse found: " + person.gameObject.name);
            }
        }
        else
        {
            Debug.Log("No Nurse found.");
        }
        // 가장 가까운 간호사의 게임 오브젝트를 반환합니다.
        return closestNurse.gameObject;
    }

    // 가장 가까운 간호사가 마스크를 착용하도록 지시하는 메서드
    public void WearingMask(GameObject closestNurse)
    {
        // OutpatientController 컴포넌트를 가져와서 nurseSignal을 false로 설정하고 코루틴을 시작합니다.
        OutpatientController outpatientController = gameObject.GetComponent<OutpatientController>();
        outpatientController.nurseSignal = false;
        outpatientController.StartCoroutine(outpatientController.WaitForNurse());
        // 간호사의 NurseController 컴포넌트를 가져옵니다.
        NurseController nurseController = closestNurse.GetComponent<NurseController>();
        if (nurseController == null)
        {
            Debug.LogError("nurseController를 찾을 수 없습니다.");
        }
        else
        {
            // 간호사가 환자에게 가도록 지시합니다.
            nurseController.StartCoroutine(nurseController.GoToPatient(gameObject));
        }
    }

    // 가장 가까운 간호사를 격리실로 보내는 메서드
    public void Quarantine(GameObject closestNurse)
    {
        // OutpatientController 컴포넌트를 가져와서 nurseSignal을 false로 설정하고 코루틴을 시작합니다.
        OutpatientController outpatientController = gameObject.GetComponent<OutpatientController>();
        outpatientController.nurseSignal = false;
        outpatientController.StartCoroutine(outpatientController.WaitForNurse());
        // 간호사의 NurseController 컴포넌트를 가져옵니다.
        NurseController nurseController = closestNurse.GetComponent<NurseController>();
        if (nurseController == null)
        {
            Debug.LogError("nurseController를 찾을 수 없습니다.");
        }
        else
        {
            // 간호사가 격리실로 가도록 지시합니다.
            nurseController.GoToNegativePressureRoom(gameObject);
        }
    }
}
