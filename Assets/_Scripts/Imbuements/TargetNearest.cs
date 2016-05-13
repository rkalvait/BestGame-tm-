using UnityEngine;
using System.Collections;

public class TargetNearest :  BaseImbue {

	public string targetTag;
	private ScriptManager manager;

	void Start() {
		manager = GetComponent<ScriptManager> ();
		if (!manager.isCustom)
			targetTag = args [1];
	}

	// Update is called once per frame
	void Update () {
		GameObject[] objects = GameObject.FindGameObjectsWithTag (targetTag);

		GameObject bestTarget = null;
		float closestDistanceSqr = Mathf.Infinity;
		Vector3 currentPosition = transform.position;
		foreach(GameObject potentialTarget in objects)
		{
			if (potentialTarget.GetComponent<ScriptManager>() != null && potentialTarget.GetComponent<ScriptManager>().owner == GetComponent<ScriptManager>().owner) continue;
			Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
			float dSqrToTarget = directionToTarget.sqrMagnitude;
			if(dSqrToTarget < closestDistanceSqr)
			{
				closestDistanceSqr = dSqrToTarget;
				bestTarget = potentialTarget;
			}
		}
		manager.target = bestTarget;
	}
}
