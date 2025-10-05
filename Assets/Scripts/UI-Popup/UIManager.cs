using DwcHyep;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    public LevelScreen levelScreen;
    public GameScreen gameScreen;

    public int levelCurrent;

    public int numberRePLay;// số lần chơi lại trong 1 level

    public Text textCoin;

    public GameObject mainMenu;

    [SerializeField] private Text topBarLevelText = null;
    //[SerializeField] private float topBarAnimDuration = 0.35f;

    private void Start()
    {      
        //ScreenManager.Instance.OnSwitchingScreens += OnSwitchingScreens;
        UpDateTextCoin();
        UpDateLevel();
    }
    public void UpDateLevel()
    {
        levelCurrent = GameManager.Instance.LevelNumber;  
    }    
    public void UpDateTextCoin()
    {
        textCoin.text = GameManager.Instance.Coins.ToString();
    }    
    public void OnMainPlayGame()
    {
        ScreenManager.Instance.Show("game");
        numberRePLay = 1;
        levelCurrent = GameManager.Instance.LevelNumber;
        GameManager.Instance.PlayGameInLevel(levelCurrent - 1, false);
        topBarLevelText.text = "LEVEL " + levelCurrent;
        mainMenu.SetActive(false);
 
    }   
    public void OnNextLevel()
    {
        GameManager.Instance.PlayGameInLevel(levelCurrent - 1, false);
        topBarLevelText.text = "LEVEL " + levelCurrent;
        numberRePLay = 1;

    }   
    public void RePlayGame()
    {
        GameManager.Instance.PlayGameInLevel(levelCurrent - 1, false);
        numberRePLay++;// cộng thêm 1 lần chơi lại

    } 

    public void RePlayGameUseHint()
    {
        System.Action SuccessEvent = () =>
        {
            GameManager.Instance.PlayGameInLevel(levelCurrent - 1, true);
            numberRePLay++;// cộng thêm 1 lần chơi lại
        };
        System.Action Failed = () =>
        {
            //Debug.LogError("???");
        };

    }    
    public void OnSelectLevel()// btn vao Level o home
    {
        ScreenManager.Instance.Show("level");
        levelScreen.UpdateUIScreen();
        mainMenu.SetActive(false);
    }
    public void OnShopSkin()
    {
        ScreenManager.Instance.Show("skin");
        mainMenu.SetActive(false);
    }    
    public void SelectLevel(int level)
    {
        GameManager.Instance.PlayGameInLevel(level - 1, false);
        ScreenManager.Instance.Show("game");
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
        GameManager.Instance.ClearBox();
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
