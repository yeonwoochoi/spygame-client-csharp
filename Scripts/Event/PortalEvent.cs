using System;
using UnityEngine;

namespace Event
{
    public class PortalMoveEventArgs : EventArgs
    {
        #region Public Variables

        public Transform player;
        public Vector3 targetPos;

        #endregion

        #region Constructor

        public PortalMoveEventArgs(Transform player, Vector3 targetPos)
        {
            this.player = player;
            this.targetPos = targetPos;
        }

        #endregion
    }
}