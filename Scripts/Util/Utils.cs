using System;
using System.Linq;
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
        
        public static T FindComponentInChildWithTag<T>(this GameObject parent, string tag) 
            where T: Component
        {
            var parentTransform = parent.transform;
            return (from Transform tr in parentTransform where tr.tag == tag select tr.GetComponent<T>()).FirstOrDefault();
        }
    }
}