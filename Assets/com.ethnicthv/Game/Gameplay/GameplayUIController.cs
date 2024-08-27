using com.ethnicthv.Game.LevelSelection;
using TMPro;
using UnityEngine;

namespace com.ethnicthv.Game.Gameplay
{
    public class GameplayUIController : MonoBehaviour
    {
        [Header("Setup")] 
        [Header("Title", order = 2)]
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject gameOverMenu;
        [SerializeField] private GameObject gameWinMenu;

        public void UpdateLevelText(int level)
        {
            levelText.text = $"Level {level}";
        }
        
    }
}