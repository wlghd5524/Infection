using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�÷��̾��� ����(������)�� ��Ÿ���� enum
//Stage1�� ���˼� ������
//Stage2�� �����˼�(������) ������
public enum InfectionState
{
    Normal,
    Stage1,
    Stage2
}
public enum Role
{
    Doctor,
    Nurse,
    Outpatient,
    Inpatient
}
public class Person : MonoBehaviour
{
    public List<Item> inventory = new List<Item>();
    public InfectionState status = InfectionState.Normal;
    public int infectionResistance = 0;
    private MeshRenderer ballRenderer;
    private CapsuleCollider coll;
    public Role role;
    private bool isWaiting;
    void Start()
    {
        Transform ballTransform = transform.Find("IsInfection");
        ballRenderer = ballTransform.GetComponent<MeshRenderer>();
        coll = GetComponent<CapsuleCollider>();
    }
    void Update()
    {

        //������ ������ ���� ���� ���� ����
        if (status == InfectionState.Stage1)
        {
            coll.radius = 0.3f;
        }
        else if (status == InfectionState.Stage2)
        {
            coll.radius = 1.0f;
        }
        else if (status == InfectionState.Normal)
        {
            coll.radius = 0.2f;
        }
        if (isWaiting)
        {
            return;
        }
        if (status != InfectionState.Normal)
        {
            ballRenderer.enabled = true;
        }
        else
        {
            ballRenderer.enabled = false;
        }

        //�����ϰ� �ִ� ��ȣ ��� ���� ���� ���׼� ����
        
    }
    public void ChangeStatus(InfectionState infection)
    {
        StartCoroutine(IncubationPeriod(infection));
    }

    public void Recover()
    {
        status = InfectionState.Normal;
    }
    private IEnumerator IncubationPeriod(InfectionState infection)
    {
        isWaiting = true;
        yield return new WaitForSeconds(5);
        isWaiting = false;
        status = infection;
    }
}
