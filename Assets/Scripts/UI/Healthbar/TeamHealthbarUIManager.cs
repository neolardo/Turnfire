using System.Collections.Generic;
using UnityEngine;

public class TeamHealthbarUIManager : MonoBehaviour
{

    [SerializeField] private TeamHealthbarUI _teamHealthbarPrefab;

    private List<TeamHealthbarUI> _healthBars;

    public void CreateHealthBars(IEnumerable<Team> teams)
    {
        _healthBars = new List<TeamHealthbarUI>();
        TeamHealthbarPosition position = 0;
        foreach (var team in teams)
        {
            var hb = Instantiate(_teamHealthbarPrefab, transform);
            hb.SetTeam(team);
            hb.SetPosition(position++);
            _healthBars.Add(hb);
        }
    }


}
