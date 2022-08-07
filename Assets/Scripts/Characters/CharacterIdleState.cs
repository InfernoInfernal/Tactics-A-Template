/// <summary>
/// Idle Character State that character assume when they are not doing anything else
/// </summary>
public class CharacterIdleState : CharacterBaseState
{
    public override void Start(CharacterStateManager StateManager)
    {
        CharacterFunctions.ChangeAnimationState(Constants.Idle, StateManager.CharacterData);
        StateManager.CharacterData.AnimatorState = Constants.Idle;
        //Delay animation movement start
        //Parent to the State Manager, as coroutines can't be started outside Monobehaviour
        //StateManager.StartCoroutine(DelayIdleAnimation(StateManager.CharacterData));
    }

    //Coroutine:
    //IEnumerator DelayIdleAnimation(CharacterGameData CharacterData)
    //{
    //    yield return new WaitForSecondsRealtime((float)Random.Range(0, 2999)/1000);
    //    CharacterFunctions.ChangeAnimationState(Constants.Idle, CharacterData);
    //}

    public override void Update(CharacterStateManager StateManager)
    {

    }
}
