using UnityEngine;

/// <summary>
/// Character State used to walk between two points
/// </summary>
public class CharacterWalkState : CharacterBaseState
{
    public override void Start(CharacterStateManager StateManager)
    {
        //Set orientation to direction based on the direction of the new point
        CharacterFunctions.ChangeOrientation(
            CharacterFunctions.DetermineOrientation(StateManager.MoveOrigin, StateManager.MoveDestination),
            StateManager.CharacterData);

        //Resume move animation if not playing
        if (StateManager.CharacterData.AnimatorState != Constants.Idle)
            CharacterFunctions.ChangeAnimationState(Constants.Idle, StateManager.CharacterData);
    }

    public override void Update(CharacterStateManager StateManager)
    {
        //Walk towards destination
        StateManager.gameObject.transform.position = Vector3.MoveTowards(StateManager.gameObject.transform.position,
            StateManager.MoveDestinationPosition, Constants.AnimationTravelSpeed * Time.deltaTime * StateManager.DestinationZMultiplier);
        
        //Notify when destination reached
        if (StateManager.gameObject.transform.position == StateManager.MoveDestinationPosition)
            StateManager.MoveToNextWaypoint();
    }
}
