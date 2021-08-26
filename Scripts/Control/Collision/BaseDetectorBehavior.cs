using Domain;
using Manager;
using Manager.Data;
using UnityEngine;

namespace Control.Collision
{
    public abstract class BaseDetectorBehavior: MonoBehaviour
    {
        protected EControlType eControlType;
        protected bool isSet = false;
        
        protected virtual T SetDetector<T>()
        {
            eControlType = GlobalDataManager.Instance.Get<EControlManager>(GlobalDataKey.ECONTROL).eControlType;
            isSet = true;
            return GetComponentInParent<T>();
        }
    }
}