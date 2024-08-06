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
        obj1.transform.LookAt(obj2.transform.position); // obj1�� obj2�� �ٶ󺸰� ����
        obj2.transform.LookAt(obj1.transform.position); // obj2�� obj1�� �ٶ󺸰� ����
    }

    public void UpdateAnimation(NavMeshAgent agent, Animator animator)
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

    public Vector3 GetPositionInFront(Transform thisTransform, Transform targetTransform, float distance)
    {
        // ��� ������Ʈ�� ���� ������Ʈ ������ ���� ���͸� ����
        Vector3 direction = -(targetTransform.position - thisTransform.position).normalized;

        // ��� ������Ʈ�� ��ġ�κ��� �� �������� ���� �Ÿ���ŭ ������ ��ġ ���
        Vector3 destination = targetTransform.position + (direction * distance);

        // �׺���̼� �޽� ���� ��ġ ���ø�
        NavMeshHit navHit;
        NavMesh.SamplePosition(destination, out navHit, distance, NavMesh.AllAreas);

        // ���ø��� ��ġ ��ȯ
        return navHit.position;
    }
}
