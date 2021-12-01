using System;
using System.Collections.Generic;
using Domain;
using UnityEngine;

namespace UI.Popup.MainScripts
{
    public enum DynastyType
    {
        Gojoseon,
        Korea,
        Balhae,
        Gogureo,
        Baekjae,
        Shilla,
        Chosun,
    }
    
    public class ChapterSelectPopupImageSelector: MonoBehaviour
    {
        [SerializeField] private Sprite[] chosunSprites;
        [SerializeField] private Sprite[] koreaSprites;
        [SerializeField] private Sprite[] balhaeSprites;
        [SerializeField] private Sprite[] goguryeoSprites;
        [SerializeField] private Sprite[] baekjaeSprites;
        [SerializeField] private Sprite[] shillaSprites;
        [SerializeField] private Sprite[] gojoseonSprites;

        private Dictionary<DynastyType, string> dynastyStringRef;

        private Dictionary<DynastyType, Sprite[]> dynastySpritesRef;
        
        private Dictionary<DynastyType, int> dynastySpritesIndexRef;

        private readonly int dynastyCount = 7;

        public void Init()
        {
            dynastyStringRef = new Dictionary<DynastyType, string>
            {
                [DynastyType.Gojoseon] = "고조선",
                [DynastyType.Gogureo] = "고구려",
                [DynastyType.Baekjae] = "백제",
                [DynastyType.Shilla] = "신라",
                [DynastyType.Balhae] = "발해",
                [DynastyType.Korea] = "고려",
                [DynastyType.Chosun] = "조선",
            }; 
            
            dynastySpritesRef = new Dictionary<DynastyType, Sprite[]>
            {
                [DynastyType.Gojoseon] = gojoseonSprites,
                [DynastyType.Gogureo] = goguryeoSprites,
                [DynastyType.Baekjae] = baekjaeSprites,
                [DynastyType.Shilla] = shillaSprites,
                [DynastyType.Balhae] = balhaeSprites,
                [DynastyType.Korea] = koreaSprites,
                [DynastyType.Chosun] = chosunSprites,
            };

            dynastySpritesIndexRef = new Dictionary<DynastyType, int>
            {
                [DynastyType.Gojoseon] = 0,
                [DynastyType.Gogureo] = 0,
                [DynastyType.Baekjae] = 0,
                [DynastyType.Shilla] = 0,
                [DynastyType.Balhae] = 0,
                [DynastyType.Korea] = 0,
                [DynastyType.Chosun] = 0,
            };
        }

        public Sprite GetRandomImg(ChapterInfo chapterInfo)
        {
            var title = chapterInfo.title;
            for (var i = 0; i < dynastyCount; i++)
            {
                var type = (DynastyType) i;
                if (title.Contains(dynastyStringRef[type]))
                {
                    var index = dynastySpritesIndexRef[type];
                    if (index > dynastySpritesRef[type].Length - 1)
                    {
                        index = dynastySpritesRef[type].Length - 1;
                    }
                    dynastySpritesIndexRef[type] = index + 1;
                    return dynastySpritesRef[type][index];
                }
            }
            return dynastySpritesRef[DynastyType.Chosun][0];
        } 
    }
}