using System;
using DG.Tweening;
using UnityEngine;

namespace com.ethnicthv.Game.GameEnd
{
    public class BgHighlightController : MonoBehaviour
    {
        private void OnEnable()
        {
            transform.localRotation = Quaternion.identity;
            transform.DORotate(new Vector3(0,0,360), 5f, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.Linear);
        }
        
        private void OnDisable()
        {
            transform.DOKill();
        }
    }
}