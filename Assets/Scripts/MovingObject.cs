using System.Collections;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    public float moveTime = 0.1f;
    public LayerMask blockingLayer;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;        
    private bool moveable;

    private Coroutine movement;
    //private IEnumerator test;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;                
        moveable = true;
    }

    protected bool Move(int xDir, int yDir) // may not need bool
    {        
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);
        
        boxCollider.enabled = false;
        RaycastHit2D hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform == null)
        {            
            movement = StartCoroutine(SmoothMovement(end));
            return true;
        }
        
        return false;
    }

    protected IEnumerator SmoothMovement(Vector3 end)
    {        
        moveable = false; 
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {                        
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
        moveable = true;        
    }


    protected bool IsMoveable()
    {
        return moveable;
    }   

    public void EnableMovement()
    {        
        moveable = true;
    }

    protected void DisableMovement()
    {
        //Debug.Log("Stopped routines");
        StopCoroutine(movement);
        moveable = false; 
    }
}