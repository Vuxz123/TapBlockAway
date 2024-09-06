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

        public void AnimatedIn(Color coverColor)
        {
            var c = coverColor;
            c.a = 1;
            cover.color = c;
            
            cover.gameObject.SetActive(true);
            
            animator.SetTrigger(In);
        }
        
        public void AnimatedOut()
        {
            var c = cover.color;
            c.a = 0;
            cover.color = c;
            
            cover.gameObject.SetActive(true);
            
            animator.SetTrigger(Out);
        }

        public void OnAnimatedOutComplete()
        {
            cover.gameObject.SetActive(false);
        }
        
        public void OnAnimatedInComplete()
        {
            cover.gameObject.SetActive(false);
        }
    }
}