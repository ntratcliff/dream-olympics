using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour {
    private Animator playerAnimator;
    private Vector3 pos;
    private Vector3 oldpos;
	// Use this for initialization
	void Start () {
        pos = transform.position;
        oldpos = pos;
        playerAnimator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        pos = transform.position;
        playerAnimator.SetFloat("Speed", (pos - oldpos).magnitude);

        oldpos = pos;
	}
}
