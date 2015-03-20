using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(FishController), true)]
public class FishInspector : Editor {

	public override void OnInspectorGUI(){
		DrawDefaultInspector();
		EditorGUILayout.Space();

		FishController fish = (target as FishController);
		if(GUILayout.Button("Send to origin")){
			fish.SetDestination(Vector3.zero);
		}
		EditorGUILayout.Space();
		GUILayout.Label("Current State:\t" + fish.intent.GetType().ToString());
		GUILayout.Label("Health\t" + fish.Vitals.health);
		GUILayout.Label("Energy\t" + fish.Vitals.energy);
		GUILayout.Label("Hunger\t" + fish.Vitals.hunger);
	}
}
