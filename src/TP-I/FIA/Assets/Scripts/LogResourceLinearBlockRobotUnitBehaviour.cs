using System;
using System.Collections;
using UnityEngine;

public class LogResourceLinearBlockRobotUnitBehaviour : RobotUnit
{

    public float weightResource = 1;
    public float resourceValue;
    public float resourceAngle;

    public float weightBlock = -1;
    public float blockValue;
    public float blockAngle;

    public float xmin = 0.0f;
    public float xmax = 1.0f;
    public float ymin = 0.0f;
    public float ymax = 1.0f;

    void Update()
    {
        // get sensor data
        resourceAngle = resourcesDetector.GetAngleToClosestResource();
        resourceValue = weightResource * resourcesDetector.GetLogarithmicOutput(xmin, xmax, ymin, ymax);

        blockAngle = blockDetector.GetAngleToClosestObstacle();
        blockValue = weightBlock * blockDetector.GetLinearOutput(xmin, xmax, ymin, ymax);

        // apply to the ball
        ApplyForce(resourceAngle, resourceValue);
        ApplyForce(blockAngle, blockValue);
    }

}