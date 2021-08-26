using System;
using Domain;
using UI.Chapter;
using UnityEngine;
using UnityEngine.UI;

namespace ChapterScripts
{
    public class ChapterMapController: MonoBehaviour
    {
        [SerializeField] private GameObject stage1Button;
        [SerializeField] private GameObject stage2Button;
        [SerializeField] private GameObject stage3Button;
        [SerializeField] private GameObject stage4Button;
        [SerializeField] private GameObject stage5Button;
        [SerializeField] private GameObject stage6Button;
        
        public void SetButtonScore(Func<StageType, int> getScore)
        {
            stage1Button.GetComponent<StageSpotButtonController>().StageScore = getScore(StageType.Stage1);
            stage2Button.GetComponent<StageSpotButtonController>().StageScore = getScore(StageType.Stage2);
            stage3Button.GetComponent<StageSpotButtonController>().StageScore = getScore(StageType.Stage3);
            stage4Button.GetComponent<StageSpotButtonController>().StageScore = getScore(StageType.Stage4);
            stage5Button.GetComponent<StageSpotButtonController>().StageScore = getScore(StageType.Stage5);
            stage6Button.GetComponent<StageSpotButtonController>().StageScore = getScore(StageType.Stage6);
        }

        public void SetStageButtonEvent(Action<StageType> openStageReadyPopupAction)
        {
            stage1Button.GetComponent<Button>().onClick.AddListener(() =>
            {
                openStageReadyPopupAction.Invoke(StageType.Stage1);
            });
            
            stage2Button.GetComponent<Button>().onClick.AddListener(() =>
            {
                openStageReadyPopupAction.Invoke(StageType.Stage2);
            });
            
            stage3Button.GetComponent<Button>().onClick.AddListener(() =>
            {
                openStageReadyPopupAction.Invoke(StageType.Stage3);
            });
            
            stage4Button.GetComponent<Button>().onClick.AddListener(() =>
            {
                openStageReadyPopupAction.Invoke(StageType.Stage4);
            });
            
            stage5Button.GetComponent<Button>().onClick.AddListener(() =>
            {
                openStageReadyPopupAction.Invoke(StageType.Stage5);
            });
            
            stage6Button.GetComponent<Button>().onClick.AddListener(() =>
            {
                openStageReadyPopupAction.Invoke(StageType.Stage6);
            });
        }   
    }
}