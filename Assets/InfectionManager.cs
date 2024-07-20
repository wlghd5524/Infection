using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectionManager : MonoBehaviour
{
    private static InfectionManager _instance = new InfectionManager();
    public static InfectionManager Instance { get { return _instance; } }
    public int infectionProbability = 30;

    //�������� ������ ������ ���� ���� Ȯ�� ����
    private Dictionary<int, float> probabilityMapping = new Dictionary<int, float>();

    //����Ƽ���� �׽�Ʈ�� ���� ���� Ȯ�� ���� (�׽�Ʈ �ܰ迡���� ���, ���� �ܰ迡�� ����)
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
