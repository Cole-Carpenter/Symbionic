using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spline : MonoBehaviour {

    public Transform[] controlPoints;

    private Color[] colorsArray = { Color.white, Color.red, Color.blue, Color.magenta, Color.black };
    private float totalLength;
    private int pos = 1;
    private float offset = 0;

    /* void OnDrawGizmos()
     {
         Gizmos.color = Color.white;

         //Draw the Catmull-Rom spline between the points
         for (int i = 0; i < controlPoints.Length; i++)
         {
             //Cant draw between the endpoints
             //Neither do we need to draw from the second to the last endpoint
             //...if we are not making a looping line
             if ((i == 0 || i == controlPoints.Length - 2 || i == controlPoints.Length - 1))
             {
                 continue;
             }

             pos = i;

             DivideCurveIntoSteps();
         }
         pos = 1;
     }*/

    private void Start()
    {
        totalLength = SimpsonsRule(0f, 1f);
    }

    Vector3 GetCatmullRomPosition(float t)
    {
        Vector3 p0 = controlPoints[pos - 1].position;
        Vector3 p1 = controlPoints[pos].position;
        Vector3 p2 = controlPoints[pos + 1].position;
        Vector3 p3 = controlPoints[pos + 2].position;

        //The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
        Vector3 a = 2f * p1;
        Vector3 b = p2 - p0;
        Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

        //The cubic polynomial: a + b * t + c * t^2 + d * t^3
        Vector3 position = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

        return position;
    }

    void DivideCurveIntoSteps()
    {
        Vector3 p0 = controlPoints[pos - 1].position;
        Vector3 p1 = controlPoints[pos].position;
        Vector3 p2 = controlPoints[pos + 1].position;
        Vector3 p3 = controlPoints[pos + 2].position;

        //How many sections do we want to divide the curve into
        int parts = 10;

        //What's the length of one section?
        float sectionLength = totalLength / (float)parts;

        //Init the variables we need in the loop
        float currentDistance = 0f + sectionLength;

        //The curve's start position
        Vector3 lastPos = p1;

        //The Bezier curve's color
        //Need a seed or the line will constantly change color
        Random.InitState(12345);

        int lastRandom = Random.Range(0, colorsArray.Length);

        for (int i = 1; i <= parts; i++)
        {
            //Use Newton–Raphsons method to find the t value from the start of the curve 
            //to the end of the distance we have
            Vector3 position = ConstantSpline(currentDistance);


            //Draw the line with a random color
            int newRandom = Random.Range(0, colorsArray.Length);

            //Get a different random number each time
            while (newRandom == lastRandom)
            {
                newRandom = Random.Range(0, colorsArray.Length);
            }

            lastRandom = newRandom;

            Gizmos.color = colorsArray[newRandom];

            Gizmos.DrawLine(lastPos, position);


            //Save the last position
            lastPos = position;

            //Add to the distance traveled on the line so far
            currentDistance += sectionLength;
        }
    }
    
    private float SimpsonsRule(float t0, float t1)
    {
        //This is the resolution and has to be even
        int n = 20;

        //Now we need to divide the curve into sections
        float delta = (t1-t0) / (float)n;

        //The main loop to calculate the length

        //Everything multiplied by 1
        float endPoints = ArcLengthIntegrand(t0) + ArcLengthIntegrand(t1);

        //Everything multiplied by 4
        float x4 = 0f;
        for (int i = 1; i < n; i += 2)
        {
            float t = t0 + delta * i;

            x4 += ArcLengthIntegrand(t);
        }

        //Everything multiplied by 2
        float x2 = 0f;
        for (int i = 2; i < n; i += 2)
        {
            float t = t0 + delta * i;

            x2 += ArcLengthIntegrand(t);
        }

        //The final length
        float length = (delta / 3f) * (endPoints + 4f * x4 + 2f * x2);

        return length;
    }
    
    private Vector3 CatmullDerivative(float t)
    {
        Vector3 p0 = controlPoints[pos - 1].position;
        Vector3 p1 = controlPoints[pos].position;
        Vector3 p2 = controlPoints[pos + 1].position;
        Vector3 p3 = controlPoints[pos + 2].position;

        //The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
        Vector3 b = p2 - p0;
        Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

        //The cubic polynomial derivative: b + c * t + d * t^2
        Vector3 position = 0.5f * (b + 2 * (c * t) + 3 * (d * t * t));

        return position;
    }

    private float ArcLengthIntegrand(float t)
    {

        //The derivative at this point (the velocity vector)
        Vector3 dPos = CatmullDerivative(t);

        float integrand = dPos.magnitude;

        return integrand;
    }

    private Vector3 ConstantSpline(float t)
    { 
        float d = totalLength * t;

        //Need an error so we know when to stop the iteration
        float error = 0.001f;

        //We also need to avoid infinite loops
        int iterations = 0;

        while (true)
        {
            //Newton's method
            float tNext = t - ((SimpsonsRule(0f, t) - d) / ArcLengthIntegrand(t));

            //Have we reached the desired accuracy?
            if (Mathf.Abs(tNext - t) < error)
            {
                break;
            }

            t = tNext;

            iterations += 1;

            if (iterations > 1000)
            {
                break;
            }
        }

        return GetCatmullRomPosition(t);
    }

    public Vector3 GetStart()
    {
        return controlPoints[pos].position;
    }

    public int GetNumPoints()
    {
        return controlPoints.Length;
    }

    public Vector3 GetNextPoint()
    {
        return controlPoints[pos + 1].position;
    }

    public Vector3 SplineMove(float t, Transform moving_body)
    {
        if ((moving_body.position - controlPoints[pos + 1].position).magnitude < 0.8f)
        {
            pos++;
            offset = t;
        }
        if (pos == controlPoints.Length - 2)
        {
            return moving_body.position;
        }
        return ConstantSpline(t - offset);
    }
}
