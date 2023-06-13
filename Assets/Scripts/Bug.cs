using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Bug : MonoBehaviour
{
    public float moveSpeed;
    public float moveRotationSpeed;
    public float rotateInPlaceSpeed;
    // public Leg leg1;
    // public Leg leg2;
    [Tooltip("left, right")]
    public Leg[] frontLegs = new Leg[2];
    [Tooltip("right, left")]
    public Leg[] midLegs = new Leg[2];
    [Tooltip("left, right")]
    public Leg[] hindLegs = new Leg[2];
    public Body body;
    public float max = .3f;
    public float min = .2f;
    void Start()
    {

    }

    // Update is called once per frame
    // [WORKING!] desired behavior 1: move in input direction (eg. up moves up, left moves left, up + left moves up/left)
    // [WORKING!] desired behavior 2: orient towards movement direction
    // [WORKING!] desired behavior 3: rotate bug body left/right with input
    // desired behavior 3.5: rotate body back towards 0 deg rotation if no input
    // desired behavior 4: limit body rotation to max leg extension? 
    void Update()
    {
        bool moving = false;
        float movementInputX = Input.GetAxis("Horizontal");
        float movementInputY = Input.GetAxis("Vertical");
        Vector3 movementInput = new Vector3(movementInputX, 0, movementInputY).normalized; // unity uses z axis for "up/down", whatever, I don't care
        if (movementInput != Vector3.zero)
        { // nb. float bullshit will make this not work, maybe fiddle with it
            moving = true;
            transform.position += moveSpeed * Time.deltaTime * movementInput;
        }
        if (moving)
        {
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, movementInput, Mathf.Deg2Rad * moveRotationSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection, Vector3.up);
            // this won't work; we need to adjust for the actual distance between the joints. 
            // instead find the offset for each footTarget from its center and compare those offsets.
            foreach (Leg[] legPair in new Leg[][] { frontLegs, midLegs, hindLegs })
            {
                Leg leg1 = legPair[0];
                Leg leg2 = legPair[1];
                Vector3 foot1Offset = leg1.footTarget.transform.position - leg1.footRangeCenter.position;
                Vector3 foot2Offset = leg2.footTarget.transform.position - leg2.footRangeCenter.position;
                float footOffsetDistance = Vector3.Distance(foot1Offset, foot2Offset);
                if (footOffsetDistance > leg1.range * max || footOffsetDistance < leg1.range * min)
                {
                    leg1.footTarget.transform.position = leg1.footRangeCenter.position + new Vector3(0, 0, leg1.range * .5f);
                    foot1Offset = leg1.footTarget.transform.position - leg1.footRangeCenter.position;
                    foot2Offset = leg2.footTarget.transform.position - leg2.footRangeCenter.position;
                    footOffsetDistance = Vector3.Distance(foot1Offset, foot2Offset);
                }
            }
        }
        float rotationInput = Input.GetAxis("Rotation");
        if (rotationInput != 0)
        {
            body.transform.rotation = Quaternion.AngleAxis(rotateInPlaceSpeed * Mathf.Sign(rotationInput) * Time.deltaTime, Vector3.up) * body.transform.rotation;
        }
        else
        {
            Vector3 newDirection = Vector3.RotateTowards(body.transform.forward, transform.forward, Mathf.Deg2Rad * rotateInPlaceSpeed * Time.deltaTime, 0.0f);
            body.transform.rotation = Quaternion.LookRotation(newDirection, Vector3.up);
        }
    }

    [ContextMenu("Reset legs")]
    void ResetLegs()
    {
        foreach (Leg[] legPair in new Leg[][] { frontLegs, midLegs, hindLegs })
        {
            Leg leg1 = legPair[0];
            Leg leg2 = legPair[1];
            leg1.footTarget.transform.position = leg1.footRangeCenter.position;
            leg2.footTarget.transform.position = leg2.footRangeCenter.position;
        }

    }
}
