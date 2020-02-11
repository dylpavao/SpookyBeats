using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneName
{
    Overworld, Loading, Crypt, Battle, GameOver
}

public static class Loader 
{    

    private static Action onLoaderCallback;    

    public static void Load(SceneName scene, Vector3 newPlayerPos)
    {
        //Loader.playerPos = playerPos;
        onLoaderCallback = () =>
        {
            Player.GetInstance().transform.position = newPlayerPos;
            SceneManager.LoadScene(scene.ToString());
            Player.GetInstance().GetComponent<MovingObject>().EnableMovement();
        };
        SceneManager.LoadScene(SceneName.Loading.ToString());
    }

    public static void LoaderCallback()
    {
        if(onLoaderCallback != null)
        {
            onLoaderCallback();
            onLoaderCallback = null;            
        }
    }
    
}
