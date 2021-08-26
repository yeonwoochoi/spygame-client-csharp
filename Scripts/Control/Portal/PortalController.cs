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
        [SerializeField] private Transform afterPortal;

        public static event EventHandler<PortalMoveEventArgs> PortalMoveEvent; 
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag != "Player") return;
            AudioManager.instance.Play(SoundType.Warp);
            EmitPortalMoveEvent(new PortalMoveEventArgs(other.gameObject.transform, afterPortal.position));
            other.gameObject.GetComponent<PlayerMoveController>().StopMove();
        }

        private void EmitPortalMoveEvent(PortalMoveEventArgs e)
        {
            if (PortalMoveEvent == null) return;
            foreach (var invocation in PortalMoveEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }
    }
}