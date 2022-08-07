using UnityEngine;

/// <summary>
/// Manager Class of the State Machine for Characters
/// Attached to a Character Game Object with the CharacterGameData Component
/// </summary>
public class CharacterStateManager : MonoBehaviour
{
    [HideInInspector]
    public CharacterGameData CharacterData;

    CharacterBaseState CharacterState;
    public CharacterIdleState Idle = new CharacterIdleState();
    public CharacterWalkState Walk = new CharacterWalkState();

    //Used for movement reference in states
    public Vector2Int moveOrigin;
    public Vector2Int moveDestination;

    void Start()
    {
        CharacterState = Idle;

        CharacterData = this.gameObject.GetComponent<CharacterGameData>();
        if (CharacterData == null)
            Debug.LogError("No paired CharacterGameData attached to GameObject!");
        else
            CharacterData.AnimatorState = Constants.Idle;

        CharacterState.Start(this);
    }

    void Update()
    {
        CharacterState.Update(this);
    }

    public void ChangeState(CharacterBaseState NewState)
    {
        CharacterState = NewState;
        CharacterState.Start(this); //Call Start before next update
    }
}
