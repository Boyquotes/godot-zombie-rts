using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 15f; // Units per second of movement
    public float scrollSpeed = 200f; // Speed of mouse wheel scroll
    public float minY = 10f;
    public float maxY = 100f;

    /**
     * Update is called once per frame
     */
    private void Update()
    {
        var position = transform.position;
        var moveOffset = panSpeed * Time.deltaTime; // Speed respective of zoom

        // Check for key presses
        if (Input.GetKey("w"))
        {
            position.z += moveOffset;
        }

        if (Input.GetKey("a"))
        {
            position.x -= moveOffset;
        }

        if (Input.GetKey("s"))
        {
            position.z -= moveOffset;
        }
        
        if (Input.GetKey("d"))
        {
            position.x += moveOffset;
        }
        
        // Update scroll
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        position.y -= scroll * scrollSpeed * Time.deltaTime;
        position.y = Mathf.Clamp(position.y, minY, maxY);
        
        // Update position
        transform.position = position;
    }
}