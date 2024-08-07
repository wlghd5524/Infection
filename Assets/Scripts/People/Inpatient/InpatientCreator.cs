using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InpatientCreator : MonoBehaviour
{
    public GameObject[] prefabs;
    public static int numberOfInpatient = 0;
    public int maxOfInpatient = 24;
    private GameObject spawnArea;
    private GameObject newInpatient;
    // Start is called before the first frame update
    void Start()
    {
        //prefabs = Resources.LoadAll<GameObject>("Prefabs/Test");
        
    }

    // Update is called once per frame
    void Update()
    {
        //if(numberOfInpatient < maxOfInpatient)
        //{
        //    // 프리팹 리스트에서 랜덤으로 하나 선택
        //    GameObject prefabToSpawn = prefabs[Random.Range(0, prefabs.Length)];
        //    spawnArea = (GameObject.Find("InpatientWaypoints")).transform.GetChild(numberOfInpatient).gameObject;
        //    // 랜덤 위치 설정
        //    Vector3 randomPosition = spawnArea.GetComponent<Waypoint>().GetRandomPointInRange();
        //    // 프리팹 생성
        //    newInpatient = Instantiate(prefabToSpawn, randomPosition, Quaternion.identity);
        //    Person newInpatientPerson = newInpatient.GetComponent<Person>();
        //    newInpatientPerson.role = Role.Inpatient;
        //    numberOfInpatient++;
        //}
    }
}
