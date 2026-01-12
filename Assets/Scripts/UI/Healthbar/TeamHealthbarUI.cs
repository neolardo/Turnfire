using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamHealthbarUI : MonoBehaviour
{
    [SerializeField] private Image _healthbarInnerContentImage;
    [SerializeField] private PixelUIScaler _healthbarContainerPixelUI;
    [SerializeField] private PixelUIScaler _textPixelUI;
    [SerializeField] private TextMeshProUGUI _teamText;
    [SerializeField] private Color _deadTeamTextColor;
    [SerializeField] private Color _selectedTeamTextColor;
    [SerializeField] private float _textColorFadeSeconds =.5f;

    private Team _team;
    private float _initialScale;
    private Color _originalTextColor;
    private readonly Vector2Int TextMarginPixels = new Vector2Int(2, 2);

    private void Awake()
    {
        _initialScale = _healthbarInnerContentImage.transform.localScale.x;
        _originalTextColor = _teamText.color;
    }

    private void Start()
    {
        GameServices.TurnStateManager.SelectedTeamChanged += OnSelectedTeamChanged;
    }

    private void OnSelectedTeamChanged(Team selectedTeam)
    {
        FadeTextOnTeamSelectionChanged(selectedTeam == _team);
    }

    public void SetTeam(Team team)
    {
        _team = team;
        SetTeamHealth(1);
        _healthbarInnerContentImage.color = team.TeamColor;
        _teamText.text = team.TeamName;
        team.TeamHealthChanged += SetTeamHealth;
        team.TeamLost += FadeTextOnTeamLost;
    }

    private void FadeTextOnTeamLost()
    {
        StartCoroutine(FadeTextColor(_teamText.color, _deadTeamTextColor, _textColorFadeSeconds));
    }

    private void FadeTextOnTeamSelectionChanged(bool isTeamSelected)
    {
        StopAllCoroutines();
        if(isTeamSelected)
        {
            StartCoroutine(FadeTextColor(_originalTextColor, _selectedTeamTextColor, _textColorFadeSeconds));
        }
        else
        {
            StartCoroutine(FadeTextColor(_selectedTeamTextColor, _originalTextColor, _textColorFadeSeconds));
        }
    }

    private IEnumerator FadeTextColor(Color start, Color target, float duration)
    {
        float t = 0;
        while (t < duration) 
        {
            _teamText.color = Color.Lerp(start, target, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
    }


    public void SetPosition(TeamHealthbarPosition position)
    {
        var healtbarSize = _healthbarContainerPixelUI.SizeInPixels;
        switch (position)
        {
            case TeamHealthbarPosition.TopLeft:
                _healthbarContainerPixelUI.SetPosition(new Vector2(0, 1), new Vector2(0, 1), new Vector2(Constants.TeamHealthbarOffsetPixelsX, -Constants.TeamHealthbarOffsetPixelsY));
                _textPixelUI.SetPosition(new Vector2(0, 1), new Vector2(0, 1), new Vector2(Constants.TeamHealthbarOffsetPixelsX + TextMarginPixels.x, -Constants.TeamHealthbarOffsetPixelsY - healtbarSize.y - TextMarginPixels.y));
                _teamText.alignment = TextAlignmentOptions.Left;
                break;

            case TeamHealthbarPosition.TopRight:
                _healthbarContainerPixelUI.SetPosition(new Vector2(1, 1), new Vector2(1, 1), new Vector2(-Constants.TeamHealthbarOffsetPixelsX, -Constants.TeamHealthbarOffsetPixelsY));
                _textPixelUI.SetPosition(new Vector2(1, 1), new Vector2(1, 1), new Vector2(-Constants.TeamHealthbarOffsetPixelsX - TextMarginPixels.x, -Constants.TeamHealthbarOffsetPixelsY - healtbarSize.y - TextMarginPixels.y));
                _teamText.alignment = TextAlignmentOptions.Right;
                break;

            case TeamHealthbarPosition.BottomLeft:
                _healthbarContainerPixelUI.SetPosition(new Vector2(0, 0), new Vector2(0, 0), new Vector2(Constants.TeamHealthbarOffsetPixelsX, Constants.TeamHealthbarOffsetPixelsY));
                _textPixelUI.SetPosition(new Vector2(0, 0), new Vector2(0, 0), new Vector2(Constants.TeamHealthbarOffsetPixelsX + TextMarginPixels.x, Constants.TeamHealthbarOffsetPixelsY + healtbarSize.y + TextMarginPixels.y));
                _teamText.alignment = TextAlignmentOptions.Left;
                break;

            case TeamHealthbarPosition.BottomRight:
                _healthbarContainerPixelUI.SetPosition(new Vector2(1, 0), new Vector2(1, 0), new Vector2(-Constants.TeamHealthbarOffsetPixelsX, Constants.TeamHealthbarOffsetPixelsY));
                _textPixelUI.SetPosition(new Vector2(1, 0), new Vector2(1, 0), new Vector2(-Constants.TeamHealthbarOffsetPixelsX - TextMarginPixels.x, Constants.TeamHealthbarOffsetPixelsY + healtbarSize.y + TextMarginPixels.y));
                _teamText.alignment = TextAlignmentOptions.Right;
                break;
        }
    }

    private void SetTeamHealth(float healthRatio)
    {
        _healthbarInnerContentImage.transform.localScale = new Vector3(_initialScale * healthRatio, _healthbarInnerContentImage.transform.localScale.y, _healthbarInnerContentImage.transform.localScale.z);
    }

}
