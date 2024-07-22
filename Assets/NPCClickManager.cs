using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCClickManager : MonoBehaviour
{
    public float boxCastDistance = 100f;  // 박스캐스트 거리
    public Vector3 boxCastSize = new Vector3(2f, 2f, 2f); // 박스캐스트 크기
    public string npcTag = "NPC"; // NPC 태그
    public void LocationOfOutpatient()
    {
        // 앞뒤 두 방향으로 박스캐스트 수행
        Vector3[] directions = { transform.forward, -transform.forward };

        foreach (var direction in directions)
        {
            RaycastHit hit;

            // 박스캐스트 수행
            if (Physics.BoxCast(transform.position, boxCastSize / 2, direction, out hit, Quaternion.identity, boxCastDistance))
            {
                Debug.Log("BoxCast hit: " + hit.collider.name); // BoxCast가 hit한 콜라이더의 이름 가져오기

                // 충돌 지점에 작은 구를 그려서 시각적으로 확인
                Debug.DrawRay(hit.point, Vector3.up * 1f, Color.green, 2f);

                // 충돌한 Collider를 가진 오브젝트와 그 부모 오브젝트들을 탐색하여 Person 컴포넌트를 찾음
                Transform currentTransform = hit.collider.transform;

                while (currentTransform != null)
                {
                    if (currentTransform.CompareTag(npcTag))
                    {
                        Person person = currentTransform.GetComponent<Person>();
                        if (person != null)
                        {
                            Debug.Log("NPC hit by BoxCast: " + person.gameObject.name); // 클릭된 NPC의 Person으로 이름 가져오기
                            gameObject.GetComponent<OutpatientController>().Stop();
                            //간호사에게 나의 위치를 알려주는 함수 호출
                            person.gameObject.GetComponent<NurseManager>().FollowPatient(gameObject.transform.position);
                            
                            
                        }
                    }
                    currentTransform = currentTransform.parent;
                }

                if (currentTransform == null)
                {
                    Debug.Log("No Person component found on the hit object or its parents.");
                }
            }
            else
            {
                Debug.Log("BoxCast did not hit anything");
            }
            
        }
    }

}
