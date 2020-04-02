using UnityEngine;
using System.Collections.Generic;

public class BeatKeeper : MonoBehaviour
{

    [SerializeField] private Beat beat;
    //[SerializeField] private DamagePopup dmgPopup;

    public float bpm;             //beats per minute   
    private float gracePeriod;       //in seconds
    private float timeRunning;       //in seconds
    private float timeBetweenBeats;  //in seconds
    private float graceLower, graceUpper;  //in seconds    
    private bool beatHit;
    private bool playedBeat;
    private bool enacted;
    private Enemy enemy;
    private Player player;
    private Queue<Beat[]> beats;
    private bool running;


    private void Start()
    {
        Time.timeScale = 1f;

        bpm = 100f;
        timeBetweenBeats = 60f / bpm;
        gracePeriod = timeBetweenBeats / 2f;
        timeRunning = gracePeriod;
        graceLower = timeBetweenBeats - gracePeriod / 2f;
        graceUpper = timeBetweenBeats + gracePeriod / 2f;
        beatHit = false;
        playedBeat = false;
        enacted = false;
        running = false;
        //Debug.Log("TBB: "+timeBetweenBeats+" GP: "+gracePeriod+" T1:"+graceLower+" T2: "+graceUpper);

        //Setup Starting Beats On Screen (6 pairs of beats around target)
        beats = new Queue<Beat[]>();
        int x1 = -1;
        int x2 = 1;
        for (int i = 0; i < 6; i++)
        {
            SpawnSetOfBeatBars(x1, x2);
            x1 -= 2;
            x2 += 2;
        }
        enemy = FindObjectOfType<Enemy>();
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (running)
        {
            timeRunning += Time.deltaTime;
            //Debug.Log(timeRunning);
            if (timeRunning >= timeBetweenBeats + gracePeriod) //end of beat section, reset to new beat section
            {
                timeRunning -= timeBetweenBeats;
                beatHit = false;
                playedBeat = false;
                enacted = false;
                beats.Dequeue(); // pop cartridge
                enemy.ResetState();
                player.ResetState();
            }
            else if (!playedBeat && timeRunning >= timeBetweenBeats) // play beat sound & spawn new beats offscreen
            {
                DestroyBeats(0f);
                SpawnSetOfBeatBars(-12, 12); // load cartridge
                FindObjectOfType<AudioManager>().Play("Beat");
                playedBeat = true;
                enemy.ChooseMove();
            }
            else if (!enacted && timeRunning >= graceUpper) // end of grace period
            {
                // enact moves
                enacted = true;
                //enemy.EnactMove();
                player.EnactMove();
            }
        }        
    }

    public void SpawnSetOfBeatBars(int leftBeatX, int rightBeatX)
    {
        Beat left = Instantiate(beat.gameObject).GetComponent<Beat>();
        left.SetSide("left");
        left.transform.position = new Vector3(leftBeatX, 0);
        Beat right = Instantiate(beat.gameObject).GetComponent<Beat>();
        right.SetSide("right");
        right.transform.position = new Vector3(rightBeatX, 0);
        beats.Enqueue(new Beat[] { left, right });
    }

    public void DestroyBeats(float delay)
    {
        Beat[] temp = beats.Peek();
        if (temp[0] != null)
            temp[0].DestroyBeat(delay);
        if (temp[1] != null)
            temp[1].DestroyBeat(delay);
    }

    //Hits beat, returns true if on beat, false otherwise
    public bool HitBeat()
    {
        bool onBeat = false;
        if (!beatHit)
        {
            if (timeRunning >= gracePeriod)
            {
                beatHit = true;
                DestroyBeats(0.25f);
                if (timeRunning >= graceLower && timeRunning <= graceUpper) // on beat
                    onBeat = true;
            }
        }

        return onBeat;
    }

    public void SetRunning(bool running)
    {
        this.running = running;
        if (running)
        {
            // play song
        }
        else
        {
            // stop song
        }
    }

    public bool IsRunning()
    {
        return running;
    }

    public float TimeBetweenBeats()
    {
        return timeBetweenBeats;
    }

    public float GetTimeRunning()
    {
        return timeRunning;
    }

}