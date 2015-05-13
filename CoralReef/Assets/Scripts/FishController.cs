using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class FishController : MonoBehaviour {

	public enum Fish_Hierarchy { Goldfish, Bass, Salmon, Tuna };
	public Fish_Hierarchy fishType;

	public FishGenetics genetics;
	public FishVitals vitals;
	public FishVitals Vitals { get { return vitals; } }

	public FishIntent intent;


	protected GameObject destinationObject;
	protected Vector3 destination = Vector3.up;
	public delegate void DestinationArrive();
	protected DestinationArrive onArrive;

	protected bool sleeping = false;

	protected Quaternion dstBearing;
	protected float dstHeading;

	protected bool init;

	public void Init(int typeIndex){
		if(init) return;

		fishType = (Fish_Hierarchy)typeIndex;
		genetics = FishGenetics.Create(fishType);
		vitals = FishVitals.Create();
		SetIntent(new IdleIntent());


		CalculateMovement();

		StartCoroutine(PerceptionCoroutine());

		init = true;
	}

	protected void SetIntent(FishIntent newIntent){
		if(newIntent == null) newIntent = new IdleIntent();

		if(intent != null){
			Debug.LogWarning(newIntent + "\t" + newIntent.intentPriority + "\t" + intent.intentPriority);
			Debug.LogWarning(intent == null || newIntent.intentPriority < intent.intentPriority);
		}
		if(intent == null || newIntent.intentPriority < intent.intentPriority){
			intent = newIntent;
			intent.fish = this;
			intent.Seek();
		}
	}

	private IEnumerator PerceptionCoroutine(){
		while(true){
			if(!CheckEnvironment()){ //Check if a hazard overrides everything.
				if(!CheckVitals()){ //Check if vitals are low.
					SetIntent(new IdleIntent()); //Wander a bit.
				}
			}
			
			yield return new WaitForSeconds(genetics.perception);
		}
	}
	
	private void Update(){
		vitals.Degrade();

		float turnRadius = ((360 / genetics.turnSpeed) / Mathf.PI) / 2;
		if(Vector3.Distance(transform.position, destination) <= turnRadius){
			SetIntent(null);
			if(onArrive != null) onArrive();
		}

		//if move
		CalculateMovement();
		transform.rotation = Quaternion.RotateTowards(transform.rotation, dstBearing, genetics.turnSpeed * intent.turnModifier * Time.deltaTime);
		transform.Translate(genetics.forwardSpeed * intent.speedModifier * Vector3.forward * Time.deltaTime);
	}

	public void SetDestination(Vector3 destination, GameObject destinationObject = null, DestinationArrive onArrive = null){
		this.destination = destination;
		this.destinationObject = destinationObject;
		this.onArrive = onArrive;
	}

	private void CalculateMovement(){
		RaycastHit hit;
		if(Physics.Raycast(transform.position, transform.forward, out hit, 5)){
			if(hit.collider.gameObject != destinationObject){
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
				return;
			}
		}
		dstBearing = Quaternion.LookRotation(destination - transform.position);
	}

	public virtual void Sleep(){
		if(!sleeping){
			SetIntent(null);
			sleeping = true;
			enabled = false;
			Invoke("Sleep", 2.0f);
		}else{
			sleeping = false;
			enabled = true;
			vitals.energy = 10;
		}
	}

	//Decision events
	protected abstract bool CheckEnvironment();
	protected virtual bool CheckVitals(){
		if(vitals.energy <= 4){
			if(vitals.energy <= 0){
				Sleep();
			}else{
				SetIntent(new ShelterIntent());
			}
			return true;
		}
		if(vitals.hunger <= 5){
			SetIntent(new FoodIntent());
			return true;
		}
		return false;
	}

	public static Vector3 GenerateRandomPoint(Vector3 location, float maxDistance){
		Vector3 point = location + new Vector3(Random.Range(-maxDistance, maxDistance), Random.Range(-maxDistance, maxDistance), Random.Range(-maxDistance, maxDistance));
		point.x = Mathf.Min(Mathf.Abs(point.x), 20) * Mathf.Sign(point.x);
		point.y = Mathf.Clamp(point.x, 0.5f, 9.5f);
		point.z = Mathf.Min(Mathf.Abs(point.z), 20) * Mathf.Sign(point.z);
		return point;
	}
}


public class FishGenetics {

	public float turnSpeed;
	public float forwardSpeed;

#if GENETIC_ALGORITHMS
	//Variances are used by GA's to offset bases based on fish level as well
	//	as a mutation factor, both which use the variance. This way the fastest
	//	possible gen1 fish can only be as fast as the next level's base.
	public static readonly float TurnSpeedBase = 25;
	public static readonly float TurnSpeedVariance = 2;

	public static readonly float SpeedBase = 1;
	public static readonly float SpeedVariance = 0.2f;

	public static readonly float SightBase = 6;
	public static readonly float SightVariance = 1;

	public static readonly float PerceptionBase = 1.5f;
	public static readonly float PerceptionVariance = -0.25f;
#else
	public static readonly float[] TurnSpeed = { 25, 27.5f, 30, 32.5f, 35 };
	public static readonly float[] Speed = { 1, 1.1f, 1.2f, 1.3f, 1.4f };
	public static readonly float[] Sight = { 6, 7, 8, 9 };
	public static readonly float[] Perception = { 1.5f, 1.0f, 0.5f, 0.5f };
#endif

	public float sight;
	public float perception;
	
	public static FishGenetics Create(FishController.Fish_Hierarchy fish, FishGenetics geneticsA = null, FishGenetics geneticsB = null){
		int typeIndex = (int)fish;

		FishGenetics data = new FishGenetics();


#if GENETIC_ALGORITHMS
		if(geneticsA == null || geneticsB == null){ //If program start, randomly set values
			data.turnSpeed = TurnSpeedBase + typeIndex*TurnSpeedVariance + Random.Range(-TurnSpeedVariance/2, TurnSpeedVariance/2);
			data.forwardSpeed = SpeedBase + typeIndex*SpeedVariance + Random.Range(-SpeedVariance/2, SpeedVariance/2);
			data.sight = SightBase + typeIndex*SightVariance + Random.Range(-SightVariance/2, SightVariance/2);
			data.perception = PerceptionBase + typeIndex*PerceptionVariance + Random.Range(-PerceptionVariance/2, PerceptionVariance/2);
		}else{ //If spawning from breeding, random pick based influenced by the two values, and shuffle a bit
			data.turnSpeed = Random.Range(geneticsA.turnSpeed, geneticsB.turnSpeed) + Random.Range(-TurnSpeedVariance/2, TurnSpeedVariance/2);
			data.forwardSpeed = Random.Range(geneticsA.forwardSpeed, geneticsB.forwardSpeed) + Random.Range(-SpeedVariance/2, SpeedVariance/2);
			data.sight = Random.Range(geneticsA.sight, geneticsB.sight) + Random.Range(-SightVariance/2, SightVariance/2);
			data.perception = Random.Range(geneticsA.perception, geneticsB.perception) + Random.Range(-PerceptionVariance/2, PerceptionVariance/2);
		}
#else
		data.GeneticCode = typeIndex * 1000 + typeIndex * 100 + typeIndex * 10 + typeIndex;
		data.turnSpeed = TurnSpeed[typeIndex];
		data.forwardSpeed = Speed[typeIndex];
		
		data.sight = Sight[typeIndex];
		data.perception = Perception[typeIndex];
#endif

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
		hunger -= .3f * Time.deltaTime;
		energy -= .1f * Time.deltaTime;
	}
}

public abstract class FishIntent {

	public FishController fish;

	public float intentPriority = 0;
	public float speedModifier = 1;
	public float turnModifier = 1;

	public abstract void Seek();
	
}