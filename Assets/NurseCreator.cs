using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NurseCreator : MonoBehaviour
{
    public static NurseCreator Instance;
    public int numberOfNurse = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        for(int i = 0;i<ObjectPoolingManager.Instance.maxOfNurse;i++)
        {
            ObjectPoolingManager.Instance.ActivateNurse(GameObject.Find("Nurse " + i));
        }




    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
