using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hovercraft_V4 : MonoBehaviour
{
    [SerializeField] Transform[] rayCheckPoints;
    [SerializeField] LayerMask terrainMask;

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

    private Rigidbody rb;

    private Transform child;

    private bool isBoosting;

    private void Awake()
    {
        child = transform.GetChild(0);
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        CheckRayPoints();
        HandleInput();

        if (!isGrounded)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
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
        if (rb.velocity.magnitude > 80) { rb.velocity = rb.velocity.normalized * 80; }
        Debug.Log(rb.velocity.magnitude);

        Rotate();
    }

    private void Rotate()
    {
        child.localEulerAngles += new Vector3(0, torque.y, 0);
        //child.localEulerAngles = new Vector3(0, child.localEulerAngles.y, 0);

        if (torque.y > 0) { torque.y = Mathf.Clamp(torque.y - rotationalDrag.y * Time.deltaTime, 0, maxTorque); }
        else if (torque.y < 0) { torque.y = Mathf.Clamp(torque.y + rotationalDrag.y * Time.deltaTime, -maxTorque, 0); }
    }
}

