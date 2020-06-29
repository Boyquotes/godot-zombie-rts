using Godot;
using Godot.Collections;

namespace ZombieRTSMapGeneration.Scripts
{
    public class Terrain : Spatial
    {
        private int _width;
        private int _height;
        private Array<Vector3> _vertices;
        private Array<Vector3> _normals;
        private Array<Vector2> _uvs;
        private ArrayMesh _temporaryMesh;
        private Dictionary<Vector2, float> _heightMapData;
        private const int QuadTextureShare = 5; // How many quads share a texture?
        private const int HeightMapIntensityMultiplier = 25;

        /**
         * Called when the node enters the scene tree for the first time.
         */
        public override void _Ready()
        {
            var heightMapImage = new HeightMap(1024,1024, "test0");
            heightMapImage.MakeImage();
            
            
            
            var heightMap = ResourceLoader.Load<Image>("res://HeightMap_IsleOfMan.jpg");

            _width = heightMap.GetWidth();
            _height = heightMap.GetHeight();
            _vertices = new Array<Vector3>();
            _normals = new Array<Vector3>();
            _uvs = new Array<Vector2>();
            _temporaryMesh = new ArrayMesh();
            _heightMapData = new Dictionary<Vector2, float>();

            // Parse height map image
            heightMap.Lock(); // Prevent any external use
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    _heightMapData.Add(new Vector2(x, y), 
                        heightMap.GetPixel(x, y).r * HeightMapIntensityMultiplier);
                }
            }

            heightMap.Unlock();

            // Generate terrain, add all triangle positions to the vertex array
            for (var x = 0; x < _width - 1; x++)
            {
                for (var y = 0; y < _height - 1; y++)
                {
                    MakeQuad(x, y);
                }
            }

            // Use surface tool to generate the mesh
            var surfaceTool = new SurfaceTool();
            surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
            surfaceTool.SetMaterial(ResourceLoader.Load<Material>("res://Materials/Terrain.tres"));

            for (var v = 0; v < _vertices.Count; v++)
            {
                var vertex = _vertices[v];
                surfaceTool.AddColor(new Color(1, 1, 1));
                surfaceTool.AddUv(_uvs[v]);
                surfaceTool.AddNormal(_normals[v]);
                surfaceTool.AddVertex(vertex);
            }

            surfaceTool.Commit(_temporaryMesh);
            GetNode<MeshInstance>("MeshInstance").Mesh = _temporaryMesh;

            // Add collision to the mesh
            GetNode<MeshInstance>("MeshInstance").Mesh.CreateTrimeshShape();
        }


        /**
         * Make the two triangles to create an overall Quad
         */
        private void MakeQuad(int x, int y)
        {
            // Triangle 1
            var vertex1 = new Vector3(x, _heightMapData[new Vector2(x, y)], -y);
            var vertex2 = new Vector3(x, _heightMapData[new Vector2(x, y + 1)], -y - 1);
            var vertex3 = new Vector3(x + 1, _heightMapData[new Vector2(x + 1, y + 1)], -y - 1);

            _vertices.Add(vertex1);
            _vertices.Add(vertex2);
            _vertices.Add(vertex3);

            var face1 = vertex2 - vertex1;
            var face2 = vertex2 - vertex3;
            var normal = face1.Cross(face2);

            // Add each to the normals list
            for (var i = 0; i < 3; i++)
            {
                _normals.Add(normal);
            }

            // Add UV coords for the overall quad to have one texture across it
            _uvs.Add(new Vector2(vertex1.x / QuadTextureShare, -vertex1.z / QuadTextureShare));
            _uvs.Add(new Vector2(vertex2.x / QuadTextureShare, -vertex2.z / QuadTextureShare));
            _uvs.Add(new Vector2(vertex3.x / QuadTextureShare, -vertex3.z / QuadTextureShare));


            /*
             * Triangle 2
             */
            vertex1 = new Vector3(x, _heightMapData[new Vector2(x, y)], -y);
            vertex2 = new Vector3(x + 1, _heightMapData[new Vector2(x + 1, y + 1)], -y - 1);
            vertex3 = new Vector3(x + 1, _heightMapData[new Vector2(x + 1, y)], -y);
            _vertices.Add(vertex1);
            _vertices.Add(vertex2);
            _vertices.Add(vertex3);

            face1 = vertex2 - vertex1;
            face2 = vertex2 - vertex3;
            normal = face1.Cross(face2); // Cross product to get normals for both sides

            // Add each to the normals list
            for (var i = 0; i < 3; i++)
            {
                _normals.Add(normal);
            }

            // Add UV coords for the overall quad to have one texture across it
            _uvs.Add(new Vector2(vertex1.x / QuadTextureShare, -vertex1.z / QuadTextureShare));
            _uvs.Add(new Vector2(vertex2.x / QuadTextureShare, -vertex2.z / QuadTextureShare));
            _uvs.Add(new Vector2(vertex3.x / QuadTextureShare, -vertex3.z / QuadTextureShare));
        }
    }
}