using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkiPole : MonoBehaviour
{
    public bool IsLeft;
    public Transform Top;
    public Transform Bottom;

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
        X = Top.position.x - Bottom.position.x;
    }
}
