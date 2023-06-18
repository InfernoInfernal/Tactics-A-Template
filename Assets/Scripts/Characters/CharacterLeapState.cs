using System.Collections;
using UnityEngine;

/// <summary>
/// Character State used to leap or jump between two points
/// </summary>
public class CharacterLeapState : CharacterBaseState
{
    //Needed for arcing the leaping movement via lerp
    Vector3 StartPosition;
    //Flag for triggering movement after displaying the character crouching for a set time via coroutine
    bool CurrentlyLeaping = false;

    public override void Start(CharacterStateManager StateManager)
    {
        //Get start position for Animation Curve Sampling
        StartPosition = StateManager.gameObject.transform.position;

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

    public override void Update(CharacterStateManager StateManager)
    {
        //Only begin the leap once the crouch animation is finished
        if (CurrentlyLeaping)
        {
            //Sampled animation curve is used to adjust the Y and Z values to give the appearence of leaping
            //While moving at a fixed pace towards X
            float SampledCurveAdjustment = StateManager.CharacterData.LeapingCurve.Evaluate(Mathf.InverseLerp(
                StartPosition.x, StateManager.MoveDestinationPosition.x, StateManager.gameObject.transform.position.x));
            //Accelerator used to gradually reduce the jump time on wider leaps
            float DistanceAccelerator = Mathf.Sqrt(Mathf.Abs(StateManager.MoveDestinationPosition.x - StartPosition.x));

            //Calculate X using travel speed, distance accelerator, and a flat multiplier to look quicker than a walk
            float NextX = Mathf.MoveTowards(StateManager.gameObject.transform.position.x, StateManager.MoveDestinationPosition.x,
                Constants.AnimationTravelSpeed * Time.deltaTime * DistanceAccelerator * 3f);
            float DistanceToNextX = Mathf.InverseLerp(StartPosition.x, StateManager.MoveDestinationPosition.x, NextX);

            //Using the distance to the next X position, modify Y and Z by a 1:2 isometric ratioed jump sampled from the curve
            float NextY = Mathf.Lerp(StartPosition.y, StateManager.MoveDestinationPosition.y, DistanceToNextX)
                + SampledCurveAdjustment;
            float NextZ = Mathf.Lerp(StartPosition.z, StateManager.MoveDestinationPosition.z, DistanceToNextX)
                + (SampledCurveAdjustment * 2f);

            //Update the actual character position with the 3 results
            StateManager.gameObject.transform.position = new Vector3(NextX, NextY, NextZ);

            //Once the leap has reached the destination, begin the touchdown crouch
            if (StateManager.gameObject.transform.position == StateManager.MoveDestinationPosition)
            {
                CurrentlyLeaping = false;
                StateManager.StartCoroutine(SecondCrouchDelay(StateManager));
            }
        }
    }

    /// <summary>
    /// Coroutine to wait briefly while displaying a crouch, then update the animation to jump and change the flag to begin leaping
    /// </summary>
    /// <param name="StateManager">StateManager handling the coroutine pause</param>
    private IEnumerator FirstCrouchDelay(CharacterStateManager StateManager)
    {
        yield return new WaitForSeconds(0.25f);
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
        yield return new WaitForSeconds(0.15f);
        StateManager.MoveToNextWaypoint();
    }
}
