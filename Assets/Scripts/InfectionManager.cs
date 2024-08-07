using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectionManager : MonoBehaviour
{
    private static InfectionManager _instance = new InfectionManager();
    public static InfectionManager Instance { get { return _instance; } }
    public int infectionProbability = 30;

    //스테이지 감염병 종류에 따른 감염 확률 매핑
    private Dictionary<int, float> probabilityMapping = new Dictionary<int, float>();

    //유니티에서 테스트를 위한 감염 확률 변수 (테스트 단계에서만 사용, 배포 단계에선 제외)
    public int stage1InfectionProbability = 30;
    public int stage2InfectionProbability = 20;

    private void Start()
    {
        infectionProbability = stage1InfectionProbability;
        probabilityMapping.Add(1, stage1InfectionProbability);
        probabilityMapping.Add(2, stage2InfectionProbability);
    }
    void Update()
    {
        if (StageManager.Instance.stage == 1)
        {
            infectionProbability = stage1InfectionProbability;
        }
        else if (StageManager.Instance.stage == 2)
        {
            infectionProbability = stage2InfectionProbability;
        }
    }
}
