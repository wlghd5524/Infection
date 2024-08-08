using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers _managers = new Managers();
    public static Managers Instance { get { return _managers; } }

    private static InfectionManager _infection = new InfectionManager();
    public static InfectionManager Infection { get { return _infection; } }

    private static ObjectPoolingManager _objectPooling = new ObjectPoolingManager();
    public static ObjectPoolingManager ObjectPooling { get { return _objectPooling; } }

    private static NPCMovementManager _NPCMovementManager = new NPCMovementManager();
    public static NPCMovementManager NPCManager { get { return _NPCMovementManager; } }

    private static StageManager _stageManager = new StageManager();
    public static StageManager Stage { get { return _stageManager; } }


    private void Awake()
    {
        _managers = this;
        _objectPooling = ObjectPooling;
        _NPCMovementManager = NPCManager;
        _stageManager = Stage;
        _infection = Infection;

        NPCManager.Init();
        ObjectPooling.Init();
        
    }
    // Start is called before the first frame update
    void Start()
    {
        Infection.Init();
    }

    // Update is called once per frame
    void Update()
    {
        Infection.UpdateInfectionProbability();
    }
}
