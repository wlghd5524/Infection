using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastManager : MonoBehaviour
{
    public static RaycastManager Instance { get; private set; }

    public string npcLayerName = "NPC"; // NPC Layer �̸�
    private LayerMask npcLayerMask;     // Culling Mask


    private GameObject lastHighlightedNPC = null; // ���������� ���̶���Ʈ�� NPC

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
        // NPC Layer�� LayerMask ����
        npcLayerMask = LayerMask.GetMask(npcLayerName);
    }

    private void FixedUpdate()
    {
        HandleMouseInput();
        //HandleMouseHover();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) // ���� ���콺 ��ư�� Ŭ���Ǹ�
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Ray�� �ð������� �׸���
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);

            // �浹�� �����ϵ��� ����, NPC Layer�� �˻�
            if (Physics.Raycast(ray, out hit, 100f, npcLayerMask))
            {
                //Debug.Log("Raycast hit: " + hit.collider.name); // Raycast�� hit�� �ݶ��̴��� �̸� ��������

                // �浹 ������ ���� ���� �׷��� �ð������� Ȯ��
                Debug.DrawRay(hit.point, Vector3.up * 1f, Color.green, 2f);

                // �浹�� Collider�� ���� ������Ʈ�� �� �θ� ������Ʈ���� Ž���Ͽ� Person ������Ʈ�� ã��
                Transform currentTransform = hit.collider.transform;

                while (currentTransform != null)
                {
                    Person person = currentTransform.GetComponent<Person>();
                    if (person != null)
                    {
                        Debug.Log("NPC clicked: " + person.gameObject.name);    // Ŭ���� NPC�� Person���� �̸� ��������
                        NPCClickManager targetNPCCLickManager = currentTransform.GetComponent<NPCClickManager>();
                        if (person.gameObject.CompareTag("OutPatient"))
                        {
                            //����ũ �����
                            //targetNPCCLickManager.WearingMask(targetNPCCLickManager.SearchNurse(person.gameObject.transform.position));

                            //���н� ��������
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
            // �浹�� Collider�� ���� ������Ʈ�� �� �θ� ������Ʈ���� Ž���Ͽ� Person ������Ʈ�� ã��
            Transform currentTransform = hit.collider.transform;

            while (currentTransform != null)
            {
                Person person = currentTransform.GetComponent<Person>();
                if (person != null)
                {
                    Debug.Log("Mouse over NPC: " + hit.collider.gameObject.name);

                    //NPCManager.Instance.HighlightNPC(hit.collider.gameObject); // HighlightNPC ȣ��


                    // ������ ���̶���Ʈ�� NPC�� ���� NPC�� �ƴϸ� ������ ���̶���Ʈ�� NPC�� ���̶���Ʈ�� ����
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

        // ���� Raycast�� NPC�� ���� ������ ��� ���̶���Ʈ ����
        if (lastHighlightedNPC != null)
        {
            //NPCManager.Instance.UnhighlightAllNPCs();
            lastHighlightedNPC = null;
        }
    }
}
