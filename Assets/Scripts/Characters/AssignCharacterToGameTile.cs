using UnityEngine;

/// <summary>
/// Script to attach to a Character to the GameTile it's standing on once enabled, then destroys itself
/// </summary>
public class AssignCharacterToGameTile : MonoBehaviour
{
    void Start()
    {
        GameObject visibleHitGameTile = GameTileFunctions.GetGameTileFromPositionalRaycast(this.transform.position);

        if (visibleHitGameTile != null)
        {
            visibleHitGameTile.GetComponent<GameTile>().OccupyingCharacter = this.gameObject;

            //TODO: move the character game object to gametile position based on pivot point
        }
        else
            Debug.LogError($"No GameTile found beneath Character {this.gameObject.name}");

        //Self-Destruct once complete
        Destroy(this);
    }
}
