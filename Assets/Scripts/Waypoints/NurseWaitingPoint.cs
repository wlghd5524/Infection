using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NurseWaitingPoint : Waypoint
{
    public DoctorOffice doctorOffice;
    // Start is called before the first frame update
    void Start()
    {
        doctorOffice = Managers.NPCManager.waypointDictionary[(ward, "OutpatientWaypoints")].Find(gameObject.name).GetComponent<DoctorOffice>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
