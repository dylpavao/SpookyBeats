using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private SceneName sceneToLoad;
    [SerializeField] private Vector3 playerStartingPos;

    public SceneName GetSceneName()
    {
        return sceneToLoad;
    }

    public Vector3 GetPlayerStartingPos()
    {
        return playerStartingPos;
    }

    public void SetPlayerStartingPos(Vector3 pos)
    {
        playerStartingPos = pos;
    }

}
