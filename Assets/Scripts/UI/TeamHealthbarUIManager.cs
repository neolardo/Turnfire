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
            hb.SetTeamColor(team.TeamColor);
            hb.SetPosition(position++);
            hb.SetTeamHealth(1);
            team.TeamHealthChanged += hb.SetTeamHealth;
            _healthBars.Add(hb);
        }
    }


}
