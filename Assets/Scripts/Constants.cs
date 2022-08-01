/// <summary>
/// Static master reference for constant project settings
/// </summary>
public static class Constants
{
    /// <summary>
    /// Tilemap Tag
    /// </summary>
    public static readonly string TilemapTag = "Tilemap";

    /// <summary>
    /// Game Tile Tag
    /// </summary>
    public static readonly string GameTileTag = "GameTile";

    /// <summary>
    /// The number of pixels in a tile single height unit
    /// </summary>
    public static readonly int PixelPerGameUnitHeight = 8;

    /// <summary>
    /// The number of game units a characteer sprite is treated as being
    /// This determines how many game units an occupied tile is treated as higher
    /// when calculating if an opposing unit can leap over it
    /// </summary>
    public static readonly int CharacterSpriteGameHeight = 3;
}