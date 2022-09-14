using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TimeCounter;

public class ChangeScene : Timer
{
    public delegate void _onChangeScene(string sceneName);
    public static _onChangeScene OnChangeScene;

    [SerializeField] bool _useTimer, _automaticSceneChange;

    [SerializeField] string _nextSceneName;

    private void Start()
    {
       if(_automaticSceneChange) ChangingScene(_useTimer, GetTimer);
    }

    private void Update()
    {
        CountDown();
    }
    void ChangingScene(bool useTime, float timer)
    {

        if (!useTime)
        {
            timer = 0;
        }

        SetTimer(timer, () => StartCoroutine(LoadingNextScene(_nextSceneName)));
    }

    //BUTTON METHOD
    public void SelectingNextScene(string sceneName)
    {
        StartCoroutine(LoadingNextScene(sceneName));
    }

    //BUTTON METHOD
    public void ExitGame()
    {
        Application.Quit();
    }

    public IEnumerator LoadingNextScene(string sceneName)
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneName);

        while (!loading.isDone)
        {           
            //ANIMACAO PARA LOADING

            yield return null;
        }
    }

    private void OnEnable()
    {
        OnChangeScene += SelectingNextScene;
    }
    private void OnDisable()
    {
        OnChangeScene -= SelectingNextScene;
    }
}
