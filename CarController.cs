using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Spring")]
    public float springRestLength;
    public float springStrength;
    public float springDamper;

    private float offset;
    private float springForce;

    [Header("Tire")]
    public List<Transform> tires;
    public float tireGripFactor;
    public float tireMass;

    [Header("Car")]
    public float carTopSpeed;
    public float motorTorque;
    public float frictionCoefficient;
    public AnimationCurve powerCurve;

    private Rigidbody carRB;

    [Header("Steering")]
    public float rearTrack;
    public float wheelBase;
    public float turnRadius;
    public float steerTime;

    private float wheelAngle;
    private float steerAngle;

    [Header("Inputs")]
    private float steerInput;
    private float accelerationInput;
    private float ackermannAngleLeft;
    private float ackermannAngleRight;

    [Header("Engine")]
    public float minSpeed;
    public float maxSpeed;
    public float minPitch;
    public float maxPitch;
    public AudioSource carAudio;
    public AudioSource crashAudio;

    private float pitchFromCar;


    private Vector3 lastVelocity = Vector3.zero;
    public float maximumDeceleration;

    private void Awake(){
        carRB = GetComponent<Rigidbody>();
    }

    private void Update(){
        if(GameManager.instance.State == GameState.Playing){
            steerInput = Input.GetAxisRaw("Horizontal");
            accelerationInput = Input.GetAxisRaw("Vertical");
        }
        if(Input.GetKeyDown(KeyCode.Escape)) GameManager.instance.Pause();

        EngineSound();

        TurnWheels();
    }

    private void FixedUpdate(){
        CalculateSuspensionForces();
        CalculateSteeringForces();
        CalculateAccelerationBrakingForces();
    }

    private void CalculateSuspensionForces(){
        foreach(Transform t in tires){
            if(Physics.Raycast(t.position,-t.up,out RaycastHit hit)){
                Vector3 springDirection = t.up;
                Vector3 tireWorldVelocity = carRB.GetPointVelocity(t.position);
                offset = springRestLength - hit.distance;
                float velocity = Vector3.Dot(springDirection,tireWorldVelocity);
                float force = (offset * springStrength) - (velocity * springDamper);
                carRB.AddForceAtPosition(springDirection*force,t.position);
                Debug.DrawRay(t.position,springDirection*force,Color.green);
            }
        }
    }

    private void CalculateSteeringForces(){
        foreach(Transform t in tires){
            if(Physics.Raycast(t.position,-t.up,out RaycastHit hit)){
                Vector3 steeringDirection = t.right;
                Vector3 tireWorldVelocity = carRB.GetPointVelocity(t.position);
                float steeringVelocity = Vector3.Dot(steeringDirection,tireWorldVelocity);
                float desiredVelocityChange = -steeringVelocity * tireGripFactor;
                float desiredAcceleration = desiredVelocityChange/Time.fixedDeltaTime;
                carRB.AddForceAtPosition(steeringDirection * tireMass * desiredAcceleration,t.position);
                Debug.DrawRay(t.position,steeringDirection * tireMass * desiredAcceleration,Color.red);
            }
        }
    }

    private void CalculateAccelerationBrakingForces(){
        foreach(Transform t in tires){
            if(Physics.Raycast(t.position,-t.up,out RaycastHit hit)){
                Vector3 accelerationDirection = t.forward;
                carRB.AddForceAtPosition(accelerationDirection * accelerationInput * motorTorque,t.position);
                Debug.DrawRay(t.position,accelerationDirection * accelerationInput * motorTorque,Color.blue);
                if(accelerationInput == 0f){
                    float frictionForce = Vector3.Dot(transform.forward,carRB.velocity) * frictionCoefficient;
                    carRB.AddForceAtPosition(-accelerationDirection * frictionForce,t.position);
                }
            }
        }
    }

    private void CalculateAckermannAngles(){
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
    }

    private void TurnWheels(){
        CalculateAckermannAngles();
        foreach(Transform t in tires){
            if(t.GetComponent<Wheel>().frontLeft){
                steerAngle = ackermannAngleLeft;
                wheelAngle = Mathf.Lerp(wheelAngle,steerAngle,steerTime * Time.deltaTime);
                t.localRotation = Quaternion.Euler(Vector3.up * wheelAngle);
                t.GetComponent<Wheel>().visualWheel.localRotation = t.localRotation;
            }
            if(t.GetComponent<Wheel>().frontRight){
                steerAngle = ackermannAngleRight;
                wheelAngle = Mathf.Lerp(wheelAngle,steerAngle,steerTime * Time.deltaTime);
                t.localRotation = Quaternion.Euler(Vector3.up * wheelAngle);
                t.GetComponent<Wheel>().visualWheel.localRotation = t.localRotation;
            }
        }
    }

    private void EngineSound(){
        float currentSpeed = carRB.velocity.magnitude;
        float pitchFromCar = carRB.velocity.magnitude/50f;

        if(currentSpeed < minSpeed){
            carAudio.pitch = minPitch;
        }

        if(currentSpeed > minSpeed && currentSpeed < maxSpeed){
            carAudio.pitch = minPitch + pitchFromCar;
        }
        
        if(currentSpeed > maxSpeed){
            carAudio.pitch = maxPitch;
        }
    }
}
