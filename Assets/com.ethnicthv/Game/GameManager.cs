using System.Collections;
using System.Collections.Generic;
using com.ethnicthv.Game.Gameplay;
using UnityEngine;

namespace com.ethnicthv.Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance { get; private set; }

        [Header("Setup")]
        public GamePlayManager mainManager; // Note: This is a reference to the main game manager

        public ScreenState screenState { get; private set; }
        
        private void Awake()
        {
            Application.targetFrameRate = 60;
            instance = this;
            screenState = ScreenState.GamePlay;
        }
        
        public bool TryChangeState(ScreenState state, out GamePlayManager main)
        {
            Debug.Log("TryChangeState: " + state + " from " + screenState);
            if (state == screenState)
            {
                main = null;
                return false;
            }
            screenState = state;
            main = mainManager;
            return true;
        }
    }

    public enum ScreenState
    {
        SkinSelection,
        LevelSelection,
        GamePlay
    }
}
