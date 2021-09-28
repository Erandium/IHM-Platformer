using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasicMovement : MonoBehaviour
{
    [SerializeField] float maxSpeed;
    [SerializeField] float gravity;
    [SerializeField] float jumpSpeed;
    [SerializeField] float airControlFactor;
    [SerializeField] float horizontalJumpSpeed;
    [SerializeField] float horizontalJumpVerticalFactor;

    private BoxCollider2D playerCollider;
    public Vector2 speed;

    private bool canJump;
    private int isAgainstWall;
    
    void Start()
    {
        playerCollider = GetComponent<BoxCollider2D>();
        speed = new Vector2(0, 0);
        canJump = true;
        isAgainstWall = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 acceleration = new Vector2(0, -gravity);

        float horizontalMovement = Input.GetAxis("Horizontal");
        if (!canJump)
        {
            horizontalMovement *= airControlFactor;
        }

        speed.x = horizontalMovement * maxSpeed;
        if (canJump && Input.GetKeyDown(KeyCode.Space))
        {
            speed.y = jumpSpeed;
            if(isAgainstWall!=0)
            {
                speed.y *= horizontalJumpVerticalFactor;
                speed.x = horizontalJumpSpeed*isAgainstWall;
                print(isAgainstWall);
                print(speed);
            }
           
           // speed.x = horizontalJumpSpeed*isAgainstWall;
            canJump = false;   
        }

        speed += acceleration * Time.deltaTime;
        

        
        Vector2 deltaMouvement = speed * Time.deltaTime;

        if (!CanMoveHorizontaly(deltaMouvement))
        {
            deltaMouvement.x = 0;
            speed.x = 0;
        }
        if (!CanMoveVerticaly(deltaMouvement))
        {
            deltaMouvement.y = 0;
            speed.y = 0;
        }

        transform.position += new Vector3(deltaMouvement.x, deltaMouvement.y, 0);


    }



    private bool CanMoveHorizontaly(Vector2 deltaMouvement)
    {
        Collider2D[] wallColliders = new Collider2D[5];
        ContactFilter2D contactFilter = new ContactFilter2D();

        playerCollider.offset = new Vector2(deltaMouvement.x, 0);
        playerCollider.size = new Vector2(1, 1);

        int a = playerCollider.OverlapCollider(contactFilter, wallColliders);

        isAgainstWall = 0;
        for(int i=0; i<a; i++)
        {
            if(wallColliders[i].transform.position.x < transform.position.x)
            {
                isAgainstWall = 1;
            }
            else
            {
                isAgainstWall = -1;
            }
            canJump = true;
        }
       

        return (a == 0);
    }

    private bool CanMoveVerticaly(Vector2 deltaMouvement)
    {
        Collider2D[] wallColliders = new Collider2D[5];
        ContactFilter2D contactFilter = new ContactFilter2D();

        playerCollider.offset = new Vector2(0, deltaMouvement.y);
        playerCollider.size = new Vector2(1f, 1);

        int a = playerCollider.OverlapCollider(contactFilter, wallColliders);
        
        for(int i = 0; i < a; i++)
        {
            if (wallColliders[i].transform.position.y < transform.position.y)
            {
                canJump = true;
            }
        }
        return (a == 0);
    }
}