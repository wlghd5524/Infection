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
    public bool isWaitingAtDoctorOffice = false;

    public GameObject targetPatient; // 타겟 환자
    public List<Waypoint> waypoints; // 웨이포인트 리스트

    public int ward = 0;
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
        Managers.NPCManager.UpdateAnimation(agent,animator);

        if (isWaiting || isRest)
        {
            return; // 기다리는 중이면 리턴
        }
        
        if (isWorking)
            return;

        if (Managers.NPCManager.isArrived(agent))
        {
            if(!isWorking)
            {
                StartCoroutine(MoveToNextWaypointAfterWait()); // 다음 작업을 위해 대기 후 이동
            }
        }
    }

    // 환자에게 이동
    public IEnumerator GoToPatient(GameObject patientGameObject)
    {
        isWorking = true; // 일하는 중으로 설정
        Vector3 targetPatientPosition = Managers.NPCManager.GetPositionInFront(transform, patientGameObject.transform, 0.5f); // 환자 앞의 임의 위치 계산
        agent.SetDestination(targetPatientPosition); // 에이전트 목적지 설정
        targetPatient = patientGameObject; // 타겟 환자 설정

        yield return new WaitUntil(() => !agent.pathPending);
        yield return new WaitUntil(() => agent.remainingDistance == 0);

        Managers.NPCManager.FaceEachOther(gameObject, targetPatient); // 간호사와 환자가 서로를 바라보게 설정
        OutpatientController targetPatientController = targetPatient.GetComponent<OutpatientController>();
        targetPatientController.nurseSignal = true; // 환자에게 간호사가 도착했음을 알림
        //targetPatientController.nurse = gameObject; // 간호사 설정
        targetPatientController.StartCoroutine(targetPatientController.FollowNurse(gameObject));
        agent.speed -= 1;
        yield return StartCoroutine(WaitAndGoToNegativePressureRoom(targetPatientController)); // 격리된 환자라면 음압실로 이동
        yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
        agent.speed += 1;
        isWorking = false;
        targetPatientController.isFollowingNurse = false;
        targetPatientController.isQuarantined = true;
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

        
        Transform parentTransform = GameObject.Find("Waypoints").transform;

        for(int i = 0;i<4;i++)
        {
            NPRoom nPRoom = parentTransform.Find("N-PRoom (" + i + ")").GetComponent<NPRoom>(); // 음압실 웨이포인트 찾기
            if (nPRoom.is_empty)
            {
                targetPatientController.nPRoom = nPRoom;
                nPRoom.is_empty = false;
                agent.SetDestination(nPRoom.GetRandomPointInRange()); // 음압실로 이동
                break;
            }
        }

        

        yield return new WaitUntil(() => !agent.pathPending);
    }

    // 대기 후 랜덤 웨이포인트로 이동 코루틴
    public IEnumerator MoveToNextWaypointAfterWait()
    {
        isWaiting = true; // 기다리는 중으로 설정
        yield return new WaitForSeconds(1.5f); // 1초 대기
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

            else if(waypoints.Count == 7) //입원실 대기 간호사들
            {
                int random = Random.Range(1, waypoints.Count);
                if (!waypoints[random].is_empty && waypoints[random] is BedWaypoint bed)
                {
                    InpatientController targetInpatientController = bed.inpatient.GetComponent<InpatientController>();
                    targetInpatientController.StartCoroutine(targetInpatientController.WaitForNurse());
                    agent.SetDestination(Managers.NPCManager.GetPositionInFront(transform,bed.inpatient.transform, 0.75f));
                    yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
                    Managers.NPCManager.FaceEachOther(bed.inpatient, gameObject);
                    yield return new WaitForSeconds(1.5f);
                    targetInpatientController.nurseSignal = true;
                }
                else
                {
                    agent.SetDestination(waypoints[0].GetRandomPointInRange());
                }
            }
            else
            {
                agent.SetDestination(waypoints[Random.Range(0, waypoints.Count)].GetRandomPointInRange()); // 랜덤 웨이포인트로 이동
            }
        }
    }
}
