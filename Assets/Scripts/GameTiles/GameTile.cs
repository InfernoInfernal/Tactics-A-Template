using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Component class for holding game specific info about a game tile object
/// (Data is derived from a Game Tile Type scriptable object)
/// </summary>
public class GameTile : MonoBehaviour
{
    public GameObject OccupyingCharacter = null; //Character object if occupied, null if not

    public GameTileType GameTileType; //Type contains the tile information

    [HideInInspector]
    public TileBase GameTileBase { get { return GameTileType.GameTileBase; } }
    [HideInInspector]
    public Sprite GameTileHighlight { get { return GameTileType.GameTileHighlight; } }
    [HideInInspector]
    public int CellPositionX;
    [HideInInspector]
    public int CellPositionY;
    [HideInInspector]
    public int CellPositionZ;

    public int GameTileSpriteHeightMaximum { get { return GameTileType.GameTileSpriteHeightMaximum; } }
    public int GameTileSpriteHeightMinimum { get { return GameTileType.GameTileSpriteHeightMinimum; } }
    public int MovementCost { get { return GameTileType.MovementCost; } }
    [HideInInspector]
    public TileSurfaceOrientation SurfaceOrientation;
    public TileInclineRise InclineRise { get { return GameTileType.InclineRise; } }
    public bool Inaccessible { get { return GameTileType.Inaccessible; } }
    public bool Water { get { return GameTileType.Water; } }
}