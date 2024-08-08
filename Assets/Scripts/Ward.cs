using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ward : MonoBehaviour
{
    public int num;
    public List<int> currentNumberOfNPC = new List<int>();

    // Start is called before the first frame update
    void Awake()
    {
        num = gameObject.name[gameObject.name.Length - 2] - '0';

        Waypoint[] waypoints = transform.GetComponentsInChildren<Waypoint>();
        foreach(Waypoint waypoint in waypoints)
        {
            if(waypoint != null)
            {
                waypoint.ward = num;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
