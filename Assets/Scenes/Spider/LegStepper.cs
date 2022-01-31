using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegStepper : MonoBehaviour
{
    [SerializeField] Transform homeTransform;
    [SerializeField] float wantStepAtDistance;
    [SerializeField] float moveDuration;

    [SerializeField] LegStepper frontLeftLegStepper;
    [SerializeField] LegStepper frontRightLegStepper;
    [SerializeField] LegStepper backLeftLegStepper;
    [SerializeField] LegStepper backRightLegStepper;
    public bool Moving;
    [SerializeField] float stepOvershootFraction;

    IEnumerator Move()
    {
    Moving = true;

    Vector3 startPoint = transform.position;
    Quaternion startRot = transform.rotation;
    Quaternion endRot = homeTransform.rotation;
    Vector3 towardHome = (homeTransform.position - transform.position); 
    float overshootDistance = wantStepAtDistance * stepOvershootFraction;
    Vector3 overshootVector = towardHome * overshootDistance;
    overshootVector = Vector3.ProjectOnPlane(overshootVector, Vector3.up);

    Vector3 endPoint = homeTransform.position + overshootVector;

    Vector3 centerPoint = (startPoint + endPoint) / 2;

    centerPoint += homeTransform.up * Vector3.Distance(startPoint, endPoint) / 1.25f;

    float timeElapsed = 0;
    do
    {
        timeElapsed += Time.deltaTime;
        float normalizedTime = timeElapsed / moveDuration;


        transform.position =
        Vector3.Lerp(
            Vector3.Lerp(startPoint, centerPoint, normalizedTime),
            Vector3.Lerp(centerPoint, endPoint, normalizedTime),
            normalizedTime
        );

        transform.rotation = Quaternion.Slerp(startRot, endRot, normalizedTime);

        yield return null;
    }
    while (timeElapsed < moveDuration);

    Moving = false;
    }

    public void TryMove()
    {
        if (Moving) return;

        float distFromHome = Vector3.Distance(transform.position, homeTransform.position);
        if (distFromHome > wantStepAtDistance)
        {
            StartCoroutine(Move());
        }
    }

    IEnumerator LegUpdateCoroutine()
    {
        while (true)
        {
            do
            {
            frontLeftLegStepper.TryMove();
            backRightLegStepper.TryMove();
            yield return null;
            } while (backRightLegStepper.Moving || frontLeftLegStepper.Moving);
            do
            {
            frontRightLegStepper.TryMove();
            backLeftLegStepper.TryMove();
            yield return null;
            } while (backLeftLegStepper.Moving || frontRightLegStepper.Moving);
        }
    }

    void Awake()
    {
        StartCoroutine(LegUpdateCoroutine());
    }
}
