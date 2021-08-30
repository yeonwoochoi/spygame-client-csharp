using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Util
{
    public static class Utils
    {
        public static void Visible(this CanvasGroup c, bool flag = true)
        {
            c.alpha = flag ? 1 : 0;
            c.interactable = flag;
            c.blocksRaycasts = flag;
        }
    }
}