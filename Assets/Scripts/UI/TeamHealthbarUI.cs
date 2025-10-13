using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PixelUIScaler))]
public class TeamHealthbarUI : MonoBehaviour
{
    [SerializeField] private Image _innerContentImage;
    private PixelUIScaler _pixelUIScaler;

    private float _initialScale;

    private void Awake()
    {
        _initialScale = _innerContentImage.transform.localScale.x;
        _pixelUIScaler = GetComponent<PixelUIScaler>();
    }

    public void SetTeamColor(Color teamColor)
    {
        _innerContentImage.color = teamColor;
    }


    public void SetPosition(TeamHealthbarPosition position)
    {
        switch (position)
        {
            case TeamHealthbarPosition.TopLeft:
                _pixelUIScaler.SetPosition(new Vector2(0, 1), new Vector2(0, 1), new Vector2(Constants.TeamHealthbarOffsetPixelsX, -Constants.TeamHealthbarOffsetPixelsY));
                break;

            case TeamHealthbarPosition.TopRight:
                _pixelUIScaler.SetPosition(new Vector2(1, 1), new Vector2(1, 1), new Vector2(-Constants.TeamHealthbarOffsetPixelsX, -Constants.TeamHealthbarOffsetPixelsY));
                break;

            case TeamHealthbarPosition.BottomLeft:
                _pixelUIScaler.SetPosition(new Vector2(1, 0), new Vector2(1, 0), new Vector2(-Constants.TeamHealthbarOffsetPixelsX, Constants.TeamHealthbarOffsetPixelsY));
                break;

            case TeamHealthbarPosition.BottomRight:
                _pixelUIScaler.SetPosition(new Vector2(0, 0), new Vector2(0, 0), new Vector2(Constants.TeamHealthbarOffsetPixelsX, Constants.TeamHealthbarOffsetPixelsY));

                break;
        }
    }

    public void SetTeamHealth(float healthRatio)
    {
        _innerContentImage.transform.localScale = new Vector3(_initialScale * healthRatio, _innerContentImage.transform.localScale.y, _innerContentImage.transform.localScale.z);
    }

}
