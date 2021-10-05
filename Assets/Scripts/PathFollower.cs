using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    [SerializeField] private Vector2[] nodes;
    [SerializeField] private float movementSpeed;

    private Vector2 lastPos;
    private Vector2 nextPos;

    private int nextPosIndex;
    private float lerpTimer;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = nodes[0];
        lastPos = nodes[0];
        nextPos = nodes[1];
        nextPosIndex = 1;
        lerpTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        lerpTimer += Time.deltaTime * movementSpeed;
        transform.position = Vector2.Lerp(lastPos, nextPos, lerpTimer);

        if (lerpTimer > 1)
        {
            lerpTimer = 0;
            lastPos = nextPos;
            nextPosIndex++;
            if (nextPosIndex >= nodes.Length)
            {
                nextPosIndex = 0;
            }
            nextPos = nodes[nextPosIndex];
        }
    }
}
