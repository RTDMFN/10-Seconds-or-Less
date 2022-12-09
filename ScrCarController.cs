using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScrCarController : MonoBehaviour
{
    public ScrWheel[] wheels;
    public Transform centerOfMass;

    [Header("Car Specs")]
    public float wheelBase; //all in meters
    public float rearTrack;
    public float turnRadius;

    [Header("Inputs")]
    public float steerInput;

    public float ackermannAngleLeft;
    public float ackermannAngleRight;

    private Rigidbody rb;

    private void Awake(){
        rb = GetComponent<Rigidbody>();
    }

    private void Update(){
        //rb.centerOfMass = centerOfMass.position;

        if(Input.GetKeyDown(KeyCode.Escape)) GameManager.instance.Pause();

        if(GameManager.instance.State == GameState.Playing) steerInput = Input.GetAxis("Horizontal");
        
        if(steerInput > 0){ //turning right
            ackermannAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * steerInput;
            ackermannAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * steerInput;
        }else if(steerInput < 0){ //turning left
            ackermannAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * steerInput;
            ackermannAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * steerInput;
        }else{
            ackermannAngleLeft = 0;
            ackermannAngleRight = 0;
        }

        foreach(ScrWheel w in wheels){
            if(w.wheelFrontLeft) w.steerAngle = ackermannAngleLeft;
            if(w.wheelFrontRight) w.steerAngle = ackermannAngleRight;
        }

        if(Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene("Testing");
    }
}
