using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.ethnicthv.Game.LevelSelection
{
    public class LevelItemController : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private Button button;
        [SerializeField] private Image icon;
        [SerializeField] private Image border;
        [SerializeField] private Image header;
        [SerializeField] private TextMeshProUGUI headerTitle;
        [SerializeField] private Image playBtn;
        [SerializeField] private TextMeshProUGUI playBtnText;
        
        private int _categoryIndex;
        private int _levelIndex;
        
        private const string CompletedText = "Play Again";
        private const string NotCompletedText = "Play";
        
        public void SetupLevelItem(CategoryController controller, int i, bool unlocked, bool completed)
        {
            _categoryIndex = controller.categoryIndex;
            _levelIndex = i;
            
            i = i + 1;// get the display index
            if (unlocked)
            {
                if (completed)
                {
                    icon.sprite = controller.levelGroupSprites[0];
                    icon.color = Color.white;
                    icon.SetNativeSize();
                    border.color = controller.levelGroupCompletedColor;
                    header.gameObject.SetActive(true);
                    header.color = controller.levelGroupCompletedColor;
                    headerTitle.text = $"{i}";
                    playBtn.gameObject.SetActive(true);
                    playBtn.color = controller.levelGroupCompletedColor;
                    playBtnText.text = CompletedText;
                }
                else
                {
                    icon.sprite = controller.levelGroupSprites[1];
                    icon.color = controller.levelGroupLockedColor;
                    icon.SetNativeSize();
                    border.color = controller.levelGroupNotCompletedColor;
                    header.gameObject.SetActive(true);
                    header.color = controller.levelGroupNotCompletedColor;
                    headerTitle.text = $"{i}";
                    playBtn.gameObject.SetActive(true);
                    playBtn.color = controller.levelGroupNotCompletedColor;
                    playBtnText.text = NotCompletedText;
                }
            }
            else
            {
                button.interactable = false;
                icon.sprite = controller.levelGroupSprites[2];
                icon.color = controller.levelGroupLockedColor;
                icon.SetNativeSize();
                border.color = controller.levelGroupLockedColor;
                header.gameObject.SetActive(false);
                playBtn.gameObject.SetActive(false);
            }
        }
        
        public void OnClick()
        {
            Debug.Log("LevelItemController OnClick: " + _categoryIndex + " " + _levelIndex);
            LevelSelectorManager.instance.OpenLevel(_categoryIndex, _levelIndex);
        }
    }
}