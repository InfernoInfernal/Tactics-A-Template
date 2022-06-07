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
/// Component of a Character Game Object that stores all relevant information for APIs
/// </summary>
public class CharacterGameData : MonoBehaviour
{
    //Animation Data
    public SpriteRenderer FrontSpriteRenderer;
    public Animator FrontAnimator;
    public SpriteRenderer BackSpriteRenderer;
    public Animator BackAnimator;

    //Character Direction
    public CharacterDirectionFacing DirectionFaced;


}
