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
        //    // ������ ����Ʈ���� �������� �ϳ� ����
        //    GameObject prefabToSpawn = prefabs[Random.Range(0, prefabs.Length)];
        //    spawnArea = (GameObject.Find("InpatientWaypoints")).transform.GetChild(numberOfInpatient).gameObject;
        //    // ���� ��ġ ����
        //    Vector3 randomPosition = spawnArea.GetComponent<Waypoint>().GetRandomPointInRange();
        //    // ������ ����
        //    newInpatient = Instantiate(prefabToSpawn, randomPosition, Quaternion.identity);
        //    Person newInpatientPerson = newInpatient.GetComponent<Person>();
        //    newInpatientPerson.role = Role.Inpatient;
        //    numberOfInpatient++;
        //}
    }
}
