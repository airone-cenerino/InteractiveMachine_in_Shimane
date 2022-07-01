using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneController: MonoBehaviour
{
    [SerializeField] private KeyCode settingSceneJumpKey = KeyCode.Escape;
    private string currentSceneName = "Main";
    public static SceneController instance
    {
        get; private set;
    }


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        
        DontDestroyOnLoad(gameObject);
    }


    private void Update()
    {
        if (Input.GetKeyDown(settingSceneJumpKey))
        {
            if (currentSceneName == "Main")
            {
                SceneManager.LoadScene("SettingScene");
                currentSceneName = "SettingScene";
            }
            else
            {
                SceneManager.LoadScene("Main");
                currentSceneName = "Main";
            }
        }
    }
}
