using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NurseWaitingPoint : Waypoint
{
    public bool is_empty = true;
    public DoctorOffice doctorOffice;
    // Start is called before the first frame update
    void Start()
    {
        doctorOffice = GameObject.Find("OutPatientWaypoints").transform.Find(gameObject.transform.parent.name).transform.Find(gameObject.name).GetComponent<DoctorOffice>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
