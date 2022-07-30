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

    //Rules of (Non-Flying/Non-Teleporting) Movement:
    //You can jump up or down an adjacent tile up to max jump height
    //When leaping horizontally, the destination tile and all tiles between must be lower than the starting point.
    //Inaccesible tiles and empty space with no tiles in the grid can be leaped over
    //You can leap horizontally and jump down at the same time up to both maximums
    //You CANNOT leap horizontally and jump vertically UP at the same time
    //You cannot share a space or pass through an opposing unit unless BypassEnemy parameter is set to true
    //You CAN leap over opposing units horizontally if you pad enough spaces (3 units) over their sprite pivot to clear the whole sprite
    //Water is treated the same as solid tiles for movement/jumping
    //Water can be treated as an inaccessible tile with AvoidWater set to true

    /// <summary>
    /// Movement for non-flying/non-teleporting units
    /// </summary>
    /// <param name="GameTileDictionary">GameTileTracker element for referencing game objects from their grid coordinates</param>
    /// <param name="OriginGameTile">Starting game tile for movement</param>
    /// <param name="MaxDistance">Distance from origin tile that can be traversed</param>
    /// <param name="MaxJumpHeight">Game Tile Height maximum that can be jumped up or down</param>
    /// <param name="MaxLeapWidth">Number of tiles that can be leapt over at once</param>
    /// <param name="BypassEnemy">Whether or not to treat opposing units as physical blockers</param>
    /// <param name="AvoidWater">Whether or not to treat water tiles as inaccessible</param>
    /// <returns>Return all tiles that can be reached with each tile paired to preceding one for movement controller via Dictionary
    /// NOTE: this includes game tiles that are occupied by allies, such as the origin game tile. 
    /// Game tiles will still need to be validated against allied occupants while performing moves with this data.</returns>
    public static Dictionary<GameObject, GameObject> GetDestinationGameTiles(
        Dictionary<Vector2Int, GameObject> GameTileDictionary, Vector2Int OriginGameTile, int MaxDistance,
        int MaxJumpHeight, int MaxLeapWidth, bool BypassEnemy = false, bool AvoidWater = false)
    {
        //Dictionary of finalized selections
        Dictionary<GameObject, GameObject> finalizedDestinations = new Dictionary<GameObject, GameObject>();
        //Origin Game Tile's character's team will be used to check against the teams of other characters for bypassing
        CharacterTeam originCharacterTeam = 
            GameTileDictionary[OriginGameTile].GetComponent<GameTile>().OccupyingCharacter.GetComponent<CharacterGameData>().Team;

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
                GameTile currentGameTileComponent = GameTileDictionary[travelPair.Key].GetComponent<GameTile>();
                int moveCost = currentGameTileComponent.MovementCost;
                int originHeightMax = currentGameTileComponent.CellPositionZ + currentGameTileComponent.GameTileSpriteHeightMaximum;
                int originHeightMin = currentGameTileComponent.CellPositionZ + currentGameTileComponent.GameTileSpriteHeightMinimum;
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
                        if (!GameTileDictionary.ContainsKey(newCoordinates)) //Verify has an actual game tile or we can just skip over
                            continue;
                        GameTile newGameTileComponent = GameTileDictionary[newCoordinates].GetComponent<GameTile>();

                        //Determine if the tile is blocked by an opposing team and we're not bypassing
                        bool opposedCharacterBlocking = false;
                        if (!BypassEnemy && newGameTileComponent.OccupyingCharacter != null)
                        {
                            CharacterTeam occupiedCharacterTeam =
                                GameTileDictionary[OriginGameTile].GetComponent<GameTile>().OccupyingCharacter.GetComponent<CharacterGameData>().Team;

                            if ((occupiedCharacterTeam != CharacterTeam.Enemy && originCharacterTeam == CharacterTeam.Enemy) ||
                                (occupiedCharacterTeam == CharacterTeam.Enemy && originCharacterTeam != CharacterTeam.Enemy))
                            {
                                opposedCharacterBlocking = true;
                            }
                        }

                        //If there is an opposer in a tile to be jumped over (and not bypassing), treat it as 3 higher for purposes of jumping over
                        //Cannot walk through blocked tile, but might be able to jump over
                        //If jumping, ensure valid tile height for jumping over (even for the first tile) or no point in proceeding in this direction

                        bool jumpBeyondInvalid = false;

                        int newHeightMax = newGameTileComponent.CellPositionZ + newGameTileComponent.GameTileSpriteHeightMaximum;
                        int newHeightMin = newGameTileComponent.CellPositionZ + newGameTileComponent.GameTileSpriteHeightMinimum;

                        if (opposedCharacterBlocking) //Padding for jumping over
                        {
                            newHeightMax += 3;
                            newHeightMin += 3;
                        }
                        
                        if (newDistance == 1)
                        {
                            if (newHeightMax > originHeightMin)
                            {
                                jumpBeyondInvalid = true; //We can't jump over but still need to evaluate this tile for walkability

                                if (opposedCharacterBlocking)
                                    break; //If we can't jump over OR land in the tile, exit this direction

                                if (originHeightMax + MaxJumpHeight < newHeightMin)
                                    break; //If too high to jump and can't go over, exit this direction
                            }
                            else if (opposedCharacterBlocking)
                            {
                                continue; //If we can't land in the tile but can still jump over, skip to next
                            }
                        }
                        else if (newDistance > 1)
                        {
                            if (originHeightMax < newHeightMin)
                            {
                                break; //Being too high for a horizontal jump means we can just exit early for this whole direction
                            }

                            if (opposedCharacterBlocking)
                            {
                                continue; //We can jump over the tile but can't land in it, skip to next
                            }
                        }

                        //Additional validation checks
                        if (finalizedDestinations.ContainsKey(GameTileDictionary[newCoordinates])   //Destination already finalized
                            || newGameTileComponent.Inaccessible                                    //Not accessible
                            || (AvoidWater && newGameTileComponent.Water)                           //Can't swim
                            || (originHeightMin - MaxJumpHeight > newHeightMax)                     //Too low to jump down to
                            )
                        {
                            continue;
                        }

                        //Upon passing all validation, add the new tile to the frontier for future exploration
                        //according to its movement cost, with origin as the preceding tile
                        frontiers[currentDistance + (newDistance * moveCost)][newCoordinates] = travelPair.Key;

                        //If we can't jump any farther after the initial walk, break the loop
                        if (jumpBeyondInvalid)
                            break;
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
    /// Highlight a game tile renderer with a given color and enabling it
    /// </summary>
    /// <param name="GameTileGameObject">game tile game object to highlight</param>
    /// <param name="NewColor">Color to apply to highlight</param>
    public static void HighlightGameTile(GameObject GameTileGameObject, Color NewColor)
    {
        SpriteRenderer spriteRenderer = GameTileGameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = NewColor;
        spriteRenderer.enabled = true;
    }

    /// <summary>
    /// Unhightlight a game tile renderer by disabling it
    /// </summary>
    /// <param name="GameTileGameObject">game tile game object to unhighlight</param>
    public static void UnhighlightGameTile(GameObject GameTileGameObject)
    {
        SpriteRenderer spriteRenderer = GameTileGameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }
}
