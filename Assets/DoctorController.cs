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

        Person newDoctorPerson = gameObject.GetComponent<Person>();
        newDoctorPerson.role = Role.Doctor;
    }

    // Update is called once per frame
    void Update()
    {

        // �ִϸ��̼�
        UpdateAnimation();

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
            if(outpatientSignal)
            {
                outpatient.GetComponent<OutpatientController>().doctorSignal = true;
                return;
            }
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
            }
        }
        StartCoroutine(UpdateMovementAnimation());
    }
    private void UpdateAnimation()
    {
        // �ִϸ��̼�
        if (!agent.isOnNavMesh)
        {
            if (animator.GetFloat("MoveSpeed") != 0)
                animator.SetFloat("MoveSpeed", 0);
            if (animator.GetBool("Grounded"))
                animator.SetBool("Grounded", false);
            return;
        }

        if (agent.remainingDistance > agent.stoppingDistance)
        {
            if (animator.GetFloat("MoveSpeed") != agent.velocity.magnitude / agent.speed)
                animator.SetFloat("MoveSpeed", agent.velocity.magnitude / agent.speed);
        }
        else
        {
            if (animator.GetFloat("MoveSpeed") != 0)
            {
                animator.SetFloat("MoveSpeed", 0);
            }

        }

        if (animator.GetBool("Grounded") != (!agent.isOnOffMeshLink && agent.isOnNavMesh))
            animator.SetBool("Grounded", !agent.isOnOffMeshLink && agent.isOnNavMesh);
    }
    private IEnumerator UpdateMovementAnimation()
    {
        while (true)
        {
            animator.SetFloat("MoveSpeed", agent.velocity.magnitude / agent.speed);
            yield return null;
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
        Vector3 direction = targetTransform.forward; // Ÿ���� ���� ����
        Vector3 destination = targetTransform.position + (direction * distance); // ������ ���
        NavMeshHit navHit;
        NavMesh.SamplePosition(destination, out navHit, distance, -1); // �׺���̼� �޽� ���� ��ġ ���ø�
        return navHit.position;
    }
}
