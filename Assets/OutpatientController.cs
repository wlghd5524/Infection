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

    // ���� �÷���
    public bool isQuarantined = false;
    public bool isFollowingNurse = false;
    public bool isWaiting = false;
    public bool isWaitingForDoctor = false;
    public bool isWaitingForNurse = false;
    public bool officeSignal = false;
    public bool nurseSignal = false;
    public bool doctorSignal = false;

    // �� ������Ʈ ����
    GameObject parentObject;
    GameObject gatewayObject;
    int randomWard;
    public GameObject nurse;
    public NPRoom nPRoom;

    private void Awake()
    {
        // ������Ʈ �ʱ�ȭ
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.avoidancePriority = Random.Range(0, 1000);

        // �� ������Ʈ ã��
        parentObject = GameObject.Find("OutPatientWaypoints");
        gatewayObject = GameObject.Find("Gateways");
        randomWard = Random.Range(0, 6);
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
        if (isWaiting)
        {
            return;
        }

        


        // �������� �����ߴ��� Ȯ��
        if (!agent.pathPending && agent.remainingDistance < 0.5f && agent.velocity.sqrMagnitude == 0f)
        {
            //if(isFollowingNurse)
            //{
            //    isFollowingNurse = false;
            //    isQuarantined = true;
            //}
            if (waypointIndex == 4 && !isWaitingForNurse && !isFollowingNurse && !isQuarantined)
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
        if (waypointIndex > 0 && waypoints[waypointIndex - 1] is DoctorOffice docOffice)
        {
            docOffice.doctor.GetComponent<DoctorController>().outpatient = gameObject;
            docOffice.doctor.GetComponent<DoctorController>().outpatientSignal = true;
            yield return new WaitForSeconds(1.0f);
            yield return new WaitUntil(() => doctorSignal);
            FaceEachOther(docOffice.doctor, gameObject);
        }
        isWaiting = true;
        yield return new WaitForSeconds(1.5f);
        isWaiting = false;
        if (isQuarantined)
        {
            agent.SetDestination(nPRoom.GetRandomPointInRange());
            yield break;
        }
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
                doc.doctor.GetComponent<DoctorController>().outpatientSignal = false;
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
        while(!officeSignal)
        {
            yield return new WaitForSeconds(1.0f);
        }
        doctorOffice.is_empty = false;
        isWaitingForDoctor = false;
    }

    //��ȣ�簡 �� ������ ��� �ڷ�ƾ
    public IEnumerator WaitForNurse()
    {
        agent.isStopped = true;
        yield return new WaitUntil(() => nurseSignal);
        agent.isStopped = false;
        
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
                .OrderBy(kvp => kvp.Value)
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

    public IEnumerator FollowNurse(GameObject nurse)
    {
        //isWaiting = true;
        //yield return new WaitForSeconds(1.0f);
        //isWaiting = false;
        this.nurse = nurse;
        isFollowingNurse = true;
        while(isFollowingNurse == true)
        {
            agent.SetDestination(nurse.transform.position);
            yield return new WaitForSeconds(0.1f);
        }
        agent.ResetPath();
        //float distance = Vector3.Distance(transform.position, nurse.transform.position);
        //if (distance > 1.0f)
        //{
        //    agent.SetDestination(nurse.transform.position);
        //}
        //else
        //{
        //    agent.ResetPath();
        //}
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
    private void FaceEachOther(GameObject obj1, GameObject obj2)
    {
        obj1.transform.LookAt(obj2.transform.position); // obj1�� obj2�� �ٶ󺸰� ����
        obj2.transform.LookAt(obj1.transform.position); // obj2�� obj1�� �ٶ󺸰� ����
    }


}