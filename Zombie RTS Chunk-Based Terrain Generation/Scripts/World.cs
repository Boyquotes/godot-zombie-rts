using Godot;
using System;
using Godot.Collections;
using ZombieRTSChunkBasedTerrainGeneration.Scripts;

public class World : Spatial
{
    private OpenSimplexNoise _noise;
    private Dictionary _chunksReady;
    private Dictionary _chunksNotReady; // For thread handling
    private Thread _thread;

    private const int Octaves = 6;
    private const int Periods = 80;
    private const int ChunkSize = 64;
    private const int ChunkCount = 16;


    /**
     * Called when the node enters the scene tree for the first time.
     */
    public override void _Ready()
    {
        // Create noise
        _noise = new OpenSimplexNoise
        {
            Seed = new Random().Next(),
            Octaves = Octaves,
            Period = Periods
        };
        
        _thread = new Thread();
        
        // Add chunk to the game
        
    }


    /**
     * Add a chunk to a given location in the game
     */
    private void AddChunk(int x, int z)
    {
        var key = $"{x},{z}";
        
    }
    

    /**
     * Called every frame. 'delta' is the elapsed time since the previous frame.
     */
    public override void _Process(float delta)
    {
    }
}