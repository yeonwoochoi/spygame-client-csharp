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

        private bool isLoaded = false;
        
        private void Start()
        {
            LoadFrontAd();
            SetTestAd();
            StageStateController.StageDoneEvent += ShowFrontAd;
            TimerController.TimeOverEvent += ShowFrontAd;
        }

        private void OnDestroy()
        {
            frontAd.Destroy();
            StageStateController.StageDoneEvent -= ShowFrontAd;
            TimerController.TimeOverEvent -= ShowFrontAd;
        }

        private void ShowFrontAd(object _, ExitStageEventArgs e)
        {
            if (e.exitType == StageExitType.GiveUp) return;
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

            if (!isLoaded) isLoaded = true;
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
            if (!isLoaded) return;
            
            isSoundMute = AudioManager.instance.IsSoundMute;
            isEffectMute = AudioManager.instance.IsEffectMute;

            AudioManager.instance.IsSoundMute = true;
            AudioManager.instance.IsEffectMute = true;
        }

        private void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e) { }


        private void HandleOnAdClosed(object sender, EventArgs e)
        {
            AudioManager.instance.IsSoundMute = isSoundMute;
            AudioManager.instance.IsEffectMute = isEffectMute;
        }
    }
}