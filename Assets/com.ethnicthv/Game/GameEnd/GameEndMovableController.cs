using DG.Tweening;
using UnityEngine;

namespace com.ethnicthv.Game.GameEnd
{
    public class GameEndMovableController : MonoBehaviour
    {
        [SerializeField] private RectTransform emoteAnimator;
        [SerializeField] private CoinProgressController coinProgressController;

        public void OnAnimationInEnd()
        {
            StartEmoteAnimation();
            coinProgressController.RunAnimation();
        }
        
        private void StartEmoteAnimation()
        {
            emoteAnimator.DOShakeRotation(1, new Vector3(0, 0, 10), 10, 90);
            emoteAnimator.DOLocalMoveY(20, 1).SetLoops(-1, LoopType.Yoyo);
        }
    }
}