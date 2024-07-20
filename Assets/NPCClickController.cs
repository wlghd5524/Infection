using UnityEngine;
using System.Collections;

public class NPCClickController : MonoBehaviour
{
    // 주 카메라를 참조하기 위한 변수
    private Camera mainCamera;
    public float lineSize = 16f;

    public LayerMask npcLayer;
    void Start()
    {
        // 주 카메라를 찾아서 변수에 저장
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera를 찾을 수 없습니다.");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("마우스 클릭 감지됨");

            // 카메라를 기준으로 레이캐스트 발사
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, npcLayer))
            {
                //Debug.Log("레이캐스트 충돌 감지됨: " + hit.collider.gameObject.name);

                if (hit.collider.gameObject.CompareTag("OutPatient"))
                {
                   Debug.Log("Outpatient 오브젝트가 클릭되었습니다: " + hit.collider.gameObject.name);
                }
                else
                {
                    //Debug.Log("태그가 Outpatient가 아님: " + hit.collider.gameObject.tag);
                }
            }
            else
            {
                //Debug.Log("레이캐스트가 충돌하지 않음");
            }
        }
    }
}
