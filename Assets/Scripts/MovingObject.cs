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
    private bool moving;
    private Vector3 lastPosition;
    private Vector3 destination;
    private float sqrRemainingDist = 0;    
    private Coroutine movement;

    public Direction dir;

    public enum Direction
    {
        Up, Down, Left, Right, 
    }

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
        if (!LayerCollision(xDir, yDir))
        {
            lastPosition = transform.position;
            destination = transform.position + new Vector3(xDir, yDir);           
            movement = StartCoroutine(SmoothMovement());            
            return true;
        }
        
        return false;
    }

    protected bool LayerCollision(int xDir, int yDir)
    {
        Vector3 start = transform.position;
        Vector3 end = start + new Vector3(xDir, yDir);

        boxCollider.enabled = false;
        RaycastHit2D hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        return hit.transform != null;
    }

    protected IEnumerator SmoothMovement()
    {        
        moving = true; 
        sqrRemainingDist = (transform.position - destination).sqrMagnitude;

        while (sqrRemainingDist > float.Epsilon)
        {
            //Debug.Log(destination);
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, destination, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDist = (transform.position - destination).sqrMagnitude;
            yield return null; //waits 1 frame
        }
        moving = false;        
    }

    protected void UpdateDestination(int xDir, int yDir)
    {
        if (sqrRemainingDist < 0.05 && !LayerCollision(xDir, yDir))
        {
            lastPosition = destination;
            destination = destination + new Vector3(xDir, yDir);
        }            
    }

    protected bool IsMoving()
    {
        return moving;
    }

    protected bool IsMoveable()
    {
        return moveable;
    }   

    public Vector3 LastPosition()
    {
        return lastPosition;
    }

    public void EnableMovement()
    {                
        moveable = true;
    }

    public void SetDestination(Vector3 dest)
    {
        destination = dest;
    }

    protected void DisableMovement(bool stopCoroutine)
    {
        if (stopCoroutine)
        {
            StopCoroutine(movement);                   
        }            

        moveable = false;
        moving = false;
    }
}