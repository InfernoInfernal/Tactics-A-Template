using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Tool for game object spawning to represent tiles in the tilemap and contain relevant information for other classes
/// </summary>
public class GameTileManager : ScriptableWizard
{
    //Tilemap to generate/degenerate files inside (you could potentially drag this in instead of using Tilemap tag)
    public Tilemap TilemapComponent;
    //Visible list of Game Tile Type scriptable objects loaded by the wizard
    public List<GameTileType> GameTileTypes = new List<GameTileType>();
    //GameTileTracker monobehaviour that contains the GameTileDictionary for tracking all the Game Object GameTiles by their grid coordinates
    [HideInInspector]
    public GameTileTracker GameTileTrackerComponent = null;

    /// <summary>
    /// Wizard Constructor
    /// </summary>
    [MenuItem ("Tools/Game Tile Manager")]
    static void GameTileManagerWizard()
    {
        DisplayWizard<GameTileManager>("Game Tile Manager", "Destroy Game Tiles", "Regenerate Game Tiles");
    }

    /// <summary>
    /// Awake function will find and load the tilemap, gametiletracker, and gametiletypes in assets
    /// </summary>
    private void Awake()
    {
        //Find Tilemap (assumes only one tilemap instance)
        var tilemapCheck = GameObject.FindGameObjectWithTag(Constants.TilemapTag);
        if (tilemapCheck == null)
        {
            Debug.LogError("No Tilemap tag found! Cannot run Game Tile Manager Wizard.");
            return;
        }
        TilemapComponent = GameObject.FindGameObjectWithTag(Constants.TilemapTag).GetComponent<Tilemap>();

        //Attach GameTileTracker if it hasn't been added to the Tilemap
        GameTileTrackerComponent = GameObject.FindGameObjectWithTag(Constants.TilemapTag).GetComponent<GameTileTracker>();
        if (GameTileTrackerComponent == null)
            GameTileTrackerComponent = GameObject.FindGameObjectWithTag(Constants.TilemapTag).AddComponent<GameTileTracker>();

        //Load GameTileTypes
        string[] gameTileTypeGUIDs = AssetDatabase.FindAssets("t:GameTileType");
        for (int i = 0; i < gameTileTypeGUIDs.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(gameTileTypeGUIDs[i]);
            GameTileTypes.Add(AssetDatabase.LoadAssetAtPath<GameTileType>(path));
        }
    }

    /// <summary>
    /// Regenerate Game Tiles Button
    /// </summary>
    void OnWizardOtherButton()
    {
        DestroyGameTiles();
        CreateGameTiles();
    }

    /// <summary>
    /// Destroy Game Tiles Button
    /// </summary>
    void OnWizardCreate()
    {
        DestroyGameTiles();
    }

    /// <summary>
    /// Generate game tiles from the tilebases in the tilemap
    /// </summary>
    private void CreateGameTiles()
    {
        Dictionary<TileBase, GameTileType> gameTileMappingDictionary = new Dictionary<TileBase, GameTileType>();

        //Without Game Tile Types, no game tiles can be loaded
        if (GameTileTypes.Count == 0)
        {
            Debug.LogError("No Game Tile Types found! GameTiles cannot be generated.");
            return;
        }
        else //Prepare Game Tile Dictionary
        {
            foreach (GameTileType gameTileType in GameTileTypes)
            {
                if (gameTileType.GameTileBase == null)
                {
                    Debug.LogError($"No TileBase class for {gameTileType.name} found! GameTiles cannot be generated.");
                    return;
                }

                gameTileMappingDictionary.Add(gameTileType.GameTileBase, gameTileType);
            }
        }

        //The Orientation mode needs to be Custom to work properly
        if (TilemapComponent.orientation != Tilemap.Orientation.Custom)
        {
            Debug.LogError("The Tilemap Component's Orientation needs to be set to Custom and use Offset, " +
                "as standard Tile Anchor orientation doesn't work with CellToWorld calculations");
            return;
        }

        //Compress bounds and prepare XYZ counters for only ranges that contain tiles
        TilemapComponent.CompressBounds();
        int xStart = TilemapComponent.cellBounds.position.x;
        int yStart = TilemapComponent.cellBounds.position.y;
        int zStart = TilemapComponent.cellBounds.position.z;
        int xCount = TilemapComponent.cellBounds.size.x;
        int yCount = TilemapComponent.cellBounds.size.y;
        int zCount = TilemapComponent.cellBounds.size.z;
        //Debug.Log($"TileScript Cellbounds xStart:{xStart} yStart:{yStart} zStart:{zStart} xCount:{xCount} yCount:{yCount} zCount:{zCount}");

        //Iterate through all tiles in the bounds and create corresponding game tiles
        for (int x = xStart; x < xStart + xCount; x++)
        {
            for (int y = yStart; y < yStart + yCount; y++)
            {
                for (int z = zStart + zCount; z > zStart - 1; z--) //Work backwards from top-down for z, so we only get the highest tiles
                {
                    //Debug.Log($"Tile Check at x:{x} y:{y} z:{z}");
                    TileBase tileBase = TilemapComponent.GetTile(new Vector3Int(x, y, z));
                    //If there's no tile, skip
                    if (tileBase == null)
                        continue;

                    //The tileBase MUST have a corresponding GameTileType, or it won't have a source to pull data from
                    if (!gameTileMappingDictionary.ContainsKey(tileBase))
                    {
                        Debug.LogError($"No corresponding GameTileType found for TileBase at GameTile X:{x} Y:{y} Z:{z}");
                        DestroyGameTiles(); //Clear out the GameTiles created thus far
                        return;
                    }

                    //Create the GameTile GameObject Instance
                    GameObject newGameTileObject = new GameObject($"GameTile Node X:{x} Y:{y} Z:{z}");
                    GameTile gameTileComponent = newGameTileObject.AddComponent<GameTile>();
                    gameTileComponent.GameTileType = gameTileMappingDictionary[tileBase];
                    newGameTileObject.tag = Constants.GameTileTag;

                    //Calculate XYZ world position and set the game object there, then parent it to the tilemap
                    gameTileComponent.CellPositionX = x;
                    gameTileComponent.CellPositionY = y;
                    gameTileComponent.CellPositionZ = z;
                    Vector3 worldPosition = TilemapComponent.CellToWorld(new Vector3Int(x, y, z));
                    //Note that the position must be offset by the Tile Anchor Offset of the Tilemap
                    newGameTileObject.transform.position = new Vector3(
                        worldPosition.x + TilemapComponent.orientationMatrix[0,3]/*TileAnchor Offset X*/, 
                        worldPosition.y + TilemapComponent.orientationMatrix[1,3]/*TileAnchor Offset Y*/,
                        worldPosition.z + TilemapComponent.orientationMatrix[2,3]/*TileAnchor Offset Z*/ + 0.001f/*Highlight Padding*/);
                    newGameTileObject.transform.parent = TilemapComponent.transform;

                    //Set Surface Orientation for GameTile, If rotation >= 180 it means inclined the other direction
                    if (gameTileComponent.GameTileType.SurfaceOrientation == TileSurfaceOrientation.Flat)
                        gameTileComponent.SurfaceOrientation = TileSurfaceOrientation.Flat;
                    else if (gameTileComponent.GameTileType.SurfaceOrientation == TileSurfaceOrientation.InclinedDownToLeft)
                    {
                        if (TilemapComponent.GetTransformMatrix(new Vector3Int(x, y, z)).rotation.eulerAngles.y < 180)
                            gameTileComponent.SurfaceOrientation = TileSurfaceOrientation.InclinedDownToLeft;
                        else
                            gameTileComponent.SurfaceOrientation = TileSurfaceOrientation.InclinedDownToRight;
                    }
                    else if (gameTileComponent.GameTileType.SurfaceOrientation == TileSurfaceOrientation.InclinedDownToRight)
                    {
                        if (TilemapComponent.GetTransformMatrix(new Vector3Int(x, y, z)).rotation.eulerAngles.y < 180)
                            gameTileComponent.SurfaceOrientation = TileSurfaceOrientation.InclinedDownToRight;
                        else
                            gameTileComponent.SurfaceOrientation = TileSurfaceOrientation.InclinedDownToLeft;
                    }

                    //Add a collider using the base tile's sprite
                    PolygonCollider2D collider = newGameTileObject.AddComponent<PolygonCollider2D>();
                    collider.pathCount = TilemapComponent.GetSprite(new Vector3Int(x, y, z)).GetPhysicsShapeCount();
                    List<Vector2> points = new List<Vector2>();
                    for (int i = 0; i < collider.pathCount; i++)
                    {
                        TilemapComponent.GetSprite(new Vector3Int(x, y, z)).GetPhysicsShape(i, points);

                        //If we have a rotated tile, we need to flip the X coordinates
                        if (TilemapComponent.GetTransformMatrix(new Vector3Int(x, y, z)).rotation.eulerAngles.y >= 180)
                        {
                            for (int j = 0; j < points.Count; j++)
                            {
                                points[j] = new Vector2(points[j].x * -1, points[j].y);
                            }
                        }

                        collider.SetPath(i, points);
                    }

                    //Add and setup highlight component
                    SpriteRenderer spriteRenderer = newGameTileObject.AddComponent<SpriteRenderer>();
                    spriteRenderer.enabled = false;
                    spriteRenderer.spriteSortPoint = SpriteSortPoint.Pivot;
                    spriteRenderer.sprite = gameTileComponent.GameTileHighlight;
                    if (gameTileComponent.SurfaceOrientation == TileSurfaceOrientation.InclinedDownToRight)
                        spriteRenderer.flipX = true;

                    //lastly, Add the GameTile's Game Object to the GameTileDictionary for tracking
                    GameTileTrackerComponent.GameTileDictionary.Add(new Vector2(x, y), newGameTileObject);
                    //Debug.Log($"GameTileDictionary Entry with x:{x} y:{y} {newGameTileObject.name}");

                    //Break after the highest tile is found and an object created, we don't need anything for lower tiles
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Destroy all game tiles in the scene
    /// </summary>
    private void DestroyGameTiles()
    {
        GameObject[] gameTileObjects = GameObject.FindGameObjectsWithTag(Constants.GameTileTag);

        foreach (GameObject gameTileObject in gameTileObjects)
        {
            DestroyImmediate(gameTileObject);
        }

        GameTileTrackerComponent.GameTileDictionary.Clear();
    }
}
