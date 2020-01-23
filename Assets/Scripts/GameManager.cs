using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public bool inBattle;

    private GameObject loadImage;
    private Text loadText;
    //public Camera mainCamera;        

    public Vector3 bottomLeft;
    public Vector3 topRight;



    //Called before start, after all objects are initialized
    void Awake()
    {
        //In case 2 GameManagers exist
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        Time.timeScale = 1f;
        inBattle = false;

        //loadImage = GameObject.Find("LoadImage");
        //loadText = GameObject.Find("LoadText").GetComponent<Text>();
        //loadImage.SetActive(false);        
    }

    void Start()
    {
        //IAnimationClipSource 
        //bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, mainCamera.nearClipPlane));
        //topRight = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, mainCamera.nearClipPlane));
        //DamagePopup.Create(Vector3.zero, 100);
    }

    public static GameManager GetInstance()
    {
        return instance;
    }

    private void OnDestroy()
    {
        Debug.Log("GameManager Destroyed");
    }

    //Change to wait for player to 
    public void StartBattle()
    {
        inBattle = true;
        loadImage.SetActive(true);
        Invoke("LoadBattle", 1);
    }

    private void LoadBattle()
    {
        GameObject.Find("Player").transform.position = new Vector3((float)-1.5, (float)2.5, 0);
        //SceneLoader sceneLoader = GameObject.Find("SceneLoader").GetComponent<SceneLoader>();
        //sceneLoader.LoadScene("Battle");
    }



    public bool InBattle()
    {
        return inBattle;
    }
}