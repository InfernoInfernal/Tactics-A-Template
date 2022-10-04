using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Character State used to jump up between two points
/// </summary>
public class CharacterJumpUpState : CharacterBaseState
{
    public override void Start(CharacterStateManager StateManager)
    {
        //Set orientation to direction based on the direction of the new point
        CharacterFunctions.ChangeOrientation(
            CharacterFunctions.DetermineOrientation(StateManager.MoveOrigin, StateManager.MoveDestination),
            StateManager.CharacterData);

        //Resume crouch animation if not playing
        if (StateManager.CharacterData.AnimatorState != Constants.Crouch)
            CharacterFunctions.ChangeAnimationState(Constants.Crouch, StateManager.CharacterData);
    }

    public override void Update(CharacterStateManager StateManager)
    {
        //TODO
    }
}
