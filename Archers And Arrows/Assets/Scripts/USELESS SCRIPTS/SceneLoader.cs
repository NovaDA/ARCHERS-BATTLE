using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public string SceneName;

    public void LoadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName);
        Debug.Log(SceneName);
        //Invoke("CallManager", 1f);
        
    }

    public void Quit()
    {
        Application.Quit();
    }
}
