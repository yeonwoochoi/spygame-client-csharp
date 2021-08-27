using System;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public static class NodeUtils
    {
        public static bool IsSamePosition(this Vector3 position, Vector3 comparison)
        {
            return Math.Abs(position.x - comparison.x) < 0.05f && Math.Abs(position.y - comparison.y) < 0.05f;
        }
    }
}