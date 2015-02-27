using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class FishController : MonoBehaviour {

	public enum Fish_Hierarchy { Goldfish, Bass, Salmon, Tuna };
	public Fish_Hierarchy fishType;

	protected FishGenetics genetics;
	protected FishVitals vitals;

	protected List<Transform> shelters = new List<Transform>();

	protected enum Destination_Types { Flee, Shelter, Food, Idle, None }
	protected Destination_Types destinationType = Destination_Types.None;
	protected GameObject destinationObject;
	protected Vector3 destination = Vector3.up;

	protected Quaternion dstBearing;
	protected float dstHeading;

	protected float turnModifier = 1;
	protected float forwardModifier = 1;

	protected bool init;

	public void Init(int typeIndex){
		if(init) return;

		fishType = (Fish_Hierarchy)typeIndex;
		genetics = FishGenetics.Create(fishType);
		vitals = FishVitals.Create();

		CalculateMovement();

		StartCoroutine(PerceptionCoroutine());

		init = true;
	}

	private IEnumerator PerceptionCoroutine(){
		while(true){
			if(!CheckEnvironment()){ //Check if a hazard overrides everything.
				//if(!CheckVitals()){ //Check if vitals are low.
				Idle(); //Wander a bit.
				//}
			}
			
			yield return new WaitForSeconds(genetics.perception);
		}
	}
	
	private void Update(){
		vitals.Degrade();

		float turnRadius = ((360 / genetics.turnSpeed) / Mathf.PI) / 2;
		if(Vector3.Distance(transform.position, destination) <= turnRadius){
			destinationType = Destination_Types.None;
//			do{
//				destination = transform.parent.position + FishSpawner.GetRandomSpawnVector3();
//			}while(Vector3.Distance(transform.position, destination) < 1);
		}
		
		//if move
		CalculateMovement();
		transform.rotation = Quaternion.RotateTowards(transform.rotation, dstBearing, genetics.turnSpeed * turnModifier * Time.deltaTime);
		
		transform.Translate(genetics.forwardSpeed * forwardModifier * Vector3.forward * Time.deltaTime);
	}

	protected void SetDestination(Destination_Types dstType, Vector3 destination, GameObject destinationObject = null){
		destinationType = dstType;
		this.destination = destination;
		this.destinationObject = destinationObject;
	}

	public void SetDestination(Vector3 destination){
		this.destination = destination;
	}

	private void CalculateMovement(){
		RaycastHit hit;
		if(Physics.Raycast(transform.position, transform.forward, out hit, 5)){
			Vector3 avoidance = Vector3.zero;
			if(hit.normal.x > 0){
				avoidance += -transform.right;
			}else{
				avoidance += transform.right;
			}

			if(hit.normal.y > 0){
				avoidance += transform.up;
			}else{
				avoidance += -transform.up;
			}

			dstBearing = Quaternion.LookRotation(avoidance);
		}else{
			dstBearing = Quaternion.LookRotation(destination - transform.position);
		}
	}

	//Decision events
	protected abstract bool CheckEnvironment();
	protected virtual bool CheckVitals(){
		if(vitals.energy <= 4){
			FindShelter();
			return true;
		}
		if(vitals.hunger <= 5){
			FindFood();
			return true;
		}
		return false;
	}

	//Targeting events
	protected abstract void FindShelter();
	protected abstract void FindFood();
	protected abstract void Idle();
	
	public static Vector3 GenerateRandomPoint(Vector3 location, float maxDistance){
		Vector3 point = location + new Vector3(Random.Range(-maxDistance, maxDistance), Random.Range(-maxDistance, maxDistance), Random.Range(-maxDistance, maxDistance));
		point.x = Mathf.Min(Mathf.Abs(point.x), 20) * Mathf.Sign(point.x);
		point.y = Mathf.Clamp(point.x, 0.5f, 9.5f);
		point.z = Mathf.Min(Mathf.Abs(point.z), 20) * Mathf.Sign(point.z);
		return point;
	}
}


public struct FishGenetics {

	public float turnSpeed;
	public float forwardSpeed;
	
	public static readonly float[] Sight = { 6, 7, 8, 9 };
	public static readonly float[] Perception = { 1.5f, 1.0f, 0.5f, 0.5f };
	
	public float sight;
	public float perception;
	
	
	public static FishGenetics Create(FishController.Fish_Hierarchy fish){
		int typeIndex = (int)fish;

		FishGenetics data = new FishGenetics();

		data.turnSpeed = 25;
		data.forwardSpeed = 1;

		data.sight = Sight[typeIndex];
		data.perception = Perception[typeIndex];

		return data;
	}
}

public struct FishVitals {

	public float health;
	public float energy;
	public float hunger;

	public static FishVitals Create(){
		FishVitals data = new FishVitals();

		data.health = 10;
		data.energy = 10;
		data.hunger = 10;

		return data;
	}

	public void Degrade(){
		return; //TEMPORARY
		hunger -= .3f * Time.deltaTime;
		energy -= .1f * Time.deltaTime;
	}
}