using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour // make static??
{
    private string sceneToLoad;

    public void LoadScene(string sceneName)
    {
        sceneToLoad = sceneName;
        Invoke("RealLoadScene", 0.25f);
    }

    private void RealLoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
        //GameObject.Find("Player").GetComponent<Player>().EnableMovement();
    }
}
