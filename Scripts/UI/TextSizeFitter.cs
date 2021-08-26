using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TextSizeFitter: MonoBehaviour
    {
        private Text text;
        private void Start()
        {
            text = GetComponent<Text>();
            
            var viewWidth = Screen.width;
            
            text.fontSize = viewWidth / 30;
        }
    }
}