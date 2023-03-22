using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeGaze : MonoBehaviour
{
	public Transform Pupil;
	public Transform Player;
	public float EyeRadius = 0.1f;
	Vector3 mPupilCenterPos;

	void Start()
	{
		mPupilCenterPos = Pupil.position;
	}

	void Update()
	{
		Vector3 lookDir = (Player.position - mPupilCenterPos);
		if (lookDir.magnitude > EyeRadius)
			lookDir = lookDir.normalized * EyeRadius;
		
		Pupil.position = mPupilCenterPos + lookDir;
	}
}
