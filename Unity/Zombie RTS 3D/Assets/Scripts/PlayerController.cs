using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private Camera _camera;

    public LayerMask _groundLayer;
    public NavMeshAgent _playerAgent;

    /**
     * Awake is called before the first frame update, and upon instantiation
     * before anything else is called
     */
    private void Awake()
    {
        _camera = Camera.main; // Search all cameras for one tagged "main"
        if (_camera != null) Debug.Log(_camera.name);
    }


    /**
     * Update is called once per frame
     */
    private void Update()
    {
        // Check for right click and issue move order
        if (Input.GetMouseButtonDown(1))
        {
            _playerAgent.SetDestination(GetPointUnderCursor());
        }
    }


    /**
     * Get the real world coordinates of the mouse cursor by ray casting from the viewport
     * onto the world surface
     */
    private Vector3 GetPointUnderCursor()
    {
        // Get real world point at position screen is
        Vector2 screenPosition = Input.mousePosition;
        var screenWorldPosition =
            _camera.ScreenToWorldPoint(screenPosition);
        const int rayCastDistance = 1000;

        Physics.Raycast(
            screenWorldPosition,
            _camera.transform.forward,
            out var hitPosition,
            rayCastDistance,
            _groundLayer
        );

        // Return the impact point of ray and collider
        return hitPosition.point;
    }
}