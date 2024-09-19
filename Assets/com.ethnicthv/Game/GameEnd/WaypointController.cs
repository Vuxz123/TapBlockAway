using UnityEngine;

namespace com.ethnicthv.Game.GameEnd
{
    public class WaypointController : MonoBehaviour
    {
        [SerializeField] private GameObject inactive;
        [SerializeField] private GameObject active;
        [SerializeField] private GameObject highlight;
        
        public void SetActive(bool isActive)
        {
            inactive.SetActive(!isActive);
            active.SetActive(isActive);
            highlight?.SetActive(false);
        }
        
        public void SetHighlight(bool isHighlight)
        {
            highlight?.SetActive(isHighlight);
        }
    }
}