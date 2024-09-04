using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyDoor : Door
{
    public bool isClosed { get; protected set; }
    public float openness;

    bool shouldOpen = false;
    public float openSpeed = 1;
    public float closeSpeed = 1;
    public Vector3 openOffset = Vector3.up;
    Vector3 initialPos;

    public AnimationCurve curve = AnimationCurve.Linear(0,0,1,1);
    void Start()
    {
        initialPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldOpen)
            OpenStep();
        else
            CloseStep();
        transform.position = initialPos + openOffset * curve.Evaluate(openness);
    }

    public void OpenStep ()
    {
        shouldOpen = true;
        openness += openSpeed * Time.deltaTime;
        if (openness >= 1)
        {
            openness = 1;
            isOpen = true;
        }
        else isOpen = false;
    }
    public void CloseStep ()
    {
        openness -= closeSpeed * Time.deltaTime;
        if (openness <= 0)
        {
            openness = 0;
            isClosed = true;
        }
        else isClosed = false;
    }

    public override void Open()
    {
        shouldOpen = true;
    }
    public override void Close()
    {
        shouldOpen = false;
    }
}
