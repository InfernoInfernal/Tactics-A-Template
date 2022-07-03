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
            GameTile assignedGameTile = visibleHitGameTile.GetComponent<GameTile>();
            float heightOffset = assignedGameTile.GameTileSpriteHeightMaximum - assignedGameTile.GameTileSpriteHeightMinimum;

            this.gameObject.transform.position = new Vector3(
                visibleHitGameTile.transform.position.x, 
                visibleHitGameTile.transform.position.y - (heightOffset / Constants.PixelPerGameUnitHeight),
                visibleHitGameTile.transform.position.z - (heightOffset / 2) + 0.25f); //Pad Character object so it renders on top of the cursor and tile

            assignedGameTile.OccupyingCharacter = this.gameObject;
        }
        else
            Debug.LogError($"No GameTile found beneath Character {this.gameObject.name}");

        //Self-Destruct once complete
        Destroy(this);
    }
}
