using DwcHyep;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    public int levelCurrent;

    public int numberRePLay;// số lần chơi lại trong 1 level

    public Text textCoin;

    public GameObject mainMenu;

    [SerializeField] private Text topBarLevelText = null;

    public void OnMainPlayGame()
    {
        numberRePLay = 1;

        topBarLevelText.text = "LEVEL " + levelCurrent;
        mainMenu.SetActive(false);
 
    }   
    public void OnNextLevel()
    {
        topBarLevelText.text = "LEVEL " + levelCurrent;
        numberRePLay = 1;

    }   
    public void RePlayGame()
    {
        numberRePLay++;// cộng thêm 1 lần chơi lại
    } 
   
    public void OnSelectLevel()// btn vao Level o home
    {
        mainMenu.SetActive(false);
    }
    public void OnShopSkin()
    {
        mainMenu.SetActive(false);
    }    
    public void SelectLevel(int level)
    {
        numberRePLay = 1;
        levelCurrent = level;
        topBarLevelText.text = "LEVEL " + levelCurrent;
        mainMenu.SetActive(false);
    }
    public void CloseMainMenu()
    {
        mainMenu.SetActive(false);
    }    
    public void OpenMainMenu()
    {
        mainMenu.SetActive(true);
    }
    public void OnNewLevelStarted()
    {
        topBarLevelText.text = "LEVEL " + levelCurrent;
    }
    /*private void OnSwitchingScreens(string fromScreenId, string toScreenId)
    {
        if (toScreenId == "game")
        {
            topBarLevelText.text = "LEVEL " + GameManager.Instance.LevelNumber;

            // Fade in the level text
            PlayTopBarAnimation(UIAnimation.Alpha(topBarLevelText.gameObject, 0f, 1f, topBarAnimDuration));
        }
        else if (fromScreenId == "game")
        {
            // Fade out the level text
            PlayTopBarAnimation(UIAnimation.Alpha(topBarLevelText.gameObject, 1f, 0f, topBarAnimDuration));
        }
    }*/

    /*private void PlayTopBarAnimation(UIAnimation anim)
    {
        anim.style = UIAnimation.Style.EaseOut;
        anim.startOnFirstFrame = true;
        anim.Play();
    }*/
}
