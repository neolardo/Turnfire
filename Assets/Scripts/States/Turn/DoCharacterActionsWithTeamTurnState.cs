public class DoCharacterActionsWithTeamTurnState : TurnState
{
    public override TurnStateType State => TurnStateType.DoCharacterActions;

    private CharacterActionManager _characterActionManager;
    private Team _team;

    public DoCharacterActionsWithTeamTurnState(CharacterActionManager characterActionManager) : base(CoroutineRunner.Instance)
    {
        _characterActionManager = characterActionManager;
    }

    public override void StartState(TurnStateContext context)
    {
        base.StartState(context);
        _team = context.CurrentTeam;
        _team.SelectTeam();
        _team.SelectNextCharacter();
        _characterActionManager.StartActionsWithCharacter(_team.CurrentCharacter);
    }

    protected override void EndState()
    {
        _team.DeselectTeam();
        _team = null;
        base.EndState();
    }

    protected override void SubscribeToEvents()
    {
        _characterActionManager.CharacterActionsFinished += EndState;
    }
    protected override void UnsubscribeFromEvents()
    {
        _characterActionManager.CharacterActionsFinished -= EndState;
    }

    public override void ForceEndState()
    {
        _characterActionManager.ForceEndActions();
    }

}
