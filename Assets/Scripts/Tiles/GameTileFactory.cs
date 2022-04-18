using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Factory of game objects spawned to represent tiles in the tilemap and contain relevant information for other classes
/// Can also pass
/// </summary>
public static class GameTileFactory
{
    static Dictionary<TileBase, GameTileType> GameTileDict = new Dictionary<TileBase, GameTileType>();

    /// <summary>
    /// Generate game tiles from the tilebases in the Tilemap
    /// Note: if any gameobjects exist from when this script was run, it is advised you run ClearAndGenerateTiles instead
    /// </summary>
    /// <param name="tilemap">Tilemap to generate the game tiles from</param>
    public static List<GameObject> GenerateGameTiles(Tilemap tilemap, Dictionary<TileBase, GameTileType> gameTileDict)
    {
        //Compress bounds and prepare XYZ counters for only ranges that contain tiles
        tilemap.CompressBounds();
        int xStart = tilemap.cellBounds.position.x;
        int yStart = tilemap.cellBounds.position.y;
        int zStart = tilemap.cellBounds.position.z;
        int xCount = tilemap.cellBounds.size.x;
        int yCount = tilemap.cellBounds.size.y;
        int zCount = tilemap.cellBounds.size.z;
        //Debug.Log($"TileScript Cellbounds xStart:{xStart} yStart:{yStart} zStart:{zStart} xCount:{xCount} yCount:{yCount} zCount:{zCount}");

        //Iterate through all tiles in the bounds and create corresponding game tiles
        for (int x = xStart; x < xStart + xCount; x++)
        {
            for (int y = yStart; y < yStart + yCount; y++)
            {
                for (int z = zStart + zCount; z > zStart - 1; z--) //Work backwards from top-down for z, so we only get the highest tiles
                {
                    TileBase tileBase;

                    GameObject newGameTileObject = new GameObject($"Gametile X:{xStart} Y:{yStart} Z:{zStart}");
                    GameTile gameTileComponent = newGameTileObject.AddComponent<GameTile>();
                    gameTileComponent.GameTileType = GameTileDict[tileBase];
                }
            }
        }

        return new List<GameObject>();
    }

    public static void ClearAndGenerateGameTiles(Tilemap tilemap)
    {
        DestroyGameTiles(tilemap);
        GenerateGameTiles(tilemap);
    }

    public static void DestroyGameTiles(Tilemap tilemap)
    {

    }
}
