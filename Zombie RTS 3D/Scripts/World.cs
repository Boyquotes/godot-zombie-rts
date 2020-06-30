using System;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;


namespace ZombieRTS.Scripts
{
    public class World : Spatial
    {
        private OpenSimplexNoise _noise;
        private Dictionary<string, Chunk> _chunksReady;
        private Dictionary<string, int> _chunksNotReady; // For thread handling
        private Thread _thread;

        private const int Octaves = 5;
        private const int Periods = 75;
        private const int ChunkSize = 128;
        private const int ChunkCount = 9;


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
         * 
         */
        public override void _Process(float delta)
        {
            UpdateChunks();
            CleanUpChunks();
            ResetChunks();
        }


        /**
         * Add a chunk to a given location in the game
         */
        private void AddChunk(int x, int z)
        {
            // Check existence and thread status before adding
            var key = $"{x},{z}";
            if (_chunksReady.ContainsKey(key) || _chunksNotReady.ContainsKey(key) 
                                              || _thread.IsActive())
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


        /**
         * Use the players location to determine the current chunks surrounding them
         */
        private void UpdateChunks()
        {
            // Get player position as a grid position
            /*var playerTranslation = GetNode<KinematicBody>("Player").Translation;
            var playerX = (int) playerTranslation.x / ChunkSize;
            var playerZ = (int) playerTranslation.z / ChunkSize;*/
            
            // Use CameraRigs location
            var cameraTranslation = GetNode<Spatial>("CameraRig").Translation;
            var cameraX = (int) cameraTranslation.x / ChunkSize;
            var cameraZ = (int) cameraTranslation.z / ChunkSize;

            // 
            var xMin = (int) (cameraX - ChunkCount * 0.5);
            var xMax = (int) (cameraX + ChunkCount * 0.5);
            var zMin = (int) (cameraZ - ChunkCount * 0.5);
            var zMax = (int) (cameraZ + ChunkCount * 0.5);
            for (var x = xMin; x < xMax; x++)
            {
                for (var z = zMin; z < zMax; z++)
                {
                    AddChunk(x, z);
                    var chunk = GetChunk(x, z);
                    if (chunk != null) chunk.DoRemove = false; // Still within view
                }
            }
        }


        /**
         * Remove chunks marked for removal
         */
        private void CleanUpChunks()
        {
            foreach (var key in _chunksReady.Keys)
            {
                var chunk = _chunksReady[key];
                if (!chunk.DoRemove) return;

                chunk.QueueFree(); // Remove from scene tree
                _chunksReady.Remove(key);
            }
        }


        /**
         * Mark all chunks for removal (allowing clean up of those out of player view)
         * at the end of each process loop. Those chunks subject to removal will have
         * been removed already, and those to keep will have been skipped.
         */
        private void ResetChunks()
        {
            foreach (var key in _chunksReady.Keys)
            {
                var chunk = _chunksReady[key];
                chunk.DoRemove = true;
            }
        }
    }
}