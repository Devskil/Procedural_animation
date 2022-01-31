using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : MonoBehaviour {
    //Head variables
    [SerializeField] Transform target;
    [SerializeField] Transform headBone;
    [SerializeField] float headMaxTurnAngle;
    [SerializeField] float headTrackingSpeed;

    //Eyes variables
    [SerializeField] Transform[] EyesBones;

    [SerializeField] float eyeTrackingSpeed;
    [SerializeField] float eyeMaxYRotation = 50;
    [SerializeField] float eyeMinYRotation = -50;
    [SerializeField] float turnSpeed;
    [SerializeField] float moveSpeed;
    [SerializeField] float turnAcceleration;
    [SerializeField] float moveAcceleration;
    [SerializeField] float minDistToTarget;
    [SerializeField] float maxDistToTarget;
    [SerializeField] float maxAngToTarget;

    Vector3 currentVelocity;
    float currentAngularVelocity;

    void Start() {
        if(target == null)
            return;
        if(headBone == null)
            return;
        if(EyesBones == null)
            return;
    }

    void LateUpdate() {
        HeadTrackingUpdate();
        EyesTrackingUpdate();
        RootMotionUpdate();
    }

    void HeadTrackingUpdate() {
        Quaternion currentLocalRotation = headBone.localRotation;
        headBone.localRotation = Quaternion.identity;

        Vector3 targetWorldLookDir = target.position - headBone.position;
        Vector3 targetLocalLookDir = headBone.InverseTransformDirection(targetWorldLookDir);


        targetLocalLookDir = Vector3.RotateTowards(
            Vector3.forward,
            targetLocalLookDir,
            Mathf.Deg2Rad * headMaxTurnAngle,
            0
        );


        Quaternion targetLocalRotation = Quaternion.LookRotation(targetLocalLookDir, Vector3.up);

        headBone.localRotation = Quaternion.Slerp(
            currentLocalRotation,
            targetLocalRotation, 
            1 - Mathf.Exp(-headTrackingSpeed * Time.deltaTime)
        );
    }

    void EyesTrackingUpdate() {
        Quaternion targetEyeRotation = Quaternion.LookRotation(
        target.position - headBone.position, 
        transform.up
        );

        foreach(var eye in EyesBones) {
            eye.rotation = Quaternion.Slerp(
            eye.rotation,
            targetEyeRotation,
            1 - Mathf.Exp(-eyeTrackingSpeed * Time.deltaTime)
            );
        }


        foreach(var eye in EyesBones) {
            float EyeCurrentYRotation = eye.localEulerAngles.y;
            if (EyeCurrentYRotation > 180) {
                EyeCurrentYRotation -= 360;
            }
            float EyeClampedYRotation = Mathf.Clamp(
            EyeCurrentYRotation,
            eyeMinYRotation,
            eyeMaxYRotation
            );
            eye.localEulerAngles = new Vector3(
            eye.localEulerAngles.x,
            EyeClampedYRotation,
            eye.localEulerAngles.z
            );    
        }
    }

    void RootMotionUpdate()
    {
       
        Vector3 towardTarget = target.position - transform.position;
        Vector3 towardTargetProjected = Vector3.ProjectOnPlane(towardTarget, transform.up);
        float angToTarget = Vector3.SignedAngle(transform.forward, towardTargetProjected, transform.up);

        float targetAngularVelocity = 0;
        if (Mathf.Abs(angToTarget) > maxAngToTarget)
        {
            if (angToTarget > 0)
            {
            targetAngularVelocity = turnSpeed;
            }
            else
            {
            targetAngularVelocity = -turnSpeed;
            }
        }

        currentAngularVelocity = Mathf.Lerp(
            currentAngularVelocity,
            targetAngularVelocity,
            1 - Mathf.Exp(-turnAcceleration * Time.deltaTime)
        );
        transform.Rotate(0, Time.deltaTime * currentAngularVelocity, 0, Space.World);


        Vector3 targetVelocity = Vector3.zero;

        if (Mathf.Abs(angToTarget) < 90)
        {
        float distToTarget = Vector3.Distance(transform.position, target.position);

        if (distToTarget > maxDistToTarget)
        {
            targetVelocity = moveSpeed * towardTargetProjected.normalized;
        }

        else if (distToTarget < minDistToTarget)
        {
            targetVelocity = moveSpeed * -towardTargetProjected.normalized;
        }
        }

        currentVelocity = Vector3.Lerp(
        currentVelocity,
        targetVelocity,
        1 - Mathf.Exp(-moveAcceleration * Time.deltaTime)
        );

        transform.position += currentVelocity * Time.deltaTime;


    }


}
