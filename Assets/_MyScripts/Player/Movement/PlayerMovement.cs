using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class PlayerMovement : MonoBehaviour
{
	[TabGroup("IsGrounded")]
	public float isGroundedHeight;
	[TabGroup("IsGrounded")]
	public Transform isGroundedCenter;
	[TabGroup("IsGrounded")]
	public bool isGrounded;
	
	[TabGroup("Movement")]
	public float jumpHeight;
	[TabGroup("Movement")]
	public float gravity = 9.81f;
	[TabGroup("Movement")]
	public float walkSpeed;
	[TabGroup("Movement")]
	public float runSpeed; //Lately will be added walkSpeed for slow walking or stealth and sprintSpeed
	
	[TabGroup("Camera")]
	public GameObject eyes;
	[TabGroup("Camera")]
	public float clampPositive;
	[TabGroup("Camera")]
	public float clampNegative;
	[TabGroup("Camera")]
	public float sensitivityX;
	[TabGroup("Camera")]
	public float sensitivityY;
	[TabGroup("Camera")]
	public float smooth;
	
	
	private Rigidbody rb;
	private PlayerControl playerControl;
	[ShowInInspector]
	float moveSpeed = 3;
	
	Vector2 moveVector;
	Vector2 camVector;
	float targetXrot;
	Vector3 move;
	
	void IsGrounded()
	{
		RaycastHit hit;
		if(Physics.Raycast(isGroundedCenter.position, Vector3.down, out hit, isGroundedHeight))
			isGrounded = true;
		else
			isGrounded = false;
	}
	
	protected void Awake()
	{
		rb = GetComponent<Rigidbody>();
		
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
	
	void OnEnable()
	{
		playerControl = new PlayerControl();
		playerControl.Keyboard.Enable();
		playerControl.Keyboard.Jump.started += Jump;
		playerControl.Keyboard.Movement.performed += Movement;
		playerControl.Keyboard.Look.performed += MouseLook;
		playerControl.Keyboard.Sprint.started += StartSprint;
		playerControl.Keyboard.Sprint.canceled += StopSprint;
	}
	
	void OnDisable()
	{
		playerControl.Keyboard.Disable();
	}
	
	void Update()
	{
		//Movement
		if(isGrounded)
		{
			moveVector = playerControl.Keyboard.Movement.ReadValue<Vector2>();
			move = new Vector3(moveVector.x * moveSpeed, 0, moveVector.y * moveSpeed);
			move.y = playerControl.Keyboard.Jump.ReadValue<float>() * jumpHeight;
			rb.velocity = new Vector3(0,0,0);
			//Debug.Log("X: " + moveVector.x + "Y: " + moveVector.y);
		}
		else
		{
			move.y -= gravity * Time.deltaTime;
		}
		transform.Translate(move * Time.deltaTime);
	}
	
	protected void LateUpdate()
	{
		IsGrounded();
	}
	
	void StartSprint(InputAction.CallbackContext context)
	{
		moveSpeed = runSpeed;
	}
	void StopSprint(InputAction.CallbackContext context)
	{	
		moveSpeed = walkSpeed;
	}
	
	void Jump(InputAction.CallbackContext context)
	{
		if(isGrounded)
		{
			//rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
		}
	}
	
	void Movement(InputAction.CallbackContext context)
	{
		//animations
	}
	
	void MouseLook(InputAction.CallbackContext context)
	{
		
		camVector = context.ReadValue<Vector2>();
		
		//Body Y rotation
		transform.rotation *= Quaternion.AngleAxis(camVector.x * Time.deltaTime * sensitivityX, Vector3.up);
		
		//Camera X rotation
		targetXrot += -camVector.y * sensitivityY * Time.deltaTime;
		targetXrot = Mathf.Clamp(targetXrot, clampNegative, clampPositive);
		eyes.transform.localRotation = Quaternion.Euler(targetXrot,0,0);
		
	}
	
	protected void OnDrawGizmos()
	{
		Debug.DrawRay(isGroundedCenter.position, Vector3.down * isGroundedHeight, Color.red);
	}
}