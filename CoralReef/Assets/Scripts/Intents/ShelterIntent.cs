using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShelterIntent : FishIntent {

	public override float intentPriority { get { return 1; } }
	public float speedModifier = 1.25f;
	public float turnModifier = 1.25f;

	protected List<Transform> shelters = new List<Transform>();

	public override void Seek(){
		Collider[] colliders = Physics.OverlapSphere(fish.transform.position, fish.genetics.sight); //Find nearby shelter
		if(colliders.Length > 0){
			Collider nearestShelter = null;
			float distance = fish.genetics.perception + 100;
			
			foreach(Collider nearby in colliders){
				if(nearby.GetComponent<Shelter>()){
					float testDist = Vector3.Distance(fish.transform.position, nearby.transform.position);
					if(testDist < distance){
						distance = testDist;
						nearestShelter = nearby;
					}
				}
			}
			
			if(nearestShelter){
				fish.SetDestination(nearestShelter.transform.position, nearestShelter.gameObject, fish.Sleep);
				return;
			}
		}
		
		if(shelters.Count > 0){ //Oh no. Try to find an old shelter.
			Transform nearestShelter = shelters[0];
			float distance = Vector3.Distance(fish.transform.position, nearestShelter.transform.position);
			
			for(int i = 1; i < shelters.Count; i++){
				float testDist = Vector3.Distance(fish.transform.position, shelters[i].transform.position);
				if(testDist < distance){
					distance = testDist;
					nearestShelter = shelters[i];
				}
			}
			
			fish.SetDestination(nearestShelter.position, nearestShelter.gameObject);
			return;
		}

		//Panic time.
		fish.SetDestination(FishController.GenerateRandomPoint(fish.transform.position, 2));
	}
}
