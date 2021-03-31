using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    [SerializeField]
    private Transform playSpace;

    [SerializeField]
    private Transform head;

    [SerializeField]
    private Hand leftHand;

    [SerializeField]
    private Hand rightHand;

    [SerializeField]
    private float rotationSpeed = 1.25f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        // MOVEMENT CONTROLS
        // Get input from the left controller and translate it into a Vector3
        Vector2 moveInput = leftHand.GetJoystickInput();
        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);

        // Move in the direction of the controller input respective to where the player is facing.
        Vector3 velocity = head.TransformDirection(moveDir) * Time.deltaTime;
        velocity.y = 0;

        // Move the entire play space to move the player
        playSpace.transform.Translate(velocity, Space.World);

        // ROTATION CONTROLS
        // Get input from the right controller
        Vector2 rotInput = rightHand.GetJoystickInput();
        float rotDirection = rotInput.normalized.x;

        // Add the input's x direction to the playSpace's rotation.
        playSpace.Rotate(0, rotDirection * rotationSpeed, 0, Space.Self);
    }
}
