using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKfoot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        leftFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit leftHit;
        RaycastHit rightHit;

        Vector3 lPos = leftFoot.TransformPoint(Vector3.zero);
        Vector3 rPos = rightFoot.TransformPoint(Vector3.zero);

        if (Physics.Raycast(lPos, -Vector3.up, out leftHit, 1))
        {
            lFPos = leftHit.point;
            lFRot = Quaternion.FromToRotation(transform.up, leftHit.normal) * transform.rotation;
        }
        if (Physics.Raycast(rPos, -Vector3.up, out rightHit, 1))
        {
            rFPos = rightHit.point;
            rFRot = Quaternion.FromToRotation(transform.up, rightHit.normal) * transform.rotation;
        }
    }

    void OnAnimatorIK()
    {
        lFWeight = anim.GetFloat("LeftFoot");
        rFWeight = anim.GetFloat("RightFoot");

        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, lFWeight);
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, rFWeight);

        anim.SetIKPosition(AvatarIKGoal.LeftFoot, lFPos + new Vector3(0f, offsety, 0f));
        anim.SetIKPosition(AvatarIKGoal.RightFoot, rFPos + new Vector3(0f, offsety, 0f));

        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, lFWeight);
        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, rFWeight);

        anim.SetIKRotation(AvatarIKGoal.LeftFoot, lFRot);
        anim.SetIKRotation(AvatarIKGoal.RightFoot, rFRot);

        /*anim.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, ikWeight);
        anim.SetIKHintPositionWeight(AvatarIKHint.RightKnee, ikWeight);

        anim.SetIKHintPosition(AvatarIKHint.LeftKnee, hintLeft.position);
        anim.SetIKHintPosition(AvatarIKHint.RightKnee, hintRight.position);*/


    }
    Animator anim;

    public float ikWeight = 1;

    public Transform leftIKTarget;
    public Transform rightIKTarget;

    public Transform hintLeft;
    public Transform hintRight;

    Vector3 lFPos;
    Vector3 rFPos;

    Quaternion lFRot;
    Quaternion rFRot;

    float lFWeight;
    float rFWeight;

    Transform leftFoot;
    Transform rightFoot;

    public float offsety;
}
