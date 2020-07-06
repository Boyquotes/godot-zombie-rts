using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f; // Units per second of movement

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


        // Update position
        transform.position = position;
    }
}