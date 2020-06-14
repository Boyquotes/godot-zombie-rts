using Godot;
using System;
using Godot.Collections;
using Array = Godot.Collections.Array;

public class Unit : KinematicBody
{
    [Export] private int _team = 0;
    private readonly Array _teamColours = new Array
    {
        ResourceLoader.Load("res://TeamOneMaterial.tres"),
        ResourceLoader.Load("res://TeamTwoMaterial.tres")
    };
    private Array<Vector3> _path = new Array<Vector3>();
    public int _pathIndex = 0;
    private const int MoveSpeed = 12;
    private Navigation _navigation;

    /**
     * Called when the node enters the scene tree for the first time.
     */
    public override void _Ready()
    {
        _navigation = (Navigation) GetParent();
        if (_team >= 0 && _team < _teamColours.Count)
        {
            GetNode<MeshInstance>("MeshInstance").MaterialOverride = (Material) _teamColours[_team];
        }
    }


    /**
     * Move the unit to the given target position
     */
    public void MoveTo(Vector3 targetPosition)
    {
        var path = _navigation.GetSimplePath(GlobalTransform.origin, targetPosition);
        _pathIndex = 0;
    }


    /**
     * Accurate calculation step for game loop
     */
    public override void _PhysicsProcess(float delta)
    {
        if (_pathIndex >= _path.Count) return; // Do nothing
        
        var movementVector = _path[_pathIndex] - GlobalTransform.origin;
        
        if (movementVector.Length() > 0.1)
        {
            _pathIndex++; // Do the next move
        }
        else
        {
            MoveAndSlide(movementVector.Normalized() * MoveSpeed, 
                new Vector3(0, 1, 0));
        }
    }


    /**
     * Show the selection ring upon unit selection
     */
    private void Select()
    {
        GetNode<MeshInstance>("SelectionRing").Show();
    }
    
    
    /**
     * Show the selection ring upon unit deselection
     */
    private void Deselect()
    {
        GetNode<MeshInstance>("SelectionRing").Hide();
    }
}
