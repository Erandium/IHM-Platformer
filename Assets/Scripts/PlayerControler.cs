using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    [SerializeField] private float gravityAcceleration;
    [SerializeField] private float detectionMargin;
    [SerializeField] private float frictionAdjustementFactor;

    [SerializeField] private float mass;
    [SerializeField] private float horizontalForce;
    [SerializeField] private float walkMaxSpeed;
    [SerializeField] private float sprintMaxSpeed;
    [SerializeField] private float airControlFactor;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float wallJumpAngleDeg;
    [SerializeField] private float dashDistance;

    private SpriteRenderer spriteRenderer;

    private bool isOnGround;
    private float platformFrictionCoeff;
    private bool isOnWall;
    private int wallDirection;
    private int nbJump;
    private bool isInPlatform;
    private float horizontalMaxSpeed;
    //private bool isDashing;

    private bool isJumpButtonHold;

    private Vector2 speed;
    private Vector2 movementBuffer;

    private BoxCollider2D playerCollider;

    // Start is called before the first frame update
    void Start()
    {
        playerCollider = GetComponent<BoxCollider2D>();
        speed = new Vector2(0, 0);
        movementBuffer = new Vector2(0, 0);

        spriteRenderer = GetComponent<SpriteRenderer>();

        isOnGround = false;
        platformFrictionCoeff = 1f;
        isOnWall = false;
        wallDirection = 0;
        nbJump = 0;
        isJumpButtonHold = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (isOnGround && Input.GetAxis("Sprint") > 0.5f)
        {
            horizontalMaxSpeed = sprintMaxSpeed;
        }
        else
        {
            horizontalMaxSpeed = walkMaxSpeed;
        }




        if (mass == 0)
        {
            speed.y -= gravityAcceleration * Time.deltaTime;
            speed.x = Input.GetAxis("Horizontal") * horizontalMaxSpeed;
        }
        else
        {
            Vector2 acceleration = new Vector2(Input.GetAxis("Horizontal") * (horizontalForce / mass), -gravityAcceleration);

            if (isOnGround)
            {
                acceleration.x += -(platformFrictionCoeff * frictionAdjustementFactor * speed.x / mass);
                acceleration.y = 0;
            }
            else if (isOnWall)
            {
                acceleration.y += -(platformFrictionCoeff * frictionAdjustementFactor * speed.y / mass);
            }
            else
            {
                acceleration.x *= airControlFactor;
            }


            speed += acceleration * Time.deltaTime;

            if (isOnGround)
            {
                speed.x = Mathf.Clamp(speed.x, -horizontalMaxSpeed, horizontalMaxSpeed);
            }
            else
            {
                speed.x = Mathf.Clamp(speed.x, -horizontalMaxSpeed * airControlFactor, horizontalMaxSpeed * airControlFactor);
            }

        }

        //jump
        if (nbJump > 0 && Input.GetAxis("Jump") > 0.5f)
        {
            if (!isJumpButtonHold)
            {
                if (isOnWall && !isOnGround)
                {
                    speed.x = jumpSpeed * Mathf.Cos(Mathf.Deg2Rad * wallJumpAngleDeg) * wallDirection;

                    speed.y = jumpSpeed * Mathf.Sin(Mathf.Deg2Rad * wallJumpAngleDeg);
                }
                else
                {
                    speed.y = jumpSpeed;
                }

                nbJump--;
                isJumpButtonHold = true;
            }

        }

        else
        {
            isJumpButtonHold = false;
        }



        Vector2 deltaMovement = speed * Time.deltaTime;
        deltaMovement += movementBuffer;
        Vector2 newPosition = ProcessColisions(deltaMovement);

        movementBuffer = Vector2.zero;

        transform.position = new Vector3(newPosition.x, newPosition.y, 0);

        

        CheckGround();
        CheckWalls();
        playerCollider.size = Vector2.one;
        playerCollider.offset = Vector2.zero;

        if (nbJump == 0)
        {
            spriteRenderer.color = Color.red;
        }
        else
        {
            spriteRenderer.color = Color.green;
        }
        SpeedDeformation(speed);
    }


    private Vector2 ProcessColisions(Vector2 deltaMovement)
    {
        Collider2D[] platformColliders = new Collider2D[5];
        ContactFilter2D contactFilter = new ContactFilter2D();

        playerCollider.offset = new Vector2(deltaMovement.x, 0);
        playerCollider.size = new Vector2(1f, 1 - detectionMargin);

        int a = playerCollider.OverlapCollider(contactFilter, platformColliders);

        if (a > 0)
        {
            isInPlatform = isInPlatform || platformColliders[a - 1].gameObject.GetComponent<PlatformData>().isTraversable;
            if (!isInPlatform)
            {
                deltaMovement.x = 0;
                speed.x = 0;
            }
        }

        playerCollider.offset = new Vector2(0, deltaMovement.y);
        playerCollider.size = new Vector2(1 - detectionMargin, 1f);

        int b = playerCollider.OverlapCollider(contactFilter, platformColliders);

        if (b > 0)
        {
            isInPlatform = isInPlatform || (platformColliders[b - 1].gameObject.GetComponent<PlatformData>().isTraversable && deltaMovement.y > 0);
            if (!isInPlatform)
            {
                if (deltaMovement.y < 0)
                {
                    isOnGround = true;
                }
                deltaMovement.y = 0;
                speed.y = 0;
            }
        }

        if (a == 0 && b == 0)
        {
            isInPlatform = false;
        }

        Vector2 newPosition = new Vector2(transform.position.x, transform.position.y) + deltaMovement;

        return newPosition;
    }

    private void CheckGround()
    {
        Collider2D[] platformColliders = new Collider2D[5];
        ContactFilter2D contactFilter = new ContactFilter2D();

        playerCollider.size = new Vector2(0.5f, 1f);
        playerCollider.offset = new Vector2(0, -detectionMargin);


        int a = playerCollider.OverlapCollider(contactFilter, platformColliders);

        if (a > 0)
        {
            isOnGround = true;
            nbJump = 2;
            platformFrictionCoeff = platformColliders[a - 1].gameObject.GetComponent<PlatformData>().frictionFactor;
        }
        else
        {
            isOnGround = false;
        }
    }

    private void CheckWalls()
    {
        Collider2D[] platformCollidersLeft = new Collider2D[5];
        Collider2D[] platformCollidersRight = new Collider2D[5];
        ContactFilter2D contactFilter = new ContactFilter2D();

        playerCollider.size = new Vector2(1f, 0.5f);

        // check left
        playerCollider.offset = new Vector2(-detectionMargin, 0);

        int left = playerCollider.OverlapCollider(contactFilter, platformCollidersLeft);

        //check right
        playerCollider.offset = new Vector2(detectionMargin, 0);

        int right = playerCollider.OverlapCollider(contactFilter, platformCollidersRight);

        if (left > 0)
        {
            isOnWall = true;
            nbJump = 2;
            wallDirection = 1;
            platformFrictionCoeff = platformCollidersLeft[left - 1].gameObject.GetComponent<PlatformData>().frictionFactor;
        }
        else if (right > 0)
        {
            isOnWall = true;
            nbJump = 2;
            wallDirection = -1;
            platformFrictionCoeff = platformCollidersRight[right - 1].gameObject.GetComponent<PlatformData>().frictionFactor;
        }
        else
        {
            isOnWall = false;
            wallDirection = 0;

        }
    }


    public void SetMovementBuffer(Vector2 movement)
    {
        movementBuffer = movement;
    }

    private void SpeedDeformation(Vector2 speed)
    {
        if (speed.x > walkMaxSpeed)
        {
            transform.localScale = new Vector3(0.7f + (0.3f * walkMaxSpeed / Mathf.Abs(speed.x)), 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        //transform.localScale
    }
}
