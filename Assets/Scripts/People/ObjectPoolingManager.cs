using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static ObjectPoolingManager Instance;

    // �ִ� �ܷ� ȯ��, �ǻ�, ��ȣ��, �Կ� ȯ�� ��
    public int maxOfOutpatient;
    public int maxOfDoctor;
    public int maxOfNurse;
    public int maxOfInpatient;

    // ��Ȱ��ȭ�� �ܷ� ȯ�� ������Ʈ�� �����ϴ� ť
    public Queue<GameObject> outpatientQueue = new Queue<GameObject>();

    void Awake()
    {
        // �̱��� ���� ����
        Instance = this;
        // �ǻ�, �ܷ� ȯ��, ��ȣ�� �ʱ�ȭ
        DoctorInitialize();
        OutpatientInitialize();
        NurseInitialize();
        InpatientInitaialize();
    }

    // �ܷ� ȯ�� �ʱ�ȭ
    private void OutpatientInitialize()
    {
        // �ܷ� ȯ�� ������ �ε�
        GameObject[] OutpatientPrefabs = Resources.LoadAll<GameObject>("Prefabs/Outpatient");
        for (int i = 0; i < maxOfOutpatient; i++)
        {
            // ������ ����Ʈ���� �������� �ϳ� �����Ͽ� ����
            GameObject newOutPatient = Instantiate(OutpatientPrefabs[Random.Range(0, OutpatientPrefabs.Length)]);
            outpatientQueue.Enqueue(newOutPatient); // ť�� �߰�
            newOutPatient.SetActive(false); // ��Ȱ��ȭ
        }
    }

    // �ǻ� �ʱ�ȭ
    private void DoctorInitialize()
    {
        // �ǻ� ������ �ε�
        GameObject[] DoctorPrefabs = Resources.LoadAll<GameObject>("Prefabs/Doctor");
        for (int i = 0; i < maxOfDoctor; i++)
        {
            // �ǻ� ���� ��ġ ����
            DoctorOffice spawnArea = GameObject.Find("DoctorWaypoints").transform.Find("Ward (" + (i / 5) + ")").transform.Find("Doctor'sOffice (" + (i % 5) + ")").GetComponent<DoctorOffice>();
            // ������ ����Ʈ���� �������� �ϳ� �����Ͽ� ����
            GameObject newDoctor = Instantiate(DoctorPrefabs[Random.Range(0, DoctorPrefabs.Length)], spawnArea.GetRandomPointInRange(), Quaternion.identity);
            newDoctor.name = "Doctor " + i;
            DoctorController doctorController = newDoctor.GetComponent<DoctorController>();

            // �ǻ� �繫�� �Ҵ�
            doctorController.waypoints.Add(spawnArea);

            // �ܷ� ȯ�� ��� ���� �Ҵ�
            GameObject parentObject = GameObject.Find("OutpatientWaypoints").transform.Find("Ward (" + (i / 5) + ")").gameObject;
            DoctorOffice waypoint = parentObject.transform.Find("Doctor'sOffice (" + (i % 5) + ")").GetComponent<DoctorOffice>();
            doctorController.waypoints.Add(waypoint);
            newDoctor.transform.position = spawnArea.transform.position;
            newDoctor.GetComponent<SkinnedMeshRenderer>().enabled = false;
            doctorController.isResting = true;

            spawnArea.doctor = newDoctor;
            waypoint.doctor = newDoctor;
        }
    }

    // ��ȣ�� �ʱ�ȭ
    private void NurseInitialize()
    {
        // ��ȣ�� ������ �ε�
        GameObject[] NursePrefabs = Resources.LoadAll<GameObject>("Prefabs/Nurse");
        for (int i = 0; i < maxOfNurse; i++)
        {
            // ��ȣ�� ���� ��ġ ����
            int ward = i / 20;
            Waypoint spawnArea = GameObject.Find("NurseWaypoints").transform.Find("Ward (" + ward + ")").transform.Find("NurseSpawnArea").gameObject.GetComponent<Waypoint>();
            GameObject newNurse = Instantiate(NursePrefabs[Random.Range(0, NursePrefabs.Length)], spawnArea.GetRandomPointInRange(), Quaternion.identity);
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
            // �Կ� ȯ�� ���� ��ġ ����
            int ward = i / 6;
            BedWaypoint spawnArea = GameObject.Find("InpatientWaypoints").transform.Find("Ward (" + ward + ")").transform.Find("BedWaypoint (" + (i % 6) + ")").gameObject.GetComponent<BedWaypoint>();
            GameObject newInpatient = Instantiate(InpatientPrefabs[Random.Range(0, InpatientPrefabs.Length)], spawnArea.GetRandomPointInRange(), Quaternion.identity);
            newInpatient.name = "Inpatient " + i;
            spawnArea.inpatient = newInpatient;
            InpatientController newInpatientController = newInpatient.GetComponent<InpatientController>();
            newInpatientController.bedWaypoint = spawnArea.gameObject;
            newInpatientController.ward = ward;
            newInpatient.GetComponent<Person>().role = Role.Inpatient;
        }
    }

    // �ܷ� ȯ�� ��Ȱ��ȭ �� �ʱ�ȭ
    public void DeactivateOutpatient(GameObject outpatient)
    {
        outpatient.GetComponent<Person>().status = InfectionState.Normal; // ���� ���� �ʱ�ȭ
        OutpatientController outpatientController = outpatient.GetComponent<OutpatientController>();
        outpatientController.waypoints.Clear(); // ��������Ʈ �ʱ�ȭ
        outpatientController.isWaiting = false; // ��� ���� �ʱ�ȭ
        outpatientController.waypointIndex = 0; // ��������Ʈ �ε��� �ʱ�ȭ
        outpatientController.doctorSignal = false; // �ǻ� ��ȣ �ʱ�ȭ
        outpatientController.nurseSignal = false; // ��ȣ�� ��ȣ �ʱ�ȭ
        outpatientController.officeSignal = false; // ����� ��ȣ �ʱ�ȭ
        outpatientQueue.Enqueue(outpatient); // ť�� �߰�
        outpatient.SetActive(false); // ��Ȱ��ȭ
    }

    // �ܷ� ȯ�� Ȱ��ȭ �� ��ġ ����
    public GameObject ActivateOutpatient(Vector3 position)
    {
        GameObject newOutpatient = outpatientQueue.Dequeue(); // ť���� �ܷ� ȯ�� ��������
        newOutpatient.transform.position = position; // ��ġ ����
        newOutpatient.SetActive(true); // Ȱ��ȭ
        return newOutpatient;
    }

    // �ǻ� ��Ȱ��ȭ �� �ʱ�ȭ
    public void DeactivateDoctor(GameObject doctor)
    {
        DoctorController doctorController = doctor.GetComponent<DoctorController>();
        doctorController.patientCount = 0; // ȯ�� �� �ʱ�ȭ
        doctorController.isWaiting = false; // ��� ���� �ʱ�ȭ
        doctorController.isResting = true; // �޽� ���� ����
        doctorController.changeSignal = false; // ��ȣ �ʱ�ȭ
        doctor.GetComponent<SkinnedMeshRenderer>().enabled = false; // ������ ��Ȱ��ȭ
    }

    // �ǻ� Ȱ��ȭ �� ��ġ ����
    public GameObject ActivateDoctor(GameObject newDoctor)
    {
        DoctorController doctorController = newDoctor.GetComponent<DoctorController>();
        doctorController.changeSignal = true; // ��ȣ ����
        newDoctor.transform.position = doctorController.waypoints[0].GetRandomPointInRange(); // ��ġ ����
        newDoctor.GetComponent<SkinnedMeshRenderer>().enabled = true; // ������ Ȱ��ȭ
        doctorController.isResting = false; // �޽� ���� ����
        return newDoctor;
    }

    // ��ȣ�� Ȱ��ȭ
    public GameObject ActivateNurse(GameObject newNurse)
    {
        newNurse.GetComponent<SkinnedMeshRenderer>().enabled = true; // ������ Ȱ��ȭ
        newNurse.GetComponent<NurseController>().isRest = false;
        return newNurse;
    }

    public void DeactivateNurse(GameObject nurse)
    {
        nurse.GetComponent<SkinnedMeshRenderer>().enabled = false; // ������ Ȱ��ȭ
        NurseController nurseController = nurse.GetComponent<NurseController>();
        nurseController.isRest = true;
        nurseController.isWaiting = false;
        nurseController.isWaitingAtDoctorOffice = false;
        nurseController.isWorking = false;
    }
}
