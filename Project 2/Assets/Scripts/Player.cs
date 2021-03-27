using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Transform playSpace;

    [SerializeField]
    private Transform head;

    [SerializeField]
    private Hand leftHand;

    [SerializeField]
    private Hand rightHand;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // MOVEMENT CONTROLS
        // Get input from the left controller and translate it into a Vector3
        Vector2 input = leftHand.GetMovementInput();
        Vector3 inputDir = new Vector3(input.x, 0, input.y);

        // Move in the direction of the controller input respective to where the player is facing.
        Vector3 moveDir = head.TransformDirection(inputDir) * Time.deltaTime;
        moveDir.y = 0;

        // Move the entire play space to move the player
        playSpace.transform.Translate(moveDir, Space.World);
    }
}
