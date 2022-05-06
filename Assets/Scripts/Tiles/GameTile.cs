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

    public float TileSpriteHeight { get { return GameTileType.TileSpriteHeight; } }
    public int MovementCost { get { return GameTileType.MovementCost; } }
    public TileSurfaceOrientation SurfaceOrientation;
    public TileInclineRise InclineRise { get { return GameTileType.InclineRise; } }
    public bool Inaccessible { get { return GameTileType.Inaccessible; } }
    public bool Liquid { get { return GameTileType.Liquid; } }
}