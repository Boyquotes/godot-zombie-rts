using Godot;
using System;
using Godot.Collections;
using Array = Godot.Collections.Array;
using Object = Godot.Object;

public class CameraBase : Spatial
{
    private const int MoveMargin = 20;
    private const int MoveSpeed = 30; // Units per second
    private const int RayLength = 1000; // Max distance of mouse click

    private Camera _camera;
    private int _team = 0;
    private Array _selectedUnits = new Array();
    private SelectionBox _selectionBox;
    private Vector2 _startSelectionPosition = new Vector2();


    /**
     * Called when the node enters the scene tree for the first time.
     */
    public override void _Ready()
    {
        _camera = GetNode<Camera>("Camera");
        _selectionBox = GetNode<SelectionBox>("SelectionBox");
    }


    /**
     * Called every frame.
     * 'delta' is the elapsed time since the previous frame.
     */
    public override void _Process(float delta)
    {
        var mousePosition = GetViewport().GetMousePosition();
        CalculateMovement(mousePosition, delta);

        /* Input handlers */
        HandleInput(mousePosition);
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


    /**
     * Move all units of the users team to the position of mouse click.
     */
    private void MoveAllUnits(Vector2 mousePosition)
    {
        var result = RayCastFromMouse(mousePosition, 1); // 1 is environment mask in decimal
        if (result.Count <= 0) return; // Do nothing
        GD.Print("Right clicked at position ", result["position"]);
        GetTree().CallGroup("Units", "MoveTo", result["position"]);
    }


    /**
     * Move all selected units to the position of mouse click.
     */
    private void MoveSelectedUnits(Vector2 mousePosition)
    {
        var result = RayCastFromMouse(mousePosition, 1); // 1 is environment mask in decimal
        if (result.Count <= 0) return; // Do nothing...
        foreach (Unit unit in _selectedUnits)
        {
            unit.MoveTo((Vector3) result["position"]);
        }
    }


    /**
     * Select units using appropriate method detected from user selection method
     */
    private void SelectUnits(Vector2 mousePosition)
    {
        var newSelectedUnits = new Array();
        
        // Check for mouse click and drag (where 16 is minimum drag tolerance)
        if (mousePosition.DistanceSquaredTo(_startSelectionPosition) < 16)
        {
            // Single click (little to no mouse click and drag)
            var unit = GetUnitUnderMouse(mousePosition);
        }
    }

    
    /**
     * 
     */
    private object GetUnitUnderMouse(Vector2 mousePosition)
    {
        // Get unit under mask, using 3 collision mask for units and env
        var result = RayCastFromMouse(mousePosition, 3);
        
        GD.Print("Result = ", result.ToString());
        // Return the unit if we hit one and its part of our team
        if (result.Count <= 0 && result["collider"].Equals("team") 
                              && result["collider"] == _team.ToString())
        {
            return result["collider"];
        }

        return null;
    }


    /**
     * Get the mouse position on the "environment" mask upon intersection.
     */
    private Dictionary RayCastFromMouse(Vector2 mousePosition, uint collisionMask)
    {
        var rayFrom = _camera.ProjectRayOrigin(mousePosition);
        var rayTo = rayFrom + _camera.ProjectRayNormal(mousePosition) * RayLength;
        var spaceState = GetWorld().DirectSpaceState;

        return spaceState.IntersectRay(rayFrom, rayTo, new Array(), collisionMask);
    }


    /**
     * Handle user inputs (button press, mouse clicks...)
     */
    private void HandleInput(Vector2 mousePosition)
    {
        // Right mouse click
        if (Input.IsActionJustPressed("main_command"))
        {
            MoveAllUnits(mousePosition);
        }
        
        // Left mouse click
        if (Input.IsActionJustPressed("alt_command"))
        {
            _selectionBox.StartSelectionPosition = mousePosition;
            _startSelectionPosition = mousePosition;
        }
        
        // Left mouse click and hold
        if (Input.IsActionPressed("alt_command"))
        {
            _selectionBox.MousePosition = mousePosition;
            _selectionBox.Visible = true;
        }
        else
        {
            _selectionBox.Visible = false;
        }
        
        // Left mouse click release (now select the units)
        if (Input.IsActionJustReleased("alt_command"))
        {
            SelectUnits(mousePosition);
        }
    }
}