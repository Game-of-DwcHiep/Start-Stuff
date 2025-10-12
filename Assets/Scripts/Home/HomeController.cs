using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeController : MonoBehaviour
{
    public GameObject homeUI;
    public GameObject selectLevelUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        homeUI.SetActive(true);
        selectLevelUI.SetActive(false);
        if(GameManager.Instance.isBackSelectLevel)
            OnPlayButton();
    }

    public void OnPlayButton()
    {
        homeUI.SetActive(false);
        selectLevelUI.SetActive(true);
    }
    public void OnBackButton()
    {
        homeUI.SetActive(true);
        selectLevelUI.SetActive(false);
    }
    public void OnSelectLevel(string sceneName)
    {
        // sceneName là tên scene bạn đã thêm trong Build Settings
        SceneManager.LoadScene(sceneName);
        GameManager.Instance.isBackSelectLevel = true;
    }
    public void OnQuitButton()
    {
        Application.Quit();
    }
}
