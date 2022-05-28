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
    static readonly string GameTileTag = "GameTile"; //Game Tile Tag

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
            GameObject VisibleGameTile = FindVisibleGameTileUnderMouse();
            //Check to see if we moved to a new tile, and proceed with updating the cursor if so
            if (VisibleGameTile != null && VisibleGameTile != CurrentGameTile)
                UpdateCurrentGameTileCursor(VisibleGameTile);
        }

        //Left Click
        if (Input.GetMouseButtonDown(0))
        {

        }

        //Right Click
        if (Input.GetMouseButtonDown(1))
        {

        }

        //Mouse Wheel Scrolled
        if (Input.mouseScrollDelta.y != 0)
        {
            
        }
    }

    GameObject FindVisibleGameTileUnderMouse()
    {
        //Check for GameTile colliders under the mouse
        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hits.Length > 0)
        {
            //Run through the collider hits and select the visibly hovered over GameTile from collider hits
            //Which can be determined from the lowest combined X&Y tilemap cell position values
            GameObject visibleHitGameTile = null;
            GameTile gameTileComponent = null;
            float lowestXY = float.MaxValue;
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject.tag == GameTileTag)
                {
                    //Get the Game Tile Component script, throw an error if it's missing since we'll need the data
                    gameTileComponent = hit.collider.gameObject.GetComponent<GameTile>();
                    if (gameTileComponent == null)
                    {
                        Debug.LogError($"No GameTile script component found at {hit.collider.gameObject.name}!");
                        return null;
                    }

                    //Lowest combined CELL value (not world) of X+Y = visible/foremost tile
                    if (gameTileComponent.CellPositionX + gameTileComponent.CellPositionY < lowestXY)
                    {
                        visibleHitGameTile = hit.collider.gameObject;
                        lowestXY = gameTileComponent.CellPositionX + gameTileComponent.CellPositionY;
                    }
                }
            }
            return visibleHitGameTile;
        }
        else
        {
            //No hits, return null
            return null;
        }
    }

    void UpdateCurrentGameTileCursor(GameObject NewGameTile)
    {
        CurrentGameTile = NewGameTile;
        GameTile gameTileComponent = CurrentGameTile.GetComponent<GameTile>();

        //Set this cursor to the position of the new GameTile.
        //The extra 0.01f to Z draws on top of the underlying tilemap sprite
        this.transform.position = new Vector3(
            CurrentGameTile.transform.position.x,
            CurrentGameTile.transform.position.y,
            CurrentGameTile.transform.position.z + 0.01f);

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