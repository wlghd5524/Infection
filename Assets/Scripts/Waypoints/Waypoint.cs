using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public bool is_empty = true;
    public Vector3 rangeSize; // ��������Ʈ ������ ũ��

    // ���� ������ ���� ��ġ�� ��ȯ
    public Vector3 GetRandomPointInRange()
    {
        Vector3 randomPoint = new Vector3(
            Random.Range(-rangeSize.x / 2, rangeSize.x / 2),
            Random.Range(-rangeSize.y / 2, rangeSize.y / 2),
            Random.Range(-rangeSize.z / 2, rangeSize.z / 2)
        );
        return transform.position + randomPoint;
    }


    // Gizmos�� ����Ͽ� ������ �ð������� ǥ��
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, rangeSize);
    }
}
