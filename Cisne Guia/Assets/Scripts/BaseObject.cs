using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseObject : MonoBehaviour
{
	public abstract void FrameUpdate();
	public abstract void PhysicsUpdate();
	public abstract void PostUpdate();
}
