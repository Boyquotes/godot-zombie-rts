using Godot;
using System;
using System.Linq;
using Godot.Collections;

public class Terrain : Spatial
{
    private int _width;
    private int _height;
    private Array<Vector3> _vertices = new Array<Vector3>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _width = 300;
        _height = 300;

        /* Add all triangle positions to the vertex array */
        for (var x = 0; x < _width; x++)
        {
            for (var y = 0; y < _height; y++)
            {
                _vertices.Add(new Vector3(x, 0, -y));
                _vertices.Add(new Vector3(x+1, 0, -y-1));
                _vertices.Add(new Vector3(x+1, 0, -y));
            }
        }
        
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}