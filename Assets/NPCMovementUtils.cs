using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class NPCMovementUtils : MonoBehaviour
{
    private static NPCMovementUtils _instance = new NPCMovementUtils();
    public static NPCMovementUtils Instance { get { return _instance; } }

    public void FaceEachOther(GameObject obj1, GameObject obj2)
    {
        obj1.transform.LookAt(obj2.transform.position); // obj1이 obj2를 바라보게 설정
        obj2.transform.LookAt(obj1.transform.position); // obj2가 obj1을 바라보게 설정
    }

    public void UpdateAnimation(NavMeshAgent agent, Animator animator)
    {
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

    public Vector3 GetPositionInFront(Transform thisTransform, Transform targetTransform, float distance)
    {
        // 대상 오브젝트와 현재 오브젝트 사이의 방향 벡터를 구함
        Vector3 direction = -(targetTransform.position - thisTransform.position).normalized;

        // 대상 오브젝트의 위치로부터 그 방향으로 일정 거리만큼 떨어진 위치 계산
        Vector3 destination = targetTransform.position + (direction * distance);

        // 네비게이션 메시 상의 위치 샘플링
        NavMeshHit navHit;
        NavMesh.SamplePosition(destination, out navHit, distance, NavMesh.AllAreas);

        // 샘플링된 위치 반환
        return navHit.position;
    }
}
