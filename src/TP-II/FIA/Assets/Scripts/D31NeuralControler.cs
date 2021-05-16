using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class D31NeuralControler : MonoBehaviour
{
    public RobotUnit agent; // the agent controller we want to use
    public int player;
    public GameObject ball;
    public GameObject MyGoal;
    public GameObject AdversaryGoal;
    public GameObject Adversary;
    public GameObject ScoreSystem;


    public int numberOfInputSensores { get; private set; }
    public float[] sensorsInput;


    // Available Information 
    [Header("Environment  Information")]
    public List<float> distanceToBall;
    public List<float> distanceToMyGoal;
    public List<float> distanceToAdversaryGoal;
    public List<float> distanceToAdversary;
    public List<float> distancefromBallToAdversaryGoal;
    public List<float> distancefromBallToMyGoal;
    public List<float> distanceToClosestWall;
    // 
    public List<float> agentSpeed;
    public List<float> ballSpeed;
    public List<float> advSpeed;
    private float FIELD_SIZE = 95.0f;
    //
    public float simulationTime = 0;
    public float distanceTravelled = 0.0f;
    public float avgSpeed = 0.0f;
    public float maxSpeed = 0.0f;
    public float currentSpeed = 0.0f;
    public float currentDistance = 0.0f;
    public int hitTheBall;
    public int hitTheWall;
    public int fixedUpdateCalls = 0;
    //
    public float maxSimulTime = 30;
    public bool GameFieldDebugMode = false;
    public bool gameOver = false;
    public bool running = false;

    private Vector3 startPos;
    private Vector3 previousPos;
    private Vector3 ballStartPos;
    private Vector3 ballPreviousPos;
    private Vector3 advPreviousPos;
    private int SampleRate = 1;
    private int countFrames = 0;
    public int GoalsOnAdversaryGoal;
    public int GoalsOnMyGoal;
    public float[] result;

    public NeuralNetwork neuralController;

    private void Awake()
    {
        // get the unit controller
        agent = GetComponent<RobotUnit>();
        numberOfInputSensores = 18;
        sensorsInput = new float[numberOfInputSensores];

        startPos = agent.transform.localPosition;
        previousPos = startPos;
        // 2021
        ballPreviousPos = ball.transform.localPosition;
        if (Adversary != null)
        {
            advPreviousPos = Adversary.transform.localPosition;
        }

        //Debug.Log(this.neuralController);
        if (GameFieldDebugMode && this.neuralController.weights == null)
        {
            Debug.Log("creating nn..!! ONLY IN GameFieldDebug SCENE THIS SHOULD BE USED!");
            int[] top = { 12, 4, 2 };
            this.neuralController = new NeuralNetwork(top, 0);

        }
        distanceToBall = new List<float>();
        distanceToMyGoal = new List<float>();
        distanceToAdversaryGoal = new List<float>();
        distanceToAdversary = new List<float>();
        distancefromBallToAdversaryGoal = new List<float>();
        distancefromBallToMyGoal = new List<float>();
        distanceToClosestWall = new List<float>();
        agentSpeed = new List<float>();
        ballSpeed = new List<float>();
        advSpeed = new List<float>();
    }


    private void FixedUpdate()
    {
        if (countFrames == 0 && ball != null)
        {
            ballStartPos = ball.transform.localPosition;
            ballPreviousPos = ballStartPos;
        }


        simulationTime += Time.deltaTime;
        if (running && fixedUpdateCalls % 10 == 0)
        {
            // updating sensors
            SensorHandling();
            // move
            result = this.neuralController.process(sensorsInput);
            float angle = result[0] * 180;
            float strength = result[1];
            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.up;
            dir.z = dir.y;
            dir.y = 0;


            // debug raycast for the force and angle being applied on the agent
            Vector3 rayDirection = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.up;
            rayDirection.z = rayDirection.y;
            rayDirection.y = 0;

            if (strength > 0)
            {
                Debug.DrawRay(this.transform.position, -rayDirection.normalized * 5, Color.cyan);
            }
            else
            {
                Debug.DrawRay(this.transform.position, rayDirection.normalized * 5, Color.black);
            }

            agent.rb.AddForce(dir * strength * agent.speed);


            // updating game status
            updateGameStatus();


            // check method
            if (endSimulationConditions())
            {
                wrapUp();
            }
            countFrames++;
        }
        fixedUpdateCalls++;
    }

    // The ambient variables are created here!
    public void SensorHandling()
    {

        Dictionary<string, ObjectInfo> objects = agent.objectsDetector.GetVisibleObjects();
        sensorsInput[0] = objects["DistanceToBall"].distance / FIELD_SIZE;
        sensorsInput[1] = objects["DistanceToBall"].angle / 360.0f;
        sensorsInput[2] = objects["MyGoal"].distance / FIELD_SIZE;
        sensorsInput[3] = objects["MyGoal"].angle / 360.0f;
        sensorsInput[4] = objects["AdversaryGoal"].distance / FIELD_SIZE;
        sensorsInput[5] = objects["AdversaryGoal"].angle / 360;
        if (objects.ContainsKey("Adversary"))
        {
            sensorsInput[6] = objects["Adversary"].distance / FIELD_SIZE;
            sensorsInput[7] = objects["Adversary"].angle / 360.0f;
        }
        else
        {
            sensorsInput[6] = -1;// -1 == não existe
            sensorsInput[7] = -1;// -1 == não existe
        }
        sensorsInput[8] = Mathf.CeilToInt(Vector3.Distance(ball.transform.localPosition, MyGoal.transform.localPosition)) / FIELD_SIZE;
        sensorsInput[9] = Mathf.CeilToInt(Vector3.Distance(ball.transform.localPosition, AdversaryGoal.transform.localPosition)) / FIELD_SIZE;
        sensorsInput[10] = objects["Wall"].distance / FIELD_SIZE;
        sensorsInput[11] = objects["Wall"].angle / 360.0f;

        // Agent speed and angle with previous position
        Vector2 pp = new Vector2(previousPos.x, previousPos.z);
        Vector2 aPos = new Vector2(agent.transform.localPosition.x, agent.transform.localPosition.z);
        aPos = aPos - pp;
        sensorsInput[12] = aPos.magnitude / FIELD_SIZE;
        sensorsInput[13] = Vector2.Angle(aPos, Vector2.right) / 360.0f;
        // Ball speed and angle with previous position
        pp = new Vector2(ballPreviousPos.x, ballPreviousPos.z);
        aPos = new Vector2(ball.transform.localPosition.x, ball.transform.localPosition.z);
        aPos = aPos - pp;
        sensorsInput[14] = aPos.magnitude / FIELD_SIZE;
        sensorsInput[15] = Vector2.Angle(aPos.normalized, Vector2.right) / 360.0f;
        // Adversary Speed and angle with previous position
        if (objects.ContainsKey("Adversary"))
        {
            Vector2 adp = new Vector2(advPreviousPos.x, advPreviousPos.z);
            Vector2 adPos = new Vector2(Adversary.transform.localPosition.x, Adversary.transform.localPosition.z);
            adPos = adPos - adp;
            sensorsInput[16] = adPos.magnitude / FIELD_SIZE;
            sensorsInput[17] = Vector2.Angle(adPos, Vector2.right) / 360.0f;
        }
        else
        {
            sensorsInput[16] = -1;
            sensorsInput[17] = -1;
        }

        if (countFrames % SampleRate == 0)
        {
            distanceToBall.Add(sensorsInput[0]);
            distanceToMyGoal.Add(sensorsInput[2]);
            distanceToAdversaryGoal.Add(sensorsInput[4]);
            distanceToAdversary.Add(sensorsInput[6]);
            distancefromBallToMyGoal.Add(sensorsInput[8]);
            distancefromBallToAdversaryGoal.Add(sensorsInput[9]);
            distanceToClosestWall.Add(sensorsInput[10]);
            // 
            agentSpeed.Add(sensorsInput[12]);
            ballSpeed.Add(sensorsInput[14]);
            advSpeed.Add(sensorsInput[16]);
        }
    }


    public void updateGameStatus()
    {
        // This is the information you can use to build the fitness function. 
        Vector2 pp = new Vector2(previousPos.x, previousPos.z);
        Vector2 aPos = new Vector2(agent.transform.localPosition.x, agent.transform.localPosition.z);
        currentDistance = Vector2.Distance(pp, aPos);
        distanceTravelled += currentDistance;
        hitTheBall = agent.hitTheBall;
        hitTheWall = agent.hitTheWall;

        // update positions!
        previousPos = agent.transform.localPosition;
        ballPreviousPos = ball.transform.localPosition;
        if (Adversary != null)
        {
            advPreviousPos = Adversary.transform.localPosition;
        }

        // get my score
        GoalsOnMyGoal = ScoreSystem.GetComponent<ScoreKeeper>().score[player == 0 ? 1 : 0];
        // get adversary score
        GoalsOnAdversaryGoal = ScoreSystem.GetComponent<ScoreKeeper>().score[player];


    }

    public void wrapUp()
    {
        avgSpeed = avgSpeed / simulationTime;
        gameOver = true;
        running = false;
        countFrames = 0;
        fixedUpdateCalls = 0;
    }


    public static float StdDev(IEnumerable<float> values)
    {
        float ret = 0;
        int count = values.Count();
        if (count > 1)
        {
            //Compute the Average
            float avg = values.Average();

            //Perform the Sum of (value-avg)^2
            float sum = values.Sum(d => (d - avg) * (d - avg));

            //Put it all together
            ret = Mathf.Sqrt(sum / count);
        }
        return ret;
    }


    //******************************************************************************************
    //* FITNESS FUNCTION UTILITIES 
    //******************************************************************************************

    // Cossine Law
    // DESCRIPTION:
    // In trigonometry, the law of cosines relates the lengths of the sides of a triangle to the cosine of one of its angles.
    // The formula for this law is  c^2 = a^2 + b^2 - 2*a*b * cos(alpha) and it used in the fitness function as a way to 
    // reward or penalize the agent according to the value of the angle he makes with the balls.
    private float CossineLawForAngle(float a, float b, float c)
    {

        // Positive Reinforcement -> 180 degrees
        if (a == 0 || b == 0)
            return Mathf.PI;

        List<float> sides = new List<float>(new float[] { a, b, c });
        sides.Sort();

        if (!(sides[2] < sides[0] + sides[1]))
        {
            float newValue = sides[2] - (sides[0] + sides[1]) + 0.00001f;
            if (a == sides[0])
            {
                a += newValue;
            }
            else if (b == sides[0])
            {
                b += newValue;
            }
            else if (c == sides[0])
            {
                c += newValue;
            }
        }
        return Mathf.Acos((a * a + b * b - c * c) / (2 * a * b));
    }

    //******************************************************************************************
    //* FITNESS FUNCTION IMPLEMENTATION 
    //******************************************************************************************

    // Defender fitness function
    // This function is used as a fitness indicator for a attacker agent.
    private float GetAttackerFitness()
    {
        float angleDegree, orientationScore = 0;
        float epsilon = 160, phi = 180 - epsilon;

        for (int i = 0; i < distanceToBall.Count(); ++i)
        {
            angleDegree = (CossineLawForAngle(distancefromBallToAdversaryGoal[i], distanceToBall[i], distanceToAdversaryGoal[i]) * 180) / Mathf.PI;
            if (angleDegree < epsilon)
            {
                orientationScore += -(-1 / epsilon * angleDegree + 1);
            }
            else
            {
                orientationScore += (1 / phi * angleDegree + (-epsilon / phi));
            }
        }

        // "Average" Score (distanceToBall.Count() for the number of the taken snapshots)
        orientationScore = orientationScore / distanceToBall.Count();

        return 50 * orientationScore
                + 10000 * GoalsOnAdversaryGoal
                - 10000 * GoalsOnMyGoal
                - 7000 * (GoalsOnAdversaryGoal == 0 ? 1 : 0)
                + 5 * (hitTheBall > 0 ? Mathf.Log10(hitTheBall) : 0)
                + 5 / distancefromBallToAdversaryGoal.Average()
                - 5 / distancefromBallToMyGoal.Average()
                + 5 / distanceToBall.Average();
    }


    // Defender fitness function
    // This function is used as a fitness indicator for a defender agent.
    private float GetDefenderFitness()
    {
        float angleDegree, orientationScore = 0;
        float epsilon = 175, phi = 180 - epsilon;

        for (int i = 0; i < distanceToBall.Count(); ++i)
        {
            angleDegree = (CossineLawForAngle(distancefromBallToAdversaryGoal[i], distanceToBall[i], distanceToAdversaryGoal[i]) * 180) / Mathf.PI;
            if (angleDegree < epsilon)
            {
                orientationScore += -(-1 / epsilon * angleDegree + 1);
            }
            else
            {
                orientationScore += (1 / phi * angleDegree + (-epsilon / phi));
            }
        }

        // "Average" Score (distanceToBall.Count() for the number of the taken snapshots)
        orientationScore = orientationScore / distanceToBall.Count();

        return 1000 * orientationScore
                - 10000 * GoalsOnMyGoal
                + 10000 * (GoalsOnMyGoal == 0 ? 1 : 0)
                + 50 * hitTheBall
                + 5 / distancefromBallToAdversaryGoal.Average()
                - 50 / distancefromBallToMyGoal.Average()
                + 50 / distanceToBall.Average()
                + 100 * avgSpeed;
    }


    // Experimental fitness function
    // This function is used to test ideias for a fitness function. Override it so it fits your needs.
    private float GetExperimentalFitness()
    {
        return 2000 * GoalsOnAdversaryGoal
               + 500 * hitTheBall
               + 50 / distanceToBall.Average()
               + 50 / distanceToAdversaryGoal.Average()
               + 50 * avgSpeed
               - 2000 * GoalsOnMyGoal
               - 1000 * (GoalsOnAdversaryGoal == 0 ? 1 : 0)
               - 500 * hitTheWall;
    }

    //******************************************************************************************
    //* FITNESS AND END SIMULATION CONDITIONS 
    //******************************************************************************************
    private bool endSimulationConditions()
    {
        return simulationTime > this.maxSimulTime;
    }

    // Fitness function selection
    private float GetFitness(MetaHeuristic.FitnessType fitnessFunction)
    {
        switch (fitnessFunction)
        {
            case MetaHeuristic.FitnessType.Attacker:
                return GetAttackerFitness();
            case MetaHeuristic.FitnessType.Defender:
                return GetDefenderFitness();
            case MetaHeuristic.FitnessType.Experimental:
                return GetExperimentalFitness();
            case MetaHeuristic.FitnessType.None:
                throw new System.Exception("404 fitness function not found");
        }
        return -float.MinValue;
    }

    // Fitness function for the Blue player
    public float GetScoreBlue(MetaHeuristic.FitnessType fitnessFunction)
    {
        return GetFitness(fitnessFunction);
    }

    // Fitness function for the Red player
    public float GetScoreRed(MetaHeuristic.FitnessType fitnessFunction)
    {
        return GetFitness(fitnessFunction);
    }

}