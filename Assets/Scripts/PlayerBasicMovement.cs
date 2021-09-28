using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasicMovement : MonoBehaviour
{
    [SerializeField] float maxSpeed;
    [SerializeField] float gravity;
    [SerializeField] float jumpImpulse;

    private BoxCollider2D playerCollider;
    private Vector2 speed;

    // Start is called before the first frame update
    void Start()
    {
        playerCollider = GetComponent<BoxCollider2D>();

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 acceleration = new Vector2(0, -gravity);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            acceleration.y = jumpImpulse;
        }

        float horizontalMovement = Input.GetAxis("Horizontal");

        speed.x = horizontalMovement * maxSpeed;
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
        playerCollider.size = new Vector2(1, 0.9f);

        int a = playerCollider.OverlapCollider(contactFilter, wallColliders);

        return (a == 0);
    }

    private bool CanMoveVerticaly(Vector2 deltaMouvement)
    {
        Collider2D[] wallColliders = new Collider2D[5];
        ContactFilter2D contactFilter = new ContactFilter2D();

        playerCollider.offset = new Vector2(0, deltaMouvement.y);
        playerCollider.size = new Vector2(0.9f, 1);

        int a = playerCollider.OverlapCollider(contactFilter, wallColliders);

        return (a == 0);
    }
}