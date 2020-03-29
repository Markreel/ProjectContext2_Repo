using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hovercraft_V4 : MonoBehaviour
{
    [SerializeField] Transform[] rayCheckPoints;
    [SerializeField] LayerMask terrainMask;

    [Header("Particle Settings: ")]
    [SerializeField] ParticleSystem smokeTrailParticleSystem;
    private float defaultSmokeParticleRateOverTime;
    private float defaultSmokeParticleStartSize;

    [Header("Actual Max Velocity: ")]
    [SerializeField] float actualMaxVelocity = 80;
    private float defaultActualVelocity = 80;

    [Header("Settings: ")]
    [SerializeField] float fallMultiplier = 2.5f;
    [Space]
    [SerializeField] float maxTorque;
    [SerializeField] float torqueMultiplier = 2;
    [Space]
    [SerializeField] float maxVelocity;
    [SerializeField] float velocityMultiplier;
    [SerializeField] float maxBoostVelocity;
    [SerializeField] float boostVelocityMultiplier;
    [Space]
    [SerializeField] float groundDistance;
    [SerializeField] float checkDistance;
    [SerializeField] Vector3 rotationalDrag;

    private Vector3 velocity;
    private Vector3 torque;
    private bool isGrounded;
    private bool isBoosting;

    private Rigidbody rb;
    private Transform child;

    private bool isEnabled = true;

    private void Awake()
    {
        child = transform.GetChild(0);
        rb = GetComponent<Rigidbody>();

        defaultSmokeParticleRateOverTime = smokeTrailParticleSystem.emissionRate;
        defaultSmokeParticleStartSize = smokeTrailParticleSystem.startSize;
        defaultActualVelocity = actualMaxVelocity;
    }

    private void Update()
    {
        HandleVelocityRelatedEffects();
    }

    private void FixedUpdate()
    {
        CheckRayPoints();
        if(isEnabled)
        {
            HandleInput();
            if (!isGrounded) { rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime; }
        }
    }

    private void CheckRayPoints()
    {
        bool _hitTerrain = false;

        foreach (var _point in rayCheckPoints)
        {
            RaycastHit _hit;

            if (Physics.Raycast(_point.position + Vector3.up, Vector3.down, out _hit, checkDistance, terrainMask.value))
            {
                if (!isGrounded) { isGrounded = true; }
                _hitTerrain = true;

                //transform.position = new Vector3(transform.position.x, _hit.point.y + groundDistance, transform.position.z); //hier zit een bug met wanneer je op muren rijd (gaat opzij ipv omhoog (pakt world y))

                transform.up = Vector3.Lerp(transform.up, _hit.normal, Time.deltaTime); //draai houd niet rekening met drastische verandering in slope
            }
        }
        if (!_hitTerrain && isGrounded) { isGrounded = false; } //transform.up = Vector3.up; }
    }

    private void HandleInput()
    {
        float _hor = Input.GetAxis("Horizontal");
        float _ver = Input.GetAxis("Vertical");

        if (Input.GetButton("Boost")) { isBoosting = true; }
        else { isBoosting = false; }

        if (Input.GetAxisRaw("Boost") > 0) { isBoosting = true; }
        else { isBoosting = false; }

        velocity.z = Mathf.Clamp(velocity.z + (_ver * velocityMultiplier * (isBoosting ? boostVelocityMultiplier : 1)) * Time.deltaTime, -maxVelocity,
            (isBoosting ? maxBoostVelocity : maxVelocity));
        torque.y = Mathf.Clamp(torque.y + _hor * torqueMultiplier * Time.deltaTime, -maxTorque, maxTorque);

        if (_hor == 0) { torque.y = Mathf.Lerp(torque.y, 0, Time.deltaTime * 3.5f); }

        Vector3 _forwardVelocity = child.forward * Time.deltaTime * velocity.z;

        //transform.position += child.forward * Time.deltaTime * velocity.z;
        //rb.AddForce(child.forward * Time.deltaTime * velocity.z * 100);
        //rb.velocity = new Vector3(_forwardVelocity.x, rb.velocity.y, _forwardVelocity.z);
        rb.AddForce(child.forward * _ver * velocityMultiplier * Time.deltaTime, ForceMode.VelocityChange);
        if (rb.velocity.magnitude > actualMaxVelocity) { rb.velocity = rb.velocity.normalized * actualMaxVelocity; }
        //Debug.Log(rb.velocity.magnitude);

        Rotate();
    }

    private void Rotate()
    {
        child.localEulerAngles += new Vector3(0, torque.y, 0);
        //child.localEulerAngles = new Vector3(0, child.localEulerAngles.y, 0);

        if (torque.y > 0) { torque.y = Mathf.Clamp(torque.y - rotationalDrag.y * Time.deltaTime, 0, maxTorque); }
        else if (torque.y < 0) { torque.y = Mathf.Clamp(torque.y + rotationalDrag.y * Time.deltaTime, -maxTorque, 0); }
    }

    private float currentAudioPitch = 1;
    private void HandleVelocityRelatedEffects()
    {
        float _multiplier = Mathf.Max(Mathf.Abs(rb.velocity.x), Mathf.Abs(rb.velocity.z));
        //Debug.Log(_multiplier);

        //Dust particles
        smokeTrailParticleSystem.emissionRate = Mathf.Clamp(defaultSmokeParticleRateOverTime / maxVelocity * _multiplier, 0, defaultSmokeParticleRateOverTime);
        smokeTrailParticleSystem.startSize = Mathf.Clamp((defaultSmokeParticleStartSize / maxVelocity * _multiplier) + 1, 0, defaultSmokeParticleStartSize);

        //Engine sound
        bool _engineOff = false;
        if(_multiplier < 5) { _engineOff = true; }

        float _targetAudioPitch = 2f / maxVelocity * _multiplier;

        float _pitch = Mathf.Lerp(currentAudioPitch, _targetAudioPitch, Time.deltaTime);
        currentAudioPitch = _pitch;

        AudioManager.Instance.SetHoverBikeSoundPitch(Mathf.Clamp(_pitch, 1f, 2f), _engineOff);
    }

    public void BehaviourOnStart()
    {
        DeactivatePlayer();
    }

    public void DeactivatePlayer()
    {
        isEnabled = false;
        velocity = Vector3.zero;
        //rb.velocity = Vector3.zero;
    }

    public void ActivatePlayer()
    {
        isEnabled = true;
    }

    public void SetMaxVelocity(float _maxVelocity)
    {
        actualMaxVelocity = _maxVelocity;
    }
}

