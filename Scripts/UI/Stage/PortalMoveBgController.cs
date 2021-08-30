using System.Collections;
using Control.Portal;
using Event;
using UnityEngine;
using Util;

namespace UI.Stage
{
    public class PortalMoveBgController: MonoBehaviour
    {
        [SerializeField] private CanvasGroup cGroup;

        private void Start()
        {
            PortalController.PortalMoveEvent += PortalMovePlayer;
        }

        private void OnDisable()
        {
            PortalController.PortalMoveEvent -= PortalMovePlayer;
        }

        private void PortalMovePlayer(object _, PortalMoveEventArgs e)
        {
            StartCoroutine(MovePlayer(e.player, e.targetPos));
        }

        private IEnumerator MovePlayer(Transform player, Vector3 targetPos)
        {
            var secondsToFade = 0.5f;
            var startValue = 0;
            var rate = 1.0f / secondsToFade;
 
            for (var x = 0.0f; x <= 1.0f; x += Time.deltaTime * rate) {
                cGroup.alpha = Mathf.Lerp(startValue, 1, x);
                yield return null;
            }
            cGroup.Visible();
            player.position = targetPos;
            yield return new WaitForSeconds(1f);
            for (var x = 0.0f; x <= 1.0f; x += Time.deltaTime * rate) {
                cGroup.alpha = Mathf.Lerp(1, startValue, x);
                yield return null;
            }
            
            cGroup.Visible(false);
        }
    }
}