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
            this.gameObject.transform.position = CharacterFunctions.GetCharacterPositionOnGameTile(visibleHitGameTile);
            visibleHitGameTile.GetComponent<GameTile>().OccupyingCharacter = this.gameObject;
        }
        else
            Debug.LogError($"No GameTile found beneath Character {this.gameObject.name}");

        //Self-Destruct once complete
        Destroy(this);
    }
}
