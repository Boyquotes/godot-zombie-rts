﻿namespace UnityEngine.U2D.Animation.TriangleNet
    .Voronoi
{
    using System;
    using Animation.TriangleNet.Topology.DCEL;

    /// <summary>
    /// Default factory for Voronoi / DCEL mesh objects.
    /// </summary>
    internal class DefaultVoronoiFactory : IVoronoiFactory
    {
        public void Initialize(int vertexCount, int edgeCount, int faceCount)
        {
        }

        public void Reset()
        {
        }

        public Vertex CreateVertex(double x, double y)
        {
            return new Vertex(x, y);
        }

        public HalfEdge CreateHalfEdge(Vertex origin, Face face)
        {
            return new HalfEdge(origin, face);
        }

        public Face CreateFace(Geometry.Vertex vertex)
        {
            return new Face(vertex);
        }
    }
}
