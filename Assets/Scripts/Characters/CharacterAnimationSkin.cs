using UnityEngine;

/// <summary>
/// Class for holding sprite animations to override for a given character
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/Character Animation Skin")]
public class CharacterAnimationSkin : ScriptableObject
{
    public AnimationClip FrontDefault;
    public AnimationClip FrontIdle;
    public AnimationClip FrontCrouch;
    public AnimationClip FrontJump;

    public AnimationClip BackDefault;
    public AnimationClip BackIdle;
    public AnimationClip BackCrouch;
    public AnimationClip BackJump;
}
