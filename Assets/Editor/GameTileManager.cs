using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Tool for game object spawning to represent tiles in the tilemap and contain relevant information for other classes
/// </summary>
public class GameTileManager : ScriptableWizard
{
    static readonly string TilemapTag = "Tilemap"; //Tilemap Tag
    static readonly string GameTileTag = "GameTile"; //Game Tile Tag
    static readonly int pixelHeight = 8; //The number of pixels in one game height unit of Y on a sprite

    //Tilemap to generate/degenerate files inside (you could potentially drag this in instead of using Tilemap tag)
    public Tilemap TilemapComponent;
    //Visible list of Game Tile Type scriptable objects loaded by the wizard
    public List<GameTileType> GameTileTypes = new List<GameTileType>();

    /// <summary>
    /// Wizard Constructor
    /// </summary>
    [MenuItem ("Tools/Game Tile Manager")]
    static void GameTileManagerWizard()
    {
        DisplayWizard<GameTileManager>("Game Tile Manager", "Destroy Game Tiles", "Regenerate Game Tiles");
    }

    /// <summary>
    /// Awake function will find and load the tilemap and gametiletypes in assets
    /// </summary>
    private void Awake()
    {
        //Find Tilemap (assumes only one tilemap instance)
        var tilemapCheck = GameObject.FindGameObjectWithTag(TilemapTag);
        if (tilemapCheck == null)
        {
            Debug.LogError("No Tilemap tag found! Cannot run Game Tile Manager Wizard.");
            return;
        }
        TilemapComponent = GameObject.FindGameObjectWithTag(TilemapTag).GetComponent<Tilemap>();

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
                gameTileMappingDictionary.Add(gameTileType.tileBase, gameTileType);
            }
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

                    //Create the GameTile GameObject Instance
                    GameObject newGameTileObject = new GameObject($"GameTile Node X:{xStart} Y:{yStart} Z:{zStart}");
                    GameTile gameTileComponent = newGameTileObject.AddComponent<GameTile>();
                    newGameTileObject.tag = GameTileTag;
                    if (gameTileMappingDictionary.ContainsKey(tileBase))
                        gameTileComponent.GameTileType = gameTileMappingDictionary[tileBase];
                    else
                    {
                        //The tileBase MUST have a corresponding GameTileType, or it won't have a source to pull data from
                        Debug.LogError($"No corresponding GameTileType found for TileBase at GameTile X:{xStart} Y:{yStart} Z:{zStart}");
                        return;
                    }

                    //Calculate XYZ world position and set the game object there, then parent it to the tilemap
                    //TODO: Fix the height offset of 0.5ish?
                    Vector3 worldPosition = TilemapComponent.CellToWorld(new Vector3Int(x, y, z));
                    newGameTileObject.transform.position = new Vector3(
                        worldPosition.x, worldPosition.y, gameTileComponent.TileSpriteHeight);
                    newGameTileObject.transform.parent = TilemapComponent.transform;

                    //Add a collider using the base tile's sprite
                    PolygonCollider2D collider = newGameTileObject.AddComponent<PolygonCollider2D>();
                    collider.pathCount = TilemapComponent.GetSprite(new Vector3Int(x, y, z)).GetPhysicsShapeCount();
                    List<Vector2> points = new List<Vector2>();
                    for (int i = 0; i < collider.pathCount; i++)
                    {
                        TilemapComponent.GetSprite(new Vector3Int(x, y, z)).GetPhysicsShape(i, points);
                        collider.SetPath(i, points);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Destroy all game tiles in the scene
    /// </summary>
    private void DestroyGameTiles()
    {
        GameObject[] gameTileObjects = GameObject.FindGameObjectsWithTag(GameTileTag);

        foreach (GameObject gameTileObject in gameTileObjects)
        {
            DestroyImmediate(gameTileObject);
        }
    }
}
