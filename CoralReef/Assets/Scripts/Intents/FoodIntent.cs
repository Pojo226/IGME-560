using UnityEngine;
using System.Collections;

public class FoodIntent : FishIntent {
	
	public float intentPriority = 2;
	public float speedModifier = 1.5f;
	public float turnModifier = 1.5f;

	public override void Seek(){
		if(fish.fishType == FishController.Fish_Hierarchy.Goldfish){
			Food[] foodLocations = GameObject.FindObjectsOfType<Food>();
			Food nearest = foodLocations[0];
			float dist = Vector3.Distance(fish.transform.position, nearest.transform.position);
			for(int i = 1; i < foodLocations.Length; i++){
				float temp = Vector3.Distance(fish.transform.position, foodLocations[i].transform.position);
				if(temp < dist){
					dist = temp;
					nearest = foodLocations[i];
				}
			}
			fish.SetDestination(nearest.transform.position, nearest.gameObject, ArriveFood);
		}else{
			Collider[] colliders = Physics.OverlapSphere(fish.transform.position, fish.genetics.sight);
			if(colliders.Length > 0){
				Collider nearestFood = null;
				float distance = fish.genetics.perception + 100;
				
				foreach(Collider nearby in colliders){
					FishController fishy = nearby.GetComponent<FishController>();
					if(fishy){
						if(fishy.fishType == fish.fishType - 1){
							float testDist = Vector3.Distance(fish.transform.position, nearby.transform.position);
							if(testDist < distance){
								distance = testDist;
								nearestFood = nearby;
							}
						}
					}
				}
				
				if(nearestFood){
					if(distance <= 1){
						fish.vitals.hunger = 10;
						GameObject.Destroy(nearestFood.gameObject);
					}else{
						fish.SetDestination(nearestFood.transform.position, nearestFood.gameObject);
					}
					return;
				}
			}
			
			fish.SetDestination(FishController.GenerateRandomPoint(fish.transform.position, 2));
		}
	}

	protected void ArriveFood(){
		fish.vitals.hunger = 10;
	}
}
