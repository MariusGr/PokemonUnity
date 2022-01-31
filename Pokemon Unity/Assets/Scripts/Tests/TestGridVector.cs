using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestGridVector
    {
        [Test]
        public void TestGridVectorEquals()
        {
            Assert.AreEqual(new GridVector(-13, -3), new GridVector(-13, -3));
            Assert.AreNotEqual(new GridVector(1, 0), new GridVector(0, 0));
            Assert.AreNotEqual(new GridVector(0, 1), new GridVector(0, 0));
        }
    }
}
