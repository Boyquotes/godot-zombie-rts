using Godot;
using System;

public class TerrainMesh : MeshInstance
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // TODO: Continue here https://docs.godotengine.org/en/stable/tutorials/shading/your_first_shader/your_second_spatial_shader.html
        
        
        var derp = Mesh.SurfaceGetMaterial(0) as ShaderMaterial;
        if (derp != null)
        {
            derp.SetShaderParam("height_scale", 2.5);
        }
        else
        {
            GD.Print("Not shader material");
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
