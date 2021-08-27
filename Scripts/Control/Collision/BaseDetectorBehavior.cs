﻿using System;
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

        private void Start()
        {
            InitDetector();
        }

        protected virtual T GetParentController<T>()
        {
            eControlType = GlobalDataManager.Instance.Get<EControlManager>(GlobalDataKey.ECONTROL).eControlType;
            return GetComponentInParent<T>();
        }
        
        protected virtual void InitDetector() {}
    }
}