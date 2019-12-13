using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainController : PrefabSingleton<MainController>
{
    // App Setting
    public DebugSettings DebugSettings;

    [HideInInspector]
    public SceneName CurrentScene = SceneName.Nil;

    public void LoadScene(SceneName scene)
    {
        if (scene == SceneName.Nil)
        {
            scene = SceneName.GroupSettingScene;
        }

        CurrentScene = scene;
        SceneManager.LoadScene(scene.ToString());
    }
}
