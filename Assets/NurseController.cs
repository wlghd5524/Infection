using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NurseController : MonoBehaviour
{
    private Animator animator; // 애니메이터 컴포넌트
    private NavMeshAgent agent; // 네비게이션 에이전트 컴포넌트

    public bool isWorking = false; // 간호사가 일하는 중인지 여부
    public bool isWaiting = false; // 간호사가 기다리는 중인지 여부

    public GameObject targetPatient; // 타겟 환자
    public List<Waypoint> waypoints; // 웨이포인트 리스트

    private void Awake()
    {
        animator = GetComponent<Animator>(); // 애니메이터 컴포넌트 할당
        agent = GetComponent<NavMeshAgent>(); // 네비게이션 에이전트 컴포넌트 할당
        agent.avoidancePriority = Random.Range(0, 100); // 에이전트 회피 우선순위 설정
    }

    // Start는 첫 프레임 업데이트 전에 호출됩니다.
    void Start()
    {

    }

    // Update는 매 프레임 호출됩니다.
    void Update()
    {
        // 애니메이션 업데이트
        //UpdateAnimation();

        if (isWaiting)
        {
            return; // 기다리는 중이면 리턴
        }
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f))
        {
            if (isWorking)
            {
                FaceEachOther(gameObject, targetPatient); // 간호사와 환자가 서로를 바라보게 설정

                targetPatient.GetComponent<OutpatientController>().nurseSignal = true; // 환자에게 간호사가 도착했음을 알림
                isWorking = false;
                if (targetPatient.GetComponent<OutpatientController>().isQuarantined)
                {
                    StartCoroutine(WaitAndGoToNegativePressureRoom(targetPatient)); // 격리된 환자라면 음압실로 이동
                }
            }
            StartCoroutine(WaitAndGo()); // 다음 작업을 위해 대기 후 이동
        }
    }

    // 환자에게 이동
    public void GoToPatient(GameObject patientGameObject)
    {
        Vector3 nearRandomPosition = GetPositionInFront(patientGameObject.transform, 1); // 환자 앞의 임의 위치 계산
        agent.SetDestination(nearRandomPosition); // 에이전트 목적지 설정
        targetPatient = patientGameObject; // 타겟 환자 설정
        isWorking = true; // 일하는 중으로 설정
    }

    // 타겟 앞의 임의 위치 계산
    private Vector3 GetPositionInFront(Transform targetTransform, float distance)
    {
        Vector3 direction = targetTransform.forward; // 타겟의 전방 방향
        Vector3 destination = targetTransform.position + direction * distance; // 목적지 계산
        NavMeshHit navHit;
        NavMesh.SamplePosition(destination, out navHit, distance, -1); // 네비게이션 메시 상의 위치 샘플링
        return navHit.position;
    }

    // 두 객체가 서로를 바라보게 설정
    private void FaceEachOther(GameObject obj1, GameObject obj2)
    {
        obj1.transform.LookAt(obj2.transform.position); // obj1이 obj2를 바라보게 설정
        obj2.transform.LookAt(obj1.transform.position); // obj2가 obj1을 바라보게 설정
    }

    // 음압실로 이동
    public void GoToNegativePressureRoom(GameObject patientGameObject)
    {
        GoToPatient(patientGameObject); // 환자에게 이동
        patientGameObject.GetComponent<OutpatientController>().isQuarantined = true; // 환자를 격리 상태로 설정
    }

    // 음압실로 이동을 위한 대기 후 이동 코루틴
    public IEnumerator WaitAndGoToNegativePressureRoom(GameObject patientGameObject)
    {
        agent.isStopped = true; // 에이전트 정지
        yield return new WaitForSeconds(1); // 1초 대기
        agent.isStopped = false; // 에이전트 재개
        OutpatientController targetPatient = patientGameObject.GetComponent<OutpatientController>();
        targetPatient.nurse = gameObject; // 간호사 설정
        targetPatient.isFollowingNurse = true; // 환자가 간호사를 따라가도록 설정
        GameObject parentObject = GameObject.Find("NurseWaypoints");
        Waypoint NPRoom = parentObject.transform.Find("N-PRoom (0)").GetComponent<Waypoint>(); // 음압실 웨이포인트 찾기
        agent.SetDestination(NPRoom.GetRandomPointInRange()); // 음압실로 이동
    }

    // 대기 후 랜덤 웨이포인트로 이동 코루틴
    public IEnumerator WaitAndGo()
    {
        isWaiting = true; // 기다리는 중으로 설정
        yield return new WaitForSeconds(1.0f); // 1초 대기
        isWaiting = false; // 기다리는 중 해제
        if(waypoints.Count > 0)
        {
            agent.SetDestination(waypoints[Random.Range(0, waypoints.Count)].GetRandomPointInRange()); // 랜덤 웨이포인트로 이동
        }
    }

    // 애니메이션 업데이트 메서드
    private void UpdateAnimation()
    {
        // NavMesh 상에 없으면 애니메이션 정지
        if (!agent.isOnNavMesh)
        {
            if (animator.GetFloat("MoveSpeed") != 0)
                animator.SetFloat("MoveSpeed", 0);
            if (animator.GetBool("Grounded"))
                animator.SetBool("Grounded", false);
            return;
        }

        // 이동 중 애니메이션
        if (agent.remainingDistance > agent.stoppingDistance)
        {
            animator.SetFloat("MoveSpeed", agent.velocity.magnitude / agent.speed);
        }
        else
        {
            animator.SetFloat("MoveSpeed", 0);
        }

        // 지면 접촉 상태 업데이트
        animator.SetBool("Grounded", !agent.isOnOffMeshLink && agent.isOnNavMesh);
    }
}
