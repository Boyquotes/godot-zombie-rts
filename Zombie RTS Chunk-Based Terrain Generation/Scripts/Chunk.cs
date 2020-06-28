using Godot;

namespace ZombieRTSChunkBasedTerrainGeneration.Scripts
{
    public class Chunk : Spatial
    {
        private MeshInstance _meshInstance; // Terrain on the chunk
        private readonly OpenSimplexNoise _noise;
        private readonly float _x; // (x,z) location on the noise map for the chunk data
        private readonly float _z;
        private readonly int _chunkSize;
        private const int HeightMultiplier = 80;


        /**
         * Initialise a new chunk
         */
        public Chunk()
        {
        }

        public Chunk(OpenSimplexNoise noise, float x, float z, int chunkSize)
        {
            _noise = noise;
            _x = x;
            _z = z;
            _chunkSize = chunkSize;
        }


        /**
         * Called when the node enters the scene tree for the first time.
         */
        public override void _Ready()
        {
            GenerateChunk();
        }


        /**
         * Called every frame. 'delta' is the elapsed time since the previous frame.
         */
        public override void _Process(float delta)
        {
        }


        /**
         * 
         */
        private void GenerateChunk()
        {
            var planeMesh = new PlaneMesh
            {
                Size = new Vector2(_chunkSize, _chunkSize),
                SubdivideDepth = (int) (_chunkSize * 0.5),
                SubdivideWidth = (int) (_chunkSize * 0.5)
            };

            // Todo: Give plane mesh a material...

            var surfaceTool = new SurfaceTool();
            var dataTool = new MeshDataTool();

            /* Create mesh using the plane and extract the array mesh from it,
             * use data tool for manipulating each vertex */
            surfaceTool.CreateFrom(planeMesh, 0);
            var arrayPlane = surfaceTool.Commit();
            var error = dataTool.CreateFromSurface(arrayPlane, 0);

            // Set the height (y) of each vertex using noise values
            for (var i = 0; i < dataTool.GetVertexCount(); i++)
            {
                var vertex = dataTool.GetVertex(i);
                vertex.y = _noise.GetNoise3d(vertex.x + _x, vertex.y, vertex.z + _z) * HeightMultiplier;
                dataTool.SetVertex(i, vertex);
            }

            // Clear old mesh before creating the new one
            for (var s = 0; s < arrayPlane.GetSurfaceCount(); s++)
            {
                arrayPlane.SurfaceRemove(s);
            }

            // Create the new mesh
            dataTool.CommitToSurface(arrayPlane);
            surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
            surfaceTool.CreateFrom(arrayPlane, 0);
            surfaceTool.GenerateNormals();

            _meshInstance = new MeshInstance {Mesh = surfaceTool.Commit()};
            _meshInstance.CreateTrimeshCollision();
            _meshInstance.CastShadow = GeometryInstance.ShadowCastingSetting.Off;
        }
    }
}