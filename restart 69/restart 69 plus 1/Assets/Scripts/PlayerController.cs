using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject bar1, leftArm, rightArm, torso, head, leftFemur, rightFemur, leftLowerLeg, rightLowerLeg, leftFoot, rightFoot; // get GameObjects from editor
    HingeJoint leftBarJoint, rightBarJoint, leftShoulder, rightShoulder, neck, leftHip, rightHip, leftKnee, rightKnee, leftAnkle, rightAnkle; // init player joints 
    JointSpring leftShoulderSpring, rightShoulderSpring, neckSpring, leftHipSpring, rightHipSpring, leftKneeSpring, rightKneeSpring, leftAnkleSpring, rightAnkleSpring; // init spring for each joint

    public Transform leftHand, rightHand; // used to check distance for regrabs

    public float strength, damp; // determine strength of noomi. strength is set dynamically, damp stays the same

    bool onBar; // tells whether noomi is currently swinging, used for regrabs and setting body positions

    bool distanceThreshold; // used so that noomi doesn't grab the bar again as soon as you let go

    GameObject lastBarGrabbed; // last bar noomi was on, used for regrabs

    // declare joints and springs, set damper
    void initJoints()
    {
        // declare joints
        leftBarJoint = leftArm.GetComponent<HingeJoint>();
        rightBarJoint = rightArm.GetComponent<HingeJoint>();
        leftShoulder = torso.GetComponents<HingeJoint>()[0];
        rightShoulder = torso.GetComponents<HingeJoint>()[1];
        neck = head.GetComponent<HingeJoint>();
        leftHip = leftFemur.GetComponent<HingeJoint>();
        rightHip = rightFemur.GetComponent<HingeJoint>();
        leftKnee = leftLowerLeg.GetComponent<HingeJoint>();
        rightKnee = rightLowerLeg.GetComponent<HingeJoint>();
        leftAnkle = leftFoot.GetComponent<HingeJoint>();
        rightAnkle = rightFoot.GetComponent<HingeJoint>();

        // declare springs
        leftShoulderSpring = leftShoulder.spring;
        rightShoulderSpring = rightShoulder.spring;
        neckSpring = neck.spring;
        leftHipSpring = leftHip.spring;
        rightHipSpring = rightHip.spring;
        leftKneeSpring = leftKnee.spring;
        rightKneeSpring = rightKnee.spring;
        leftAnkleSpring = leftAnkle.spring;
        rightAnkleSpring = rightAnkle.spring;

        // set damper because i don't plan on changing it dynamically
        leftShoulderSpring.damper = damp;
        rightShoulderSpring.damper = damp;
        neckSpring.damper = damp;
        leftHipSpring.damper = damp;
        rightHipSpring.damper = damp;
        leftKneeSpring.damper = damp;
        rightKneeSpring.damper = damp;
        leftAnkleSpring.damper = damp;
        rightAnkleSpring.damper = damp;
    }

    // Start is called before the first frame update
    void Start()
    {
        initJoints();

        // set variables to do regrabs / set body position correctly
        onBar = true;
        distanceThreshold = false;
        lastBarGrabbed = bar1;
    }

    // FixedUpdate is called once every physics frame
    void FixedUpdate()
    {
        checkRegrabs();

        // logic to determine which body position / state noomi should be in
        if (Input.GetKey(KeyCode.UpArrow))
        {
            // let go
            Destroy(leftBarJoint);
            Destroy(rightBarJoint);
            onBar = false;

            // make physics engine detect collisions between arms and bar again
            Physics.IgnoreCollision(lastBarGrabbed.GetComponent<Collider>(), leftArm.GetComponent<Collider>(), false);
            Physics.IgnoreCollision(lastBarGrabbed.GetComponent<Collider>(), rightArm.GetComponent<Collider>(), false);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            // arch
            if (onBar)
            {
                // normal strength
                setBodyPosition(-20, 1, 30, 2, -20, 1, -40, 2, -30, 2);
            }
            else
            {
                // extra strength
                setBodyPosition(-20, 1, 30, 2, -20, 3, -40, 2, -30, 2);
            }
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            // tuck
            if (onBar)
            {
                // normal strength
                setBodyPosition(120, 1, -80, 2, 150, 1, -120, 2, 30, 2);
            }
            else
            {
                // extra strength so he can actually pull it in the air
                setBodyPosition(150, 3, -80, 2, 150, 4, -120, 2, 30, 2);
            }

        }
        else
        {
            // default
            if (onBar || !distanceThreshold) // if close to the bar, hold the pike (helpful for some moves)
            {
                // pull up pike thing
                setBodyPosition(150, 1, 0, 2, 120, 1, 0, 2, 30, 2);
            }
            else
            {
                // landing
                setBodyPosition(90, 1, 0, 2, 60, 1, -90, 2, 30, 2);
            }

        }
    }

    // check if any bars are close enough to regrab (and that the player left the last bar), if so call regrab function
    void checkRegrabs()
    {
        foreach (GameObject bar in GameObject.FindGameObjectsWithTag("bar"))
        {
            float leftHandDistance = Vector3.Distance(leftHand.position, bar.transform.position);
            float rightHandDistance = Vector3.Distance(leftHand.position, bar.transform.position);
            if (bar == lastBarGrabbed)
            {
                // must use distance threshold so noomi doesn't grab again when you try to let go
                if (leftHandDistance > 1 && rightHandDistance > 1)
                {
                    distanceThreshold = true;
                }
                if (distanceThreshold && leftHandDistance < 0.4 && rightHandDistance < 0.4)
                {
                    regrab(bar);
                }
            }
            else
            {
                // don't need distance threshold
                if (leftHandDistance < 0.4 && rightHandDistance < 0.4)
                {
                    regrab(bar);
                }
            }
        }
    }

    // creates the appearance of regrabbing by creating a joint near the bar and moving it into the middle 
    void regrab(GameObject bar)
    {
        // ignore physics collisions between arms and bar
        Physics.IgnoreCollision(bar.GetComponent<Collider>(), leftArm.GetComponent<Collider>(), true);
        Physics.IgnoreCollision(bar.GetComponent<Collider>(), rightArm.GetComponent<Collider>(), true);
        // create joints between arms and bar
        leftBarJoint = leftArm.AddComponent<HingeJoint>();
        leftBarJoint.anchor = new Vector3(0, 1, 0);
        leftBarJoint.axis = new Vector3(0, 0, 1);
        leftBarJoint.autoConfigureConnectedAnchor = false;
        rightBarJoint = rightArm.AddComponent<HingeJoint>();
        rightBarJoint.anchor = new Vector3(0, 1, 0);
        rightBarJoint.axis = new Vector3(0, 0, 1);
        rightBarJoint.autoConfigureConnectedAnchor = false;
        // move joints to proper position
        leftBarJoint.connectedAnchor = new Vector3(bar.transform.position.x, bar.transform.position.y, 0.3f);
        rightBarJoint.connectedAnchor = new Vector3(bar.transform.position.x, bar.transform.position.y, -0.3f);
        // reset variables
        lastBarGrabbed = bar;
        distanceThreshold = false;
        onBar = true;
    }

    // set the angle of the player joints to match a desired body position
    void setBodyPosition(float shoulderAngle, float shoulderStrength, float neckAngle, float neckStrength, float hipAngle, float hipStrength, float kneeAngle, float kneeStrength, float ankleAngle, float ankleStrength)
    {
        // for each joint, set target position, strength, and then reset the spring with the new attributes
        leftShoulderSpring.targetPosition = shoulderAngle;
        leftShoulderSpring.spring = strength * shoulderStrength;
        leftShoulder.spring = leftShoulderSpring;
        rightShoulderSpring.targetPosition = shoulderAngle;
        rightShoulderSpring.spring = strength * shoulderStrength;
        rightShoulder.spring = rightShoulderSpring;

        neckSpring.targetPosition = neckAngle;
        neckSpring.spring = strength * neckStrength;
        neck.spring = neckSpring;

        leftHipSpring.targetPosition = hipAngle;
        leftHipSpring.spring = strength * hipStrength;
        leftHip.spring = leftHipSpring;
        rightHipSpring.targetPosition = hipAngle;
        rightHipSpring.spring = strength * hipStrength;
        rightHip.spring = rightHipSpring;

        leftKneeSpring.targetPosition = kneeAngle;
        leftKneeSpring.spring = strength * kneeStrength;
        leftKnee.spring = leftKneeSpring;
        rightKneeSpring.targetPosition = kneeAngle;
        rightKneeSpring.spring = strength * kneeStrength;
        rightKnee.spring = rightKneeSpring;

        leftAnkleSpring.targetPosition = ankleAngle;
        leftAnkleSpring.spring = strength * ankleStrength;
        leftAnkle.spring = leftAnkleSpring;
        rightAnkleSpring.targetPosition = ankleAngle;
        rightAnkleSpring.spring = strength * ankleStrength;
        rightAnkle.spring = rightAnkleSpring;
    }
}