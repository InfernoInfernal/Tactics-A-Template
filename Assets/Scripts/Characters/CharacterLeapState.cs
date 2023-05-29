using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Character State used to leap or jump down between two points
/// </summary>
public class CharacterLeapState : CharacterBaseState
{
    //Flag for triggering movement after displaying the character crouching for a set time via coroutine
    bool CurrentlyLeaping = false;

    public override void Start(CharacterStateManager StateManager)
    {
        //Set orientation to direction based on the direction of the new point
        CharacterFunctions.ChangeOrientation(
            CharacterFunctions.DetermineOrientation(
                new Vector2Int(StateManager.MoveOrigin.x, StateManager.MoveOrigin.y),
                new Vector2Int(StateManager.MoveDestination.x, StateManager.MoveDestination.y)),
                StateManager.CharacterData);

        //Resume crouch animation if not playing
        if (StateManager.CharacterData.AnimatorState != Constants.Crouch)
        {
            CharacterFunctions.ChangeAnimationState(Constants.Crouch, StateManager.CharacterData);
            StateManager.CharacterData.AnimatorState = Constants.Crouch;
        }

        //Start a coroutine through the StateMachine, since it requires MonoBehaviour
        StateManager.StartCoroutine(FirstCrouchDelay(StateManager));
    }

    /// <summary>
    /// Coroutine to wait briefly while displaying a crouch, then update the animation to jump and change the flag to begin leaping
    /// </summary>
    /// <param name="StateManager">StateManager handling the coroutine pause</param>
    private IEnumerator FirstCrouchDelay(CharacterStateManager StateManager)
    {
        yield return new WaitForSeconds(.25f);
        CharacterFunctions.ChangeAnimationState(Constants.Jump, StateManager.CharacterData);
        StateManager.CharacterData.AnimatorState = Constants.Jump;
        CurrentlyLeaping = true;
    }

    /// <summary>
    /// Coroutine to crouch on leap touchdown before continuing to the next waypoint
    /// </summary>
    /// <param name="StateManager">StateManager handling the coroutine pause</param>
    private IEnumerator SecondCrouchDelay(CharacterStateManager StateManager)
    {
        CharacterFunctions.ChangeAnimationState(Constants.Crouch, StateManager.CharacterData);
        StateManager.CharacterData.AnimatorState = Constants.Crouch;
        yield return new WaitForSeconds(.15f);
        StateManager.MoveToNextWaypoint();
    }

    public override void Update(CharacterStateManager StateManager)
    {
        //Only begin the leap once the crouch animation is finished
        if (CurrentlyLeaping)
        {
            StateManager.gameObject.transform.position = Vector3.MoveTowards(StateManager.gameObject.transform.position,
                StateManager.MoveDestinationPosition, Constants.AnimationTravelSpeed * Time.deltaTime * StateManager.DestinationZMultiplier * 3);

            //Once the leap has reached the destination, begin the touchdown crouch
            if (StateManager.gameObject.transform.position == StateManager.MoveDestinationPosition)
            {
                CurrentlyLeaping = false;
                StateManager.StartCoroutine(SecondCrouchDelay(StateManager));
            }
        }
    }
}
