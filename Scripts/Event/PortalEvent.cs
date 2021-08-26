using System;
using UnityEngine;

namespace Event
{
    public class PortalMoveEventArgs : EventArgs
    {
        public Transform player;
        public Vector3 targetPos;

        public PortalMoveEventArgs(Transform player, Vector3 targetPos)
        {
            this.player = player;
            this.targetPos = targetPos;
        }
    }
}