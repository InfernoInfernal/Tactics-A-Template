using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Class for holding game specific info about a tile base painted by tile palette
/// </summary>
public class GameTileType : ScriptableObject
{
    public TileBase tileBase;

    public int tileSpriteHeight; //when added with Z, Used for jump and move calculation
    public int movementCost = 1; //Cost to walk to the tile from an adjacent one using normal movement
    public bool inaccessible = false; //Set to true if the tile isn't traversable to ground units
    public bool liquid = false; //Whether the tile is liquid (Water or lava, for example)
}
