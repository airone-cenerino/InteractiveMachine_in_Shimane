using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class IntervalTimeManager : MonoBehaviour
{
    [SerializeField] private float intervalTime;
    private InputFieldValueGetter inputFieldValueGetter;
    public static IntervalTimeManager instance
    {
        get; private set;
    }


    void Awake()
    {
        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        intervalTime = PlayerPrefs.GetFloat("IntervalTime", intervalTime);
    }


    // シーン開始時
    void SceneLoaded(Scene nextScene, LoadSceneMode mode)
    {
        if (instance != this)
            return;

            if (nextScene.name == "Main")
        {
            if (intervalTime >= 0f)
            {
                MovieManager movieManager = GameObject.Find("MovieManager").GetComponent<MovieManager>();
                movieManager.SetIntervalTime(intervalTime);
            }
        }
        else
        {
            inputFieldValueGetter = GameObject.Find("IntervalTimeGetter").GetComponent<InputFieldValueGetter>();
        }
    }


    // シーン終了時
    void OnSceneUnloaded(Scene scene)
    {
        if (scene.name == "SettingScene")
        {
            intervalTime = inputFieldValueGetter.GetInputValue();
            PlayerPrefs.SetFloat("IntervalTime", intervalTime);
            PlayerPrefs.Save();
        }
    }
}
