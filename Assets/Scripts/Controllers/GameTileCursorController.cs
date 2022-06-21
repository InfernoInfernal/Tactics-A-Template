using UnityEngine;

/// <summary>
/// Orientation setting for an inclined cursor
/// </summary>
public enum InclineOrientation
{
    DownToLeft,
    DownToRight
}

/// <summary>
/// Attached to the Game Tile Cursor, this script will capture input related to it while the parent cursor object is enabled
/// It uses the GameTile colliders to direct its position and for getting pertinent info
/// It will move the game tile cursor accordingly and do something for button presses. Maybe events or message to a buffer?
/// </summary>
public class GameTileCursorController : MonoBehaviour
{
    public Sprite FlatCursorSprite;
    public Sprite SingleInclinedCursorSprite;
    public InclineOrientation SingleOrientation;
    public Sprite DoubleInclinedCursorSprite;
    public InclineOrientation DoubleOrientation;

    private SpriteRenderer CursorSpriteRenderer;
    private GameObject CurrentGameTile;
    private TileSurfaceOrientation CurrentInclineOrientation;
    private TileInclineRise CurrentInclineRise;

    private void Awake()
    {
        CursorSpriteRenderer = this.GetComponent<SpriteRenderer>();
        if (CursorSpriteRenderer == null)
            Debug.LogError("No SpriteRenderer attached to Game Tile Cursor!");
    }

    void Update()
    {
        //Mouse has moved since last frame
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            GameObject VisibleGameTile = 
                GameTileFunctions.GetGameTileFromPositionalRaycast(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            //Check to see if we moved to a new tile, and proceed with updating the cursor if so
            if (VisibleGameTile != null && VisibleGameTile != CurrentGameTile)
                UpdateCurrentGameTileCursor(VisibleGameTile);

            //Lerp Camera chase to tile Tile?
        }

        //Left Click
        if (Input.GetMouseButtonDown(0))
        {
            //Test Toggle Animation between Default and Idle
            GameTile currentGameTileComponent = CurrentGameTile.GetComponent<GameTile>();
            if (currentGameTileComponent != null && currentGameTileComponent.OccupyingCharacter != null)
            {
                Debug.Log("Character Found, toggling animation");
                CharacterGameData cData = currentGameTileComponent.OccupyingCharacter.GetComponent<CharacterGameData>();
                //CharacterFunctions.ChangeAnimationState("Default", cData);
                CharacterFunctions.ChangeAnimationState("Idle", cData);
            }
            //-Test Toggle Animation between Default and Idle
        }

        //Right Click
        if (Input.GetMouseButtonDown(1))
        {
            //Test Rotate
            GameTile currentGameTileComponent = CurrentGameTile.GetComponent<GameTile>();
            if (currentGameTileComponent != null && currentGameTileComponent.OccupyingCharacter != null)
            {
                Debug.Log("Character Found, rotating");
                CharacterGameData cData = currentGameTileComponent.OccupyingCharacter.GetComponent<CharacterGameData>();
                if ((int)cData.DirectionFaced == 3)
                    CharacterFunctions.ChangeOrientation(0, cData);
                else
                    CharacterFunctions.ChangeOrientation(cData.DirectionFaced+1, cData);
            }
            //-Test Rotate
        }

        //Mouse Wheel Scrolled
        if (Input.mouseScrollDelta.y != 0)
        {
            
        }
    }

    /// <summary>
    /// Updates the Game Tile Cursor to conform to the current game tile passed
    /// </summary>
    /// <param name="NewGameTile">The new Game Tile to conform the cursor to</param>
    void UpdateCurrentGameTileCursor(GameObject NewGameTile)
    {
        CurrentGameTile = NewGameTile;
        GameTile gameTileComponent = CurrentGameTile.GetComponent<GameTile>();

        //Set this cursor to the position of the new GameTile.
        //The extra 0.01f to Z draws on top of the underlying tilemap sprite
        this.transform.position = new Vector3(
            CurrentGameTile.transform.position.x,
            CurrentGameTile.transform.position.y,
            CurrentGameTile.transform.position.z + 0.125f);

        //Check incline and orientation and update them if needed
        if (CurrentInclineRise != gameTileComponent.InclineRise)
        {
            switch (gameTileComponent.InclineRise)
            {
                case TileInclineRise.Zero:
                    CursorSpriteRenderer.sprite = FlatCursorSprite;
                    break;
                case TileInclineRise.Single:
                    CursorSpriteRenderer.sprite = SingleInclinedCursorSprite;
                    break;
                case TileInclineRise.Double:
                    CursorSpriteRenderer.sprite = DoubleInclinedCursorSprite;
                    break;
            }
            CurrentInclineRise = gameTileComponent.InclineRise;
        }
        if (CurrentInclineOrientation != gameTileComponent.SurfaceOrientation)
        {
            switch (gameTileComponent.SurfaceOrientation)
            {
                case TileSurfaceOrientation.Flat:
                    break;
                case TileSurfaceOrientation.InclinedDownToLeft:
                    if (SingleOrientation == InclineOrientation.DownToLeft)
                        CursorSpriteRenderer.flipX = false;
                    else if (SingleOrientation == InclineOrientation.DownToRight)
                        CursorSpriteRenderer.flipX = true;
                    break;
                case TileSurfaceOrientation.InclinedDownToRight:
                    if (SingleOrientation == InclineOrientation.DownToRight)
                        CursorSpriteRenderer.flipX = false;
                    else if (SingleOrientation == InclineOrientation.DownToLeft)
                        CursorSpriteRenderer.flipX = true;
                    break;
            }
            CurrentInclineOrientation = gameTileComponent.SurfaceOrientation;
        }
    }
}