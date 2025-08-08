using UnityEngine;

[CreateAssetMenu(fileName = "NewStage", menuName = "Game/Stage")]
public class StageData : ScriptableObject
{
    public GameObject stagePrefab;
    public bool isStoryStage;
    public string stageName;
}
