using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static ObjectPoolingManager Instance;

    // 최대 외래 환자와 의사 수
    public int maxOfOutpatient;
    public int maxOfDoctor;
    public int maxOfNurse;

    // 비활성화된 외래 환자와 의사 오브젝트를 저장하는 큐
    public Queue<GameObject> outpatientQueue = new Queue<GameObject>();
    public Queue<GameObject> doctorQueue = new Queue<GameObject>();

    void Awake()
    {
        // 싱글톤 패턴 구현
        Instance = this;
        // 의사와 외래 환자 초기화
        DoctorInitialize();
        OutpatientInitialize();
        NurseInitialize();
    }

    private void OutpatientInitialize()
    {
        // 외래 환자 프리팹 로드
        GameObject[] OutpatientPrefabs = Resources.LoadAll<GameObject>("Prefabs/Outpatient");
        for (int i = 0; i < maxOfOutpatient; i++)
        {
            // 프리팹 리스트에서 랜덤으로 하나 선택하여 생성
            GameObject newOutPatient = Instantiate(OutpatientPrefabs[Random.Range(0,OutpatientPrefabs.Length)]);
            outpatientQueue.Enqueue(newOutPatient);
            newOutPatient.SetActive(false);
        }
    }

    private void DoctorInitialize()
    {
        // 의사 프리팹 로드
        GameObject[] DoctorPrefabs = Resources.LoadAll<GameObject>("Prefabs/Doctor");
        for (int i = 0; i < maxOfDoctor; i++)
        {
            DoctorOffice spawnArea = GameObject.Find("DoctorWaypoints").transform.Find("Ward (" + (i / 5) + ")").transform.Find("Doctor'sOffice (" + (i % 5) + ")").GetComponent<DoctorOffice>();


            // 프리팹 리스트에서 랜덤으로 하나 선택하여 생성
            GameObject newDoctor = Instantiate(DoctorPrefabs[Random.Range(0, DoctorPrefabs.Length)], spawnArea.GetRandomPointInRange(), Quaternion.identity);
            newDoctor.name = "Doctor " + i;
            DoctorController doctorController = newDoctor.GetComponent<DoctorController>();

            // 의사 사무실 할당

            doctorController.waypoints.Add(spawnArea);


            // 외래 환자 대기 구역 할당
            GameObject parentObject = GameObject.Find("OutPatientWaypoints").transform.Find("Ward ("+(i/5)+")").gameObject;
            DoctorOffice waypoint = parentObject.transform.Find("Doctor'sOffice (" + (i%5) + ")").GetComponent<DoctorOffice>();
            doctorController.waypoints.Add(waypoint);
            newDoctor.transform.position = spawnArea.transform.position;
            newDoctor.GetComponent<SkinnedMeshRenderer>().enabled = false;
            doctorController.isResting = true;

            spawnArea.doctor = newDoctor;
            waypoint.doctor = newDoctor;
        }
    }

    private void NurseInitialize()
    {
        //간호사 프리팹 로드
        GameObject[] NursePrefabs = Resources.LoadAll<GameObject>("Prefabs/Nurse");
        for (int i = 0;i<maxOfNurse;i++)
        {
            Waypoint spawnArea = GameObject.Find("NurseWaypoints").transform.Find("Ward (" +(i/20)+ ")").transform.Find("NurseSpawnArea").gameObject.GetComponent<Waypoint>();
            GameObject newNurse = Instantiate(NursePrefabs[Random.Range(0, NursePrefabs.Length)], spawnArea.GetRandomPointInRange(), Quaternion.identity);
            newNurse.name = "Nurse " + i;
            NurseController nurseController = newNurse.GetComponent<NurseController>();
        }
    }



    // 외래 환자 비활성화 및 초기화
    public void DeactivateOutpatient(GameObject outpatient)
    {
        outpatient.GetComponent<Person>().status = InfectionState.Normal;
        OutpatientController outpatientController = outpatient.GetComponent<OutpatientController>();
        outpatientController.waypoints.Clear();
        outpatientController.isWaiting = false;
        outpatientController.waypointIndex = 0;
        outpatientController.signal = false;
        outpatientQueue.Enqueue(outpatient);
        outpatient.SetActive(false);
    }

    // 외래 환자 활성화 및 위치 설정
    public GameObject ActivateOutpatient(Vector3 position)
    {
        GameObject newOutpatient = outpatientQueue.Dequeue();
        newOutpatient.transform.position = position;
        newOutpatient.SetActive(true);
        return newOutpatient;
    }

    // 의사 비활성화 및 초기화
    public void DeactivateDoctor(GameObject doctor)
    {
        DoctorController doctorController = doctor.GetComponent<DoctorController>();
        doctorController.patientCount = 0;
        doctorController.isWaiting = false;
        doctorController.isResting = true;
        doctorController.signal = false;
        doctor.GetComponent<SkinnedMeshRenderer>().enabled = false;
        //doctor.SetActive(false);
    }

    // 의사 활성화 및 위치 설정
    public GameObject ActivateDoctor(GameObject newDoctor)
    {
        DoctorController doctorController = newDoctor.GetComponent<DoctorController>();
        doctorController.signal = true;
        newDoctor.transform.position = doctorController.waypoints[0].GetRandomPointInRange();
        doctorController.age++; // 교대 횟수 증가 (아사 현상 방지)
        newDoctor.GetComponent<SkinnedMeshRenderer>().enabled = true;
        doctorController.isResting = false;
        //newDoctor.SetActive(true);
        return newDoctor;
    }
}