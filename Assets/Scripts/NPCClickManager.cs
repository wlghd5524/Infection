using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NPCClickManager Ŭ������ NPC(���⼭�� ��ȣ��)�� �˻��ϰ�, ����ũ�� �����ϰų� �ݸ��ϴ� ���� �۾��� �����մϴ�.
public class NPCClickManager : MonoBehaviour
{
    // �ڽ�ĳ��Ʈ�� �Ÿ� ����
    public float boxCastDistance = 100f;  // �ڽ�ĳ��Ʈ �Ÿ�
    // �ڽ�ĳ��Ʈ�� ũ�� ����
    public Vector3 boxCastSize = new Vector3(100f, 1f, 100f); // �ڽ�ĳ��Ʈ ũ��
    // NPC�� �±׸� ���� (���⼭�� 'Nurse')
    public string npcTag = "Nurse"; // Nurse �±�

    // ��ȣ�縦 �˻��ϴ� �޼���, origin ��ġ���� ���� ����� ��ȣ�縦 ã���ϴ�.
    public GameObject SearchNurse(Vector3 origin)
    {
        Transform closestNurse = null;
        float closestDistance = Mathf.Infinity;
        // 'Nurse' �±׸� ���� ��� ���� ������Ʈ�� ã���ϴ�.
        GameObject[] nurses = GameObject.FindGameObjectsWithTag(npcTag);

        foreach (GameObject nurse in nurses)
        {
            // �� ��ȣ���� NurseController ������Ʈ�� �����ɴϴ�.
            NurseController nurseController = nurse.GetComponent<NurseController>();
            // ��ȣ�簡 �ٹ� ���̰� �ǻ� �繫�ǿ��� ��� ���̸� �ǳʶݴϴ�.
            if (nurseController.isWorking || nurseController.isWaitingAtDoctorOffice || nurseController.isRest)
            {
                continue;
            }
            // ��ȣ�簡 ���� ���� �ִ��� Ȯ���մϴ�.
            if (Mathf.Abs(origin.y - nurse.transform.position.y) <= 1.0f)
            {
                // origin�� ��ȣ�� ������ �Ÿ��� ����մϴ�.
                float distance = Vector3.Distance(origin, nurse.transform.position);
                // ���� ����� ��ȣ�縦 ������Ʈ�մϴ�.
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNurse = nurse.transform;
                }
            }
        }
        // ���� ����� ��ȣ�簡 ������ �α׿� �̸��� ����մϴ�.
        if (closestNurse != null)
        {
            Person person = closestNurse.GetComponent<Person>();
            if (person != null)
            {
                Debug.Log("Closest Nurse found: " + person.gameObject.name);
            }
        }
        else
        {
            Debug.Log("No Nurse found.");
        }
        // ���� ����� ��ȣ���� ���� ������Ʈ�� ��ȯ�մϴ�.
        return closestNurse.gameObject;
    }

    // ���� ����� ��ȣ�簡 ����ũ�� �����ϵ��� �����ϴ� �޼���
    public void WearingMask(GameObject closestNurse)
    {
        // OutpatientController ������Ʈ�� �����ͼ� nurseSignal�� false�� �����ϰ� �ڷ�ƾ�� �����մϴ�.
        OutpatientController outpatientController = gameObject.GetComponent<OutpatientController>();
        outpatientController.nurseSignal = false;
        outpatientController.StartCoroutine(outpatientController.WaitForNurse());
        // ��ȣ���� NurseController ������Ʈ�� �����ɴϴ�.
        NurseController nurseController = closestNurse.GetComponent<NurseController>();
        if (nurseController == null)
        {
            Debug.LogError("nurseController�� ã�� �� �����ϴ�.");
        }
        else
        {
            // ��ȣ�簡 ȯ�ڿ��� ������ �����մϴ�.
            nurseController.StartCoroutine(nurseController.GoToPatient(gameObject));
        }
    }

    // ���� ����� ��ȣ�縦 �ݸ��Ƿ� ������ �޼���
    public void Quarantine(GameObject closestNurse)
    {
        // OutpatientController ������Ʈ�� �����ͼ� nurseSignal�� false�� �����ϰ� �ڷ�ƾ�� �����մϴ�.
        OutpatientController outpatientController = gameObject.GetComponent<OutpatientController>();
        outpatientController.nurseSignal = false;
        outpatientController.StartCoroutine(outpatientController.WaitForNurse());
        // ��ȣ���� NurseController ������Ʈ�� �����ɴϴ�.
        NurseController nurseController = closestNurse.GetComponent<NurseController>();
        if (nurseController == null)
        {
            Debug.LogError("nurseController�� ã�� �� �����ϴ�.");
        }
        else
        {
            // ��ȣ�簡 �ݸ��Ƿ� ������ �����մϴ�.
            nurseController.GoToNegativePressureRoom(gameObject);
        }
    }
}
