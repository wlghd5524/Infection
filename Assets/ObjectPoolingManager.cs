using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static ObjectPoolingManager Instance;

    // �ִ� �ܷ� ȯ�ڿ� �ǻ� ��
    public int maxOfOutpatient;
    public int maxOfDoctor;
    public int maxOfNurse;

    // ��Ȱ��ȭ�� �ܷ� ȯ�ڿ� �ǻ� ������Ʈ�� �����ϴ� ť
    public Queue<GameObject> outpatientQueue = new Queue<GameObject>();
    public Queue<GameObject> doctorQueue = new Queue<GameObject>();

    void Awake()
    {
        // �̱��� ���� ����
        Instance = this;
        // �ǻ�� �ܷ� ȯ�� �ʱ�ȭ
        DoctorInitialize();
        OutpatientInitialize();
        NurseInitialize();
    }

    private void OutpatientInitialize()
    {
        // �ܷ� ȯ�� ������ �ε�
        GameObject[] OutpatientPrefabs = Resources.LoadAll<GameObject>("Prefabs/Outpatient");
        for (int i = 0; i < maxOfOutpatient; i++)
        {
            // ������ ����Ʈ���� �������� �ϳ� �����Ͽ� ����
            GameObject newOutPatient = Instantiate(OutpatientPrefabs[Random.Range(0,OutpatientPrefabs.Length)]);
            outpatientQueue.Enqueue(newOutPatient);
            newOutPatient.SetActive(false);
        }
    }

    private void DoctorInitialize()
    {
        // �ǻ� ������ �ε�
        GameObject[] DoctorPrefabs = Resources.LoadAll<GameObject>("Prefabs/Doctor");
        for (int i = 0; i < maxOfDoctor; i++)
        {
            DoctorOffice spawnArea = GameObject.Find("DoctorWaypoints").transform.Find("Ward (" + (i / 5) + ")").transform.Find("Doctor'sOffice (" + (i % 5) + ")").GetComponent<DoctorOffice>();


            // ������ ����Ʈ���� �������� �ϳ� �����Ͽ� ����
            GameObject newDoctor = Instantiate(DoctorPrefabs[Random.Range(0, DoctorPrefabs.Length)], spawnArea.GetRandomPointInRange(), Quaternion.identity);
            newDoctor.name = "Doctor " + i;
            DoctorController doctorController = newDoctor.GetComponent<DoctorController>();

            // �ǻ� �繫�� �Ҵ�

            doctorController.waypoints.Add(spawnArea);


            // �ܷ� ȯ�� ��� ���� �Ҵ�
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
        //��ȣ�� ������ �ε�
        GameObject[] NursePrefabs = Resources.LoadAll<GameObject>("Prefabs/Nurse");
        for (int i = 0;i<maxOfNurse;i++)
        {
            Waypoint spawnArea = GameObject.Find("NurseWaypoints").transform.Find("Ward (" +(i/20)+ ")").transform.Find("NurseSpawnArea").gameObject.GetComponent<Waypoint>();
            GameObject newNurse = Instantiate(NursePrefabs[Random.Range(0, NursePrefabs.Length)], spawnArea.GetRandomPointInRange(), Quaternion.identity);
            newNurse.name = "Nurse " + i;
            NurseController nurseController = newNurse.GetComponent<NurseController>();
        }
    }



    // �ܷ� ȯ�� ��Ȱ��ȭ �� �ʱ�ȭ
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

    // �ܷ� ȯ�� Ȱ��ȭ �� ��ġ ����
    public GameObject ActivateOutpatient(Vector3 position)
    {
        GameObject newOutpatient = outpatientQueue.Dequeue();
        newOutpatient.transform.position = position;
        newOutpatient.SetActive(true);
        return newOutpatient;
    }

    // �ǻ� ��Ȱ��ȭ �� �ʱ�ȭ
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

    // �ǻ� Ȱ��ȭ �� ��ġ ����
    public GameObject ActivateDoctor(GameObject newDoctor)
    {
        DoctorController doctorController = newDoctor.GetComponent<DoctorController>();
        doctorController.signal = true;
        newDoctor.transform.position = doctorController.waypoints[0].GetRandomPointInRange();
        doctorController.age++; // ���� Ƚ�� ���� (�ƻ� ���� ����)
        newDoctor.GetComponent<SkinnedMeshRenderer>().enabled = true;
        doctorController.isResting = false;
        //newDoctor.SetActive(true);
        return newDoctor;
    }
}