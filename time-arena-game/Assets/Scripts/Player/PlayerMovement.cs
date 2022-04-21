using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour, Debuggable
{
    [SerializeField] private HudDebugPanel _debugPanel;
    [SerializeField] private PlayerController _player;
    public PhotonView View;
    public CharacterController CharacterBody;
    public Transform PlayerTransform;
    public LayerMask GroundMask;
    public GameObject CameraHolder;

    private float _speed;
    private float _groundCheckRadius;
    private Vector3 _velocity;
    private float _jumpPower;
    private float _gravity;
    private bool _isGrounded;
    private bool _isCeiling;
    private float _xRot;
    private float _mouseSensitivity;
    private bool _lockMovement;
    private bool _lockRotation;
    private bool _activated;
    private Vector3[] _hiderSpawnPoints;
	private Vector3 _seekerSpawnPoint;


    // ------------ UNITY FUNCTIONS ------------

    void Awake()
    {
        _speed = 5f;
        _groundCheckRadius = 0.5f;
        _jumpPower = 3f;
        _gravity = 40f;
        _isGrounded = true;
        _isCeiling = false;
        _xRot = 0f;
        _mouseSensitivity = 100f;
        _lockMovement = false;
        _lockRotation = false;
        _activated = true;
        _seekerSpawnPoint = new Vector3(-24f, -5f, -18f);
        _hiderSpawnPoints =  new Vector3[] {
			new Vector3(-19f, -5f, -33f),
			new Vector3(-25f, -5f, -31f), 
			new Vector3(-11f, -5f, -30f), 
			new Vector3(-18f, -5f, -39f), 
			new Vector3(-25f, -5f, -36f)
		};
    }

    void OnEnable()
    {
        GameController.gameActive += OnGameActive;
        GameController.gameStarted += OnGameStarted;
        GameController.gameEnded += OnGameEnded;
        PauseManager.paused += OnPaused;
    }

    void OnDisable()
    {
        GameController.gameActive -= OnGameActive;
        GameController.gameStarted -= OnGameStarted;
        GameController.gameEnded -= OnGameEnded;
        PauseManager.paused -= OnPaused;
    }

    void Start()
    {
        Physics.IgnoreLayerCollision(Constants.LayerOutsideReality, Constants.LayerPlayer);
        Physics.IgnoreLayerCollision(Constants.LayerOutsideReality, Constants.LayerOutsideReality);

        _debugPanel.Register(this);
    }

    void Update()
    {
        if (View.IsMine && _activated)
        {
            UpdatePosition();
            UpdateRotation();
        }
    }


    // ------------ ON EVENT FUNCTIONS ------------

    private void OnGameActive(GameController game)
    {
        _lockMovement = true;
        MoveToSpawnPoint();
    }

    private void OnGameStarted() { _lockMovement = false; }

    private void OnGameEnded(Constants.Team winningTeam) { _activated = false; }

    private void OnPaused(bool isPaused)
    {
        _lockMovement = isPaused;
        _lockRotation = isPaused;
    }
    
    // Called when mouse sensitivity is changed in the pause screen.
    public void OnMouseSensChange(float sensitivity) { _mouseSensitivity = sensitivity; }


    // ------------ PRIVATE METHODS ------------

    private void UpdatePosition()
    {
        // Sprint speed.
        if (Input.GetKey("left shift") && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))) _speed = 10f;
		else _speed = 5f;

        // Get movement axis values.
        float xMove = _lockMovement ? 0 : Input.GetAxis("Horizontal");
        float zMove = _lockMovement ? 0 : Input.GetAxis("Vertical");

        // Check if player's bottom intersects with any environment object.
        Vector3 groundCheck = PlayerTransform.position;
        groundCheck.y -= 1f;
        _isGrounded = Physics.CheckSphere(groundCheck, _groundCheckRadius, GroundMask);

        //Check if player's head intersects with any environment object.
        Vector3 ceilingCheck = PlayerTransform.position;
        ceilingCheck.y += 0.6f;
        _isCeiling = Physics.CheckSphere(ceilingCheck, _groundCheckRadius, GroundMask);

        // Set and normalise movement vector.
        Vector3 movement = (transform.right * xMove) + (transform.forward * zMove);
        if (movement.magnitude != 1 && movement.magnitude != 0)
        {
            movement /= movement.magnitude;
        }

        // Transform according to movement vector.
        CharacterBody.Move(movement * _speed * Time.deltaTime);

		// Jump control.
		if (Input.GetButtonDown("Jump") && _isGrounded && !_lockMovement)
        {
			_velocity.y += Mathf.Sqrt(_jumpPower * 2f * _gravity);
		}

        // Gravity effect.
        _velocity.y -= _gravity * Time.deltaTime;
		if (_velocity.y <= -100f) _velocity.y = -100f;

		// Reset vertical velocity value when grounded.
		if (_isGrounded && _velocity.y < 0) _velocity.y = 0f;

        // Reset vertical velocity when head it hitting ceiling.
        if (_isCeiling && _velocity.y > 0) _velocity.y = 0f;

        // Move player according to gravity.
        CharacterBody.Move(_velocity * Time.deltaTime);
    }

    private void UpdateRotation()
    {
        // Rotate player about y and playercam about x.
		// Get axis values from input.
        // deltaTime used for fps correction.
		float mouseX = _lockRotation ? 0 : Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
		float mouseY = _lockRotation ? 0 : Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

		// Invert vertical rotation and restrict up/down.
		_xRot -= mouseY;
		_xRot = Mathf.Clamp(_xRot, -90f, 90f);

		// Apply rotation.
		CameraHolder.transform.localRotation = Quaternion.Euler(_xRot, 0f, 0f);

        // Rotate player about y axis with mouseX movement.
		transform.Rotate(Vector3.up * mouseX);
    }

    private void MoveToSpawnPoint()
	{
		if (_player.Team == Constants.Team.Miner)
		{
			int index = Random.Range(0, _hiderSpawnPoints.Length);
			Vector3 position = _hiderSpawnPoints[index];
			transform.position = position;
		}
		else transform.position = _seekerSpawnPoint;
	}

    // ------------ PUBLIC METHODS ------------

    public Hashtable GetDebugValues()
    {
        Hashtable debugValues = new Hashtable();
        debugValues.Add("IsGrounded", _isGrounded);
        return debugValues;
    }
}
