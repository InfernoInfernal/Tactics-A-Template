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
        characterGameData.CharacterAnimator.SetTrigger(newAnimationState);
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
                characterGameData.CharacterSpriteLibrary.spriteLibraryAsset = characterGameData.FrontSpriteLibraryAsset;
                characterGameData.CharacterSpriteRenderer.flipX = false;
                break;
            case CharacterDirectionFacing.FrontRight:
                characterGameData.CharacterSpriteLibrary.spriteLibraryAsset = characterGameData.FrontSpriteLibraryAsset;
                characterGameData.CharacterSpriteRenderer.flipX = true;
                break;
            case CharacterDirectionFacing.BackRight:
                characterGameData.CharacterSpriteLibrary.spriteLibraryAsset = characterGameData.BackSpriteLibraryAsset;
                characterGameData.CharacterSpriteRenderer.flipX = false;
                break;
            case CharacterDirectionFacing.BackLeft:
                characterGameData.CharacterSpriteLibrary.spriteLibraryAsset = characterGameData.BackSpriteLibraryAsset;
                characterGameData.CharacterSpriteRenderer.flipX = true;
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
        if (destinationCoordinates.x - originCoordinates.x < 0 && Math.Abs(destinationCoordinates.x - originCoordinates.x)
            >= Math.Abs(destinationCoordinates.y - originCoordinates.y))    //Down & Left
        {
            return CharacterDirectionFacing.FrontLeft;
        }
        else if (destinationCoordinates.y - originCoordinates.y < 0 && Math.Abs(destinationCoordinates.x - originCoordinates.x)
            <= Math.Abs(destinationCoordinates.y - originCoordinates.y))    //Down & Right
        {
            return CharacterDirectionFacing.FrontRight;
        }
        else if (destinationCoordinates.y - originCoordinates.y > 0 && Math.Abs(destinationCoordinates.x - originCoordinates.x)
            <= Math.Abs(destinationCoordinates.y - originCoordinates.y))    //Up & Left
        {
            return CharacterDirectionFacing.BackLeft;
        }
        else                                                                //Up & Right
        {
            return CharacterDirectionFacing.BackRight;
        }
    }

    /// <summary>
    /// Gets the Vector3 position a character should be at to render correctly over a GameTile
    /// </summary>
    /// <param name="GameTile">Game Tile GameObject the character is to stand on</param>
    /// <returns>The Vector3 Position for characters to the linked tile</returns>
    public static Vector3 GetCharacterPositionOnGameTile(GameObject GameTile)
    {
        GameTile gameTileComponent = GameTile.GetComponent<GameTile>();

        return new Vector3(
            GameTile.transform.position.x,
            GameTile.transform.position.y - ((float)gameTileComponent.InclineGameHeight / Constants.PixelPerGameUnitHeight),
            GameTile.transform.position.z - ((float)gameTileComponent.InclineGameHeight / 2) + 0.75f);
            //Pad Character object so it renders on top of the cursor and tile
    }
}
