using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCClickManager : MonoBehaviour
{
    public float boxCastDistance = 100f;  // �ڽ�ĳ��Ʈ �Ÿ�
    public Vector3 boxCastSize = new Vector3(2f, 2f, 2f); // �ڽ�ĳ��Ʈ ũ��
    public string npcTag = "NPC"; // NPC �±�
    public void LocationOfOutpatient()
    {
        // �յ� �� �������� �ڽ�ĳ��Ʈ ����
        Vector3[] directions = { transform.forward, -transform.forward };

        foreach (var direction in directions)
        {
            RaycastHit hit;

            // �ڽ�ĳ��Ʈ ����
            if (Physics.BoxCast(transform.position, boxCastSize / 2, direction, out hit, Quaternion.identity, boxCastDistance))
            {
                Debug.Log("BoxCast hit: " + hit.collider.name); // BoxCast�� hit�� �ݶ��̴��� �̸� ��������

                // �浹 ������ ���� ���� �׷��� �ð������� Ȯ��
                Debug.DrawRay(hit.point, Vector3.up * 1f, Color.green, 2f);

                // �浹�� Collider�� ���� ������Ʈ�� �� �θ� ������Ʈ���� Ž���Ͽ� Person ������Ʈ�� ã��
                Transform currentTransform = hit.collider.transform;

                while (currentTransform != null)
                {
                    if (currentTransform.CompareTag(npcTag))
                    {
                        Person person = currentTransform.GetComponent<Person>();
                        if (person != null)
                        {
                            Debug.Log("NPC hit by BoxCast: " + person.gameObject.name); // Ŭ���� NPC�� Person���� �̸� ��������
                            gameObject.GetComponent<OutpatientController>().Stop();
                            //��ȣ�翡�� ���� ��ġ�� �˷��ִ� �Լ� ȣ��
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
