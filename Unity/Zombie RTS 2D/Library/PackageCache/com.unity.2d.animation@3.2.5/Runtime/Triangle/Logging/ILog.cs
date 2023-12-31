﻿// -----------------------------------------------------------------------
// <copyright file="ILog.cs" company="">
// Triangle.NET code by Christian Woltering, http://triangle.codeplex.com/
// </copyright>
// -----------------------------------------------------------------------

namespace UnityEngine.U2D.Animation.TriangleNet
    .Logging
{
    using System.Collections.Generic;

    internal enum LogLevel
    {
        Info = 0,
        Warning = 1,
        Error = 2
    }

    /// <summary>
    /// A basic log interface.
    /// </summary>
    internal interface ILog<T> where T : ILogItem
    {
        void Add(T item);
        void Clear();

        void Info(string message);
        void Error(string message, string info);
        void Warning(string message, string info);

        IList<T> Data { get; }

        LogLevel Level { get; }
    }
}
