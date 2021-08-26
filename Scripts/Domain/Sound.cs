using System;
using UnityEngine;

namespace Domain
{
    public enum SoundType
    {
        Background, GameOver, StageClear, Timer, Correct, Wrong, Warp, Meet, ItemUse, Explosion
    }

    [Serializable]
    public class Sound
    {
        public SoundType type;
        
        public AudioClip clip;

        public bool isEffect;
        
        [Range(0f, 1f)]
        public float volume;
        
        [Range(.1f, 3f)]
        public float pitch;

        public bool loop;
        
        [HideInInspector] public AudioSource source;
    }

    [Serializable]
    public class SoundManager
    {
        public bool isSoundMute;
        public bool isEffectMute;

        public static SoundManager Create()
        {
            var soundInfo = new SoundManager
            {
                isSoundMute = false,
                isEffectMute = false
            };
            return soundInfo;
        }
    }
}