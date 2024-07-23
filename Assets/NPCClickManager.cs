using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCClickManager : MonoBehaviour
{
    public float boxCastDistance = 100f;  // �ڽ�ĳ��Ʈ �Ÿ�
    public Vector3 boxCastSize = new Vector3(100f, 1f, 100f); // �ڽ�ĳ��Ʈ ũ��
    public string npcTag = "Nurse"; // Nurse �±�

    public void SearchNurse(Vector3 origin)
    {
        Transform closestNurse = null;
        float closestDistance = Mathf.Infinity;
        GameObject[] nurses = GameObject.FindGameObjectsWithTag(npcTag);

        foreach (GameObject nurse in nurses)
        {
            if(nurse.GetComponent<NurseController>().isWorking)
            {
                continue;
            }
            // ���� ���� �ִ��� Ȯ��
            if (Mathf.Abs(origin.y - nurse.transform.position.y) <= 1.0f)
            {
                float distance = Vector3.Distance(origin, nurse.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNurse = nurse.transform;
                }
            }
        }

        if (closestNurse != null)
        {
            Person person = closestNurse.GetComponent<Person>();
            if (person != null)
            {
                Debug.Log("Closest Nurse found: " + person.gameObject.name);
                OutpatientController outpatientController = gameObject.GetComponent<OutpatientController>();
                outpatientController.nurseSignal = false;
                outpatientController.StartCoroutine(outpatientController.WaitForNurse());
                NurseController nurseController = person.gameObject.transform.GetComponent<NurseController>();
                if(nurseController == null)
                {
                    Debug.LogError("nurseController�� ã�� �� �����ϴ�.");
                }
                else
                {
                    nurseController.GoToPatient(gameObject);
                }
            }
        }
        else
        {
            Debug.Log("No Nurse found.");
        }
    }
}
