using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoctorController : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;
    public List<Waypoint> waypoints = new List<Waypoint>();
    public bool isWaiting = false;
    public int patientCount = 0;
    public bool isResting = false;
    public bool changeSignal = false;
    public bool outpatientSignal = false;

    public GameObject outpatient;

    // Start is called before the first frame update
    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        agent.avoidancePriority = Random.Range(0, 100);
        Person newDoctorPerson = gameObject.GetComponent<Person>();
        newDoctorPerson.role = Role.Doctor;
    }

    // Update is called once per frame
    void Update()
    {
        // �ִϸ��̼�
        NPCMovementUtils.Instance.UpdateAnimation(agent, animator);

        if (isResting)
        {
            return;
        }

        //if (patientCount >= patientMaxCount && waypoints[1] is DoctorOffice doctorOffice)
        //{
        //    if (doctorOffice.waitingQueue.Count == 0 && doctorOffice.is_empty)
        //    {
        //        StartCoroutine(Rest());
        //        DoctorCreator.Instance.ChangeDoctor(gameObject);
        //        return;
        //    }
        //}

        if (isWaiting)
        {
            return;
        }
        if (!agent.pathPending && agent.remainingDistance < 0.5f && agent.velocity.sqrMagnitude == 0f)
        {
            StartCoroutine(MoveToNextWaypointAfterWait());
        }
            

    }
    private IEnumerator MoveToNextWaypointAfterWait()
    {
        isWaiting = true;
        yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
        isWaiting = false;



        if (!agent.isOnNavMesh)
        {
            Debug.LogError("NavMeshAgent�� ������̼� �غ� ���� �ʾҽ��ϴ�. Ȱ��ȭ ����, Ȱ��ȭ ����, NavMesh ��ġ ���θ� Ȯ���ϼ���.");
        }

        if (waypoints[1] is DoctorOffice doctorOffice)
        {
            if (!outpatientSignal)
            {
                agent.SetDestination(waypoints[0].GetRandomPointInRange());
            }
            else
            {
                agent.SetDestination(GetPositionInFront(outpatient.transform, 0.75f));
                yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance < 0.5f && agent.velocity.sqrMagnitude == 0f);
                outpatient.GetComponent<OutpatientController>().doctorSignal = true;
            }
        }
    }
    public IEnumerator Rest()
    {
        isResting = true;
        if(!changeSignal)
        {
            yield return new WaitForSeconds(1);
        }
        isResting = false;
        changeSignal = false;
    }
    private Vector3 GetPositionInFront(Transform targetTransform, float distance)
    {
        // ��� ������Ʈ�� ���� ������Ʈ ������ ���� ���͸� ����
        Vector3 direction = -(targetTransform.position - transform.position).normalized;

        // ��� ������Ʈ�� ��ġ�κ��� �� �������� ���� �Ÿ���ŭ ������ ��ġ ���
        Vector3 destination = targetTransform.position + (direction * distance);

        // �׺���̼� �޽� ���� ��ġ ���ø�
        NavMeshHit navHit;
        NavMesh.SamplePosition(destination, out navHit, distance, NavMesh.AllAreas);

        // ���ø��� ��ġ ��ȯ
        return navHit.position;
    }
}
