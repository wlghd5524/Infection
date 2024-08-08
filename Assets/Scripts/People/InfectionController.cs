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
        // 충돌한 오브젝트의 레이어가 지정된 레이어 마스크에 포함되어 있는지 확인
        if (other.gameObject == gameObject || thisPersonStatus == InfectionState.Normal)
        {
            return;
        }

        Person otherPerson = other.GetComponent<Person>();
        if(otherPerson == null)
        {
            return;
        }
        if(delayList.Contains(otherPerson))
        {
            //Debug.Log("이미 접촉된 사람");
            return;
        }
        if (otherPerson.status != InfectionState.Normal)
        {
            return;
        }
        int random = Random.Range(0, Managers.Infection.infectionProbability);
        //감염되는 사람의 감염 저항성을 고려하여 감염 확률 계산
        int totalRandom = Random.Range(0, 101);
        if (random - otherPerson.infectionResistance >= totalRandom)
        {
            //Debug.Log(random - otherPerson.infectionResistance + " 값이 나왔기 때문에 감염됨");
            otherPerson.ChangeStatus(thisPersonStatus);
        }
        else
        {
            //Debug.Log(random - otherPerson.infectionResistance + "값이 나왔기 때문에 감염되지 않음");
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