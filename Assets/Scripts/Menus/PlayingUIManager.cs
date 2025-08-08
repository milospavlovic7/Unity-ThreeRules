using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text levelText;
    [SerializeField] private Text movesText;

    public void SetLevelNumber(int levelNumber)
    {
        levelText.text = $"Level: {levelNumber}";
    }

    public void SetMovesCount(int moves)
    {
        movesText.text = $"Moves: {moves}";
    }
}
