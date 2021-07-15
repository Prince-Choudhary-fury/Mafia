using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPPCharController : MonoBehaviour
{

    public GameObject player;
    public Camera playerCam;

    public int playerSpeed;

    public int playerWalkSpeed = 2;
    public int playerSprintSpeed = 5;
    public float jumpSpeed = 15f;
    public float rotationSpeed = 15f;
    public float AnimationBlendSpeed = 2f;

    //x
    private float horizontalInput;
    //z
    private float verticalInput;
    private float desiredAnimationSpeed = 0f;
    private float animationSpeed;
    private float playerSpeedY = 0;
    private float Gravity = -9.81f;
    private Vector3 playerMovement;
    private Vector3 verticalMovement;
    private Vector3 rotatedMovement;
    private float desiredRotation;
    private Quaternion currentRotation;
    private Quaternion targetRotation;

    private bool isSprinting = false;
    private bool isJumping = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            isJumping = true;
            player.GetComponent<Animator>().SetTrigger("Jump");

            playerSpeedY += jumpSpeed;

        }

        if (!player.GetComponent<CharacterController>().isGrounded)
        {
            playerSpeedY += Gravity * Time.deltaTime;
        }
        else
        {
            playerSpeedY = 0;
        }

        isSprinting = Input.GetKey(KeyCode.LeftShift);

        playerMovement = new Vector3(horizontalInput, 0, verticalInput).normalized;

        rotatedMovement = Quaternion.Euler(0, playerCam.transform.rotation.eulerAngles.y, 0) * playerMovement;
        verticalMovement = Vector3.up * playerSpeedY;

        if (isSprinting && !isJumping)
        {
            playerSpeed = playerSprintSpeed;
            animationSpeed = 1;
        }
        else if (playerSpeedY < 0)
        {
            playerSpeed = playerWalkSpeed;
            animationSpeed = 0.5f;
        }
        player.GetComponent<Animator>().SetFloat("SpeedY", playerSpeedY / jumpSpeed);
        if (isJumping && playerSpeedY < 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(player.transform.position, Vector3.down, out hit, 0.5f, LayerMask.GetMask("Default")));
            {
                isJumping = false;
                player.GetComponent<Animator>().SetTrigger("Land");
            }
        }
        player.GetComponent<CharacterController>().Move((verticalMovement + ( rotatedMovement * playerSpeed)) * Time.deltaTime);
        if(rotatedMovement.magnitude > 0)
        {
            desiredRotation = Mathf.Atan2(rotatedMovement.x,rotatedMovement.z) * Mathf.Rad2Deg;
            desiredAnimationSpeed = animationSpeed;
        }
        else
        {
            desiredAnimationSpeed = 0;
        }
        player.GetComponent<Animator>().SetFloat("Speed", Mathf.Lerp(player.GetComponent<Animator>().GetFloat("Speed"), desiredAnimationSpeed, AnimationBlendSpeed * Time.deltaTime));
        currentRotation = player.transform.rotation;
        targetRotation = Quaternion.Euler(0, desiredRotation, 0);

        player.transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
