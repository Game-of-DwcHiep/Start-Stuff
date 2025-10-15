using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public GameObject helloText;
    public GameObject tutorial;
    public GameObject uIActionManager;
    
    void Start()
    {
        uIActionManager.SetActive(false);
        helloText.SetActive(true);
        tutorial.SetActive(true);
    }
    public void FinishTutorial()
    {
        helloText.SetActive(false);
        uIActionManager.SetActive(true);
        tutorial.SetActive(false);
    }
}
