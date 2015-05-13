using UnityEngine;
using System.Collections;

public class FleeIntent : FishIntent {
	
	public override float intentPriority { get { return 0; } }
	public float speedModifier = 1.5f;
	public float turnModifier = 1.5f;
	
	public override void Seek(){
		Collider[] colliders = Physics.OverlapSphere(fish.transform.position, fish.genetics.sight); //Find nearby fishes
		if(colliders.Length > 0){
			foreach(Collider nearby in colliders){
				FishController otherFish = nearby.GetComponent<FishController>();
				if(otherFish){
					int offset = ((int)otherFish.fishType) - ((int)fish.fishType);
					if(offset > 0 && offset < 2){ //If the other fish is bigger, and not too big...
						fish.SetDestination(fish.transform.position + (fish.transform.position - nearby.transform.position) * 2, null, null);
					}
				}
			}
		}
	}
}