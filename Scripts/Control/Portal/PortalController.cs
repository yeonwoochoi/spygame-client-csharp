using System;
using System.Collections;
using Control.Movement;
using Domain;
using Event;
using Manager;
using UnityEngine;
using Util;

namespace Control.Portal
{
    public class PortalController: MonoBehaviour
    {
        #region Private Variable

        [SerializeField] private Transform afterPortal;

        #endregion

        #region Event
        public static event EventHandler<PortalMoveEventArgs> PortalMoveEvent;

        #endregion

        #region Event Method

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag != "Player") return;
            AudioManager.instance.Play(SoundType.Warp);
            EmitPortalMoveEvent(new PortalMoveEventArgs(other.gameObject.transform, afterPortal.position));
            other.gameObject.GetComponent<PlayerMoveController>().StopMove();
        }

        #endregion

        #region Private Method

        // TODO()
        private void EmitPortalMoveEvent(PortalMoveEventArgs e)
        {
            if (PortalMoveEvent == null) return;
            foreach (var invocation in PortalMoveEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }

        #endregion
    }
}