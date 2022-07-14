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
    public int GameTileSpriteHeightMaximum; //Game unit height from tile bottom - needed for jump and move calculations
    public int GameTileSpriteHeightMinimum; //For flat tiles, same as Maximum. For inclined, game height
                                          //from the lowest point of the slant to the tile's bottom
    public int MovementCost = 1; //Cost to walk to the tile from an adjacent one using normal movement
    public TileSurfaceOrientation SurfaceOrientation; //The default orientation of the tile's incline, if any
    public TileInclineRise InclineRise; //The rise of the tile's incline, if any
    public bool Inaccessible = false; //Set to true if the tile isn't traversable to ground units
    public bool Liquid = false; //Whether the tile is liquid (water or lava, for example)
}
