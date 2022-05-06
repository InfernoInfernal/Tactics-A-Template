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

    private GameObject CurrentGameTile;
    private SpriteRenderer CursorSpriteRenderer;
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
            //Check for GameTile colliders under the mouse
            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hits.Length > 0)
            {
                //Run through the collider hits and select the highest GameTile via its Z position value
                GameObject highestGameTile = null;
                foreach (RaycastHit2D hit in hits)
                {
                    float highestZ = float.MinValue;
                    if (hit.collider.gameObject.tag == GameTileTag 
                        && hit.collider.gameObject.transform.position.z > highestZ)
                    {
                        highestGameTile = hit.collider.gameObject;
                        highestZ = hit.collider.gameObject.transform.position.z;
                    }
                }

                //Check to see if we moved to a new tile, and proceed with updating the cursor if so
                if (CurrentGameTile != highestGameTile)
                {
                    CurrentGameTile = highestGameTile;

                    //Set this cursor to the position of the new GameTile.
                    //Add 0.525f to Z: +0.5 to uniformly hover over the tile and overlapping neighbors of equal height and lower,
                    //and the extra 0.025 will pad for any overlapping pixels on edges of said neighboring tilemap sprites
                    this.transform.position = new Vector3(
                        CurrentGameTile.transform.position.x,
                        CurrentGameTile.transform.position.y,
                        CurrentGameTile.transform.position.z + 0.025f);

                    //Get the Game Tile Component script, throw an error if it's missing since we'll need the data
                    GameTile gameTileComponent = CurrentGameTile.GetComponent<GameTile>();
                    if (gameTileComponent == null)
                    {
                        Debug.LogError($"No GameTile script component found at {CurrentGameTile.name}");
                        return;
                    }

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
}