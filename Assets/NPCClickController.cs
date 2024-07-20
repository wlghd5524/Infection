using UnityEngine;
using System.Collections;

public class NPCClickController : MonoBehaviour
{
    // �� ī�޶� �����ϱ� ���� ����
    private Camera mainCamera;
    public float lineSize = 16f;

    public LayerMask npcLayer;
    void Start()
    {
        // �� ī�޶� ã�Ƽ� ������ ����
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera�� ã�� �� �����ϴ�.");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("���콺 Ŭ�� ������");

            // ī�޶� �������� ����ĳ��Ʈ �߻�
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, npcLayer))
            {
                //Debug.Log("����ĳ��Ʈ �浹 ������: " + hit.collider.gameObject.name);

                if (hit.collider.gameObject.CompareTag("OutPatient"))
                {
                   Debug.Log("Outpatient ������Ʈ�� Ŭ���Ǿ����ϴ�: " + hit.collider.gameObject.name);
                }
                else
                {
                    //Debug.Log("�±װ� Outpatient�� �ƴ�: " + hit.collider.gameObject.tag);
                }
            }
            else
            {
                //Debug.Log("����ĳ��Ʈ�� �浹���� ����");
            }
        }
    }
}
