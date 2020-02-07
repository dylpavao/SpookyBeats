using UnityEngine;

public class CameraFollow : MonoBehaviour
{    

    private Transform player;

    private void Start()
    {        
        player = GameObject.Find("Player").transform;
        //player = Player.GetInstance().transform;
    }

    //Causes world jitters due to Move in MovingObject
    private void FixedUpdate()
    {
        transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
    }
}
