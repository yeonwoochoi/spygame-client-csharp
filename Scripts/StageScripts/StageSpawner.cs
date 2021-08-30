using Control.Movement;
using Domain;
using Manager;
using Manager.Data;
using UnityEngine;

namespace StageScripts
{
    public class StageSpawner: MonoBehaviour
    {
        #region Private Variables

        [SerializeField] private Transform stageParent;
        [SerializeField] private Transform spyParent;
        [SerializeField] private Transform boxParent;
        [SerializeField] private JoystickMoveController joystickMoveController;
        private Stage currentStage;
        private StageStateController stageStateController;
        
        private EControlType eControlType;

        #endregion

        #region Event Method

        private void Start()
        {
            eControlType = GlobalDataManager.Instance.Get<EControlManager>(GlobalDataKey.ECONTROL).eControlType;
            stageStateController = GetComponent<StageStateController>();
            SetStage();
        }

        #endregion

        #region Private Method

        private void SetStage()
        {
            currentStage = LoadingManager.Instance.stage;
            stageStateController.currentStage = currentStage;
            stageStateController.SetStageState();
            var stageObj = Instantiate(currentStage.stagePrefab, Vector3.zero, Quaternion.identity);
            stageObj.transform.SetParent(stageParent);
            var stageSceneController = stageObj.GetComponent<StageSceneController>();
            stageSceneController.qna = LoadingManager.Instance.qna;
            stageSceneController.SetStageObjParent(stageParent, spyParent, boxParent);
            stageSceneController.SetCurrentStage(currentStage, joystickMoveController, eControlType);
        }

        #endregion
    }
}