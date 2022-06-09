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
            //TODO: Setup a static script to reference for pixels per game unit height
            GameTile assignedGameTile = visibleHitGameTile.GetComponent<GameTile>();
            float heightOffset = (assignedGameTile.TileSpriteHeightMaximum - assignedGameTile.TileSpriteHeightMinimum) / 8/*pixels per game unit height*/;

            this.gameObject.transform.position = new Vector3(
                visibleHitGameTile.transform.position.x, 
                visibleHitGameTile.transform.position.y - heightOffset, //O
                visibleHitGameTile.transform.position.z + 0.02f); //Pad Character object so it renders on top of the cursor and tile

            assignedGameTile.OccupyingCharacter = this.gameObject;
        }
        else
            Debug.LogError($"No GameTile found beneath Character {this.gameObject.name}");

        //Self-Destruct once complete
        Destroy(this);
    }
}
