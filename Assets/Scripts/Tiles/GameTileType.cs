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
    public TileBase tileBase;

    public float TileSpriteHeight; //when added with Z, Used for jump and move calculation.
                                   //For slanted tiles, uses midpoint of the incline's rise
    public int MovementCost = 1; //Cost to walk to the tile from an adjacent one using normal movement
    public TileSurfaceOrientation SurfaceOrientation; //The default orientation of the tile's incline, if any
    public TileInclineRise InclineRise; //The rise of the tile's incline, if any
    public bool Inaccessible = false; //Set to true if the tile isn't traversable to ground units
    public bool Liquid = false; //Whether the tile is liquid (Water or lava, for example)
}
