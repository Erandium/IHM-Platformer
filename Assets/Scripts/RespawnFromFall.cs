using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnFromFall : MonoBehaviour
{
    private BoxCollider2D collider;
    private GameObject player;
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D[] platformColliders = new Collider2D[5];
        ContactFilter2D contactFilter = new ContactFilter2D();

        int a = collider.OverlapCollider(contactFilter, platformColliders);

        if (a > 0)
        {
            player.transform.position = this.gameObject.transform.GetChild(0).transform.position;
        }
    }

    
}
