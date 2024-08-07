using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class OutpatientController : MonoBehaviour
{
    // 컴포넌트 참조
    private Animator animator;
    private NavMeshAgent agent;

    // 웨이포인트 관련 변수
    public List<Waypoint> waypoints = new List<Waypoint>();
    public int waypointIndex = 0;

    // 상태 플래그
    public bool isQuarantined = false;
    public bool isFollowingNurse = false;
    public bool isWaiting = false;
    public bool isWaitingForDoctor = false;
    public bool isWaitingForNurse = false;

    public bool officeSignal = false;
    public bool nurseSignal = false;
    public bool doctorSignal = false;

    // 씬 오브젝트 참조
    GameObject parentObject;
    GameObject gatewayObject;
    int randomWard;
    public GameObject nurse;
    public NPRoom nPRoom;

    private void Awake()
    {
        // 컴포넌트 초기화
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.avoidancePriority = Random.Range(0, 100);

        // 씬 오브젝트 찾기
        parentObject = GameObject.Find("OutpatientWaypoints");
        gatewayObject = GameObject.Find("Gateways");
        randomWard = Random.Range(0, 6);
    }

    private void OnEnable()
    {
        // 첫 번째 웨이포인트 추가
        AddWaypoint(parentObject.transform, $"CounterWaypoint (0)");
    }

    private void Update()
    {
        // 애니메이션 업데이트
        NPCMovementUtils.Instance.UpdateAnimation(agent,animator);

        // 대기 중이면 이동 처리하지 않음
        if (isWaiting)
        {
            return;
        }

        // 목적지에 도착했는지 확인
        if (NPCMovementUtils.Instance.isArrived(agent))
        {
            if (waypointIndex == 4 && !isWaitingForNurse && !isFollowingNurse && !isQuarantined)
            {
                // 모든 웨이포인트를 방문했으면 비활성화
                ObjectPoolingManager.Instance.DeactivateOutpatient(gameObject);
                OutpatientCreator.numberOfOutpatient--;
                return;
            }
            
            else
            {
                // 다음 웨이포인트로 이동
                StartCoroutine(MoveToNextWaypointAfterWait());
            }
        }
    }

    // 다음 웨이포인트로 이동하는 코루틴
    private IEnumerator MoveToNextWaypointAfterWait()
    {
        if (waypointIndex > 0 && waypoints[waypointIndex - 1] is DoctorOffice docOffice)
        {
            DoctorController targetDoctor = docOffice.doctor.GetComponent<DoctorController>();
            targetDoctor.outpatient = gameObject;
            targetDoctor.outpatientSignal = true;
            yield return new WaitForSeconds(1.0f);
            yield return new WaitUntil(() => doctorSignal);
            NPCMovementUtils.Instance.FaceEachOther(docOffice.doctor, gameObject);
        }
        isWaiting = true;
        yield return new WaitForSeconds(1.5f);
        isWaiting = false;
        if (isQuarantined)
        {
            agent.SetDestination(nPRoom.GetRandomPointInRange());
            yield break;
        }

        // 현재 웨이포인트 인덱스에 따라 다음 웨이포인트 추가
        AddNextWaypoint();

        if (waypointIndex < waypoints.Count)
        {
            
            // 의사 사무실인 경우 대기
            if (waypoints[waypointIndex] is DoctorOffice doctorOffice)
            {
                StartCoroutine(WaitForDoctorOffice(doctorOffice));
            }
            else if (waypointIndex > 0 && waypoints[waypointIndex - 1] is DoctorOffice doc)
            {
                DoctorController doctorController = doc.doctor.GetComponent<DoctorController>();
                doc.is_empty = true;
                doc.doctor.GetComponent<StressController>().stress += (++doctorController.patientCount / 10) + 1;
                doctorController.outpatientSignal = false;
            }

            // 다음 웨이포인트로 이동
            if (!isWaitingForDoctor)
            {
                agent.SetDestination(waypoints[waypointIndex++].GetRandomPointInRange());
            }
            
        }
    }

    // 다음 웨이포인트 추가 메서드
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

    // 의사 사무실 대기 코루틴
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

    //간호사가 올 때까지 대기 코루틴
    public IEnumerator WaitForNurse()
    {
        agent.isStopped = true;
        yield return new WaitUntil(() => nurseSignal);
        agent.isStopped = false;
    }

    // 웨이포인트 추가 메서드
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
                // 의사 사무실 선택 로직
                SelectDoctorOffice(waypointTransform, childName);
            }
            else
            {
                if (!waypoints.Contains(comp))
                {
                    waypoints.Add(comp);
                }
            }
        }
        else
        {
            Debug.LogWarning($"Can't find waypoint: {childName}");
        }
    }

    // 의사 사무실 선택 메서드
    private void SelectDoctorOffice(Transform officeTransform, string childName)
    {
        // 가능한 의사 사무실 목록 생성
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

        // 최적의 의사 사무실 선택
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
            Debug.LogError("의사를 찾을 수 없습니다.");
        }
    }

    public IEnumerator FollowNurse(GameObject nurse)
    {
        this.nurse = nurse;
        isFollowingNurse = true;
        while(isFollowingNurse == true)
        {
            agent.SetDestination(nurse.transform.position);
            yield return new WaitForSeconds(0.1f);
        }
        agent.ResetPath();
    }
}