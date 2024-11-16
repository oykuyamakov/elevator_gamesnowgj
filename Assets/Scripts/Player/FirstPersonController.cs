using InputManagement;
using Roro.Scripts.GameManagement;
using UnityCommon.Runtime.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace _3rd_Party.Systems.StarterAssets.FirstPersonController.Scripts
{
	[RequireComponent(typeof(CharacterController))]
	public class FirstPersonController : MonoBehaviour
	{
		//this the same asset code ill look into it wtf is happening - yes
		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 4.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 6.0f;
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

		
		[Header("Player Grounded")]
		[SerializeField]
		private bool m_CheckForGrounded = true;
		public bool Grounded = true;
		public float GroundedOffset = -0.14f;
		public float RayLength = 10f;
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;

        public static Vector3 PlayerPosition;

		// cinemachine
		private float _cinemachineTargetPitch;

		// player
		private float _speed;
		private float _rotationVelocity;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;
		
		private CharacterController _controller;
		[SerializeField]
		private GameObject _mainCamera;

		public GameObject plCamera;

		private const float _threshold = 0.01f;

		private InputManager inputManager;

		private bool IsCurrentDeviceMouse => inputManager.IsCurrentDeviceMouse;

		private void Awake()
		{
            GameManager.Instance.SetPlayer(this);
		}

		private void Start()
		{
			PlayerPosition = transform.position;
			// TODO!!
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}

			plCamera = _mainCamera;
			
			_controller = GetComponent<CharacterController>();
			inputManager = InputManager.Instance;
		}

		private void Update()
		{
            
            PlayerPosition = transform.position;
			Move();
		}

		private void LateUpdate()
		{
			CameraRotation();
		}
		
		private bool GroundedCheckWithOffset(Vector3 offset)
		{
			if (!m_CheckForGrounded)
				return true;
			
			Vector3 spherePosition = transform.position.WithY(transform.position.y + GroundedOffset) + offset;
			
			Physics.Raycast(spherePosition, Vector3.down, out var hit, RayLength, GroundLayers, QueryTriggerInteraction.Ignore);

			// if(hit.collider != null)
			// 	Debug.Log(hit.collider.name);

			
			Grounded = hit.collider != null;
			return hit.collider != null;
		}

		private void CameraRotation()
		{
			if (!(inputManager.Look.sqrMagnitude >= _threshold)) 
				return;
			
			//Don't multiply mouse input by Time.deltaTime
			float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
				
			_cinemachineTargetPitch += (-inputManager.Look.y) * RotationSpeed * deltaTimeMultiplier;
			_rotationVelocity = inputManager.Look.x * RotationSpeed * deltaTimeMultiplier;

			// clamp our pitch rotation
			_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

			// Update Cinemachine camera target pitch
			CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

			// rotate the player left and right
			transform.Rotate(Vector3.up * _rotationVelocity);
		}

		private void Move()
		{
			float targetSpeed = inputManager.Sprint ? SprintSpeed : MoveSpeed;
			
			if (inputManager.Move == Vector2.zero) targetSpeed = 0.0f;

			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = inputManager.AnalogMovement ? inputManager.Move.magnitude : 1f;

			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}

			Vector3 inputDirection = new Vector3(inputManager.Move.x, 0.0f, inputManager.Move.y).normalized;

			if (inputManager.Move != Vector2.zero)
			{
				inputDirection = transform.right * inputManager.Move.x + transform.forward * inputManager.Move.y;
			}

			if (!GroundedCheckWithOffset(inputDirection * 1.2f))
			{
				_controller.SimpleMove(Vector3.zero);
				return;
			}
            
			_controller.SimpleMove(inputDirection.normalized * (_speed));
		}
		
		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			var position = transform.position;
			Gizmos.DrawSphere(new Vector3(position.x, position.y + GroundedOffset, position.z), 0.1f);
			var rayPos = position.WithY(position.y + GroundedOffset);
            Gizmos.DrawLine(rayPos, rayPos.WithY(rayPos.y - RayLength));
		}
	}
}