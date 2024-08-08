using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingManager
{
    // 최대 외래 환자, 의사, 간호사, 입원 환자 수
    public int maxOfOutpatient = 100;
    public int maxOfDoctor = 30;
    public int maxOfNurse = 120;
    public int maxOfInpatient = 36;

    // 비활성화된 외래 환자 오브젝트를 저장하는 큐
    public Queue<GameObject> outpatientQueue = new Queue<GameObject>();


    public void Init()
    {
        // 의사, 외래 환자, 간호사 초기화
        DoctorInitialize();
        OutpatientInitialize();
        NurseInitialize();
        InpatientInitaialize();
    }

    // 외래 환자 초기화
    private void OutpatientInitialize()
    {
        // 외래 환자 프리팹 로드
        GameObject[] OutpatientPrefabs = Resources.LoadAll<GameObject>("Prefabs/Outpatient");
        for (int i = 0; i < maxOfOutpatient; i++)
        {
            // 프리팹 리스트에서 랜덤으로 하나 선택하여 생성
            GameObject newOutPatient = Object.Instantiate(OutpatientPrefabs[Random.Range(0, OutpatientPrefabs.Length)]);
            outpatientQueue.Enqueue(newOutPatient); // 큐에 추가
            newOutPatient.SetActive(false); // 비활성화
        }
    }

    // 의사 초기화
    private void DoctorInitialize()
    {
        // 의사 프리팹 로드
        GameObject[] DoctorPrefabs = Resources.LoadAll<GameObject>("Prefabs/Doctor");
        for (int i = 0; i < maxOfDoctor; i++)
        {
            int ward = i / 5;
            // 의사 스폰 위치 설정
            DoctorOffice spawnArea = Managers.NPCManager.waypointDictionary[(ward, "DoctorWaypoints")].Find("Doctor'sOffice (" + (i % 5) + ")").GetComponent<DoctorOffice>();
            //DoctorOffice spawnArea = GameObject.Find("DoctorWaypoints").transform.Find("Ward (" + (i / 5) + ")").transform.Find("Doctor'sOffice (" + (i % 5) + ")").GetComponent<DoctorOffice>();
            // 프리팹 리스트에서 랜덤으로 하나 선택하여 생성
            GameObject newDoctor = Object.Instantiate(DoctorPrefabs[Random.Range(0, DoctorPrefabs.Length)], spawnArea.GetRandomPointInRange(), Quaternion.identity);
            newDoctor.name = "Doctor " + i;
            DoctorController doctorController = newDoctor.GetComponent<DoctorController>();

            // 의사 사무실 할당
            doctorController.waypoints.Add(spawnArea);

            // 외래 환자 대기 구역 할당
            DoctorOffice waypoint = Managers.NPCManager.waypointDictionary[(ward, "OutpatientWaypoints")].Find("Doctor'sOffice (" + (i % 5) + ")").GetComponent<DoctorOffice>();
            doctorController.waypoints.Add(waypoint);
            newDoctor.transform.position = spawnArea.transform.position;
            newDoctor.GetComponent<SkinnedMeshRenderer>().enabled = false;
            doctorController.isResting = true;

            spawnArea.doctor = newDoctor;
            waypoint.doctor = newDoctor;
        }
    }

    // 간호사 초기화
    private void NurseInitialize()
    {
        // 간호사 프리팹 로드
        GameObject[] NursePrefabs = Resources.LoadAll<GameObject>("Prefabs/Nurse");
        for (int i = 0; i < maxOfNurse; i++)
        {
            // 간호사 스폰 위치 설정
            int ward = i / 20;
            Waypoint spawnArea = Managers.NPCManager.waypointDictionary[(ward, "NurseWaypoints")].Find("NurseSpawnArea").gameObject.GetComponent<Waypoint>();
            GameObject newNurse = Object.Instantiate(NursePrefabs[Random.Range(0, NursePrefabs.Length)], spawnArea.GetRandomPointInRange(), Quaternion.identity);
            newNurse.name = "Nurse " + i;
            newNurse.GetComponent<NurseController>().ward = ward;
            newNurse.GetComponent<NurseController>().isRest = true;
            newNurse.GetComponent<SkinnedMeshRenderer>().enabled = false;
        }
    }


    private void InpatientInitaialize()
    {
        GameObject[] InpatientPrefabs = Resources.LoadAll<GameObject>("Prefabs/Inpatient");
        for (int i = 0; i < maxOfInpatient; i++)
        {
            // 입원 환자 스폰 위치 설정
            int ward = i / 6;
            BedWaypoint spawnArea = Managers.NPCManager.waypointDictionary[(ward, "InpatientWaypoints")].Find("BedWaypoint (" + (i % 6) + ")").gameObject.GetComponent<BedWaypoint>();
            GameObject newInpatient = Object.Instantiate(InpatientPrefabs[Random.Range(0, InpatientPrefabs.Length)], spawnArea.GetRandomPointInRange(), Quaternion.identity);
            newInpatient.name = "Inpatient " + i;
            spawnArea.inpatient = newInpatient;
            InpatientController newInpatientController = newInpatient.GetComponent<InpatientController>();
            newInpatientController.bedWaypoint = spawnArea.gameObject;
            newInpatientController.ward = ward;
            newInpatient.GetComponent<Person>().role = Role.Inpatient;
        }
    }

    // 외래 환자 비활성화 및 초기화
    public void DeactivateOutpatient(GameObject outpatient)
    {
        outpatient.GetComponent<Person>().status = InfectionState.Normal; // 감염 상태 초기화
        OutpatientController outpatientController = outpatient.GetComponent<OutpatientController>();
        outpatientController.waypoints.Clear(); // 웨이포인트 초기화
        outpatientController.isWaiting = false; // 대기 상태 초기화
        outpatientController.waypointIndex = 0; // 웨이포인트 인덱스 초기화
        outpatientController.doctorSignal = false; // 의사 신호 초기화
        outpatientController.nurseSignal = false; // 간호사 신호 초기화
        outpatientController.officeSignal = false; // 진료실 신호 초기화
        outpatientQueue.Enqueue(outpatient); // 큐에 추가
        outpatient.SetActive(false); // 비활성화
    }

    // 외래 환자 활성화 및 위치 설정
    public GameObject ActivateOutpatient(Vector3 position)
    {
        GameObject newOutpatient = outpatientQueue.Dequeue(); // 큐에서 외래 환자 가져오기
        newOutpatient.transform.position = position; // 위치 설정
        newOutpatient.SetActive(true); // 활성화
        return newOutpatient;
    }

    // 의사 비활성화 및 초기화
    public void DeactivateDoctor(GameObject doctor)
    {
        DoctorController doctorController = doctor.GetComponent<DoctorController>();
        doctorController.patientCount = 0; // 환자 수 초기화
        doctorController.isWaiting = false; // 대기 상태 초기화
        doctorController.isResting = true; // 휴식 상태 설정
        doctorController.changeSignal = false; // 신호 초기화
        doctor.GetComponent<SkinnedMeshRenderer>().enabled = false; // 렌더러 비활성화
    }

    // 의사 활성화 및 위치 설정
    public GameObject ActivateDoctor(GameObject newDoctor)
    {
        DoctorController doctorController = newDoctor.GetComponent<DoctorController>();
        doctorController.changeSignal = true; // 신호 설정
        newDoctor.transform.position = doctorController.waypoints[0].GetRandomPointInRange(); // 위치 설정
        newDoctor.GetComponent<SkinnedMeshRenderer>().enabled = true; // 렌더러 활성화
        doctorController.isResting = false; // 휴식 상태 해제
        return newDoctor;
    }

    // 간호사 활성화
    public GameObject ActivateNurse(GameObject newNurse)
    {
        newNurse.GetComponent<SkinnedMeshRenderer>().enabled = true; // 렌더러 활성화
        newNurse.GetComponent<NurseController>().isRest = false;
        return newNurse;
    }

    public void DeactivateNurse(GameObject nurse)
    {
        nurse.GetComponent<SkinnedMeshRenderer>().enabled = false; // 렌더러 활성화
        NurseController nurseController = nurse.GetComponent<NurseController>();
        nurseController.isRest = true;
        nurseController.isWaiting = false;
        nurseController.isWaitingAtDoctorOffice = false;
        nurseController.isWorking = false;
    }
}
