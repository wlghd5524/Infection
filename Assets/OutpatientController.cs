using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class OutpatientController : MonoBehaviour
{
    // ������Ʈ ����
    private Animator animator;
    private NavMeshAgent agent;

    // ��������Ʈ ���� ����
    public List<Waypoint> waypoints = new List<Waypoint>();
    public int waypointIndex = 0;
    private static int[] officeCount = { 0, 0, 0, 0, 0, 0 };

    // ���� �÷���
    public bool isWaiting = false;
    public bool isWaitingForDoctor = false;
    public bool signal = false;

    // �� ������Ʈ ����
    GameObject parentObject;
    GameObject gatewayObject;
    int randomWard;

    private void Awake()
    {
        // ������Ʈ �ʱ�ȭ
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.avoidancePriority = Random.Range(0, 100);

        // �� ������Ʈ ã��
        parentObject = GameObject.Find("OutPatientWaypoints");
        gatewayObject = GameObject.Find("Gateways");
        randomWard = Random.Range(0, 2);
    }

    private void OnEnable()
    {
        // ù ��° ��������Ʈ �߰�
        AddWaypoint(parentObject.transform, $"CounterWaypoint (0)");
    }

    private void Update()
    {
        // �ִϸ��̼� ������Ʈ
        UpdateAnimation();

        // ��� ���̸� �̵� ó������ ����
        if (isWaiting || isWaitingForDoctor)
        {
            return;
        }

        // �������� �����ߴ��� Ȯ��
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (waypointIndex == 4)
            {
                // ��� ��������Ʈ�� �湮������ ��Ȱ��ȭ
                ObjectPoolingManager.Instance.DeactivateOutpatient(gameObject);
                OutpatientCreator.numberOfOutpatient--;
                return;
            }
            else
            {
                // ���� ��������Ʈ�� �̵�
                StartCoroutine(MoveToNextWaypointAfterWait());
            }
        }
    }

    // �ִϸ��̼� ������Ʈ �޼���
    private void UpdateAnimation()
    {
        // NavMesh �� ������ �ִϸ��̼� ����
        if (!agent.isOnNavMesh)
        {
            if (animator.GetFloat("MoveSpeed") != 0)
                animator.SetFloat("MoveSpeed", 0);
            if (animator.GetBool("Grounded"))
                animator.SetBool("Grounded", false);
            return;
        }

        // �̵� �� �ִϸ��̼�
        if (agent.remainingDistance > agent.stoppingDistance)
        {
            animator.SetFloat("MoveSpeed", agent.velocity.magnitude / agent.speed);
        }
        else
        {
            animator.SetFloat("MoveSpeed", 0);
        }

        // ���� ���� ���� ������Ʈ
        animator.SetBool("Grounded", !agent.isOnOffMeshLink && agent.isOnNavMesh);
    }

    // ���� ��������Ʈ�� �̵��ϴ� �ڷ�ƾ
    private IEnumerator MoveToNextWaypointAfterWait()
    {
        isWaiting = true;
        yield return new WaitForSeconds(1.5f);
        isWaiting = false;

        // ���� ��������Ʈ �ε����� ���� ���� ��������Ʈ �߰�
        AddNextWaypoint();

        if (waypointIndex < waypoints.Count)
        {
            // �ǻ� �繫���� ��� ���
            if (waypoints[waypointIndex] is DoctorOffice doctorOffice)
            {
                StartCoroutine(WaitForDoctorOffice(doctorOffice));
            }
            else if (waypointIndex > 0 && waypoints[waypointIndex - 1] is DoctorOffice doc)
            {
                doc.is_empty = true;
            }

            // ���� ��������Ʈ�� �̵�
            if (!isWaitingForDoctor)
            {
                agent.SetDestination(waypoints[waypointIndex++].GetRandomPointInRange());
                StartCoroutine(UpdateMovementAnimation());
            }
        }
    }

    // ���� ��������Ʈ �߰� �޼���
    private void AddNextWaypoint()
    {
        switch (waypointIndex)
        {
            case 0:
                AddWaypoint(parentObject.transform, $"CounterWaypoint (0)");
                break;
            case 1:
                AddWaypoint(parentObject.transform, $"SofaWaypoint (0)");
                break;
            case 2:
                if (waypoints.Count < 3)
                {
                    AddWaypoint(parentObject.transform, $"Doctor'sOffice (0)");
                }
                break;
            case 3:
                AddWaypoint(gatewayObject.transform, $"Gateway ({Random.Range(0, 2)})");
                break;
        }
    }

    // �ǻ� �繫�� ��� �ڷ�ƾ
    private IEnumerator WaitForDoctorOffice(DoctorOffice doctorOffice)
    {
        isWaitingForDoctor = true;
        while (!signal)
        {
            yield return new WaitForSeconds(1);
        }
        doctorOffice.is_empty = false;
        isWaitingForDoctor = false;
    }

    // ��������Ʈ �߰� �޼���
    private void AddWaypoint(Transform parent, string childName)
    {
        Transform waypointTransform;
        if (waypointIndex == 3)
        {
            waypointTransform = parent.Find(childName);
        }
        else
        {
            waypointTransform = parent.Find("Ward (" + randomWard + ")").Find(childName);
        }
        if (waypointTransform != null)
        {
            Waypoint comp = waypointTransform.gameObject.GetComponent<Waypoint>();
            if (comp is DoctorOffice)
            {
                // �ǻ� �繫�� ���� ����
                SelectDoctorOffice(waypointTransform, childName);
            }
            else
            {
                if (!waypoints.Contains(comp))
                {
                    waypoints.Add(comp);
                    Debug.Log($"Added waypoint: {childName}");
                }
            }
        }
        else
        {
            Debug.LogWarning($"Can't find waypoint: {childName}");
        }
    }

    // �ǻ� �繫�� ���� �޼���
    private void SelectDoctorOffice(Transform officeTransform, string childName)
    {
        // ������ �ǻ� �繫�� ��� ����
        Dictionary<DoctorOffice, int> countDic = new Dictionary<DoctorOffice, int>();
        for (int i = 0; i < 5; i++)
        {
            DoctorOffice doctorOffice = officeTransform.parent.Find("Doctor'sOffice ("+ i +")").gameObject.GetComponent<DoctorOffice>();
            GameObject searchedDoctor = doctorOffice.doctor;
            DoctorController doctorController = searchedDoctor.GetComponent<DoctorController>();
            if (doctorController.isResting)
            {
                continue;
            }
            int patientCount = doctorController.patientCount + doctorOffice.waitingQueue.Count;
            if (!countDic.ContainsKey(doctorOffice))
            {
                countDic.Add(doctorOffice, patientCount);
            }
        }

        // ������ �ǻ� �繫�� ����
        if (countDic.Count > 0)
        {
            DoctorOffice searchedOffice = countDic
                .OrderBy(kvp => kvp.Key.doctor.GetComponent<DoctorController>().age)
                .ThenBy(kvp => kvp.Value)
                .FirstOrDefault().Key;
            searchedOffice.waitingQueue.Enqueue(this);
            waypoints.Add(searchedOffice);
            countDic.Clear();
        }
        else
        {
            Debug.LogError("�ǻ縦 ã�� �� �����ϴ�.");
        }
    }

    // �̵� �ִϸ��̼� ������Ʈ �ڷ�ƾ
    private IEnumerator UpdateMovementAnimation()
    {
        while (true)
        {
            animator.SetFloat("MoveSpeed", agent.velocity.magnitude / agent.speed);
            yield return null;
        }
    }

}