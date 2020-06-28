using Godot;

namespace ZombieRTSChunkBasedTerrainGeneration.Scripts
{
    public class Chunk : Spatial
    {
        private MeshInstance _meshInstance; // Terrain on the chunk
        private OpenSimplexNoise _noise;
        private float _x; // (x,z) location on the noise map for the chunk data
        private float _y;
        private int _chunkSize;


        /**
         * Initialise a new chunk
         */
        public Chunk(OpenSimplexNoise noise, float x, float y, int chunkSize)
        {
            _noise = noise;
            _x = x;
            _y = y;
            _chunkSize = chunkSize;
        }

        
        /**
         * Called when the node enters the scene tree for the first time.
         */
        public override void _Ready()
        {
        }


        /**
         * Called every frame. 'delta' is the elapsed time since the previous frame.
         */
        public override void _Process(float delta)
        {
        }
    }
}