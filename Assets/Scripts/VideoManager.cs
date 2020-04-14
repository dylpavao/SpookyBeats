using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    public RawImage rawImage;
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;
    public bool trigger = false;

    public void Update()
    {
        if (!videoPlayer.isPlaying && trigger) // end of video
        {
            if (GameManager.GetInstance().InMainMenu())
            {
                Loader.Load(SceneName.Overworld, new Vector3(-7.5f, -2.5f, 0));
            }
            else
            {
                Loader.Load(SceneName.MainMenu, new Vector3(-7.5f, -2.5f, 0)); // make dynamic
            }            
        }
        if (videoPlayer.isPlaying && !trigger)
        {            
            trigger = true;
        }    
    }

    public void PrepareVideo()
    {
        StartCoroutine(PlayVideo());
    }

    IEnumerator PlayVideo()
    {
        videoPlayer.Prepare();
        WaitForSeconds waitForSeconds = new WaitForSeconds(1); // combine with yield
        while (!videoPlayer.isPrepared)
        {
            yield return waitForSeconds;
            break;
        }
        
        if(GameManager.GetInstance().InMainMenu() && GameObject.Find("Characters") != null)
            GameObject.Find("Characters").SetActive(false);

        GameManager.GetInstance().SetVideoScreenActive(true);
        rawImage.texture = videoPlayer.texture;
        videoPlayer.Play();
        audioSource.Play();        
    }

    public bool PlayingVideo()
    {
        return videoPlayer.isPlaying;
    }
}
