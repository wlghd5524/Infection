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

    public GameObject targetPatient; // Ÿ�� ȯ��
    public List<Waypoint> waypoints; // ��������Ʈ ����Ʈ

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
        //UpdateAnimation();

        if (isWaiting)
        {
            return; // ��ٸ��� ���̸� ����
        }
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f))
        {
            if (isWorking)
            {
                FaceEachOther(gameObject, targetPatient); // ��ȣ��� ȯ�ڰ� ���θ� �ٶ󺸰� ����

                targetPatient.GetComponent<OutpatientController>().nurseSignal = true; // ȯ�ڿ��� ��ȣ�簡 ���������� �˸�
                isWorking = false;
                if (targetPatient.GetComponent<OutpatientController>().isQuarantined)
                {
                    StartCoroutine(WaitAndGoToNegativePressureRoom(targetPatient)); // �ݸ��� ȯ�ڶ�� ���нǷ� �̵�
                }
            }
            StartCoroutine(WaitAndGo()); // ���� �۾��� ���� ��� �� �̵�
        }
    }

    // ȯ�ڿ��� �̵�
    public void GoToPatient(GameObject patientGameObject)
    {
        Vector3 nearRandomPosition = GetPositionInFront(patientGameObject.transform, 1); // ȯ�� ���� ���� ��ġ ���
        agent.SetDestination(nearRandomPosition); // ������Ʈ ������ ����
        targetPatient = patientGameObject; // Ÿ�� ȯ�� ����
        isWorking = true; // ���ϴ� ������ ����
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
        GoToPatient(patientGameObject); // ȯ�ڿ��� �̵�
        patientGameObject.GetComponent<OutpatientController>().isQuarantined = true; // ȯ�ڸ� �ݸ� ���·� ����
    }

    // ���нǷ� �̵��� ���� ��� �� �̵� �ڷ�ƾ
    public IEnumerator WaitAndGoToNegativePressureRoom(GameObject patientGameObject)
    {
        agent.isStopped = true; // ������Ʈ ����
        yield return new WaitForSeconds(1); // 1�� ���
        agent.isStopped = false; // ������Ʈ �簳
        OutpatientController targetPatient = patientGameObject.GetComponent<OutpatientController>();
        targetPatient.nurse = gameObject; // ��ȣ�� ����
        targetPatient.isFollowingNurse = true; // ȯ�ڰ� ��ȣ�縦 ���󰡵��� ����
        GameObject parentObject = GameObject.Find("NurseWaypoints");
        Waypoint NPRoom = parentObject.transform.Find("N-PRoom (0)").GetComponent<Waypoint>(); // ���н� ��������Ʈ ã��
        agent.SetDestination(NPRoom.GetRandomPointInRange()); // ���нǷ� �̵�
    }

    // ��� �� ���� ��������Ʈ�� �̵� �ڷ�ƾ
    public IEnumerator WaitAndGo()
    {
        isWaiting = true; // ��ٸ��� ������ ����
        yield return new WaitForSeconds(1.0f); // 1�� ���
        isWaiting = false; // ��ٸ��� �� ����
        if(waypoints.Count > 0)
        {
            agent.SetDestination(waypoints[Random.Range(0, waypoints.Count)].GetRandomPointInRange()); // ���� ��������Ʈ�� �̵�
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
