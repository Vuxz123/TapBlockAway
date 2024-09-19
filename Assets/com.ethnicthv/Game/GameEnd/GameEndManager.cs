using com.ethnicthv.Game.Gameplay;
using UnityEngine;

namespace com.ethnicthv.Game.GameEnd
{
    public class GameEndManager : MonoBehaviour
    {
        public static GameEndManager instance { get; private set; }
        
        private void Awake()
        {
            instance = this;
        }
        
        public void ShowGameEnd(GamePlayManager gamePlayManager)
        {
            
        }
    }
}