using UnityEngine;
using System.Collections;

public class IdleIntent : FishIntent {
	
	public float intentPriority = 3;
	public float speedModifier = 1f;
	public float turnModifier = 1f;

	public override void Seek(){
		fish.SetDestination(FishController.GenerateRandomPoint(fish.transform.position, 4), null, Seek);
	}
}
