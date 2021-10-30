using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject selectLevel;

    private CanvasGroup mainMenuGroup;
    private CanvasGroup selectLevelGroup;

    private void Start()
    {
        mainMenuGroup = mainMenu.GetComponent<CanvasGroup>();
        selectLevelGroup = selectLevel.GetComponent<CanvasGroup>();
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Scene1");
    }

    public void SelectLevel()
    {
        mainMenuGroup.alpha = 0f;
        mainMenuGroup.blocksRaycasts = false;

        selectLevelGroup.alpha = 1f;
        selectLevelGroup.blocksRaycasts = true;
    }

    public void LoadLevel1(){
        SceneManager.LoadScene(1);
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene(2);
    }

    
}
