using Godot;
using System;

public class Unit : KinematicBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private Array path;
    private int path_ind = 0;
    private const int MOVE_SPEED = 12;
    private Node nav;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        nav = GetParent();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
