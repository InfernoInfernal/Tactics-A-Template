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
            CharacterFunctions.DetermineOrientation(
                new Vector2Int(StateManager.MoveOrigin.x, StateManager.MoveOrigin.y),
                new Vector2Int(StateManager.MoveDestination.x, StateManager.MoveDestination.y)),
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
