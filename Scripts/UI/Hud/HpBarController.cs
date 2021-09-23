using Event;
using StageScripts;
using TutorialScripts;
using UI.Popup.Qna;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Hud
{
    public class HpBarController: MonoBehaviour
    {
        [SerializeField] private Slider hpSlider;
        [SerializeField] private Gradient hpGradient;
        [SerializeField] private Image fill;

        private void Start()
        {
            SetMaxHp();
            StageStateController.UpdateStageStateEvent += SetHealth;
            TutorialStateController.UpdateTutorialStateEvent += SetHealth;
        }

        private void OnDisable()
        {
            StageStateController.UpdateStageStateEvent -= SetHealth;
            TutorialStateController.UpdateTutorialStateEvent -= SetHealth;
        }
        
        private void SetMaxHp()
        {
            hpSlider.maxValue = StageStateController.PlayerHp;
            hpSlider.value = StageStateController.PlayerHp;

            fill.color = hpGradient.Evaluate(1f);
        }

        private void SetHealth(object _, UpdateStageStateEventArgs e)
        {
            hpSlider.value = e.hp;
            fill.color = hpGradient.Evaluate(hpSlider.normalizedValue);
        }
    }
}