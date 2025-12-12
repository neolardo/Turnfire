using TMPro;
using UnityEngine;

public class BotSimulationText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textUI;

    private void Start()
    {
        SetText(BotEvaluationStatistics.CurrentSimulationCount.ToString());
    }

    public void SetText(string text)
    {
        textUI.text = text;
    }
}
