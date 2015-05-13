using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FishManager : MonoBehaviour {

	private static Dictionary<System.Type, List<FishController>> fishLookup;

	private void Awake(){
		fishLookup = new Dictionary<System.Type, List<FishController>>();
	}

	public static void AddFish(FishController newFish){
		System.Type fishType = newFish.GetType();
		if(!fishLookup.ContainsKey(fishType)){
			fishLookup.Add(fishType, new List<FishController>());
		}
		fishLookup[fishType].Add(newFish);
	}

	public static void RemoveFish(FishController oldFish){
		fishLookup[oldFish.GetType()].Remove(oldFish);
	}


	private void OnGUI(){
		string info = "TYPE\t#\tTURN\tSPEED\tSIGHT\tPERCEPTION\n";


		foreach(KeyValuePair<System.Type, List<FishController>> dictEntry in fishLookup){
			List<FishController> fishOfType = dictEntry.Value;
			info += dictEntry.Key.ToString() + "\t" + fishOfType.Count + "\t";

			float avgTurn = 0;
			float avgSpeed = 0;
			float avgSight = 0;
			float avgPerception = 0;

			foreach(FishController fish in fishOfType){
				avgTurn += fish.genetics.turnSpeed;
				avgSpeed += fish.genetics.forwardSpeed;
				avgSight += fish.genetics.sight;
				avgPerception += fish.genetics.perception;
			}
			avgTurn /= fishOfType.Count;
			avgSpeed /= fishOfType.Count;
			avgSight /= fishOfType.Count;
			avgPerception /= fishOfType.Count;

			info += avgTurn + "\t" + avgSpeed + "\t" + avgSight + "\t" + avgPerception + "\n";
		}

		GUI.TextArea(new Rect(5, 5, 500, 100), info);
	}

}
