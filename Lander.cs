using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour {

    public static Lander Instance { get; private set;}

    public event EventHandler OnUpForce;
    public event EventHandler OnRightForce;
    public event EventHandler OnLeftForce;
    public event EventHandler OnBeforeForce;
    public event EventHandler OnCoinPickup;
    public event EventHandler<OnLandedEventArgs> OnLanded;
    public class OnLandedEventArgs : EventArgs {
        public int score;
    }

    private Rigidbody2D landerRigidBody2D;
    private float fuelAmount;
    private float fuelAmountMax = 10f;

    private void Awake() {
        fuelAmount = fuelAmountMax;

        Instance = this;

        landerRigidBody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void FixedUpdate() {
        OnBeforeForce?.Invoke(this, EventArgs.Empty);
        //Debug.Log(fuelAmount);
        if(fuelAmount <= 0f) {
            // No fuel
            return;
        }

        /* Used for only one fuel call, instead of multiple per key pressed
        if (Keyboard.current.upArrowKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed) {
            // Pressing any input
            ConsumeFuel();
        }
        */

        if (Keyboard.current.upArrowKey.isPressed) {
            float force = 700f;
            landerRigidBody2D.AddForce(force * transform.up * Time.deltaTime);
            ConsumeFuel();
            OnUpForce?.Invoke(this, EventArgs.Empty);
        }
        if (Keyboard.current.leftArrowKey.isPressed) {
            float turnSpeed = +100f;
            landerRigidBody2D.AddTorque(turnSpeed * Time.deltaTime);
            ConsumeFuel();
            OnLeftForce?.Invoke(this, EventArgs.Empty);
        }
        if (Keyboard.current.rightArrowKey.isPressed) {
            float turnSpeed = -100f;
            landerRigidBody2D.AddTorque(turnSpeed * Time.deltaTime);
            ConsumeFuel();
            OnRightForce?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision2D) {
        if (!collision2D.gameObject.TryGetComponent(out LandingPad landingPad)) {
            Debug.Log("Crashed on the Terrain!");
            return;
        }

        float softLandingVelocityMagnitude = 4f;
        float relativeVelocityMagnitude = collision2D.relativeVelocity.magnitude;
        if (relativeVelocityMagnitude > softLandingVelocityMagnitude) {
            // Landed too hard!
            Debug.Log("Landed too hard!");        
            return;
        }

        float dotVector = Vector2.Dot(Vector2.up, transform.up);
        float minDotVector = .90f;
        if (dotVector < minDotVector) {
            // Landed on a too steep angle!
            Debug.Log("Landed on a too steep angle!");
            return;
        }

        Debug.Log("Successful Landing!");
        
        float maxScoreAmountLandingAngle = 100;
        float scoreDotVectorMultiplier = 10f;
        float landingAngleScore = maxScoreAmountLandingAngle - Mathf.Abs(dotVector - 1f) * scoreDotVectorMultiplier * maxScoreAmountLandingAngle;

        float maxScoreAmountLandingSpeed = 100;
        float landingSpeedScore = (softLandingVelocityMagnitude - relativeVelocityMagnitude) * maxScoreAmountLandingSpeed;

        Debug.Log("LandingAngleScore: " + landingAngleScore);
        Debug.Log("LandingSpeedScore: " + landingSpeedScore);

        int score = Mathf.RoundToInt((landingAngleScore + landingSpeedScore) * landingPad.GetScoreMultiplier());

        Debug.Log("score: " + score);
        OnLanded?.Invoke(this, new OnLandedEventArgs {
           score = score, 
        });
    }

    private void OnTriggerEnter2D(Collider2D collider2D) {
        if (collider2D.gameObject.TryGetComponent(out FuelPickup fuelPickup)){
            float addFuelAmount = 10f;
            fuelAmount += addFuelAmount; 
            if (fuelAmount > fuelAmountMax) {
                fuelAmount = fuelAmountMax;
            }
            fuelPickup.DestroySelf();
        }

        if (collider2D.gameObject.TryGetComponent(out CoinPickup coinPickup)) {
            OnCoinPickup?.Invoke(this, EventArgs.Empty);
            coinPickup.DestroySelf();
        }
    }

    private void ConsumeFuel() {
        float fuelConsumptionAmount = 1f;
        fuelAmount -= fuelConsumptionAmount * Time.deltaTime;
    }

    public float GetFuelAmount() {
        return fuelAmount;
    }

    public float GetFuelAmountNormalized() {
        return fuelAmount / fuelAmountMax;
    }

    public float GetSpeedX() {
        return landerRigidBody2D.linearVelocityX;
    }
    
    public float GetSpeedY() {
        return landerRigidBody2D.linearVelocityY;
    }
}