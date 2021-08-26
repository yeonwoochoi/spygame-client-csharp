using System;
using System.Collections;
using System.Collections.Generic;
using Domain;
using Event;
using UnityEngine;
using GoogleMobileAds.Api;
using Manager.Data;
using StageScripts;
using UI.Stage;
using UnityEngine.UI;


namespace Manager
{
    public class AdmobManager: MonoBehaviour
    {
        private const string frontID = "ca-app-pub-3940256099942544/8691691433";
        private InterstitialAd frontAd;

        private bool isSoundMute;
        private bool isEffectMute;

        private bool isStageClear = false;

        /*
        public static event EventHandler<StageClearEventArgs> CloseStageAdEvent;
        public static event EventHandler<GameOverEventArgs> CloseGameOverAdEvent;
        */
        
        private void Start()
        {
            LoadFrontAd();
            SetTestAd();
            StageStateController.StageClearEvent += ShowFrontAd;
            StageStateController.GameOverEvent += ShowFrontAd;
            TimerController.TimeOverEvent += ShowFrontAd;
        }

        private void OnDestroy()
        {
            frontAd.Destroy();
            StageStateController.StageClearEvent -= ShowFrontAd;
            StageStateController.GameOverEvent -= ShowFrontAd;
            TimerController.TimeOverEvent -= ShowFrontAd;
        }

        private void ShowFrontAd(object _, StageClearEventArgs e)
        {
            isStageClear = true;
            StartCoroutine(ShowFrontAd());
        }
        private void ShowFrontAd(object _, GameOverEventArgs e)
        {
            isStageClear = false;
            StartCoroutine(ShowFrontAd());
        }

        private void SetTestAd()
        {
            MobileAds.Initialize(initStatus => { });
            const string testDeviceId = "3EB9646190EF6C44";
            var deviceIds = new List<string>();
            deviceIds.Add(testDeviceId);
            
            var requestConfiguration = new RequestConfiguration
                    .Builder()
                    .SetTestDeviceIds(deviceIds)
                    .build();
            
            MobileAds.SetRequestConfiguration(requestConfiguration);
        }

        private void LoadFrontAd()
        {
            frontAd = new InterstitialAd(frontID);

            frontAd.OnAdLoaded += HandleOnAdLoaded;

            frontAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;

            frontAd.OnAdClosed += HandleOnAdClosed;

            frontAd.LoadAd(GetAdRequest());
        }
        
        private AdRequest GetAdRequest()
        {
            return new AdRequest.Builder().Build();
        }

        private IEnumerator ShowFrontAd()
        {
            while (!frontAd.IsLoaded())
            {
                yield return null;
            }
            
            frontAd.Show();
            LoadFrontAd();
        }

        private void HandleOnAdLoaded(object sender, EventArgs e)
        {
            var soundManager = GlobalDataManager.Instance.Get<SoundManager>(GlobalDataKey.SOUND);
            isSoundMute = soundManager.isSoundMute;
            isEffectMute = soundManager.isEffectMute;

            AudioManager.instance.IsSoundMute = false;
            AudioManager.instance.IsEffectMute = false;
        }

        private void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            
        }


        private void HandleOnAdClosed(object sender, EventArgs e)
        {
            AudioManager.instance.IsSoundMute = isSoundMute;
            AudioManager.instance.IsEffectMute = isEffectMute;
            /*
            if (isStageClear) EmitCloseStageAdEvent(new StageClearEventArgs());
            else EmitCloseStageAdEvent(new GameOverEventArgs());
            */
        }

        /*
        private void EmitCloseStageAdEvent(StageClearEventArgs e)
        {
            if (CloseStageAdEvent == null) return;
            foreach (var invocation in CloseStageAdEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }
        
        private void EmitCloseStageAdEvent(GameOverEventArgs e)
        {
            if (CloseGameOverAdEvent == null) return;
            foreach (var invocation in CloseGameOverAdEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }
        */
    }
}