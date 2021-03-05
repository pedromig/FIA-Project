using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDetectorScript : MonoBehaviour
{

    public float angleOfSensors = 10f;
    public float rangeOfSensors = 10f;
    protected Vector3 initialTransformUp;
    protected Vector3 initialTransformFwd;
    public float strength;
    public float angleToClosestObj;
    public int numObjects;
    public bool debugMode;
    // Start is called before the first frame update
    void Start()
    {

        initialTransformUp = this.transform.up;
        initialTransformFwd = this.transform.forward;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // YOUR CODE HERE

    }

    public float GetAngleToClosestObstacle()
    {
        return angleToClosestObj;
    }

    public float GetLinearOutput()
    {
        return strength;
    }

    public virtual float GetGaussianOutput()
    {
        // YOUR CODE HERE
        throw new NotImplementedException();
    }

    public virtual float GetLogaritmicOutput()
    {
        // YOUR CODE HERE
        throw new NotImplementedException();
    }
}
