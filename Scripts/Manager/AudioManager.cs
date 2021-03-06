using System;
using Domain;
using Manager.Data;
using UnityEngine;
using UnityEngine.Audio;

namespace Manager
{
    public class AudioManager: MonoBehaviour
    {
        #region Private Variables

        [SerializeField] private Sound[] sounds;

        private SoundManager soundManager;
        private bool isSoundMute;
        private bool isEffectMute;

        #endregion

        #region Static Variable

        public static AudioManager instance = null;

        #endregion
        
        #region Getter

        public bool GetIsSoundMute()
        {
            return isSoundMute;
        }

        public bool GetIsEffectMute()
        {
            return isEffectMute;
        }

        #endregion

        #region Setter

        public void SetIsSoundMute(bool flag)
        {
            isSoundMute = flag;
            foreach (var sound in sounds)
            {
                if (sound.isEffect) continue;
                sound.source.volume = isSoundMute ? 0 : 1;
                soundManager.isSoundMute = isSoundMute;
                GlobalDataManager.Instance.Set(GlobalDataKey.SOUND, soundManager);
            }
        }

        public void SetIsEffectMute(bool flag)
        {
            isEffectMute = flag;
            foreach (var sound in sounds)
            {
                if (!sound.isEffect) continue;
                sound.source.volume = isEffectMute ? 0 : 1;
                soundManager.isEffectMute = isEffectMute;
                GlobalDataManager.Instance.Set(GlobalDataKey.SOUND, soundManager);
            }
        }

        #endregion

        #region Event Methods

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            DontDestroyOnLoad(gameObject);

            foreach (var sound in sounds)
            {
                sound.source = gameObject.AddComponent<AudioSource>();
                sound.source.clip = sound.clip;
                sound.source.volume = sound.volume;
                sound.source.pitch = sound.pitch;
                sound.source.loop = sound.loop;
            }

            // Set global sound mute setting
            soundManager = GlobalDataManager.Instance.Get<SoundManager>(GlobalDataKey.SOUND);
            if (soundManager == null)
            {
                soundManager = SoundManager.Create();
                GlobalDataManager.Instance.Set(GlobalDataKey.SOUND, soundManager);
            }
            
            SetIsSoundMute(soundManager.isSoundMute);
            SetIsEffectMute(soundManager.isEffectMute);
        }

        #endregion

        #region Public Methods

        public void Play(SoundType soundType)
        {
            var sound = Array.Find(sounds, s => s.type == soundType);
            sound?.source.Play();
        }
        
        public void Stop(SoundType soundType)
        {
            var sound = Array.Find(sounds, s => s.type == soundType);
            sound?.source.Stop();
        }

        public void StopAll()
        {
            foreach (var sound in sounds)
            {
                sound.source.Stop();
            }
        }

        #endregion
    }
}