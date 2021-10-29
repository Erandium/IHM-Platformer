using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextLevel : MonoBehaviour
{
    private BoxCollider2D loadPointCollider;
    // Start is called before the first frame update
    void Start()
    {
        loadPointCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D[] platformColliders = new Collider2D[5];
        ContactFilter2D contactFilter = new ContactFilter2D();

        int a = loadPointCollider.OverlapCollider(contactFilter, platformColliders);

        for (int i = 0; i < a; i++)
        {
            PlayerControler player = platformColliders[i].gameObject.GetComponent<PlayerControler>();
            if (player != null)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
}
