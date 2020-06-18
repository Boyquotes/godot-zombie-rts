using Godot;
using System;
using Godot.Collections;
using Array = Godot.Collections.Array;
using Object = Godot.Object;

public class CameraBase : Spatial
{
    private const int MoveMargin = 30;
    private const int MoveSpeed = 30; // Units per second
    private const int RayLength = 1000; // Max distance of mouse click
    
    private Camera _camera;
    private const int Team = 0;
    private Array<Unit> _selectedUnits = new Array<Unit>();
    private SelectionBox _selectionBox;
    private Vector2 _startSelectionPosition;
    private bool _mouseInViewport = true;


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
     * Get notifications from the OS to detect events external to the game
     */
    public override void _Notification(int what)
    {
        switch (what)
        {
            case NotificationWmMouseEnter:
                _mouseInViewport = true;
                break;
            case NotificationWmMouseExit:
                _mouseInViewport = false;
                break;
        }
    }

    
    /**
     * Move camera depending on mouse position on edges of screen
     */
    private void CalculateMovement(Vector2 mousePosition, float delta)
    {
        var viewportSize = GetViewport().Size;
        var moveVector = new Vector3();

        
        /* Dont move anymore if mouse out of viewport */
        if (_mouseInViewport)
        {
            /* Increase/decrese the movement vectors x/y depending on the viewport
            edge the mouse is pushing against */
            if (mousePosition.x < MoveMargin) moveVector.x--;
            if (mousePosition.y < MoveMargin) moveVector.z--;
            if (mousePosition.x > viewportSize.x - MoveMargin) moveVector.x++;
            if (mousePosition.y > viewportSize.y - MoveMargin) moveVector.z++;
        }
        else
        {
            moveVector.x = 0;
            moveVector.z = 0;
        }

        var axis = new Vector3(0, 1, 0);
        var phi = RotationDegrees.y;
        moveVector = moveVector.Rotated(axis, phi);
        GlobalTranslate(moveVector * delta * MoveSpeed);
    }


    /*/**
     * NOTE! Kept for potential future use...
     * Move all units of the users team to the position of mouse click.
     #1#
    private void MoveAllUnits(Vector2 mousePosition)
    {
        // Get object under mask, using 1 collision mask for env
        var result = RayCastFromMouse(mousePosition, 1);
        if (result.Count <= 0) return; // Do nothing
        GD.Print("Right clicked at position ", result["position"]);
        GetTree().CallGroup("Units", "MoveTo", result["position"]);
    }*/


    /**
     * Move all selected units to the position of mouse click.
     */
    private void MoveSelectedUnits(Vector2 mousePosition)
    {
        // Get object under mask, using 1 collision mask for env
        var result = RayCastFromMouse(mousePosition, 1);
        if (result.Count <= 0) return; // Do nothing...
        foreach (var unit in _selectedUnits)
        {
            unit.MoveTo((Vector3) result["position"]);
        }
    }


    /**
     * Select units using appropriate method detected from user selection method
     */
    private void SelectUnits(Vector2 mousePosition)
    {
        var newSelectedUnits = new Array<Unit>();

        // Check for mouse click and drag (where 16 is minimum drag tolerance)
        if (mousePosition.DistanceSquaredTo(_startSelectionPosition) < 16)
        {
            // Single click (little to no mouse click and drag)
            var unit = GetUnitUnderMouse(mousePosition);
            if (unit != null) newSelectedUnits.Add(unit);
        }
        else
        {
            // Click and drag (box select units)
            newSelectedUnits = GetUnitsInBox(_startSelectionPosition, mousePosition);
        }

        // Deselect old units, select new, and update the selected units list
        if (newSelectedUnits.Count == 0) return; // do nothing...

        foreach (var unit in _selectedUnits)
        {
            unit.Deselect();
        }

        foreach (var unit in newSelectedUnits)
        {
            unit.Select();
        }

        _selectedUnits = newSelectedUnits;
    }


    /**
     * Get a list of units that are on the users team and in the selection box
     */
    private Array<Unit> GetUnitsInBox(Vector2 topLeft, Vector2 bottomRight)
    {
        /* Verify top left is top left, otherwise swap them over
        (dragged up left instead of down right) */
        if (topLeft.x > bottomRight.x)
        {
            var swap = topLeft.x;
            topLeft.x = bottomRight.x;
            bottomRight.x = swap;
        }

        if (topLeft.y > bottomRight.y)
        {
            var swap = topLeft.y;
            topLeft.y = bottomRight.y;
            bottomRight.y = swap;
        }

        // Create rectangle from the coordinates
        var box = new Rect2(topLeft, bottomRight - topLeft);

        // Get those units in our team and in the selection box
        var boxSelectedUnits = new Array<Unit>();
        foreach (Unit unit in GetTree().GetNodesInGroup("Units"))
        {
            var unitProjectedPosition = _camera.UnprojectPosition(unit.GlobalTransform.origin);
            if (unit.Team == Team && box.HasPoint(unitProjectedPosition))
            {
                boxSelectedUnits.Add(unit);
            }
        }

        return boxSelectedUnits;
    }


    /**
     * Get the unit in the users team that is under the mouse on a single select click
     */
    private Unit GetUnitUnderMouse(Vector2 mousePosition)
    {
        // Get object under mask, using 3 collision mask for units and env
        var result = RayCastFromMouse(mousePosition, 3);
        if (result.Count <= 0) return null; // do nothing...

        // Return the unit if we hit one and its part of our team
        if (result["collider"] is Unit unit && unit.Team == Team) return unit;

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
            //MoveAllUnits(mousePosition);
            MoveSelectedUnits(mousePosition);
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