using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader 
{

    public enum Scene
    {
        Overworld, Loading, Crypt, Battle, 
    }

    private static Action onLoaderCallback;
    public static Vector3 playerPos;

    public static void Load(Scene scene)
    {
        //Loader.playerPos = playerPos;
        onLoaderCallback = () =>
        {
            SceneManager.LoadScene(scene.ToString());            
        };
        SceneManager.LoadScene(Scene.Loading.ToString());
    }

    public static void LoaderCallback()
    {
        if(onLoaderCallback != null)
        {
            onLoaderCallback();
            onLoaderCallback = null;
            Player.GetInstance().GetComponent<MovingObject>().EnableMovement();
        }
    }
    
}
