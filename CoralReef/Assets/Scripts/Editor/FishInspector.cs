using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(FishController), true)]
public class FishInspector : Editor {

	public override void OnInspectorGUI(){
		DrawDefaultInspector();
		EditorGUILayout.Space();
		if(GUILayout.Button("Send to origin")){
			(target as FishController).SetDestination(Vector3.zero);
		}
	}
}
