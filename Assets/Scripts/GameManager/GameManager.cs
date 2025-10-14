using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    public bool isBackSelectLevel = false;

    [Header("Material Settings")]
    public Material defaultMat;
    public Material highlightMat;

    public Vector3 targetPos;
    public void WinGame()
    {
        StartCoroutine(MoveCameraToWinPosition());
    }
    IEnumerator MoveCameraToWinPosition()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            yield break;
        }

        //Vector3 targetPosition = new Vector3(20f, 16f, -6f);
        //Vector3 targetPosition = new Vector3(200f, 73f, -75f);
        Quaternion targetRotation = Quaternion.Euler(60f, 0f, 0f);

        float duration = 2.5f; // thời gian di chuyển (giây)
        float elapsed = 0f;

        Vector3 startPos = mainCam.transform.position;
        Quaternion startRot = mainCam.transform.rotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            mainCam.transform.position = Vector3.Lerp(startPos, targetPos, t);
            mainCam.transform.rotation = Quaternion.Slerp(startRot, targetRotation, t);
            yield return null;
        }

        mainCam.transform.position = targetPos;
        mainCam.transform.rotation = targetRotation;
        yield return new WaitForSeconds(2f);
        OnSelectLevel("Menu");
    }

    public void OnSelectLevel(string sceneName)
    {
        // sceneName là tên scene bạn đã thêm trong Build Settings
        SceneManager.LoadScene(sceneName);
    }
}
