using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    [SerializeField] private float gravityAcceleration;
    [SerializeField] private float detectionMargin;
    [SerializeField] private float frictionAdjustementFactor;
    [SerializeField] private int dashGranularity;

    [SerializeField] private float mass;
    [SerializeField] private float horizontalForce;
    [SerializeField] private float walkMaxSpeed;
    [SerializeField] private float sprintMaxSpeed;
    [SerializeField] private float airControlFactor;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float wallJumpAngleDeg;
    [SerializeField] private float dashDistance;

    private SpriteRenderer spriteRenderer;
    private ParticleSystem groundParticleSystem;
    private ParticleSystem wallParticleSystem;

    private bool isOnGround;
    private bool wasOnGround;
    private float platformFrictionCoeff;
    private bool isOnWall;
    private int wallDirection;
    private int nbJump;
    private float horizontalMaxSpeed;
    private bool isInPlatform;
    private int nbDash;
    private bool isDashing;

    private bool isJumpButtonHold;
    private bool isDashButtonHold;

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
        groundParticleSystem = transform.Find("GroundParticles").gameObject.GetComponent<ParticleSystem>();
        wallParticleSystem = transform.Find("WallParticles").gameObject.GetComponent<ParticleSystem>();

        isOnGround = false;
        platformFrictionCoeff = 1f;
        isOnWall = false;
        wallDirection = 0;
        nbJump = 0;
        isJumpButtonHold = false;
        nbDash = 0;
        isDashButtonHold = false;
        isInPlatform = false;
        wasOnGround = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isOnGround && !wasOnGround)
        {
            groundParticleSystem.Play();
            wasOnGround = true;
        }

        if (!isOnGround)
        {
            wasOnGround = false;
        }

        if (isOnWall && speed.y < 0)
        {
            wallParticleSystem.Play();
        }

        
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

            if (isOnGround && !isInPlatform)
            {
                acceleration.x += -(platformFrictionCoeff * frictionAdjustementFactor * speed.x / mass);
                acceleration.y = 0;
            }
            else if (isOnWall && !isInPlatform)
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
        movementBuffer = Vector2.zero;
        Vector2 newPosition = ProcessColisions(deltaMovement);

        
        //dash
        if (nbDash > 0 && Input.GetAxis("Dash") > 0.5f)
        {
            if (!isDashButtonHold)
            {
                Vector2 dashPosition = transform.position + Mathf.Sign(Input.GetAxis("Horizontal")) * dashDistance * Vector3.right;
                newPosition = ProcessDash(dashPosition);
                nbDash--;
                isDashButtonHold = true;
            }
        }
        else
        {
            isDashButtonHold = false;
        }

        transform.position = new Vector3(newPosition.x, newPosition.y, 0);

        

        CheckGround();
        CheckWalls();
        playerCollider.size = Vector2.one;
        playerCollider.offset = Vector2.zero;

        switch (nbJump)
        {
            case 0:
                spriteRenderer.color = Color.red;
                break;
            case 1:
                spriteRenderer.color = Color.yellow;
                break;
            default:
                spriteRenderer.color = Color.green;
                break;
        }
        SpeedDeformation(speed);
    }


    private Vector2 ProcessColisions(Vector2 deltaMovement)
    {
        Collider2D[] platformColliders = new Collider2D[5];
        ContactFilter2D contactFilter = new ContactFilter2D();

        playerCollider.offset = new Vector2(deltaMovement.x, 0);
        playerCollider.size = new Vector2(1f, 1 - detectionMargin);

        int horizontal = playerCollider.OverlapCollider(contactFilter, platformColliders);

        if (horizontal > 0)
        {
            bool hitHorizontalRigidPlatform = false;
            for (int i = 0; i < horizontal; i++)
            {
                hitHorizontalRigidPlatform = hitHorizontalRigidPlatform 
                    || !platformColliders[i].gameObject.GetComponent<PlatformData>().isTraversable;
            }
            
            if (hitHorizontalRigidPlatform)
            {
                deltaMovement.x = 0;
                speed.x = 0;
            }
            else
            {
                isInPlatform = true;
            }
        }

        playerCollider.offset = new Vector2(0, deltaMovement.y);
        playerCollider.size = new Vector2(1 - detectionMargin, 1f);

        int vertical = playerCollider.OverlapCollider(contactFilter, platformColliders);

        

        if (vertical > 0)
        {
            bool hitVerticalRigidPlatform = (deltaMovement.y < 0 && !isInPlatform);

            for (int i = 0; i < vertical; i++)
            {
                bool traversablePlatform = platformColliders[i].gameObject.GetComponent<PlatformData>().isTraversable;
                hitVerticalRigidPlatform = hitVerticalRigidPlatform || !traversablePlatform;
            }

            if (hitVerticalRigidPlatform)
            {
                if (deltaMovement.y < 0)
                {
                    isOnGround = true;
                }
                deltaMovement.y = 0;
                speed.y = 0;
            }
            else
            {
                isInPlatform = true;
            }
        }
        
        if(horizontal == 0 && vertical == 0)
        {
            isInPlatform = false;
        }

        Vector2 newPosition = new Vector2(transform.position.x, transform.position.y) + deltaMovement;

        return newPosition;
    }

    private Vector2 ProcessDash(Vector2 destination)
    {
        Vector2 start = transform.position;
        Vector2 step = (destination - start) / dashGranularity;

        Collider2D[] platformColliders = new Collider2D[5];
        ContactFilter2D contactFilter = new ContactFilter2D();
        playerCollider.size = new Vector2(1f, 1 - detectionMargin);

        int stepIndex = 0;
        for (int i = 1; i <= dashGranularity; i++)
        {
            playerCollider.offset = i * step;
            int collision = playerCollider.OverlapCollider(contactFilter, platformColliders);
            if (collision > 0)
            {
                break;
            }
            stepIndex = i;
        }
        playerCollider.offset = Vector2.zero;
        playerCollider.size = Vector2.one;

        return (start + stepIndex * step);
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
            nbDash = 1;
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
            nbDash = 1;
            wallDirection = 1;
            platformFrictionCoeff = platformCollidersLeft[left - 1].gameObject.GetComponent<PlatformData>().frictionFactor;
        }
        else if (right > 0)
        {
            isOnWall = true;
            nbJump = 2;
            nbDash = 1;
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
