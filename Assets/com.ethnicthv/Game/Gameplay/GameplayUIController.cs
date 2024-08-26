using UnityEngine;

namespace com.ethnicthv.Game.Gameplay
{
    public class GameplayUIController : MonoBehaviour
    {
        [Header("Setup")] 
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject gameOverMenu;
        [SerializeField] private GameObject gameWinMenu;

        private void Awake()
        {
            pauseMenu.SetActive(false);
            gameOverMenu.SetActive(false);
            gameWinMenu.SetActive(false);
        }

        public void ShowPauseMenu()
        {
            pauseMenu.SetActive(true);
        }

        public void HidePauseMenu()
        {
            pauseMenu.SetActive(false);
        }

        public void ShowGameOverMenu()
        {
            gameOverMenu.SetActive(true);
        }

        public void HideGameOverMenu()
        {
            gameOverMenu.SetActive(false);
        }

        public void ShowGameWinMenu()
        {
            gameWinMenu.SetActive(true);
        }

        public void HideGameWinMenu()
        {
            gameWinMenu.SetActive(false);
        }
        
    }
}