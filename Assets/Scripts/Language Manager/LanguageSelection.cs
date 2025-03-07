using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LanguageSelection : MonoBehaviour
{
    private string selectedCode;
    private void Start()
    {
        selectedCode = LanguageController.Instance.GetLanguage();
    }
    public void LanguageButtons(string languageCode)
    {
        selectedCode = languageCode;
        SelectLanguage();
    }
    public void SelectLanguage()
    {
        LanguageController.Instance.SetLanguage(selectedCode);
        PlayerPrefs.SetString("Language", selectedCode);

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);

    }
}
