using System;
using com.ethnicthv.Game.Data;
using UnityEngine;

namespace com.ethnicthv.Game.Gameplay
{
    public class GameControlTutorialController : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private GameObject tapTutorial;
        [SerializeField] private GameObject rotateTutorial;
        [SerializeField] private GameObject mzTutorial;
        
        private GameInternalSetting.TutorialType _currentTutorialType;

        public void Setup(int category, int currentLevel)
        {
            _currentTutorialType = GameInternalSetting.GetTutorialType(category, currentLevel);
        }
        
        public void ShowTutorial()
        {
            switch (_currentTutorialType)
            {
                case GameInternalSetting.TutorialType.Tap:
                    tapTutorial.SetActive(true);
                    break;
                case GameInternalSetting.TutorialType.Rotate:
                    rotateTutorial.SetActive(true);
                    break;
                case GameInternalSetting.TutorialType.Move2Zoom:
                    mzTutorial.SetActive(true);
                    break;
                case GameInternalSetting.TutorialType.None:
                default:
                    break;
            }
        }

        public void HideTapTutorial()
        {
            tapTutorial.SetActive(false);
        }

        public void HideRotateTutorial()
        {
            rotateTutorial.SetActive(false);
        }
        
        public void HideMzTutorial()
        {
            mzTutorial.SetActive(false);
        }
    }
}