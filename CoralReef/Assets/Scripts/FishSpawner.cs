using UnityEngine;
using System.Collections;

public class FishSpawner : MonoBehaviour {

	private static FishSpawner instance;

	public GameObject[] Fish;
	public int[] FishToSpawn;

	public Vector2 spawnHeight;
	public Vector2 spawnRadius;

	// Use this for initialization
	private void Start () {
		instance = this;

		if(Fish.Length != FishToSpawn.Length){
			Debug.LogError("Uneven # of fish and spawn numbers!");
			return;
		}

		StartCoroutine(SpawnCoroutine());
	}
	
	private IEnumerator SpawnCoroutine(){
		for(int i = 0; i < FishToSpawn.Length; i++){
			for(int j = 0; j < FishToSpawn[i]; j++){
				GameObject newFish = ((GameObject)(GameObject.Instantiate(Fish[i], transform.position + GetRandomSpawnVector3(), GetRandomSpawnQuaternion())));
				newFish.transform.parent = transform;
				newFish.transform.localScale = Vector3.one;

				newFish.GetComponent<FishController>().Init(i);
				yield return new WaitForEndOfFrame();
			}
		}
	}

	public static Vector3 GetRandomSpawnVector3(){
		return new Vector3(Random.Range(instance.spawnRadius.x, instance.spawnRadius.y),
		                   Random.Range(instance.spawnHeight.x, instance.spawnHeight.y),
		                   Random.Range(instance.spawnRadius.x, instance.spawnRadius.y));
	}

	private Quaternion GetRandomSpawnQuaternion(){
		return Quaternion.Euler(new Vector3(0, Random.Range(0, 359), 0));
	}
}
