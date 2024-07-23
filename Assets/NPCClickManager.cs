using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCClickManager : MonoBehaviour
{
    public float boxCastDistance = 100f;  // 박스캐스트 거리
    public Vector3 boxCastSize = new Vector3(100f, 1f, 100f); // 박스캐스트 크기
    public string npcTag = "Nurse"; // Nurse 태그

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
            // 같은 층에 있는지 확인
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
                    Debug.LogError("nurseController를 찾을 수 없습니다.");
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
