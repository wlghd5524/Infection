using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoctorOffice : Waypoint
{
    public Queue<OutpatientController> waitingQueue = new Queue<OutpatientController>();
    public bool is_empty = true;
    public GameObject doctor;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (is_empty && waitingQueue.Count > 0)
        {
            OutpatientController next = waitingQueue.Peek();
            if (next.isWaitingForDoctor)
            {
                next = waitingQueue.Dequeue();
                doctor.GetComponent<StressController>().stress += (++doctor.GetComponent<DoctorController>().patientCount / 10) + 1;
                is_empty = false;
                next.officeSignal = true;
            }
        }
    }
}
