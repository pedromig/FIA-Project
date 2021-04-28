using System;
using System.Collections;
using UnityEngine;

public class GaussianRobotUnitBehaviour : RobotUnit
{

    public float weightResource = 1;
    public float resourceValue;
    public float resourceAngle;

    public float weightBlock = -1;
    public float blockValue;
    public float blockAngle;

    public float resourceSigma = 0.12f;
    public float blockSigma = 0.12f;

    public float resourceMean = 0.5f;
    public float blockMean = 0.5f;


    public float xmin = 0.0f;
    public float xmax = 1.0f;
    public float ymin = 0.0f;
    public float ymax = 1.0f;

    void Update()
    {

        // get sensor data
        resourceAngle = resourcesDetector.GetAngleToClosestResource();
        resourceValue = weightResource * resourcesDetector.GetGaussianOutput(xmin, xmax, ymin, ymax, resourceMean, resourceSigma);

        blockAngle = blockDetector.GetAngleToClosestObstacle();
        blockValue = weightBlock * blockDetector.GetGaussianOutput(xmin, xmax, ymin, ymax, blockMean, blockSigma);

        // apply to the ball
        ApplyForce(resourceAngle, resourceValue);
        ApplyForce(blockAngle, blockValue);

    }



}