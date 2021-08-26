using System;
using Domain;
using Manager.Data;
using UnityEngine;
using UnityEngine.Audio;

namespace Manager
{
    public class AudioManager: MonoBehaviour
    {
        [SerializeField] private Sound[] sounds;

        private SoundManager soundManager;

        private bool isSoundMute;
        private bool isEffectMute;

        public bool IsSoundMute
        {
            get => isSoundMute;
            set
            {
                isSoundMute = value;
                foreach (var sound in sounds)
                {
                    if (sound.isEffect) continue;
                    sound.source.volume = isSoundMute ? 0 : 1;
                    soundManager.isSoundMute = isSoundMute;
                    GlobalDataManager.Instance.Set(GlobalDataKey.SOUND, soundManager);
                }
            }
        }

        public bool IsEffectMute
        {
            get => isEffectMute;
            set
            {
                isEffectMute = value;
                foreach (var sound in sounds)
                {
                    if (!sound.isEffect) continue;
                    sound.source.volume = isEffectMute ? 0 : 1;
                    soundManager.isEffectMute = isEffectMute;
                    GlobalDataManager.Instance.Set(GlobalDataKey.SOUND, soundManager);
                }
            }
        }

        public static AudioManager instance;
        private void Awake()
        {
            if (instance == null) instance = this;
            else
            {
                Destroy(gameObject);
                return;
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
                var initSoundInfo = SoundManager.Create();
                GlobalDataManager.Instance.Set(GlobalDataKey.SOUND, initSoundInfo);
            }
            
            IsSoundMute = GlobalDataManager.Instance.Get<SoundManager>(GlobalDataKey.SOUND).isSoundMute;
            IsEffectMute = GlobalDataManager.Instance.Get<SoundManager>(GlobalDataKey.SOUND).isEffectMute;
        }

        private void Start()
        {
            Play(SoundType.Background);
        }

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
    }
}