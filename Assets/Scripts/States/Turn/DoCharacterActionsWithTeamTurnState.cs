public class DoCharacterActionsWithTeamTurnState : TurnState
{
    public override TurnStateType State => TurnStateType.DoCharacterActions;

    private CharacterActionManager _characterActionManager;

    public DoCharacterActionsWithTeamTurnState(CharacterActionManager characterActionManager) : base(CoroutineRunner.Instance)
    {
        _characterActionManager = characterActionManager;
    }

    public override void StartState(TurnStateContext context)
    {
        base.StartState(context);
        var character = SelectNextCharacterInTeam(context);
        _characterActionManager.StartActionsWithCharacter(character);
    }

    private Character SelectNextCharacterInTeam(TurnStateContext context)
    {
        context.TeamCharacterEnumerator.MoveNext(out var _);
        return context.TeamCharacterEnumerator.Current;
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
