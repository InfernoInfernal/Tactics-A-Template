using UnityEngine;

/// <summary>
/// Useful Static Functions revolving around GameTile
/// </summary>
public static class GameTileFunctions
{
    /// <summary>
    /// Fire a raycast at a position, and retrieves the GameTile from the hit colliders at that location
    /// Deduces the one that is visible to user from the rendering order by the lowest combined cell values of X and Y
    /// </summary>
    /// <param name="RaycastPosition">Position for raycast to try and hit GameTile colliders</param>
    /// <returns>The game object with the visible gametile, or null if none found
    ///  (also null if a GameTile is missing its component, which logs an error)</returns>
    public static GameObject GetGameTileFromPositionalRaycast(Vector3 RaycastPosition)
    {
        //Check for GameTile colliders under the character
        RaycastHit2D[] hits = Physics2D.RaycastAll(RaycastPosition, Vector2.zero);

        if (hits.Length == 0)
            return null; //No hits, return null

        //Run through the collider hits and select the visibly occupied GameTile from collider hits
        //Which can be determined from the lowest combined X&Y tilemap cell position values
        GameObject visibleHitGameTile = null;
        float lowestXY = float.MaxValue;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject.tag == Tag.GameTile)
            {
                //Get the Game Tile Component script, throw an error and exit if it's missing since this should never happen
                GameTile gameTileComponent = hit.collider.gameObject.GetComponent<GameTile>();
                if (gameTileComponent == null)
                {
                    Debug.LogError($"No GameTile script component found on {hit.collider.gameObject.name}!");
                    return null;
                }

                //Lowest combined CELL value (not world) of X+Y = visible/foremost tile
                if (gameTileComponent.CellPositionX + gameTileComponent.CellPositionY < lowestXY)
                {
                    visibleHitGameTile = hit.collider.gameObject;
                    lowestXY = gameTileComponent.CellPositionX + gameTileComponent.CellPositionY;
                }
            }
        }

        return visibleHitGameTile;
    }
}
