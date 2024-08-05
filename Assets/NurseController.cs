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
    public bool isRest = false;
    //public bool arriveNPRoom = false;
    public bool isWaitingAtDoctorOffice = false;

    public GameObject targetPatient; // 타겟 환자
    public List<Waypoint> waypoints; // 웨이포인트 리스트

    public int ward = 0;
    private void Awake()
    {
        animator = GetComponent<Animator>(); // 애니메이터 컴포넌트 할당
        agent = GetComponent<NavMeshAgent>(); // 네비게이션 에이전트 컴포넌트 할당
        agent.avoidancePriority = Random.Range(0, 1000); // 에이전트 회피 우선순위 설정
    }

    // Start는 첫 프레임 업데이트 전에 호출됩니다.
    void Start()
    {

    }

    // Update는 매 프레임 호출됩니다.
    void Update()
    {
        // 애니메이션 업데이트
        UpdateAnimation();

        if (isWaiting || isRest)
        {
            return; // 기다리는 중이면 리턴
        }
        
        if (isWorking)
            return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f))
        {
            //if (isWorking)
            //{
            //    FaceEachOther(gameObject, targetPatient); // 간호사와 환자가 서로를 바라보게 설정
            //    OutpatientController targetPatientController = targetPatient.GetComponent<OutpatientController>();
            //    targetPatientController.nurseSignal = true; // 환자에게 간호사가 도착했음을 알림
            //    targetPatientController.nurse = gameObject; // 간호사 설정
            //    targetPatientController.isFollowingNurse = true; // 환자가 간호사를 따라가도록 설정
            //    if (targetPatient.GetComponent<OutpatientController>().isFollowingNurse)
            //    {
            //        StartCoroutine(WaitAndGoToNegativePressureRoom(targetPatient)); // 격리된 환자라면 음압실로 이동
            //    }
            //    if(targetPatient.GetComponent<OutpatientController>().isQuarantined)
            //    {
            //        isWorking = false;
            //    }
                
            //}
            if(!isWorking)
            {
                StartCoroutine(WaitAndGo()); // 다음 작업을 위해 대기 후 이동
            }
            
        }
    }

    // 환자에게 이동
    public IEnumerator GoToPatient(GameObject patientGameObject)
    {
        isWorking = true; // 일하는 중으로 설정
        Vector3 nearRandomPosition = GetPositionInFront(patientGameObject.transform, 1); // 환자 앞의 임의 위치 계산
        agent.SetDestination(nearRandomPosition); // 에이전트 목적지 설정
        targetPatient = patientGameObject; // 타겟 환자 설정

        yield return new WaitUntil(() => !agent.pathPending);
        yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance);

        FaceEachOther(gameObject, targetPatient); // 간호사와 환자가 서로를 바라보게 설정
        OutpatientController targetPatientController = targetPatient.GetComponent<OutpatientController>();
        targetPatientController.nurseSignal = true; // 환자에게 간호사가 도착했음을 알림
        //targetPatientController.nurse = gameObject; // 간호사 설정
        targetPatientController.StartCoroutine(targetPatientController.FollowNurse(gameObject));
        agent.speed -= 1;
        yield return StartCoroutine(WaitAndGoToNegativePressureRoom(targetPatientController)); // 격리된 환자라면 음압실로 이동
        yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance);
        agent.speed += 1;
        isWorking = false;
        targetPatientController.isFollowingNurse = false;
        targetPatientController.isQuarantined = true;
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
        StartCoroutine(GoToPatient(patientGameObject)); // 환자에게 이동
    }

    // 음압실로 이동을 위한 대기 후 이동 코루틴
    IEnumerator WaitAndGoToNegativePressureRoom(OutpatientController targetPatientController)
    {
        //agent.isStopped = true; // 에이전트 정지
        //yield return new WaitForSeconds(1); // 1초 대기
        //agent.isStopped = false; // 에이전트 재개

        
        GameObject parentObject = GameObject.Find("NurseWaypoints");

        for(int i = 0;i<4;i++)
        {
            NPRoom nPRoom = parentObject.transform.Find("N-PRoom (" + i + ")").GetComponent<NPRoom>(); // 음압실 웨이포인트 찾기
            if (nPRoom.is_Empty)
            {
                targetPatientController.nPRoom = nPRoom;
                nPRoom.is_Empty = false;
                agent.SetDestination(nPRoom.GetRandomPointInRange()); // 음압실로 이동
                break;
            }
        }

        

        yield return new WaitUntil(() => !agent.pathPending);
    }

    // 대기 후 랜덤 웨이포인트로 이동 코루틴
    public IEnumerator WaitAndGo()
    {
        isWaiting = true; // 기다리는 중으로 설정
        yield return new WaitForSeconds(1.0f); // 1초 대기
        isWaiting = false; // 기다리는 중 해제
        if(waypoints.Count > 0)
        {

            if (waypoints.Count == 5)  //진료실 앞 대기 간호사들
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
                agent.SetDestination(waypoints[Random.Range(0, waypoints.Count)].GetRandomPointInRange()); // 랜덤 웨이포인트로 이동
            }
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
