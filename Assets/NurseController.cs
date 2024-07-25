using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NurseController : MonoBehaviour
{
    private Animator animator; // �ִϸ����� ������Ʈ
    private NavMeshAgent agent; // �׺���̼� ������Ʈ ������Ʈ

    public bool isWorking = false; // ��ȣ�簡 ���ϴ� ������ ����
    public bool isWaiting = false; // ��ȣ�簡 ��ٸ��� ������ ����
    public bool isRest = false;
    //public bool arriveNPRoom = false;
    public bool isWaitingAtDoctorOffice = false;

    public GameObject targetPatient; // Ÿ�� ȯ��
    public List<Waypoint> waypoints; // ��������Ʈ ����Ʈ

    public int ward = 0;
    private void Awake()
    {
        animator = GetComponent<Animator>(); // �ִϸ����� ������Ʈ �Ҵ�
        agent = GetComponent<NavMeshAgent>(); // �׺���̼� ������Ʈ ������Ʈ �Ҵ�
        agent.avoidancePriority = Random.Range(0, 1000); // ������Ʈ ȸ�� �켱���� ����
    }

    // Start�� ù ������ ������Ʈ ���� ȣ��˴ϴ�.
    void Start()
    {

    }

    // Update�� �� ������ ȣ��˴ϴ�.
    void Update()
    {
        // �ִϸ��̼� ������Ʈ
        UpdateAnimation();

        if (isWaiting || isRest)
        {
            return; // ��ٸ��� ���̸� ����
        }
        
        if (isWorking)
            return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f))
        {
            //if (isWorking)
            //{
            //    FaceEachOther(gameObject, targetPatient); // ��ȣ��� ȯ�ڰ� ���θ� �ٶ󺸰� ����
            //    OutpatientController targetPatientController = targetPatient.GetComponent<OutpatientController>();
            //    targetPatientController.nurseSignal = true; // ȯ�ڿ��� ��ȣ�簡 ���������� �˸�
            //    targetPatientController.nurse = gameObject; // ��ȣ�� ����
            //    targetPatientController.isFollowingNurse = true; // ȯ�ڰ� ��ȣ�縦 ���󰡵��� ����
            //    if (targetPatient.GetComponent<OutpatientController>().isFollowingNurse)
            //    {
            //        StartCoroutine(WaitAndGoToNegativePressureRoom(targetPatient)); // �ݸ��� ȯ�ڶ�� ���нǷ� �̵�
            //    }
            //    if(targetPatient.GetComponent<OutpatientController>().isQuarantined)
            //    {
            //        isWorking = false;
            //    }
                
            //}
            if(!isWorking)
            {
                StartCoroutine(WaitAndGo()); // ���� �۾��� ���� ��� �� �̵�
            }
            
        }
    }

    // ȯ�ڿ��� �̵�
    public IEnumerator GoToPatient(GameObject patientGameObject)
    {
        isWorking = true; // ���ϴ� ������ ����
        Vector3 nearRandomPosition = GetPositionInFront(patientGameObject.transform, 1); // ȯ�� ���� ���� ��ġ ���
        agent.SetDestination(nearRandomPosition); // ������Ʈ ������ ����
        targetPatient = patientGameObject; // Ÿ�� ȯ�� ����

        yield return new WaitUntil(() => !agent.pathPending);
        yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance);

        FaceEachOther(gameObject, targetPatient); // ��ȣ��� ȯ�ڰ� ���θ� �ٶ󺸰� ����
        OutpatientController targetPatientController = targetPatient.GetComponent<OutpatientController>();
        targetPatientController.nurseSignal = true; // ȯ�ڿ��� ��ȣ�簡 ���������� �˸�
        //targetPatientController.nurse = gameObject; // ��ȣ�� ����
        targetPatientController.StartCoroutine(targetPatientController.FollowNurse(gameObject));
        
        yield return StartCoroutine(WaitAndGoToNegativePressureRoom(targetPatientController)); // �ݸ��� ȯ�ڶ�� ���нǷ� �̵�
        yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance);

        isWorking = false;
        targetPatientController.isFollowingNurse = false;
        targetPatientController.isQuarantined = true;
    }

    // Ÿ�� ���� ���� ��ġ ���
    private Vector3 GetPositionInFront(Transform targetTransform, float distance)
    {
        Vector3 direction = targetTransform.forward; // Ÿ���� ���� ����
        Vector3 destination = targetTransform.position + direction * distance; // ������ ���
        NavMeshHit navHit;
        NavMesh.SamplePosition(destination, out navHit, distance, -1); // �׺���̼� �޽� ���� ��ġ ���ø�
        return navHit.position;
    }

    // �� ��ü�� ���θ� �ٶ󺸰� ����
    private void FaceEachOther(GameObject obj1, GameObject obj2)
    {
        obj1.transform.LookAt(obj2.transform.position); // obj1�� obj2�� �ٶ󺸰� ����
        obj2.transform.LookAt(obj1.transform.position); // obj2�� obj1�� �ٶ󺸰� ����
    }

    // ���нǷ� �̵�
    public void GoToNegativePressureRoom(GameObject patientGameObject)
    {
        StartCoroutine(GoToPatient(patientGameObject)); // ȯ�ڿ��� �̵�
    }

    // ���нǷ� �̵��� ���� ��� �� �̵� �ڷ�ƾ
    IEnumerator WaitAndGoToNegativePressureRoom(OutpatientController targetPatientController)
    {
        //agent.isStopped = true; // ������Ʈ ����
        //yield return new WaitForSeconds(1); // 1�� ���
        //agent.isStopped = false; // ������Ʈ �簳

        
        GameObject parentObject = GameObject.Find("NurseWaypoints");

        for(int i = 0;i<4;i++)
        {
            NPRoom nPRoom = parentObject.transform.Find("N-PRoom (" + i + ")").GetComponent<NPRoom>(); // ���н� ��������Ʈ ã��
            if (nPRoom.is_Empty)
            {
                targetPatientController.nPRoom = nPRoom;
                nPRoom.is_Empty = false;
                agent.SetDestination(nPRoom.GetRandomPointInRange()); // ���нǷ� �̵�
                break;
            }
        }

        

        yield return new WaitUntil(() => !agent.pathPending);
    }

    // ��� �� ���� ��������Ʈ�� �̵� �ڷ�ƾ
    public IEnumerator WaitAndGo()
    {
        isWaiting = true; // ��ٸ��� ������ ����
        yield return new WaitForSeconds(1.0f); // 1�� ���
        isWaiting = false; // ��ٸ��� �� ����
        if(waypoints.Count > 0)
        {

            if (waypoints.Count == 5)  //����� �� ��� ��ȣ���
            {
                for(int i = 0;i<5;i++)
                {
                    if(waypoints[i] is NurseWaitingPoint nurseWaitingPoint)
                    {
                        if(nurseWaitingPoint.is_empty && !nurseWaitingPoint.doctorOffice.doctor.GetComponent<DoctorController>().isResting && isWaitingAtDoctorOffice == false)
                        {
                            nurseWaitingPoint.is_empty = false;
                            isWaitingAtDoctorOffice = true;
                            agent.SetDestination(waypoints[i].GetRandomPointInRange());
                        }
                    }
                }
            }
            else
            {
                agent.SetDestination(waypoints[Random.Range(0, waypoints.Count)].GetRandomPointInRange()); // ���� ��������Ʈ�� �̵�
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
}
