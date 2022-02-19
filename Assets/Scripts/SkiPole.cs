using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkiPole : MonoBehaviour
{
    public bool IsLeft;

    public const int SampleSize = 10;
    public const float SampleTime = 0.05f;

    Vector3 previous;
    Vector2[] samples = new Vector2[SampleSize];
    int pointer;
    float sampleTimer;

    public float X { get; private set; }


    // Start is called before the first frame update
    void Start()
    {
        previous = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var p = transform.position;
        sampleTimer += Time.deltaTime;
        if (sampleTimer > SampleTime)
        {
            sampleTimer = 0;
            var d = p - previous;
            samples[pointer] = d;
            pointer = (pointer + 1) % samples.Length;
            previous = p;
        }

        if (samples.Count(x => x.y < 0) > SampleSize / 2)
        {
            // stick moving down
            var avg = samples.Select(x => x.x).Average();
            X = avg;
        }
    }
}
