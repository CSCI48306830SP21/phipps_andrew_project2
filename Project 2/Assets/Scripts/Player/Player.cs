using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class Player : Entity
{
    [SerializeField]
    private Transform playSpace;

    [SerializeField]
    private Transform head;
    public Transform Head => head;

    [SerializeField]
    private Hand leftHand;

    [SerializeField]
    private Hand rightHand;

    [SerializeField]
    private float movementSpeed = 1f;

    [SerializeField]
    private float rotationSpeed = 1.25f;

    [SerializeField]
    private Image healthVignette;

    [SerializeField]
    private GameObject gameOverUI;

    [SerializeField]
    private Text gameOverFloorText;

    [SerializeField]
    private AudioSource audioSource;
    public AudioSource AudioSource => audioSource;

    [SerializeField]
    private AudioSource musicSource;
    public AudioSource MusicSource => musicSource;

    private Rigidbody rb;

    private Vector3 velocity;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody>();

        // Disable UI
        gameOverUI.SetActive(false);
        SetHealthVignetteAlpha(0);
    }

    // Update is called once per frame
    void Update()
    {
        // Disable controls if dead
        //if (IsDead)
            //return;

        // MOVEMENT CONTROLS
        // Get input from the left controller and translate it into a Vector3
        Vector2 moveInput = leftHand.GetJoystickInput();
        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);

        // Move in the direction of the controller input respective to where the player is facing.
        velocity = head.TransformDirection(moveDir) * movementSpeed;
        velocity.y = 0;

        // ROTATION CONTROLS
        // Get input from the right controller
        Vector2 rotInput = rightHand.GetJoystickInput();
        float rotDirection = rotInput.normalized.x;

        // Add the input's x direction to the playSpace's rotation.
        playSpace.Rotate(0, rotDirection * rotationSpeed, 0, Space.Self);
    }

    void FixedUpdate() {
        // Move the entire play space to move the player
        rb.MovePosition(rb.position + velocity * Time.deltaTime);
    }

    public override void TakeDamage(int damage) {
        base.TakeDamage(damage);

        // Set the health vignette to our current health percent. 100% HP = 0, 50% HP = 0.5, etc.
        SetHealthVignetteAlpha(((float)MaxHealth - Health) / MaxHealth);
    }

    public override void AddHealth(int health) {
        base.AddHealth(health);

        // Set the health vignette to our current health percent. 100% HP = 0, 50% HP = 0.5, etc.
        SetHealthVignetteAlpha(((float)MaxHealth - Health) / MaxHealth);
    }

    protected override void Die() {
        base.Die();

        GameOver();
    }

    /// <summary>
    /// Sets the alpha of the health vignette's color to the provided value.
    /// </summary>
    /// <param name="a"></param>
    private void SetHealthVignetteAlpha(float a) {
        // Temp color  value
        Color vignetteColor = healthVignette.color;
        vignetteColor.a = a; // Set alpha

        // Set color
        healthVignette.color = vignetteColor;
    }

    /// <summary>
    /// Shows game over UI.
    /// </summary>
    private void GameOver() {
        SetHealthVignetteAlpha(1);
        gameOverUI.SetActive(true);
        gameOverFloorText.text = "Floor: " + GameManager.Instance.CurrentFloor;
    }
}
