using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public bool is_empty = true;
    public Vector3 rangeSize; // 웨이포인트 범위의 크기

    // 범위 내에서 랜덤 위치를 반환
    public Vector3 GetRandomPointInRange()
    {
        Vector3 randomPoint = new Vector3(
            Random.Range(-rangeSize.x / 2, rangeSize.x / 2),
            Random.Range(-rangeSize.y / 2, rangeSize.y / 2),
            Random.Range(-rangeSize.z / 2, rangeSize.z / 2)
        );
        return transform.position + randomPoint;
    }


    // Gizmos를 사용하여 범위를 시각적으로 표시
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, rangeSize);
    }
}
