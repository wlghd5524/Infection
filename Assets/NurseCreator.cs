using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NurseCreator : MonoBehaviour
{
    public static NurseCreator Instance; // NurseCreator�� �̱��� �ν��Ͻ�
    public int numberOfNurse = 0; // ���� ��ȣ�� ��

    // Start�� ù ������ ������Ʈ ���� ȣ��˴ϴ�.
    void Start()
    {
        Instance = this; // �̱��� �ν��Ͻ� ����
        for (int i = 0; i < ObjectPoolingManager.Instance.maxOfNurse; i++) // �ִ� ��ȣ�� ����ŭ ���� ����
        {
            if (i % 20 < 10) // ���ǿ� ���� Ư�� ��ȣ�縸 Ȱ��ȭ
            {
                GameObject newNurse = GameObject.Find("Nurse " + i); // ��ȣ�� ��ü ã��
                ObjectPoolingManager.Instance.ActivateNurse(newNurse); // ��ȣ�� Ȱ��ȭ
                Transform ward = GameObject.Find("NurseWaypoints").transform.Find("Ward (" + i / 20 + ")"); // ���� ã��

                switch (i % 10) // ��ȣ���� ������ ���� ��������Ʈ ����
                {
                    case 0:
                        newNurse.GetComponent<NurseController>().waypoints.Add(ward.transform.Find("PatientRoom").GetComponent<Waypoint>()); // ȯ�ڽ� ��������Ʈ �߰�
                        break;
                    case 1:
                    case 2:
                    case 3:
                        for (int j = 0; j < 5; j++)
                        {
                            newNurse.GetComponent<NurseController>().waypoints.Add(ward.transform.Find("Doctor'sOffice (" + j + ")").GetComponent<Waypoint>()); // �ǻ�� ��������Ʈ �߰�
                        }
                        break;
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                        newNurse.GetComponent<NurseController>().waypoints.Add(ward.transform.Find("Counter").GetComponent<Waypoint>()); // ī���� ��������Ʈ �߰�
                        break;
                    case 8:
                    case 9:
                        newNurse.GetComponent<NurseController>().waypoints.Add(GameObject.Find("OutPatientWaypoints").transform.Find("Ward (" + i / 20 + ")").transform.Find("CounterWaypoint (0)").GetComponent<Waypoint>()); // �ܷ�ȯ�� ī���� ��������Ʈ �߰�
                        newNurse.GetComponent<NurseController>().waypoints.Add(GameObject.Find("OutPatientWaypoints").transform.Find("Ward (" + i / 20 + ")").transform.Find("SofaWaypoint (0)").GetComponent<Waypoint>()); // �ܷ�ȯ�� ���� ��������Ʈ �߰�
                        break;
                }
            }
        }
    }

    // Update�� �� ������ ȣ��˴ϴ�.
    void Update()
    {

    }
}
