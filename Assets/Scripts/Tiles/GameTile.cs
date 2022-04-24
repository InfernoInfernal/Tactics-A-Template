using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Component class for holding game specific info about a game tile object
/// (Data is derived from a Game Tile Type scriptable object)
/// </summary>
public class GameTile : MonoBehaviour
{
    public GameTileType GameTileType; //Type contains the tile information

    [HideInInspector]
    public TileBase TileBase { get { return GameTileType.tileBase; } }

    public int TileSpriteHeight { get { return GameTileType.tileSpriteHeight; } }
    public int MovementCost { get { return GameTileType.movementCost; } }
    public bool Inaccessible { get { return GameTileType.inaccessible; } }
    public bool Liquid { get { return GameTileType.liquid; } }
}