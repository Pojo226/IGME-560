using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public float speed;

    private void Update(){
        if(Input.GetKey(KeyCode.W)){
            transform.position += transform.forward * speed * Time.deltaTime;
        }

        if(Input.GetKey(KeyCode.S)){
            transform.position -= transform.forward * speed * Time.deltaTime;
        }

        if(Input.GetKey(KeyCode.A)){
            transform.position -= transform.right * speed * Time.deltaTime;
        }

        if(Input.GetKey(KeyCode.D)){
            transform.position += transform.right * speed * Time.deltaTime;
        }

        if(Input.GetKey(KeyCode.Space)){
            transform.position += transform.up * speed * Time.deltaTime;
        }

        if(Input.GetKey(KeyCode.LeftControl)){
            transform.position -= transform.up * speed * Time.deltaTime;
        }
    }

}
