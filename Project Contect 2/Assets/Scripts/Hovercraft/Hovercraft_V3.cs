using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fog in canyon (kijk of t dynamisch is)
/// </summary>

public class Hovercraft_V3 : MonoBehaviour
{
    [SerializeField] Transform[] rayCheckPoints;
    [SerializeField] LayerMask terrainMask;

    [Header("Settings: ")]
    [SerializeField] float maxStepHeight = 2f;
    [SerializeField] float maxSlopeAngle = 25f;
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
    [SerializeField] float velocityDrag;

    private Vector3 velocity;
    private Vector3 torque;
    private Transform child;
    private Vector3 previousPosition;

    private Rigidbody rb;

    private bool isBoosting;

    private void Awake()
    {
        child = transform.GetChild(0);
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //CheckCollision();
        HandleInput();
    }

    private bool CheckCollision()
    {
        foreach (var _point in rayCheckPoints)
        {
            RaycastHit _hit;

            if (Physics.Raycast(_point.position + Vector3.up * 100, Vector3.down, out _hit, Mathf.Infinity, terrainMask.value))
            {
                //Debug.Log("ANGLE: " + Vector3.Angle(transform.up, _hit.normal) + " |  HEIGHT: " + Vector3.Distance(_point.position, _hit.point));
                if (Vector3.Angle(transform.up, _hit.normal) < maxSlopeAngle && Vector3.Distance(_point.position, _hit.point) < maxStepHeight)
                {
                    transform.up = Vector3.Lerp(transform.up, _hit.normal, Time.deltaTime * 15f);
                    transform.position = new Vector3(transform.position.x, _hit.point.y + groundDistance, transform.position.z);
                    return false;
                }
                else { return true; }
            }
            else { return false; }
        }
        return false;
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

        bool _colliding = CheckCollision();
        if (_colliding) { velocity = Vector3.zero; transform.position = previousPosition; }

        previousPosition = transform.position;
        transform.position += child.forward * Time.deltaTime * velocity.z;

        Debug.Log("Velocity: " + velocity.z);

        //Add drag
        if (_ver == 0 || _ver != Mathf.Sign(velocity.z)) //lekker bezig mark, nu level design aan de hand van dit maken en ff snelheid enzo chill instellen
        {
            if (velocity.z > 0) { velocity.z = Mathf.Clamp(velocity.z - velocityDrag, -maxVelocity, (isBoosting ? maxBoostVelocity : maxVelocity)); }
            else if (velocity.z < 0) { velocity.z = Mathf.Clamp(velocity.z + velocityDrag, -maxVelocity, (isBoosting ? maxBoostVelocity : maxVelocity)); }
        }


        //transform.position += IsColliding() ? Vector3.zero : child.forward * Time.deltaTime * velocity.z;

        Rotate();
    }

    private void Rotate()
    {
        child.localEulerAngles += new Vector3(0, torque.y, 0);

        if (torque.y > 0) { torque.y = Mathf.Clamp(torque.y - rotationalDrag.y * Time.deltaTime, 0, maxTorque); }
        else if (torque.y < 0) { torque.y = Mathf.Clamp(torque.y + rotationalDrag.y * Time.deltaTime, -maxTorque, 0); }
    }

    private bool IsColliding()
    {
        RaycastHit _hit;

        Vector3 _direction = child.forward * Time.deltaTime * velocity.z;

        if (Physics.Raycast(transform.position, _direction, out _hit, velocity.z, terrainMask.value))
        {
            return true;
        }
        else { return false; }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.collider.gameObject.layer == terrainMask.value)
    //    {
    //        transform.position = previousPosition;
    //    }
    //}
}
