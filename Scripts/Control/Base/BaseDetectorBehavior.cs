using Domain;
using Manager;
using Manager.Data;
using UnityEngine;

namespace Control.Base
{
    public abstract class BaseDetectorBehavior: MonoBehaviour
    {
        protected EControlType eControlType;
        protected bool isSet = false;

        private void Start()
        {
            InitDetector();
        }

        protected T GetParentController<T>()
        {
            eControlType = GlobalDataManager.Instance.Get<EControlManager>(GlobalDataKey.ECONTROL).eControlType;
            return GetComponentInParent<T>();
        }
        
        protected virtual void InitDetector() {}
    }
}