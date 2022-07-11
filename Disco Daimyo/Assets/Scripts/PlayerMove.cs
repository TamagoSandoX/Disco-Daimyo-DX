using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
	public float speed = 20f;

	private Rigidbody rb;

	public Vector3 rayPos;


	private float minX;
	private float maxX;

	public GameObject leftBumper;
	public GameObject rightBumper;

	// Use this for initialization
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		minX = leftBumper.transform.localPosition.x + 1f;
		maxX = rightBumper.transform.localPosition.x - 1f;
	}

	// Update is called once per frame
	void Update()
	{

	}

	private Vector3 GetMousePosition()
	{
		// create a ray from the camera
		// passing through the mouse position
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		// find out where the ray intersects the XZ plane
		Plane plane = new Plane(Vector3.up, Vector3.zero);
		float distance = 0;
		plane.Raycast(ray, out distance);
		rayPos = ray.GetPoint(distance);
		return ray.GetPoint(distance);
	}

	/*void OnDrawGizmos()
	{
		// draw the mouse ray
		Gizmos.color = Color.yellow;
		Vector3 pos = GetMousePosition();
		Gizmos.DrawLine(Camera.main.transform.position, pos);
	}*/

	void FixedUpdate()
	{
		Vector3 pos = GetMousePosition();
		Vector3 dir = pos - rb.position;
		Vector3 vel = dir.normalized * speed;
		// check is this speed is going to overshoot the target
		float move = speed * Time.fixedDeltaTime;
		float distToTarget = dir.magnitude;
		if (move > distToTarget)
		{
			// scale the velocity down appropriately
			vel = vel * distToTarget / move;
			
		}
		rb.velocity = vel;
		transform.position = new Vector3 (Mathf.Clamp(transform.position.x, minX, maxX), transform.position.y, transform.position.z);
	}
}
