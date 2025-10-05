using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingGame : MonoBehaviour
{
    public float loadingTimer;

    public float currentTimer;

    public Image loadingBar;

    public Image movingImage; // Thêm một Image di chuyển 

    public bool load = true;

    public string nameScene;

    public Text textLoading;

    private AsyncOperation asyncLoad;

    // Giá trị vị trí bắt đầu và kết thúc
    public float startX = -180f;
    public float endX = 180f;
    void Start()
    {
        currentTimer = 0.05f;
        StartCoroutine(LoadSceneAsync(nameScene));
    }
    // Update is called once per frame
    void Update()
    {
        if (load)
        {
            currentTimer += Time.deltaTime;
            if (currentTimer >= loadingTimer)
            {
                textLoading.text = "100%";
                asyncLoad.allowSceneActivation = true;
            }
            loadingBar.fillAmount = Mathf.Min(1.0f, currentTimer / loadingTimer);
            textLoading.text = Mathf.RoundToInt(currentTimer / loadingTimer * 100f ).ToString() + "%";

            // Di chuyển hình ảnh theo fillAmount của thanh loading
            UpdateMovingImagePosition();
        }
    }
    private void UpdateMovingImagePosition()
    {
        if (movingImage != null)
        {
            // Tính toán vị trí mới dựa trên fillAmount
            float newX = Mathf.Lerp(startX, endX, loadingBar.fillAmount);

            // Cập nhật vị trí của Image
            movingImage.rectTransform.anchoredPosition = new Vector2(newX, movingImage.rectTransform.anchoredPosition.y);
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {

        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {

            asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
        else
        {
            Debug.LogError("Scene " + sceneName + " null trong Build Settings!");
        }
    }  
}
