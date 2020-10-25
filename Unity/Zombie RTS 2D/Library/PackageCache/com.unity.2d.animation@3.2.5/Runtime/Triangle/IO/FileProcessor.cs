﻿// -----------------------------------------------------------------------
// <copyright file="FileProcessor.cs" company="">
// Triangle.NET code by Christian Woltering, http://triangle.codeplex.com/
// </copyright>
// -----------------------------------------------------------------------

namespace UnityEngine.U2D.Animation.TriangleNet
    .IO
{
    using System;
    using System.Collections.Generic;
    using Animation.TriangleNet.Geometry;
    using Animation.TriangleNet.Meshing;

    internal static class FileProcessor
    {
        static List<IFileFormat> formats;

        static FileProcessor()
        {
            formats = new List<IFileFormat>();

            // Add Triangle file format as default.
            formats.Add(new TriangleFormat());
        }

        internal static void Add(IFileFormat format)
        {
            formats.Add(format);
        }

        internal static bool IsSupported(string file)
        {
            foreach (var format in formats)
            {
                if (format.IsSupported(file))
                {
                    return true;
                }
            }

            return false;
        }

        #region Polygon read/write

        /// <summary>
        /// Read a file containing polygon geometry.
        /// </summary>
        /// <param name="filename">The path of the file to read.</param>
        /// <returns>An instance of the <see cref="IPolygon" /> class.</returns>
        internal static IPolygon Read(string filename)
        {
            foreach (IPolygonFormat format in formats)
            {
                if (format != null && format.IsSupported(filename))
                {
                    return format.Read(filename);
                }
            }

            throw new Exception("File format not supported.");
        }

        /// <summary>
        /// Save a polygon geometry to disk.
        /// </summary>
        /// <param name="mesh">An instance of the <see cref="IPolygon" /> class.</param>
        /// <param name="filename">The path of the file to save.</param>
        internal static void Write(IPolygon polygon, string filename)
        {
            foreach (IPolygonFormat format in formats)
            {
                if (format != null && format.IsSupported(filename))
                {
                    format.Write(polygon, filename);
                    return;
                }
            }

            throw new Exception("File format not supported.");
        }

        #endregion

        #region Mesh read/write

        /// <summary>
        /// Read a file containing a mesh.
        /// </summary>
        /// <param name="filename">The path of the file to read.</param>
        /// <returns>An instance of the <see cref="IMesh" /> interface.</returns>
        internal static IMesh Import(string filename)
        {
            foreach (IMeshFormat format in formats)
            {
                if (format != null && format.IsSupported(filename))
                {
                    return format.Import(filename);
                }
            }

            throw new Exception("File format not supported.");
        }

        /// <summary>
        /// Save a mesh to disk.
        /// </summary>
        /// <param name="mesh">An instance of the <see cref="IMesh" /> interface.</param>
        /// <param name="filename">The path of the file to save.</param>
        internal static void Write(IMesh mesh, string filename)
        {
            foreach (IMeshFormat format in formats)
            {
                if (format != null && format.IsSupported(filename))
                {
                    format.Write(mesh, filename);
                    return;
                }
            }

            throw new Exception("File format not supported.");
        }

        #endregion
    }
}
