﻿using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using UnityEngine.TestTools;

namespace UnityEditor.TestTools.TestRunner
{
    interface ITestListProvider
    {
        IEnumerator<ITest> GetTestListAsync(TestPlatform platform);
    }
}
