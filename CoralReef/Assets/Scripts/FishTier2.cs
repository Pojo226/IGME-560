using UnityEngine;
using System.Collections;

public class FishTier2 : FishController {
	
	protected override bool CheckEnvironment(){
		Collider[] colliders = Physics.OverlapSphere(transform.position, genetics.sight); //Find nearby fishes
		if(colliders.Length > 0){
			foreach(Collider nearby in colliders){
				FishController fish = nearby.GetComponent<FishController>();
				if(fish && fish != this){
					int offset = ((int)fish.fishType) - ((int)fishType);
					if(offset > 0 && offset <= 2){ //If the other fish is bigger, and not too big...
						SetIntent(new FleeIntent());
						return true;
					}
				}
			}
		}
		return false;
	}
}
