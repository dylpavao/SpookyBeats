using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderCallback : MonoBehaviour
{
    private bool firstUpdate = true;

    private void Update()
    {
        if (firstUpdate)
        {
            firstUpdate = false;
            Player.GetInstance().transform.position = Loader.playerPos;            
            Loader.LoaderCallback();
        }
    }
}
