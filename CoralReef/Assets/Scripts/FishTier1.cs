using UnityEngine;
using System.Collections;

public class FishTier1 : FishController {

	protected override bool CheckEnvironment(){
		Collider[] colliders = Physics.OverlapSphere(transform.position, genetics.sight); //Find nearby fishes
		if(colliders.Length > 0){
			foreach(Collider nearby in colliders){
				FishController fish = nearby.GetComponent<FishController>();
				if(fish){
					int offset = ((int)fish.fishType) - ((int)fishType);
					if(offset > 0 && offset < 2){ //If the other fish is bigger, and not too big...
						SetDestination(Destination_Types.Flee, transform.position + (transform.position - nearby.transform.position) * 2);
						return true;
					}
				}
			}
		}
		return false;
	}

	protected override void FindShelter(){
		if(destinationType == Destination_Types.Shelter) return;

		Collider[] colliders = Physics.OverlapSphere(transform.position, genetics.sight); //Find nearby shelter
		if(colliders.Length > 0){
			Collider nearestShelter = null;
			float distance = genetics.perception + 100;

			foreach(Collider nearby in colliders){
				if(nearby.GetComponent<Shelter>()){
					float testDist = Vector3.Distance(transform.position, nearby.transform.position);
					if(testDist < distance){
						distance = testDist;
						nearestShelter = nearby;
					}
				}
			}

			if(nearestShelter){
				if(distance <= 2){
					//Sleep!

				}else{
					SetDestination(Destination_Types.Shelter, nearestShelter.transform.position, nearestShelter.gameObject);
				}
				return;
			}
		}

		if(shelters.Count > 0){ //Oh no. Try to find an old shelter.
			Transform nearestShelter = shelters[0];
			float distance = Vector3.Distance(transform.position, nearestShelter.transform.position);

			for(int i = 1; i < shelters.Count; i++){
				float testDist = Vector3.Distance(transform.position, shelters[i].transform.position);
				if(testDist < distance){
					distance = testDist;
					nearestShelter = shelters[i];
				}
			}

			SetDestination(Destination_Types.Shelter, nearestShelter.position, nearestShelter.gameObject);
			return;
		}

		//Panic time.
		SetDestination(Destination_Types.Shelter, GenerateRandomPoint(transform.position, 2));
	}

	protected override void FindFood(){

	}

	protected override void Idle(){
		if(destinationType == Destination_Types.None){
			SetDestination(Destination_Types.Idle, GenerateRandomPoint(transform.position, 2));
		}
	}
}
