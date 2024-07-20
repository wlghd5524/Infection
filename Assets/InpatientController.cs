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
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        GameObject parentObject = GameObject.Find("InpatientWaypoints");
        Transform waypointTransform = parentObject.transform.Find("InpatientWaypoint (");
        waypoints.Add(waypointTransform.gameObject.GetComponent<Waypoint>());
        waypointTransform = parentObject.transform.Find("ToiletWaypoint (" + (InpatientCreator.numberOfInpatient - 1) / 6 + ")");
        waypoints.Add(waypointTransform.gameObject.GetComponent<Waypoint>());
        parentObject = GameObject.Find("OutpatientWaypoints");
        waypointTransform = parentObject.transform.Find("");
    }

    // Update is called once per frame
    void Update()
    {
        if(isWaiting)
        {
            return;
        }
        StartCoroutine(MoveToNextWaypointAfterWait());
        // 애니메이션
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
    private IEnumerator MoveToNextWaypointAfterWait()
    {
        isWaiting = true;
        yield return new WaitForSeconds(Random.Range(1.0f,2.0f));
        isWaiting = false;
        agent.SetDestination(waypoints[Random.Range(0,waypoints.Count)].GetRandomPointInRange());
        StartCoroutine(UpdateMovementAnimation());
    }
    private IEnumerator UpdateMovementAnimation()
    {
        while (true)
        {
            animator.SetFloat("MoveSpeed", agent.velocity.magnitude / agent.speed);
            yield return null;
        }
    }
}
