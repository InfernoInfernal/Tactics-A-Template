using System;
using UnityEngine;

/// <summary>
/// Useful Static Functions revolving around Characters
/// </summary>
public static class CharacterFunctions
{
    /// <summary>
    /// Changes the animation clip played for the character object
    /// </summary>
    /// <param name="newAnimationState">The new animation clip to be played</param>
    /// <param name="characterGameData">The game data component of the character game object</param>
    public static void ChangeAnimationState(string newAnimationState, CharacterGameData characterGameData)
    {
        characterGameData.FrontAnimator.SetTrigger(newAnimationState);
        characterGameData.BackAnimator.SetTrigger(newAnimationState);
    }

    /// <summary>
    /// Changes the orientation of the character and their correspoding sprite
    /// </summary>
    /// <param name="newOrientation">The new orientation for the character to look at</param>
    /// <param name="characterGameData">The game data component of the character game object</param>
    public static void ChangeOrientation(CharacterDirectionFacing newOrientation, CharacterGameData characterGameData)
    {
        switch (newOrientation)
        {
            case CharacterDirectionFacing.FrontLeft:
                characterGameData.BackSpriteRenderer.enabled = false;
                characterGameData.FrontSpriteRenderer.flipX = false;
                characterGameData.FrontSpriteRenderer.enabled = true;
                break;
            case CharacterDirectionFacing.FrontRight:
                characterGameData.BackSpriteRenderer.enabled = false;
                characterGameData.FrontSpriteRenderer.flipX = true;
                characterGameData.FrontSpriteRenderer.enabled = true;
                break;
            case CharacterDirectionFacing.BackRight:
                characterGameData.FrontSpriteRenderer.enabled = false;
                characterGameData.BackSpriteRenderer.flipX = false;
                characterGameData.BackSpriteRenderer.enabled = true;
                break;
            case CharacterDirectionFacing.BackLeft:
                characterGameData.FrontSpriteRenderer.enabled = false;
                characterGameData.BackSpriteRenderer.flipX = true;
                characterGameData.BackSpriteRenderer.enabled = true;
                break;
            default:
                Debug.LogError($"Invalid Character Orientation");
                return;
        }

        characterGameData.DirectionFaced = newOrientation;
    }

    /// <summary>
    /// Orientation calculator based on the Vector2Int parameters of an origin and destination game tile
    /// </summary>
    /// <param name="originCoordinates">Vector2Int coordinates where the character is</param>
    /// <param name="destinationCoordinates">Vector2Int coordinates where the character is looking at</param>
    /// <returns>CharacterDirectionFacing of the orientation towards the destination game tile</returns>
    public static CharacterDirectionFacing DetermineOrientation(Vector2Int originCoordinates, Vector2Int destinationCoordinates)
    {
        //Prioritize Down or Left if tiles are exactly diagonal with early exit
        if (destinationCoordinates.x - originCoordinates.x > 0 && Math.Abs(destinationCoordinates.x - originCoordinates.x)
            >= Math.Abs(destinationCoordinates.y - originCoordinates.y))    //Down & Left
        {
            return CharacterDirectionFacing.FrontLeft;
        }
        else if (destinationCoordinates.y - originCoordinates.y > 0 && Math.Abs(destinationCoordinates.x - originCoordinates.x)
            <= Math.Abs(destinationCoordinates.y - originCoordinates.y))    //Down & Right
        {
            return CharacterDirectionFacing.FrontRight;
        }
        else if (destinationCoordinates.y - originCoordinates.y < 0 && Math.Abs(destinationCoordinates.x - originCoordinates.x)
            <= Math.Abs(destinationCoordinates.y - originCoordinates.y))    //Up & Left
        {
            return CharacterDirectionFacing.BackLeft;
        }
        else                                                                //Up & Right
        {
            return CharacterDirectionFacing.BackRight;
        }
    }
}
