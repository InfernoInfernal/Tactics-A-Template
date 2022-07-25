using System.Collections.Generic;
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
            if (hit.collider.gameObject.tag == Constants.GameTileTag)
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

    //Dijkstra:
    //- Return all tiles with each tile paired to preceding one for movement controller via Dictionary

    //Rules of (Non-Flying/Non-Teleporting) Movement:
    //When jumping horizontally, the destination tile and all tiles between must be lower than the starting point

    /// <summary>
    /// Movement for non-flying/non-teleporting units
    /// </summary>
    /// <param name="GameTileDictionary"></param>
    /// <param name="OriginGameTile"></param>
    /// <param name="MaxDistance"></param>
    /// <param name="MaxJumpHeight"></param>
    /// <param name="MaxLeapWidth">Number of tiles that can be leapt over at once</param>
    /// <param name="BypassEnemy"></param>
    /// <param name="AvoidWater"></param>
    /// <returns>Return dictionary of tile mappings for pathfinding
    /// NOTE: this includes game tiles that are occupied, such as the origin game tile. 
    /// Game tiles will still need to be validated against occupants while performing moves with this data.</returns>
    public static Dictionary<GameObject, GameObject> GetDestinationGameTiles(
        Dictionary<Vector2Int, GameObject> GameTileDictionary, Vector2Int OriginGameTile, int MaxDistance,
        int MaxJumpHeight, int MaxLeapWidth, bool BypassEnemy = false, bool AvoidWater = false)
    {
        //Dictionary of finalized selections
        Dictionary<GameObject, GameObject> finalizedDestinations = new Dictionary<GameObject, GameObject>();

        //frontiers breakdown:
        //List Index = Total Movement Cost for destinations from first origin
        //Dictionary.Key = Destination, Dictionary.Value = previous backtrace Origin
        List<Dictionary<Vector2Int, Vector2Int>> frontiers = new List<Dictionary<Vector2Int, Vector2Int>>();

        for (int i = 0; i <= MaxDistance; i++) //Initialize frontier entries up to possible distance
        {
            frontiers.Add(new Dictionary<Vector2Int, Vector2Int>());
        }

        frontiers[0].Add(OriginGameTile, OriginGameTile); //origin tile's origin is itself for the sake of having a valid mapping

        //Begin Search, iterate through until all possibilities around each frontier tile until exhausted at the max distance
        for (int currentDistance = 0; currentDistance <= MaxDistance; currentDistance++)
        {
            foreach (KeyValuePair<Vector2Int, Vector2Int> travelPair in frontiers[currentDistance])
            {
                //Verify and accept the tile if it's the lowest value found
                if (finalizedDestinations.ContainsKey(GameTileDictionary[travelPair.Key]))
                    continue;
                finalizedDestinations.Add(GameTileDictionary[travelPair.Key], GameTileDictionary[travelPair.Value]);

                //Loop through distance for each direction for walking/jumping
                int moveCost = GameTileDictionary[travelPair.Key].GetComponent<GameTile>().MovementCost;
                for (int direction = 0; direction < 4; direction++)
                {
                    for (int newDistance = 1; newDistance < MaxLeapWidth + 1; newDistance++)
                    {
                        //if maxDistance is exceeded, no need for further searching in this direction
                        if (currentDistance + (newDistance * moveCost) > MaxDistance)
                            break;

                        Vector2Int newCoordinates = new Vector2Int();
                        switch (direction)
                        {
                            case 0:
                                newCoordinates = new Vector2Int(travelPair.Key.x + newDistance, travelPair.Key.y);
                                break;
                            case 1:
                                newCoordinates = new Vector2Int(travelPair.Key.x - newDistance, travelPair.Key.y);
                                break;
                            case 2:
                                newCoordinates = new Vector2Int(travelPair.Key.x, travelPair.Key.y + newDistance);
                                break;
                            case 3:
                                newCoordinates = new Vector2Int(travelPair.Key.x, travelPair.Key.y - newDistance);
                                break;
                        }

                        //If we are jumping, make sure the tile height is valid or no point in proceeding in this direction
                        if (newDistance > 1)
                        {
                            //TODO
                            //If Tile Occupied, !bypass +3
                            break;
                        }

                        //Additional validation checks
                        if (!GameTileDictionary.ContainsKey(newCoordinates) //Has an actual game tile
                            || finalizedDestinations.ContainsKey(GameTileDictionary[newCoordinates]) //Destination not finalized
                            || GameTileDictionary[newCoordinates].GetComponent<GameTile>().Inaccessible //Is accessible
                            //TODO: Remaining Checks
                            )
                        {
                            continue;
                        }
                        

                        if (!BypassEnemy 
                            && GameTileDictionary[newCoordinates].GetComponent<GameTile>().OccupyingCharacter != null
                            ) //Enemy Check
                            //When jumping horizontally, the destination tile and all tiles between must be lower than the starting point
                            //Tiles with enemies count as 3 height greater, unless BypassEnemy = true
                        {
                            //TODO
                        }

                        if (AvoidWater) //Avoid Water Check
                        {
                            //TODO
                        }



                        //Upon passing all validation, add the new tile to the frontier for future exploration
                        //according to its movement cost, with origin as the preceding tile
                        frontiers[currentDistance + (newDistance * moveCost)][newCoordinates] = travelPair.Key;
                    }
                }
            }
        }

        //Set the origin tile's source to null before returning, so that it can be recognized as the starting point
        finalizedDestinations[GameTileDictionary[OriginGameTile]] = null;

        return finalizedDestinations;
    }

    /// <summary>
    /// Breadth First Search
    /// Expand across the grid equally with frontier
    /// Only grab a tile if its height is within min/max of a blast AoE radius
    /// </summary>
    /// <param name="GameTileDictionary"></param>
    /// <param name="OriginGameTile"></param>
    /// <param name="MaxDistance"></param>
    /// <returns>Dictionary Key: GameTile Game Object Value: int distance from center</returns>
    public static Dictionary<GameObject, int> GetGameTilesInBurst(
        Dictionary<Vector2Int, GameObject> GameTileDictionary, Vector2Int OriginGameTile, int MaxDistance)
    {
        throw new System.Exception("GetGameTilesInBurst not yet implemented");
    }

    /// <summary>
    /// Highlight a game tile renderer with a given color
    /// </summary>
    /// <param name="GameTileGameObject">game tile game object to highlight</param>
    /// <param name="NewColor">Color to apply to highlight</param>
    /// <param name="HighlightedRenderers">GameTileTracker element for managing game tile highlights</param>
    public static void HighlightGameTile(GameObject GameTileGameObject, Color NewColor,
        Dictionary<Renderer, Color> HighlightedRenderers)
    {
        SpriteRenderer spriteRenderer = GameTileGameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = NewColor;
        spriteRenderer.enabled = true;
        HighlightedRenderers[spriteRenderer] = NewColor;
    }

    /// <summary>
    /// Unhightlight all game tile renderers in a list
    /// </summary>
    /// <param name="GameTileGameObject">game tile game object to unhighlight</param>
    /// <param name="HighlightedRenderers">GameTileTracker element for managing game tile highlights</param>
    public static void UnhighlightGameTile(GameObject GameTileGameObject,
        Dictionary<Renderer, Color> HighlightedRenderers)
    {
        SpriteRenderer spriteRenderer = GameTileGameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        HighlightedRenderers.Remove(spriteRenderer);
    }
}
