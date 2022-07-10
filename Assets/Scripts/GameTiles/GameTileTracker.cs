using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script attached to the Tilemap by the Game Tile Manager for tracking all the Game Object GameTiles in a scene by their grid coordinates
/// Particularly critical for the use of pathfinding algorithms
/// </summary>
public class GameTileTracker : MonoBehaviour
{
    public Dictionary<Vector2Int, GameObject> GameTileDictionary = new Dictionary<Vector2Int, GameObject>();
}