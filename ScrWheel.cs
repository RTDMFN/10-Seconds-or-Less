using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrWheel : MonoBehaviour
{
    private Rigidbody rb;

    public bool wheelFrontLeft;
    public bool wheelFrontRight;
    public bool wheelRearLeft;
    public bool wheelRearRight;
    
    [Header("Suspension")]
    public float restLength;
    public float springTravel;
    public float springStiffness;
    public float damperStiffness;
    
    private float minLength;
    private float maxLength;
    private float lastLength;
    private float springLength;
    private float springForce;
    private float springVelocity;
    private float damperForce;

    [Header("Wheel")]
    public float steerAngle;
    public float steerTime;

    private Vector3 suspensionForce;
    private Vector3 wheelVelocityLS;
    private float Fx;
    private float Fy;
    private float wheelAngle;

    [Header("Wheel")]
    public float wheelRadius;
    public float motorTorque;

    [Header("Visuals")]
    public Transform visualWheel;

    private void Awake(){
        rb = transform.root.GetComponent<Rigidbody>();
    }

    private void Start(){
        minLength = restLength - springTravel;
        maxLength = restLength + springTravel;
    }

    private void Update(){
        wheelAngle = Mathf.Lerp(wheelAngle,steerAngle,steerTime * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(Vector3.up * wheelAngle);
        visualWheel.localRotation = transform.localRotation;

        Debug.DrawRay(transform.position,-transform.up * (springLength + wheelRadius),Color.green);
    }

    private void FixedUpdate(){
        if(Physics.Raycast(transform.position,-transform.up, out RaycastHit hit, maxLength + wheelRadius)){
            lastLength = springLength;
            
            springLength = hit.distance - wheelRadius;
            springLength = Mathf.Clamp(springLength,minLength,maxLength);
            springVelocity = (lastLength - springLength)/Time.fixedDeltaTime;
            springForce = springStiffness * (restLength - springLength);

            damperForce = damperStiffness * springVelocity;

            suspensionForce = (springForce + damperForce) * transform.up;

            wheelVelocityLS = transform.InverseTransformDirection(rb.GetPointVelocity(hit.point));
            if(GameManager.instance.State == GameState.Playing)Fx = Input.GetAxis("Vertical") * springForce * motorTorque;
            Fy = wheelVelocityLS.x * springForce;

            rb.AddForceAtPosition(suspensionForce + (Fx * transform.forward) + (Fy * -transform.right),hit.point);
        }
    }
}
