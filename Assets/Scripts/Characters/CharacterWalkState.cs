using System.Collections;
using System.Collections.Generic;
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
            CharacterFunctions.DetermineOrientation(StateManager.moveOrigin, StateManager.moveDestination),
            StateManager.CharacterData);

        //Resume move animation if not playing
        if (StateManager.CharacterData.AnimatorState != Constants.Idle)
            CharacterFunctions.ChangeAnimationState(Constants.Idle, StateManager.CharacterData);
    }

    public override void Update(CharacterStateManager StateManager)
    {
        //TODO:
        //Walk towards destination

        //Notify when destination reached?

    }
}
