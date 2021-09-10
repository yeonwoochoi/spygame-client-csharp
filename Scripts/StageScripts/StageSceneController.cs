using Control.Movement;
using Domain;
using Manager;
using Manager.Data;
using UnityEngine;

namespace StageScripts
{
    public class StageSceneController: MonoBehaviour
    {
        #region Private Variables

        [SerializeField] private GameObject[] stagePrefabs;
        [SerializeField] private UnityEngine.Camera camera;
        [SerializeField] private Transform stageParent;
        [SerializeField] private Transform spyParent;
        [SerializeField] private Transform boxParent;
        [SerializeField] private JoystickMoveController joystickMoveController;

        private EControlType eControlType;

        #endregion

        #region Event Method

        private void Start()
        {
            eControlType = GlobalDataManager.Instance.Get<EControlManager>(GlobalDataKey.ECONTROL).eControlType;
            SetStage();
        }

        #endregion

        #region Getter

        private GameObject GetStagePrefab()
        {
            var chapterIndex = (int) LoadingManager.Instance.chapterType;
            var stageIndex = (int) LoadingManager.Instance.stageType;
            var index = chapterIndex * 6 + stageIndex;
            return stagePrefabs[index];
        }

        #endregion

        #region Private Method

        private void SetStage()
        {
            //TODO (stage prefab) : 하드 코딩이니 서버 생기면 수정 
            var stageObj = Instantiate(GetStagePrefab(), Vector3.zero, Quaternion.identity);
            stageObj.transform.SetParent(stageParent);
            
            var stageSceneController = stageObj.GetComponent<StageSceneSpawner>();
            stageSceneController.SetStageObjParent(stageParent, spyParent, boxParent);
            stageSceneController.SetCurrentStage(joystickMoveController, eControlType, camera);
        }

        #endregion
    }
}