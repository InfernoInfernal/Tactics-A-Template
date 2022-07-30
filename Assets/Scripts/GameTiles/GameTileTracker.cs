using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script attached to the Tilemap by the Game Tile Manager for tracking various components used for pathfinding
/// This includes a dictionary with all the Game Object GameTiles in a scene by their grid coordinates
/// Any current pathfinding map of game tiles linked to their preceding game tile
/// And current list of the highlighted game tile path from the map
/// </summary>
public class GameTileTracker : MonoBehaviour
{
    public Dictionary<Vector2Int, GameObject> GameTileDictionary = new Dictionary<Vector2Int, GameObject>();
    public Dictionary<GameObject, GameObject> DestinationPathfindingMap = new Dictionary<GameObject, GameObject>();
    public HashSet<GameObject> HighlightedPath = new HashSet<GameObject>();

    private void Awake()
    {
        GameObject[] gameTiles = GameObject.FindGameObjectsWithTag(Constants.GameTileTag);
        foreach (GameObject gameTile in gameTiles)
        {
            GameTile gameTileComponent = gameTile.GetComponent<GameTile>();
            GameTileDictionary.Add(new Vector2Int(gameTileComponent.CellPositionX, gameTileComponent.CellPositionY), gameTile);
            //Debug.Log($"GameTileDictionary Entry Logged with x:{gameTileComponent.CellPositionX} y:{gameTileComponent.CellPositionY} as {gameTile.name}");
        }
    }
}