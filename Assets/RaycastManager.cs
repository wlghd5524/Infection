using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastManager : MonoBehaviour
{
    public static RaycastManager Instance { get; private set; }

    public string npcLayerName = "NPC"; // NPC Layer 이름
    private LayerMask npcLayerMask;     // Culling Mask


    private GameObject lastHighlightedNPC = null; // 마지막으로 하이라이트된 NPC

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //mainCamera = Camera.main;
        // NPC Layer의 LayerMask 설정
        npcLayerMask = LayerMask.GetMask(npcLayerName);
    }

    private void FixedUpdate()
    {
        HandleMouseInput();
        //HandleMouseHover();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) // 왼쪽 마우스 버튼이 클릭되면
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Ray를 시각적으로 그리기
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);

            // 충돌을 감지하도록 설정, NPC Layer만 검사
            if (Physics.Raycast(ray, out hit, 100f, npcLayerMask))
            {
                //Debug.Log("Raycast hit: " + hit.collider.name); // Raycast가 hit한 콜라이더의 이름 가져오기

                // 충돌 지점에 작은 구를 그려서 시각적으로 확인
                Debug.DrawRay(hit.point, Vector3.up * 1f, Color.green, 2f);

                // 충돌한 Collider를 가진 오브젝트와 그 부모 오브젝트들을 탐색하여 Person 컴포넌트를 찾음
                Transform currentTransform = hit.collider.transform;

                while (currentTransform != null)
                {
                    Person person = currentTransform.GetComponent<Person>();
                    if (person != null)
                    {
                        Debug.Log("NPC clicked: " + person.gameObject.name);    // 클릭된 NPC의 Person으로 이름 가져오기
                        NPCClickManager targetNPCCLickManager = currentTransform.GetComponent<NPCClickManager>();
                        if (person.gameObject.CompareTag("OutPatient"))
                        {
                            //마스크 씌우기
                            //targetNPCCLickManager.WearingMask(targetNPCCLickManager.SearchNurse(person.gameObject.transform.position));

                            //음압실 데려가기
                            targetNPCCLickManager.Quarantine(targetNPCCLickManager.SearchNurse(person.gameObject.transform.position));
                        }
                            
                        //UIManager.Instance.ToggleNPCInfo(person);
                        break;
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
                Debug.Log("Raycast did not hit anything");
            }
        }
    }

    private void HandleMouseHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, npcLayerMask))
        {
            // 충돌한 Collider를 가진 오브젝트와 그 부모 오브젝트들을 탐색하여 Person 컴포넌트를 찾음
            Transform currentTransform = hit.collider.transform;

            while (currentTransform != null)
            {
                Person person = currentTransform.GetComponent<Person>();
                if (person != null)
                {
                    Debug.Log("Mouse over NPC: " + hit.collider.gameObject.name);

                    //NPCManager.Instance.HighlightNPC(hit.collider.gameObject); // HighlightNPC 호출


                    // 마지막 하이라이트된 NPC가 현재 NPC가 아니면 마지막 하이라이트된 NPC의 하이라이트를 해제
                    if (lastHighlightedNPC != null && lastHighlightedNPC != hit.collider.gameObject)
                    {
                        //NPCManager.Instance.UnhighlightNPC(lastHighlightedNPC);
                    }

                    lastHighlightedNPC = hit.collider.gameObject;
                    return;
                }
                currentTransform = currentTransform.parent;
            }
        }

        // 만약 Raycast가 NPC에 닿지 않으면 모든 하이라이트 해제
        if (lastHighlightedNPC != null)
        {
            //NPCManager.Instance.UnhighlightAllNPCs();
            lastHighlightedNPC = null;
        }
    }
}
