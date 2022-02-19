using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SkiPole : MonoBehaviour
{
    public bool IsLeft;

    Vector3 previous;
    public float X { get; private set; }
    float x0;

    float? minY, maxY;
    bool waitForReset;

    // Start is called before the first frame update
    void Start()
    {
        previous = transform.position;
        x0 = previous.x;
    }

    // Update is called once per frame
    void Update()
    {
        var p = 0.9f * previous + 0.1f * transform.position;

        if (minY == null || p.y < minY) minY = p.y;
        if (maxY == null || p.y > maxY) maxY = p.y;

        if (!waitForReset || minY == null || maxY == null)
        {
            if (p.y < previous.y)
            {
                // stick moving down
                //X = Mathf.Sign(p.x - x0);
                X = 0.5f * X + 0.5f * (p.x - x0);
            }
            else
            {
                waitForReset = true;
            }
        }
        if (p.y > 0.5f * (minY + maxY))
        {
            waitForReset = false;
            x0 = p.x;
        }
        previous = p;
    }
}
