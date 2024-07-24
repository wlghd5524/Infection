using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NurseController : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;

    public bool isWorking = false;

    public bool isWaiting = false;

    public GameObject targetPatient;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.avoidancePriority = Random.Range(0, 100);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isWaiting)
        {
            return;
        }
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f))
        {
            if(isWorking)
            {
                FaceEachOther(gameObject, targetPatient);

                targetPatient.GetComponent<OutpatientController>().nurseSignal = true;
                isWorking = false;
                if(targetPatient.GetComponent<OutpatientController>().isQuarantined)
                {
                    StartCoroutine(WaitAndGoToNegativePressureRoom(targetPatient));
                }
            }
        }
    }
    public void GoToPatient(GameObject patientGameObject)
    {
        Vector3 nearRandomPosition = GetPositionInFront(patientGameObject.transform, 1);
        agent.SetDestination(nearRandomPosition);
        targetPatient = patientGameObject;
        isWorking = true;
    }
    private Vector3 GetPositionInFront(Transform targetTransform, float distance)
    {
        Vector3 direction = targetTransform.forward;
        Vector3 destination = targetTransform.position + direction * distance;
        NavMeshHit navHit;
        NavMesh.SamplePosition(destination, out navHit, distance, -1);
        return navHit.position;
    }
    private void FaceEachOther(GameObject obj1, GameObject obj2)
    {
        // obj1이 obj2를 바라보게 설정
        obj1.transform.LookAt(obj2.transform.position);
        // obj2가 obj1을 바라보게 설정
        obj2.transform.LookAt(obj1.transform.position);
    }

    public void GoToNegativePressureRoom(GameObject patientGameObject)
    {
        GoToPatient(patientGameObject);
        patientGameObject.GetComponent<OutpatientController>().isQuarantined = true;
    }
    public IEnumerator WaitAndGoToNegativePressureRoom(GameObject patientGameObject)
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(1);
        agent.isStopped = false;
        OutpatientController targetPatient = patientGameObject.GetComponent<OutpatientController>();
        targetPatient.nurse = gameObject;
        targetPatient.isFollowingNurse = true;
        GameObject parentObject = GameObject.Find("NurseWaypoints");
        Waypoint NPRoom = parentObject.transform.Find("N-PRoom (0)").GetComponent<Waypoint>();
        agent.SetDestination(NPRoom.GetRandomPointInRange());
    }
}
