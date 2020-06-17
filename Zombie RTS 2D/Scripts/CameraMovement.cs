using Godot;
using System;

public class CameraMovement : Camera2D
{
    [Export] public float speed = (float) 10.0;


    /**
     *  Called when the node enters the scene tree for the first time.
     */
    public override void _Ready()
    {
    }

    /**
     *  Called every frame. 'delta' is the elapsed time since the previous frame.
     */
    public override void _Process(float delta)
    {
        // If both left and right keys pressed, then no movement
        var input_x = (Input.IsActionPressed("ui_right") ? 1 : 0)
                      - (Input.IsActionPressed("ui_left") ? 1 : 0);
         Position.x += input_x;
    }
}