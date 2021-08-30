using System;
using UnityEngine;

namespace Domain
{
    #region Enum

    public enum SoundType
    {
        Background, GameOver, StageClear, Timer, Correct, Wrong, Warp, Meet, ItemUse, Explosion
    }

    #endregion

    [Serializable]
    public class Sound
    {
        #region Public Variables

        public SoundType type;
        
        public AudioClip clip;

        public bool isEffect;
        
        [Range(0f, 1f)]
        public float volume;
        
        [Range(.1f, 3f)]
        public float pitch;

        public bool loop;
        
        [HideInInspector] public AudioSource source;

        #endregion
    }

    [Serializable]
    public class SoundManager
    {
        #region Public Variables

        public bool isSoundMute;
        public bool isEffectMute;

        #endregion

        #region Static Method

        public static SoundManager Create()
        {
            var soundInfo = new SoundManager
            {
                isSoundMute = false,
                isEffectMute = false
            };
            return soundInfo;
        }

        #endregion
    }
}