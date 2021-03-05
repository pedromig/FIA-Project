using System;
using System.Collections;
using UnityEngine;

public class LinearRobotUnitBehaviour : RobotUnit
{
    public float weightResource;
    public float resourceValue;
    public float resourceAngle;

    void Update()
    {

        // get sensor data
        resourceAngle = resourcesDetector.GetAngleToClosestResource();

        resourceValue = weightResource * resourcesDetector.GetLinearOutput();

        // apply to the ball
        ApplyForce(resourceAngle, resourceValue); // go towards

        

    }


}






