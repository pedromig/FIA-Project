using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DetectorScript : MonoBehaviour
{
    public float angleOfSensors = 10.0f;
    public float rangeOfSensors = 10.0f;
    protected Vector3 initialTransformUp;
    protected Vector3 initialTransformFwd;
    public float strength;
    public float angleToClosestObject;
    public int numObjects;
    public bool debugMode;
    private void Start()
    {
        initialTransformUp = this.transform.up;
        initialTransformFwd = this.transform.forward;
    }


    public float GetLinearOutput(float xmin, float xmax, float ymin, float ymax)
    {   
        float energy = ymin;
        if ((xmin <= strength) && (strength <= xmax))
        {
            energy = strength;
            if (energy >= ymax)
                energy = ymax;
            else if (energy <= ymin)
                energy = ymin;
        }
        return energy;
    }

    
    private float Gaussian(float m, float s)
    {
        return s != 0 ? (float)(Math.Pow(Math.E, -0.5 * Math.Pow((strength - m) / (s), 2)) / (s * Math.Sqrt(2 * Math.PI))) : 0;
    }

    public float GetGaussianOutput(float xmin, float xmax, float ymin, float ymax, float mean, float sigma)
    {
        float energy = ymin;
        if ((xmin <= strength) && (strength <= xmax))
        {
            energy = Gaussian(mean, sigma);
            if (energy >= ymax)
                energy = ymax;
            else if (energy <= ymin)
                energy = ymin;
        }
        return energy;
    }

    private float InverseLog(float x)
    {
        return (x > 0 ? -(float) Math.Log(x) : 0);
    }

    public float GetLogarithmicOutput(float xmin, float xmax, float ymin, float ymax)
    {
        float energy = ymin;
        if ((xmin <= strength) && (strength <= xmax))
        {
            energy = InverseLog(strength);  
            if (energy >= ymax)
                energy = ymax;
            else if (energy <= ymin)
                energy = ymin;
        }
        return energy;
    }

    public List<ObjectInfo> GetVisibleObjects(string objectTag)
    {
        RaycastHit hit;
        List<ObjectInfo> objectsInformation = new List<ObjectInfo>();

        for (int i = 0; i * angleOfSensors < 360f; i++)
        {
            if (Physics.Raycast(this.transform.position, Quaternion.AngleAxis(-angleOfSensors * i, initialTransformUp) * initialTransformFwd, out hit, rangeOfSensors))
            {

                if (hit.transform.gameObject.CompareTag(objectTag))
                {
                    if (debugMode)
                    {
                        Debug.DrawRay(this.transform.position, Quaternion.AngleAxis((-angleOfSensors * i), initialTransformUp) * initialTransformFwd * hit.distance, Color.red);
                    }
                    ObjectInfo info = new ObjectInfo(hit.distance, angleOfSensors * i + 90);
                    objectsInformation.Add(info);
                }
            }
        }

        objectsInformation.Sort();

        return objectsInformation;
    }

    private void LateUpdate()
    {
        this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, this.transform.parent.rotation.z * -1.0f);

    }
}