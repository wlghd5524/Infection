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
    public bool isWaitingAtDoctorOffice = false;

    public GameObject targetPatient; // Ÿ�� ȯ��
    public List<Waypoint> waypoints; // ��������Ʈ ����Ʈ

    public int ward = 0;
    private void Awake()
    {
        animator = GetComponent<Animator>(); // �ִϸ����� ������Ʈ �Ҵ�
        agent = GetComponent<NavMeshAgent>(); // �׺���̼� ������Ʈ ������Ʈ �Ҵ�
        agent.avoidancePriority = Random.Range(0, 100); // ������Ʈ ȸ�� �켱���� ����
    }

    // Start�� ù ������ ������Ʈ ���� ȣ��˴ϴ�.
    void Start()
    {

    }

    // Update�� �� ������ ȣ��˴ϴ�.
    void Update()
    {
        // �ִϸ��̼� ������Ʈ
        NPCMovementUtils.Instance.UpdateAnimation(agent,animator);

        if (isWaiting || isRest)
        {
            return; // ��ٸ��� ���̸� ����
        }
        
        if (isWorking)
            return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f))
        {
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
        Vector3 targetPatientPosition = NPCMovementUtils.Instance.GetPositionInFront(transform, patientGameObject.transform, 0.5f); // ȯ�� ���� ���� ��ġ ���
        agent.SetDestination(targetPatientPosition); // ������Ʈ ������ ����
        targetPatient = patientGameObject; // Ÿ�� ȯ�� ����

        yield return new WaitUntil(() => !agent.pathPending);
        yield return new WaitUntil(() => agent.remainingDistance == 0);

        NPCMovementUtils.Instance.FaceEachOther(gameObject, targetPatient); // ��ȣ��� ȯ�ڰ� ���θ� �ٶ󺸰� ����
        OutpatientController targetPatientController = targetPatient.GetComponent<OutpatientController>();
        targetPatientController.nurseSignal = true; // ȯ�ڿ��� ��ȣ�簡 ���������� �˸�
        //targetPatientController.nurse = gameObject; // ��ȣ�� ����
        targetPatientController.StartCoroutine(targetPatientController.FollowNurse(gameObject));
        agent.speed -= 1;
        yield return StartCoroutine(WaitAndGoToNegativePressureRoom(targetPatientController)); // �ݸ��� ȯ�ڶ�� ���нǷ� �̵�
        yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance);
        agent.speed += 1;
        isWorking = false;
        targetPatientController.isFollowingNurse = false;
        targetPatientController.isQuarantined = true;
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
}
