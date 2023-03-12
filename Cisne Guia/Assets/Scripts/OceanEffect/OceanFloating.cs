using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanFloating : MonoBehaviour
{
	public float airDrag = 1;
	public float waterDrag = 10;
	public bool attachToSurface = true;
	public bool affectDirection = false;
	public bool floatRotate = false;
	
	[SerializeField]
	List<Transform> floatingPoints;
	[SerializeField]
	OceanController waves;
	[SerializeField]
	Rigidbody body;

	//water line
	protected float waterLine;
	protected Vector3[] waterLinePoints;

	public Vector3 center { get { return transform.position + centerOffset; } }
	protected Vector3 centerOffset;
	protected Vector3 smoothVectorRotation;
	protected Vector3 targetUp;

	private void Awake()
	{
		SetBody();

		SetFloatingPoints();

		//compute center
		waterLinePoints = new Vector3[floatingPoints.Count];
		for (int i = 0; i < floatingPoints.Count; ++i)
		{ waterLinePoints[i] = floatingPoints[i].position; }

		centerOffset = GetCenter(waterLinePoints) - transform.position;

		this.AddObserver(OnOceanNotification, Notifications.OceanReference);
	}

	void SetBody()
	{
		body = GetComponent<Rigidbody>();
		body.useGravity = false;
	}

	void SetFloatingPoints()
	{
		floatingPoints = new List<Transform>();
		Transform[] t = GetComponentsInChildren<Transform>();
		for(int i = 0; i < t.Length; ++i)
		{ if (t[i].CompareTag("floatingPoint")) { floatingPoints.Add(t[i]); } }
	}

	public void OnOceanNotification(object sender, EventArgs args)
	{
		waves = (OceanController)((MessageArgs)args).message;
	}

	public static Vector3 GetCenter(Vector3[] points)
	{
		Vector3 center = Vector3.zero;
		for (int i = 0; i < points.Length; ++i)
		{ center += (points[i] / points.Length); }
		return center;
	}

	public void PhysicsUpdate()
	{
		//default water surface
		float newWaterLine = 0;
		bool pointUnderWater = false;

		//set waterLinePoints and waterLine
		for (int i = 0; i < floatingPoints.Count; ++i)
		{
			//height
			waterLinePoints[i] = floatingPoints[i].position;
			waterLinePoints[i].y = waves.GetHeight(floatingPoints[i].position);
			newWaterLine += waterLinePoints[i].y / floatingPoints.Count;
			if (waterLinePoints[i].y > floatingPoints[i].position.y)
			{ pointUnderWater = true; }
		}

		float waterLineDelta = newWaterLine - waterLine;
		waterLine = newWaterLine;

		//compute up vector 
		targetUp = GetNormal(waterLinePoints);

		//gravity
		Vector3 gravity = Physics.gravity;
		body.drag = airDrag;

		if (waterLine > center.y)
		{
			body.drag = waterDrag;
			//under water
			if (attachToSurface)
			{
				//attach to water surface
				body.position = new Vector3(body.position.x, waterLine - centerOffset.y, body.position.z);
			}
			else
			{
				//go up
				gravity = affectDirection ? targetUp * -Physics.gravity.y : -Physics.gravity;
				//body.position += Vector3.up * waterLineDelta * 0.9f;
				transform.Translate(Vector3.up * waterLineDelta * 0.9f);
			}
		}
		//body.AddForce(gravity);
		body.AddForce(gravity * Mathf.Clamp(Mathf.Abs(waterLine - center.y), 0, 1));

		if (floatRotate)
		{
			//rotation
			if (pointUnderWater)
			{
				//attach to wate surface
				targetUp = Vector3.SmoothDamp(transform.up, targetUp, ref smoothVectorRotation, 0.2f);
				body.rotation = Quaternion.FromToRotation(transform.up, targetUp) * body.rotation;
			}
		}
	}

	public static Vector3 GetNormal(Vector3[] points)
	{
		//https://www.ilikebigbits.com/2015_03_04_plane_from_points.html
		if (points.Length < 3)
		{ return Vector3.up; }

		Vector3 center = GetCenter(points);

		float xx = 0f, xy = 0f, xz = 0f, yy = 0f, yz = 0f, zz = 0f;

		for (int i = 0; i < points.Length; i++)
		{
			Vector3 r = points[i] - center;
			xx += r.x * r.x;
			xy += r.x * r.y;
			xz += r.x * r.z;
			yy += r.y * r.y;
			yz += r.y * r.z;
			zz += r.z * r.z;
		}

		float det_x = yy * zz - yz * yz;
		float det_y = xx * zz - xz * xz;
		float det_z = xx * yy - xy * xy;

		if (det_x > det_y && det_x > det_z)
		{ return new Vector3(det_x, xz * yz - xy * zz, xy * yz - xz * yy).normalized; }
		if (det_y > det_z)
		{ return new Vector3(xz * yz - xy * zz, det_y, xy * xz - yz * xx).normalized; }
		else
		{ return new Vector3(xy * yz - xz * yy, xy * xz - yz * xx, det_z).normalized; }
	}

	/*private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		if (floatingPoints == null) return;

		for (int i = 0; i < floatingPoints.Length; ++i)
		{
			if (floatingPoints[i] == null) continue;

			if (waves != null)
			{
				//draw cube
				Gizmos.color = Color.red;
				Gizmos.DrawCube(waterLinePoints[i], Vector3.one * 0.3f);
			}
			//draw sphere
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(floatingPoints[i].position, 0.1f);
		}
		//draw center
		if (Application.isPlaying)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawCube(new Vector3(center.x, waterLine, center.z), Vector3.one * 1f);
			Gizmos.DrawRay(new Vector3(center.x, waterLine, center.z), targetUp.normalized * 3f);
		}
	}*/
}
