using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InpatientController : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;
    public List<Waypoint> waypoints = new List<Waypoint>();
    private bool isWaiting = false;
    public bool nurseSignal = false;
    public GameObject bedWaypoint;
    public int ward;
    public GameObject nurse;
    private int prevWaypointIndex;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        waypoints.Add(bedWaypoint.GetComponent<Waypoint>());
        waypoints[0].is_empty = false;
        Transform waypointTransform = bedWaypoint.transform.parent;
        waypoints.Add(waypointTransform.Find("ToiletWaypoint").gameObject.GetComponent<Waypoint>());
        for(int i = 0;i<3;i++)
        {
            waypoints.Add(waypointTransform.Find("VendingMachineWaypoint (" + i + ")").gameObject.GetComponent<Waypoint>());
        }
        waypoints.Add(Managers.NPCManager.waypointDictionary[(ward, "OutpatientWaypoints")].Find("CounterWaypoint (0)").gameObject.GetComponent<Waypoint>());
    }

    // Update is called once per frame
    void Update()
    {
        // 애니메이션
        Managers.NPCManager.UpdateAnimation(agent, animator);
        if (isWaiting)
        {
            return;
        }
        // 목적지에 도착했는지 확인
        if (Managers.NPCManager.isArrived(agent))
            StartCoroutine(MoveToNextWaypointAfterWait());
    }
    private IEnumerator MoveToNextWaypointAfterWait()
    {

        if (prevWaypointIndex == 1)
        {
            waypoints[1].is_empty = true;
        }

        isWaiting = true;
        yield return new WaitForSeconds(2.0f);
        isWaiting = false;
        
        float random = Random.Range(1, 101);
        if(random <= 85)
        {
            agent.SetDestination(waypoints[0].GetRandomPointInRange());
            yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
            waypoints[0].is_empty = false;
            prevWaypointIndex = 0;
        }
        else if(random <= 90 && waypoints[1].is_empty)
        {
            waypoints[0].is_empty = true;
            waypoints[1].is_empty = false;
            agent.SetDestination(waypoints[1].GetRandomPointInRange());
            prevWaypointIndex = 1;
        }
        else if(random <= 95)
        {
            waypoints[0].is_empty = true;
            agent.SetDestination(waypoints[Random.Range(2, 5)].GetRandomPointInRange());
            prevWaypointIndex = 2;
        }
        else
        {
            waypoints[0].is_empty = true;
            agent.SetDestination(waypoints[5].GetRandomPointInRange());
            prevWaypointIndex = 5;
        }
    }

    //간호사가 올 때까지 대기 코루틴
    public IEnumerator WaitForNurse()
    {
        agent.isStopped = true;
        yield return new WaitUntil(() => nurseSignal);
        yield return new WaitForSeconds(2.0f);
        agent.isStopped = false;
        nurseSignal = false;
    }
}
