using System;
using UnityEngine;
using UnityEngine.UI;

namespace com.ethnicthv.Game.Gameplay
{
    public class AnimatedInOutController : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private Animator animator;
        [SerializeField] private Image cover;
        
        private static readonly int In = Animator.StringToHash("In");
        private static readonly int Out = Animator.StringToHash("Out");

        private Action _onComplete;

        public void AnimatedIn(Color coverColor, Action onComplete = null)
        {
            _onComplete = onComplete;
            var c = coverColor;
            c.a = 1;
            cover.color = c;
            
            cover.gameObject.SetActive(true);
            
            animator.SetBool(Out, false);
            animator.SetBool(In, true);
        }
        
        public void AnimatedOut(Action onComplete = null)
        {
            _onComplete = onComplete;
            var c = cover.color;
            c.a = 0;
            cover.color = c;
            
            cover.gameObject.SetActive(true);
            
            animator.SetBool(In, false);
            animator.SetBool(Out, true);
        }

        public void OnAnimatedOutComplete()
        {
            _onComplete?.Invoke();
            cover.gameObject.SetActive(false);
        }
        
        public void OnAnimatedInComplete()
        {
            cover.gameObject.SetActive(false);
            _onComplete?.Invoke();
        }
    }
}