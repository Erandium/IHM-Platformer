using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float dragMargin;
    [SerializeField] private float upMovementExtra;
    private BoxCollider2D platformCollider;

    private Vector2 previousPos;
    private Vector2 currentPos;

    // Start is called before the first frame update
    void Start()
    {
        previousPos = transform.localPosition;
        currentPos = previousPos;
        platformCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        currentPos = transform.localPosition;

        Collider2D[] platformColliders = new Collider2D[5];
        ContactFilter2D contactFilter = new ContactFilter2D();

        platformCollider.offset = new Vector2(0, dragMargin);
        //platformCollider.size = Vector2.one;

        int a = platformCollider.OverlapCollider(contactFilter, platformColliders);

        if (a > 0)
        {
            PlayerControler player = platformColliders[0].gameObject.GetComponent<PlayerControler>();
            if (player != null)
            {
                Vector2 movement = currentPos - previousPos;
                if(movement.y > 0)
                {
                    movement.y += upMovementExtra;
                }
                player.SetMovementBuffer(movement);
            }
        }

        platformCollider.offset = Vector2.zero;

        previousPos = currentPos;
    }
}


