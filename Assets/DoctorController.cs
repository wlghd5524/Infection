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
        // 애니메이션
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
            Debug.LogError("NavMeshAgent가 내비게이션 준비가 되지 않았습니다. 활성화 상태, 활성화 여부, NavMesh 위치 여부를 확인하세요.");
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
        // 대상 오브젝트와 현재 오브젝트 사이의 방향 벡터를 구함
        Vector3 direction = -(targetTransform.position - transform.position).normalized;

        // 대상 오브젝트의 위치로부터 그 방향으로 일정 거리만큼 떨어진 위치 계산
        Vector3 destination = targetTransform.position + (direction * distance);

        // 네비게이션 메시 상의 위치 샘플링
        NavMeshHit navHit;
        NavMesh.SamplePosition(destination, out navHit, distance, NavMesh.AllAreas);

        // 샘플링된 위치 반환
        return navHit.position;
    }
}
