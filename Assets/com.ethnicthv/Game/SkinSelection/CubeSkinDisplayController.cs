using com.ethnicthv.Game.Cube;
using DG.Tweening;
using UnityEngine;

namespace com.ethnicthv.Game.Home
{
    public class CubeSkinDisplayController : CubeController
    {
        [Header("Setup")]
        public Transform displayTransform;
        
        public void Display()
        {
            displayTransform.DORotate(new Vector3(0, 360, 0), 1, RotateMode.FastBeyond360).SetEase(Ease.OutCubic);
            displayTransform.DOLocalMoveY(1, 1).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                displayTransform.DOLocalMoveY(0.8f, 1)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            });
        }
        
        public new void Reset() {
            Debug.Log("Reset");
            displayTransform.DOKill();
            displayTransform.localRotation = Quaternion.identity;
            displayTransform.DOLocalMoveY(0, 0.5f).SetEase(Ease.OutCubic);
        }
    }
}