using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Character State used to leap or jump down between two points
/// </summary>
public class CharacterLeapState : CharacterBaseState
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

        //Delay animation movement start
        //Parent to the State Manager, as coroutines can't be started outside Monobehaviour
        StateManager.StartCoroutine(DelayNextAnimation(StateManager, JumpStage.Leap));
    }

    //Coroutine:
    //Delay crouch time, then activate leap switch -OR- move to next waypoint

    //Update:
    /*
     * Switch:
     * Wait
     * Leap Curve, activate Fall
     * Fall, activate Crouch
     * Finished, move to next waypoint
    */

    IEnumerator DelayNextAnimation(CharacterStateManager StateManager, JumpStage NextStage)
    {
        yield return new WaitForSecondsRealtime(Constants.AnimationTravelSpeed * 0.5f);
        StateManager.AnimationJumpStage = NextStage;
    }

    public override void Update(CharacterStateManager StateManager)
    {
        switch (StateManager.AnimationJumpStage)
        {
            case JumpStage.Leap:
                //TODO: Bezier Curve Animation, calculate destination point above the destination tile inline with origin?
                StateManager.gameObject.transform.position = Vector3.MoveTowards(StateManager.gameObject.transform.position, 
                    StateManager.MoveDestinationPosition, Constants.AnimationTravelSpeed * Time.deltaTime * StateManager.DestinationZMultiplier);
                if (StateManager.gameObject.transform.position == StateManager.MoveDestinationPosition)
                    StateManager.AnimationJumpStage = JumpStage.FallDown;
                break;
            
            case JumpStage.FallDown:
                StateManager.gameObject.transform.position = Vector3.MoveTowards(StateManager.gameObject.transform.position, 
                    StateManager.MoveDestinationPosition, Constants.AnimationTravelSpeed * Time.deltaTime * StateManager.DestinationZMultiplier);
                if (StateManager.gameObject.transform.position == StateManager.MoveDestinationPosition)
                {
                    StateManager.AnimationJumpStage = JumpStage.DoNothing;
                    CharacterFunctions.ChangeAnimationState(Constants.Crouch, StateManager.CharacterData);
                    StateManager.StartCoroutine(DelayNextAnimation(StateManager, JumpStage.Finished));
                }
                break;
            
            case JumpStage.Finished:
                StateManager.MoveToNextWaypoint();
                break;
            
            default:
                break;
        }
    }
}
