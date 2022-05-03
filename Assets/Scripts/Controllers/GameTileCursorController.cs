using UnityEngine;

/// <summary>
/// Attached to the Game Tile Cursor, this script will capture input related to it while the parent cursor object is enabled
/// It uses the GameTile colliders to direct its position and for getting pertinent info
/// It will move the game tile cursor accordingly and do something for button presses. Maybe events or message to a buffer?
/// </summary>
public class GameTileCursorController : MonoBehaviour
{
    static readonly string GameTileTag = "GameTile"; //Game Tile Tag

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

                //Set this cursor to the position of the highest found GameTile.
                //Add 1.05f to Z: +1 to uniformly hover over the tile and overlapping neighbors of equal height and lower,
                //and the extra 0.05 will pad for any overlapping pixels on edges of said neighboring tilemap sprites
                this.transform.position = new Vector3(
                    highestGameTile.transform.position.x,
                    highestGameTile.transform.position.y, 
                    highestGameTile.transform.position.z + 1.05f);
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