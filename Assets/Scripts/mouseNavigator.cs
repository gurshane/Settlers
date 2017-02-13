using UnityEngine;
using System.Collections;

public class mouseNavigator : MonoBehaviour {

    public Transform target;
    public Transform reverseTarget;
    public float upSpeed;
    public float downSpeed;
    public float leftSpeed;
    public float rightSpeed;
    public float zoomSpeed;

    Vector3 initialPosition;
    Quaternion initialRotation;

	// Use this for initialization
	void Start () {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {

        transform.LookAt(target);

        float step = zoomSpeed * Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.R))
        {
            transform.position = initialPosition;
            transform.rotation = initialRotation;
        }

        if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(new Vector3(-2, 0, 0) * Time.deltaTime * leftSpeed);

        }else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(new Vector3(2, 0, 0) * Time.deltaTime * rightSpeed);

        }else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(new Vector3(0, 2, 0) * Time.deltaTime * upSpeed);

        }else  if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(new Vector3(0, -2, 0) * Time.deltaTime * downSpeed);

        }else if (Input.GetKeyDown(KeyCode.W) || Input.GetKey(KeyCode.W))
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);

        }else  if (Input.GetKeyDown(KeyCode.S) || Input.GetKey(KeyCode.S))
        {
            transform.position =  Vector3.MoveTowards(transform.position, reverseTarget.position, step);

        }
    }
}
