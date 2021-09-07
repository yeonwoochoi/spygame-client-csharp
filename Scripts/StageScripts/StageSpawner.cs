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

        [SerializeField] private GameObject[] stagePrefabs;
        [SerializeField] private UnityEngine.Camera camera;
        [SerializeField] private Transform stageParent;
        [SerializeField] private Transform spyParent;
        [SerializeField] private Transform boxParent;
        [SerializeField] private JoystickMoveController joystickMoveController;

        private ChapterType currentChapterType;
        private PseudoStageInfo currentStageInfo;
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

        #region Getter

        private GameObject GetStagePrefab()
        {
            var chapterIndex = (int) currentChapterType;
            var stageIndex = (int) currentStageInfo.stageType;
            var index = chapterIndex * 6 + stageIndex;
            return stagePrefabs[index];
        }

        #endregion

        #region Private Method

        private void SetStage()
        {
            currentChapterType = LoadingManager.Instance.chapterType;
            currentStageInfo = PseudoChapter.Instance.GetStageInfo(currentChapterType, LoadingManager.Instance.stageType);
            
            stageStateController.SetStageState();
            
            //TODO (stage prefab) : 하드 코딩이니 서버 생기면 수정 
            var stageObj = Instantiate(GetStagePrefab(), Vector3.zero, Quaternion.identity);
            stageObj.transform.SetParent(stageParent);
            
            var stageSceneController = stageObj.GetComponent<StageSceneController>();
            stageSceneController.SetStageObjParent(stageParent, spyParent, boxParent);
            stageSceneController.SetCurrentStage(currentStageInfo, joystickMoveController, eControlType, camera);
        }

        #endregion
    }
}