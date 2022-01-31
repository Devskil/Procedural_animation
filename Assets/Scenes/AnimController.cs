using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    Animator animator;
    void Start () {
        animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        animator.SetFloat("x", Input.GetAxis("Horizontal")); 
        animator.SetFloat("y", Input.GetAxis("Vertical"));
    }
}
