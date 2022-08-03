using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Enum to set the direction of the incline type for the tile or if it is just flat
/// </summary>
public enum TileSurfaceOrientation
{
    Flat,
    InclinedDownToLeft,
    InclinedDownToRight
}

/// <summary>
/// Enum for the game height of an incline's rise (Zero if flat / no incline)
/// </summary>
public enum TileInclineRise
{
    Zero,
    Single,
    Double
}

/// <summary>
/// Class for holding game specific info about a tile base painted by tile palette
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/Game Tile Type")]
public class GameTileType : ScriptableObject
{
    public TileBase GameTileBase;
    public Sprite GameTileHighlight;

    [Space(10)]
    public int InclineGameHeight; //Game height for inclined tiles, 0 if flat
                                  //subtracted from Z to get the lowest point of the incline (highest is Z itself)
    public int MovementCost = 1; //Cost to walk to the tile from an adjacent one using normal movement
    public TileSurfaceOrientation SurfaceOrientation; //The default orientation of the tile's incline, if any
    public TileInclineRise InclineRise; //The rise of the tile's incline, if any
    public bool Inaccessible = false; //Set to true if the tile cannot be landed on (but could be leapt or flown over)
    public bool Water = false; //Whether the tile is water
}
