using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[ExecuteAlways]
public class Leg : MonoBehaviour
{
    public enum LEG_INDICES { hip = 0, knee = 1, ankle = 2 } // it's 11:30 at night I don't care what they're called
    public enum LEG_SIDE { left = -1, right = 1 }
    public enum KNEE_BEND_DIRECTION { down = -1, up = 1 }

    public Transform footTarget;
    public Transform footRangeCenter;
    public LineRenderer legVisual;
    [Tooltip("thigh, shin, foot")]
    public float[] segmentLengths = new float[3];
    public LEG_SIDE legSide;
    public KNEE_BEND_DIRECTION kneeBendDirection;
    public bool kneeUp;
    public float DEBUG_restingDistance;
    public float DEBUG_maxRange;
    public float DEBUG_currentDistance;
    public float maxExtension;
    public float range;
    public float moveSpeed;
    void Start()
    {
        footTarget.position = footRangeCenter.position;
        DEBUG_restingDistance = Vector3.Distance(transform.position, footRangeCenter.position);
        DEBUG_maxRange = Vector3.Distance(transform.position, footRangeCenter.position) + range;
        segmentLengths[0] = (Vector3.Distance(transform.position, footRangeCenter.position) / 2) + range * .4f;
        segmentLengths[1] = (Vector3.Distance(transform.position, footRangeCenter.position) / 2) + range * .4f;
        maxExtension = segmentLengths[0] + segmentLengths[1];
    }

    // Update is called once per frame
    void Update()
    {
        legVisual.SetPosition(0, transform.position); // hard set bc this is the attach point
        Vector3 footPosition = legVisual.GetPosition((int)LEG_INDICES.ankle);
        // legVisual.SetPosition(footIdx, Vector3.MoveTowards(footPosition, footTarget.position, moveSpeed * Time.deltaTime));
        for (int i = legVisual.positionCount - 1; i >= 0; i--)
        {
            Vector3 idealPointPosition = transform.position;
            switch (i)
            {
                case (int)LEG_INDICES.hip:
                    idealPointPosition = transform.position;
                    break;
                case (int)LEG_INDICES.knee: // TODO: make this intelligible
                    Vector3 lineMidpoint = Vector3.Lerp(footPosition, legVisual.GetPosition((int)LEG_INDICES.hip), .5f);
                    DEBUG_currentDistance = Vector3.Distance(legVisual.GetPosition((int)LEG_INDICES.ankle), legVisual.GetPosition((int)LEG_INDICES.hip));
                    idealPointPosition = lineMidpoint +
                    Vector3.Cross( // returns a line parallel to BOTH the line made between ankle and hip and also the up vector
                       legVisual.GetPosition((int)LEG_INDICES.ankle) - legVisual.GetPosition((int)LEG_INDICES.hip),
                      Vector3.up
                    ).normalized
                    * (int)kneeBendDirection * (int)legSide
                    * Mathf.Lerp( // return a value between 0 and thigh length, based on how close together the ankle and hip are
                      segmentLengths[0], 0,
                      // e.g. if thigh and knee length are both .25, then we should return .25 if the 
                      (Vector3.Distance(legVisual.GetPosition((int)LEG_INDICES.ankle), legVisual.GetPosition((int)LEG_INDICES.hip)) / (segmentLengths[0] + segmentLengths[1])));
                    break;
                case (int)LEG_INDICES.ankle:
                    idealPointPosition = Vector3.MoveTowards(footPosition, footTarget.position, moveSpeed * Time.deltaTime);
                    break;
            }
            legVisual.SetPosition(i, idealPointPosition);
            // // points extend from body towards foot
            // // the nearer they are to the body, the closer their ideal point is to it
            // float lerpValue = i == footIdx ? 0 : 1f / (footIdx - i);
            // Vector3 idealPointPosition = Vector3.Lerp(footTarget.position, transform.position, lerpValue);
            // Debug.Log("footTargetPosition: " + footTarget.position + ", joint position " + transform.position + ", lerp value " + lerpValue + ", ideal pos " + idealPointPosition);
            // legVisual.SetPosition(i, Vector3.MoveTowards(footPosition, idealPointPosition, moveSpeed * (Time.deltaTime / i)));
        }
        if (Vector3.Distance(footTarget.position, footRangeCenter.position) > range)
        {
            // point 1 is footposition
            // point 2 is transform position
            // x3 = (x1+x2+y2-y1) / 2
            // y3 = (x1-x2+y1+y2) / 2
            float x3 = (footPosition.x + transform.position.x + transform.position.z - footPosition.z) / 2;
            float y3 = (footPosition.x - transform.position.x + transform.position.z + footPosition.z) / 2;
            footTarget.position += (footRangeCenter.position - footTarget.position) * 1.9f;
            // Vector3 rightAngleSpot = new Vector3(x3, 0, y3);
            // footTarget.position += (rightAngleSpot - footTarget.position) * 1.9f;
        }
    }

    public bool IsOverextended()
    {
        return Vector3.Distance(legVisual.GetPosition(0), legVisual.GetPosition(2)) > maxExtension;
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(footRangeCenter.position, range);
        Vector3 footPosition = legVisual.GetPosition(1);
        float x3 = (footPosition.x + transform.position.x + transform.position.z - footPosition.z) / 2;
        float y3 = (footPosition.x - transform.position.x + transform.position.z + footPosition.z) / 2;
        Gizmos.DrawLine(transform.position, new Vector3(x3, 0, y3));
    }
}
