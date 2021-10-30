using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnFromFall : MonoBehaviour
{
    private BoxCollider2D fallCollider;
    void Start()
    {
        fallCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D[] platformColliders = new Collider2D[5];
        ContactFilter2D contactFilter = new ContactFilter2D();

        int a = fallCollider.OverlapCollider(contactFilter, platformColliders);

        for (int i = 0; i < a; i++)
        {
            PlayerControler player = platformColliders[i].gameObject.GetComponent<PlayerControler>();
            if (player != null)
            {
                player.gameObject.transform.position = this.gameObject.transform.GetChild(0).transform.position;
                player.SetSpeed(Vector2.zero);
            }
        }
    }

    
}
