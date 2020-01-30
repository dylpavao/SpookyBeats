using UnityEngine;

public class Beat : MonoBehaviour
{
    public float distance = 1;
    private float speed;
    private string sig;
    private BeatKeeper keeper;
    private string side; // which side of target    
    private bool active;

    void Start()
    {
        keeper = GameObject.Find("BeatKeeper").GetComponent<BeatKeeper>();
        speed = distance / keeper.TimeBetweenBeats();
        active = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            if (side == "left")
            {
                transform.position += Vector3.right * speed * Time.deltaTime;
                if (transform.position.x >= 0)
                {
                    active = false;             // redundant???                                                                 
                }
            }
            if (side == "right")
            {
                transform.position += Vector3.left * speed * Time.deltaTime;
                if (transform.position.x <= 0)
                {
                    active = false;
                }
            }

        }
    }

    public void DestroyBeat(float delay)
    {
        speed = 0;
        Destroy(gameObject, delay);
    }

    public void SetSide(string side)
    {
        this.side = side;
    }

    public void SetName(string sig)
    {
        this.sig = sig;
    }

    public string GetName()
    {
        return sig;
    }

}