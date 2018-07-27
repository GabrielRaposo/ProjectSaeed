public class BossStateMachine {

    IBossState currentState;

    public void ChangeState(IBossState newState)
    {
        currentState = newState;
        currentState.Enter();
    }
}
