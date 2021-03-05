using System;
using System.Collections;
using UnityEngine;

public class LinearRobotUnitBehaviour : RobotUnit
{
    public float weightResource = 1;
    public float resourceValue;
    public float resourceAngle;
    
    public float weightBlock = -1;
    public float blockValue;
    public float blockAngle;

    void Update()
    {
        // get sensor data
        resourceAngle = resourcesDetector.GetAngleToClosestResource();
        resourceValue = weightResource * resourcesDetector.GetLinearOutput();
        
        blockAngle = blockDetector.GetAngleToClosestObstacle();
        blockValue = weightBlock * blockDetector.GetLinearOutput();
        
        // apply to the ball
        ApplyForce(resourceAngle, resourceValue); // go towards
        ApplyForce(blockAngle, blockValue);

    }


}






