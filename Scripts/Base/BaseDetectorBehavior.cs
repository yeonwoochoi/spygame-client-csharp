using Domain;
using Manager;
using Manager.Data;
using UnityEngine;

namespace Base
{
    public abstract class BaseDetectorBehavior: MonoBehaviour
    {
        #region Protected Variables

        protected bool isSet = false;
        protected EControlType eControlType;

        #endregion

        #region Event Method

        protected virtual void Start()
        {
            InitDetector();
        }

        #endregion

        #region Protected Methods

        protected T GetParentController<T>()
        {
            eControlType = GlobalDataManager.Instance.Get<EControlManager>(GlobalDataKey.ECONTROL).eControlType;
            return GetComponentInParent<T>();
        }
        
        protected virtual void InitDetector() {}

        #endregion
    }
}