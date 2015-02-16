using UnityEngine;
using System.Collections;

public class FishController : MonoBehaviour {

	protected FishGenetics genetics;


	protected Vector3 destination = Vector3.one;
	protected float dstHeading;

	protected float turnSpeedMax = 25;

	protected bool init;

	public void Init(int typeIndex){
		if(init) return;

		genetics = FishGenetics.Create(typeIndex);

		init = true;
	}

	// Update is called once per frame
	void Update () {
		if(Vector3.Distance(transform.position, destination) <= 0.5f){
			Debug.Log("Redirecting!");
			destination = transform.parent.position + FishSpawner.GetRandomSpawnVector3(); //Temporary!
		}

		//if move
		Vector3 dstCross = Vector3.Cross(transform.forward, destination - transform.position);
		int yawDir = dstCross.y > 0 ? 1 : -1;
		//int pitchDir = dstCross.x > 0 ? 1 : -1;

		transform.Rotate(0, yawDir * turnSpeedMax * Time.deltaTime, 0);

		transform.Translate(Vector3.forward * Time.deltaTime);
	}
}


public struct FishGenetics {
	
	public enum Fish_Hierarchy { Goldfish, Bass, Salmon, Tuna };
	
	public static float[] Fish_Sight_Breadth = { 10, 20, 30, 40 };
	public static float[] Fish_Sight_Depth = { 10, 20, 30, 40 };

	public Fish_Hierarchy fishType;
	public float sight;
	
	
	public static FishGenetics Create(int typeIndex){

		FishGenetics data = new FishGenetics();

		data.fishType = (Fish_Hierarchy)typeIndex;
		data.sight = Fish_Sight_Breadth[typeIndex];

		return data;
	}
}