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
    [SerializeField] private int jumpNumber;
    [SerializeField] private float wallJumpAngleDeg;
    [SerializeField] private float variableJumpFactor; // need to be < 1
    [SerializeField] private float dashDistance;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooldown;
    [SerializeField] private int dashNumber;

    public void setGravity(float truc)
    {
        gravityAcceleration = truc;
    }
    public void setDetectionMargin(float truc)
    {
        gravityAcceleration = truc;
    }
    public void setFrictionAdjustmentFactor(float truc)
    {
        frictionAdjustementFactor = truc;
    }
    public void setDashGranularity(int truc)
    {
        dashGranularity = truc;
    }
    public void setMass(float truc)
    {
        mass = truc;
    }
    public void setHorizontalForce(float truc)
    {
        horizontalForce = truc;
    }
    public void setWalkMaxSpeed(float truc)
    {
        walkMaxSpeed = truc;
    }
    public void setSprintMaxSpeed(float truc)
    {
        sprintMaxSpeed = truc;
    }
    public void setAirControlFactor(float truc)
    {
        airControlFactor = truc;
    }
    public void setJumpSpeed(float truc)
    {
        jumpSpeed = truc;
    }
    public void setJumpNumber(int truc)
    {
        jumpNumber = truc;
    }
    public void setWallJumpAngleDegre(float truc)
    {
        wallJumpAngleDeg = truc;
    }
    public void setVariableJumpFactor(float truc)
    {
        variableJumpFactor = truc;
    }
    public void setDashDistance(float truc)
    {
        dashDistance = truc;
    }
    public void setDashCooldown(float truc)
    {
        dashCooldown = truc;
    }
    public void setDashDuration(float truc)
    {
        dashDuration = truc;
    }
    public void setDashNumber(int truc)
    {
        dashNumber = truc;
    }


    private SpriteRenderer spriteRenderer;
    private ParticleSystem groundParticleSystem;
    private ParticleSystem wallParticleSystemLeft;
    private ParticleSystem wallParticleSystemRight;

    private float currGravityAcceleration;

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
    private float dashDirection;
    private float dashTimer;
    private float dashSpeed;

    private bool isJumpButtonHold;
    private bool isDashButtonHold;

    private Vector2 speed;
    private Vector2 movementBuffer;

    private BoxCollider2D playerCollider;

    private GameObject spawnPoint;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        playerCollider = GetComponent<BoxCollider2D>();
        speed = new Vector2(0, 0);
        movementBuffer = new Vector2(0, 0);

        spriteRenderer = GetComponent<SpriteRenderer>();
        groundParticleSystem = transform.Find("GroundParticles").gameObject.GetComponent<ParticleSystem>();
        wallParticleSystemLeft = transform.Find("WallParticlesLeft").gameObject.GetComponent<ParticleSystem>();
        wallParticleSystemRight = transform.Find("WallParticlesRight").gameObject.GetComponent<ParticleSystem>();
        currGravityAcceleration = gravityAcceleration;

        isOnGround = false;
        platformFrictionCoeff = 1f;
        isOnWall = false;
        wallDirection = 0;

        nbJump = 0;
        isJumpButtonHold = false;
        isInPlatform = false;
        wasOnGround = false;

        nbDash = 0;
        isDashing = false;
        dashDirection = 0;
        dashTimer = 0;
        dashSpeed = dashDistance / dashDuration;
        isDashButtonHold = false;

        spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");
        transform.position = spawnPoint.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        /* timers */
        dashTimer += Time.deltaTime;



        /* physics */

        // state

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
            if (wallDirection == 1)
            {
                wallParticleSystemLeft.Play();
            }
            else if (wallDirection == -1)
            {
                wallParticleSystemRight.Play();
            }
        }



        //dash detection
        if (nbDash > 0 && Input.GetAxis("Dash") > 0.5f)
        {
            if (!isDashButtonHold && !isDashing && dashTimer > dashCooldown)
            {
                isDashing = true;
                nbDash--;
                isDashButtonHold = true;
                dashTimer = 0;
                dashDirection = Mathf.Sign(Input.GetAxis("Horizontal"));
            }
        }
        else
        {
            isDashButtonHold = false;
        }



        Vector2 newPosition;

        if(!isDashing)
        {

            // if not dashing, do the phisic normaly

            // speed

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
                speed.y -= currGravityAcceleration * Time.deltaTime;
                speed.x = Input.GetAxis("Horizontal") * horizontalMaxSpeed;
            }
            else
            {
                Vector2 acceleration = new Vector2(Input.GetAxis("Horizontal") * (horizontalForce / mass), -currGravityAcceleration);

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
                    currGravityAcceleration = gravityAcceleration * variableJumpFactor;
                }


            }
            else
            {
                isJumpButtonHold = false;
                currGravityAcceleration = gravityAcceleration;
            }

            if (gravityAcceleration != currGravityAcceleration && speed.y < 0)
            {
                // on remet une gravité normale quand on retombe
                currGravityAcceleration = gravityAcceleration;
            }

           // movement

            Vector2 deltaMovement = speed * Time.deltaTime;
            deltaMovement += movementBuffer;
            movementBuffer = Vector2.zero;
            newPosition = ProcessColisions(deltaMovement);
        
        }
        else
        {
            if (dashTimer > dashDuration)
            {
                isDashing = false;
                print("dash timeout");
            }
            Vector2 deltaMovement = new Vector2(dashSpeed * dashDirection * Time.deltaTime, 0);
            newPosition = ProcessDash(deltaMovement);
        }

        

        transform.position = new Vector3(newPosition.x, newPosition.y, 0);

        

        CheckGround();
        CheckWalls();
        playerCollider.size = Vector2.one;
        playerCollider.offset = Vector2.zero;



        /* animation-debug */

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

    private Vector2 ProcessDash(Vector2 deltaMovement)
    {
        Vector2 start = transform.position;
        Vector2 destination = start + deltaMovement;
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
                isDashing = false;
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
            nbJump = jumpNumber;
            nbDash = dashNumber;
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
            nbJump = jumpNumber;
            nbDash = dashNumber;
            wallDirection = 1;
            if (gameObject.GetComponent<PlatformData>() != null)
            {
                platformFrictionCoeff = platformCollidersLeft[left - 1].gameObject.GetComponent<PlatformData>().frictionFactor;
            }
        }
        else if (right > 0)
        {
            isOnWall = true;
            nbJump = jumpNumber;
            nbDash = dashNumber;
            wallDirection = -1;
            if (gameObject.GetComponent<PlatformData>() != null)
            {
                platformFrictionCoeff = platformCollidersRight[right - 1].gameObject.GetComponent<PlatformData>().frictionFactor;
            }
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

    public void SetSpeed(Vector2 newSpeed)
    {
        speed = newSpeed;
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
