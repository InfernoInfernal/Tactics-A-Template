using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Direction the Character is facing from the camera's perspective - front is facing towards camera, back is facing away from it
/// left is the camera's left, and right is the camera's right
/// </summary>
public enum CharacterDirectionFacing
{
    FrontLeft,
    FrontRight,
    BackLeft,
    BackRight
}

/// <summary>
/// Unit Type: Player units are controlled by the player, enemies and allies by AI, though allies can be moved through
/// </summary>
public enum CharacterTeam
{
    Player,
    Enemy,
    Ally
}

/// <summary>
/// Component of a Character Game Object that stores all relevant game data for APIs
/// </summary>
public class CharacterGameData : MonoBehaviour
{
    //Animation Data
    public SpriteRenderer FrontSpriteRenderer;
    public Animator FrontAnimator;
    public SpriteRenderer BackSpriteRenderer;
    public Animator BackAnimator;

    //Team the Character is controlled by
    public CharacterTeam Team;

    //Character Direction
    public CharacterDirectionFacing DirectionFaced;

    //Number of Game Tiles that can be traversed in one move
    public int Movement;
    //Number of game height units that can be jumped up
    public int HorizontalJump;
    //Number of game tiles that can be jumped across
    public int VerticalJump;
}
