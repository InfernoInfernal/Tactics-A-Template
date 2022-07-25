using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script attached to the Tilemap by the Game Tile Manager for tracking all the Game Object GameTiles in a scene by their grid coordinates
/// And all of the currently highlighted game tile renderers with their colors
/// Particularly critical for the use of pathfinding algorithms
/// </summary>
public class GameTileTracker : MonoBehaviour
{
    public Dictionary<Vector2Int, GameObject> GameTileDictionary = new Dictionary<Vector2Int, GameObject>();
    public Dictionary<Renderer, Color> HighlightedRenderers = new Dictionary<Renderer, Color>();

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