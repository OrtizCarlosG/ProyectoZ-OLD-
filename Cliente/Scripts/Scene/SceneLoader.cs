using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Text _percentText;
    public Image _loadImage;
    void Start()
    {
        SceneManager.LoadScene(LoadData.sceneToLoad);
       // StartCoroutine(loadScene());
    }


    IEnumerator loadScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(LoadData.sceneToLoad, LoadSceneMode.Single);
        float progressValue = Mathf.Clamp01(asyncLoad.progress);
        _percentText.text = $"Cargando...{Mathf.Round(progressValue * 100)}%";
        _loadImage.fillAmount = progressValue;
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
