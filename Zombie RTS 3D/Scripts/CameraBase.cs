using Godot;
using System;

public class CameraBase : Spatial
{
    private const int MoveMargin = 20;
    private const int MoveSpeed = 30; // Units per second
    private const int RayLength = 1000;

    private Node _cam;
    private int _team = 0;
    private Node _selectionBox;
    private Vector2 _startSelectionPosition = new Vector2();


    /**
     * Called when the node enters the scene tree for the first time.
     */
    public override void _Ready()
    {
        _cam = GetNode("Camera");
        _selectionBox = GetNode("SelectionBox");
    }


    /**
     * Called every frame.
     * 'delta' is the elapsed time since the previous frame.
     */
    public override void _Process(float delta)
    {
        var mousePosition = GetViewport().GetMousePosition();
        CalculateMovement(mousePosition, delta);
    }


    /**
     * Move camera depending on mouse position on edges of screen
     */
    private void CalculateMovement(Vector2 mousePosition, float delta)
    {
        var viewportSize = GetViewport().Size;
        var moveVector = new Vector3();

        /* Increase/decrese the movement vectors x/y depending on the viewport
         edge the mouse is pushing against */
        if (mousePosition.x < MoveMargin) moveVector.x--;
        if (mousePosition.y < MoveMargin) moveVector.z--;
        if (mousePosition.x > viewportSize.x - MoveMargin) moveVector.x++;
        if (mousePosition.y > viewportSize.y - MoveMargin) moveVector.z++;

        var axis = new Vector3(0, 1, 0);
        var phi = RotationDegrees.y;
        
        moveVector = moveVector.Rotated(axis, phi);
        GlobalTranslate(moveVector * delta * MoveSpeed);
    }
}