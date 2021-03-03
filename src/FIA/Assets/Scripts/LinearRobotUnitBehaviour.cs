using System;
using System.Collections;
using UnityEngine;

public class LinearRobotUnitBehaviour : RobotUnit
{
    public float weightResource;
    public float resourceValue;
    public float resouceAngle;

    void Update()
    {

        // get sensor data
        resouceAngle = resourcesDetector.GetAngleToClosestResource();

        resourceValue = weightResource * resourcesDetector.GetLinearOuput();

        // apply to the ball
        applyForce(resouceAngle, resourceValue); // go towards

        

    }


}






