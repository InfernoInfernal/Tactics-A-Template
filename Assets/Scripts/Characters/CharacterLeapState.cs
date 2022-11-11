using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Character State used to leap or jump down between two points
/// </summary>
public class CharacterLeapState : CharacterBaseState
{
    //Needed for arcing the leap movement via lerp
    Vector3 ArcStartPosition;
    Vector3 ArcEndPosition;

    public override void Start(CharacterStateManager StateManager)
    {
        //Record Leap start. Note that this will be different for JumpUp
        ArcStartPosition = StateManager.gameObject.transform.position;
        ArcEndPosition = new Vector3(
            StateManager.MoveDestinationPosition.x,
            //1 Game Height Unit = 0.5 Y distance, division based on... What? Height of Collider? Distance just / 2?
            //Falling start point after parabola should be final leap destination +/- Y difference in height (Z/2)
            //TODO: Consider slanted tiles and their variance...
            StateManager.MoveDestinationPosition.y,
            StateManager.MoveDestinationPosition.z);

        //Set orientation to direction based on the direction of the new point
        CharacterFunctions.ChangeOrientation(
            CharacterFunctions.DetermineOrientation(StateManager.MoveOrigin, StateManager.MoveDestination),
            StateManager.CharacterData);

        //Resume crouch animation if not playing
        if (StateManager.CharacterData.AnimatorState != Constants.Crouch)
            CharacterFunctions.ChangeAnimationState(Constants.Crouch, StateManager.CharacterData);

        //Delay animation movement start
        //Parent to the State Manager, as coroutines can't be started outside Monobehaviour
        StateManager.StartCoroutine(DelayNextAnimation(StateManager, JumpStage.Leap, Constants.Jump));
    }

    //Coroutine:
    //Delay crouch time, then activate leap switch -OR- move to next waypoint

    //Update:
    /*
     * Do Nothing, set Curve
     * Leap Curve, set Fall
     * Fall, set Crouch
     * Do Nothing, set Finished
     * Finished, set Do Nothing, move to next waypoint
    */

    IEnumerator DelayNextAnimation(CharacterStateManager StateManager, JumpStage NextStage, string NextAnimation = null)
    {
        yield return new WaitForSecondsRealtime(Constants.AnimationTravelSpeed * 0.25f);
        if (NextAnimation != null)
            CharacterFunctions.ChangeAnimationState(NextAnimation, StateManager.CharacterData);
        StateManager.AnimationJumpStage = NextStage;
    }

    public override void Update(CharacterStateManager StateManager)
    {
        switch (StateManager.AnimationJumpStage)
        {
            case JumpStage.Leap:
                //TODO: Parabola / Bezier Curve Animation, calculate destination point above the destination tile inline with origin?
                //TODO: Update MoveDestinationPosition when given animation is concluded?
                float arcHeight = 0.333f;
                float dist = ArcEndPosition.x - ArcStartPosition.x;
                float nextX = Mathf.MoveTowards(StateManager.gameObject.transform.position.x, ArcEndPosition.x, 
                    Constants.AnimationTravelSpeed * Time.deltaTime * StateManager.DestinationZMultiplier * 2f);
                float baseY = Mathf.Lerp(ArcStartPosition.y, ArcEndPosition.y, (nextX - ArcStartPosition.x) / dist);
                float nextZ = ArcEndPosition.z; //TODO: Actual Math
                float arc = arcHeight * (nextX - ArcStartPosition.x) * (nextX - ArcEndPosition.x) / (-0.25f * dist * dist);
                StateManager.gameObject.transform.position = new Vector3(nextX, baseY + arc, nextZ);

                //StateManager.gameObject.transform.position = Vector3.MoveTowards(StateManager.gameObject.transform.position, 
                //    StateManager.MoveDestinationPosition, Constants.AnimationTravelSpeed * Time.deltaTime * StateManager.DestinationZMultiplier);
                if (StateManager.gameObject.transform.position == ArcEndPosition)
                    StateManager.AnimationJumpStage = JumpStage.FallDown;
                break;
            
            //1 Game Height Unit = 0.5 Y distance, division based on... What? Height of Collider? Distance just / 2?
            //Falling start point after parabola should be final leap destination +/- Y difference in height
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
                StateManager.AnimationJumpStage = JumpStage.DoNothing;
                StateManager.MoveToNextWaypoint();
                break;
            
            default: //Do Nothing
                break;
        }
    }
}
