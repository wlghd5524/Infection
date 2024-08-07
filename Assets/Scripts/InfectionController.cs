using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectionController : MonoBehaviour
{
    private List<Person> delayList = new List<Person>();
    public LayerMask layerMask;
    void OnTriggerEnter(Collider other)
    {
        InfectionState thisPersonStatus = GetComponent<Person>().status;
        // �浹�� ������Ʈ�� ���̾ ������ ���̾� ����ũ�� ���ԵǾ� �ִ��� Ȯ��
        if (((1 << other.gameObject.layer) & layerMask) == 0 || other.gameObject == gameObject || thisPersonStatus == InfectionState.Normal)
        {
            return;
        }

        Person otherPerson = other.GetComponent<Person>();
        if(delayList.Contains(otherPerson))
        {
            //Debug.Log("�̹� ���˵� ���");
            return;
        }
        if (otherPerson.status != InfectionState.Normal)
        {
            return;
        }
        int random = Random.Range(0, InfectionManager.Instance.infectionProbability);
        //�����Ǵ� ����� ���� ���׼��� ����Ͽ� ���� Ȯ�� ���
        int totalRandom = Random.Range(0, 101);
        if (random - otherPerson.infectionResistance >= totalRandom)
        {
            //Debug.Log(random - otherPerson.infectionResistance + " ���� ���Ա� ������ ������");
            otherPerson.ChangeStatus(thisPersonStatus);
        }
        else
        {
            //Debug.Log(random - otherPerson.infectionResistance + "���� ���Ա� ������ �������� ����");
        }
        delayList.Add(otherPerson);
        StartCoroutine(CoRemoveDelay(otherPerson));
    }

    IEnumerator CoRemoveDelay(Person person)
    {
        yield return new WaitForSeconds(0.5f);
        delayList.Remove(person);
    }
}