using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectScene : MonoBehaviour
{
    public void OnSelectLevel(string sceneName)
    {
        // sceneName là tên scene bạn đã thêm trong Build Settings
        SceneManager.LoadScene(sceneName);
    }
}
