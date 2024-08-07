using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    private static StageManager _instance = new StageManager();
    public static StageManager Instance { get { return _instance; } }
    public int stage = 1;

    public void ChangeStage(int stage)
    {

    }
}
