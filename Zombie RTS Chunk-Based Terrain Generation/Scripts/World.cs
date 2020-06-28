using Godot;
using System;
using Godot.Collections;
using ZombieRTSChunkBasedTerrainGeneration.Scripts;
using Array = Godot.Collections.Array;

public class World : Spatial
{
    private OpenSimplexNoise _noise;
    private Dictionary<string, Chunk> _chunksReady;
    private Dictionary<string, int> _chunksNotReady; // For thread handling
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

        _chunksReady = new Dictionary<string, Chunk>();
        _chunksNotReady = new Dictionary<string, int>();
        
        _thread = new Thread(); // Load chunks outside the main thread
    }


    /**
     * Add a chunk to a given location in the game
     */
    private void AddChunk(int x, int z)
    {
        // Check existence and thread status before adding
        var key = $"{x},{z}";
        if (_chunksReady.ContainsKey(key) || _chunksNotReady.ContainsKey(key) || _thread.IsActive())
        {
            return;
        }

        // Start thread to load chunk
        _thread.Start(this, "LoadChunk", new Array {_thread, x, z});
        _chunksNotReady.Add(key, 1);
    }


    /**
     * 
     */
    private void LoadChunk(Array userData)
    {
        var thread = userData[0] as Thread;

        // Multiply by chunk size to get position in noise vs grid position
        var x = (int) userData[1] * ChunkSize;
        var z = (int) userData[2] * ChunkSize;

        // Create chunk and set its position
        var chunk = new Chunk(_noise, x, z, ChunkSize)
        {
            Translation = new Vector3(x, 0, z)
        };

        CallDeferred("LoadDone", chunk, thread);
    }


    /**
     * Once chunk generated, add it to the scene and dictionary, and await thread
     */
    private void LoadDone(Chunk chunk, Thread thread)
    {
        AddChild(chunk);
        var key = $"{chunk.X / ChunkSize},{chunk.Z / ChunkSize}";
        _chunksReady.Add(key, chunk);
        _chunksNotReady.Remove(key);
        thread.WaitToFinish(); // Prevent add chunk conflicts
    }


    /**
     * 
     */
    private Chunk GetChunk(int x, int z)
    {
        var key = $"{x},{z}";
        return _chunksReady.ContainsKey(key) ? _chunksReady[key] : null;
    }
}