using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public abstract class PPController: MonoBehaviour
{
	public abstract void TriggerPP(Constants.JumpDirection direction, bool jumpOut);
}
public abstract class DisController: MonoBehaviour
{
	public abstract void TriggerDissolve(Constants.JumpDirection direction, bool jumpOut);
}


public class TimeConn : MonoBehaviour, DissolveUser
{
	[SerializeField] private HudDebugPanel _debugPanel;
	[SerializeField] private PlayerController _player;
	[SerializeField] private ParticleController _particles;
	[SerializeField] private PhotonView _view;
	[SerializeField] private TailManager _tailManager;
	[SerializeField] private PPController _ppController;
	[SerializeField] private DissolveController _disMinerController;
	[SerializeField] private DissolveController _disGuardianController;

	private DissolveController _disController;
	private SceneController _sceneController;
	private TimeLord _timelord;
	private bool _isJumping;
	private Constants.JumpDirection _jumpDirection;
	private bool _setJumpState;
	private float _forwardsJumpCooldown;
	private float _backJumpCooldown;
	private bool _timeTravelEnabled;
	private bool _isDissolving;
	private int _syncTimer;


	// ------------ UNITY METHODS ------------

	void Awake()
	{
		_isJumping = false;
		_jumpDirection = Constants.JumpDirection.Static;
		_setJumpState = false;
		_forwardsJumpCooldown = 15f;
		_backJumpCooldown = 15f;
		_timeTravelEnabled = true;
		_isDissolving = false;
		_syncTimer = 10;
	}

	void OnEnable()
	{
		GameController.gameActive += OnGameActive;
		GameController.gameStarted += OnGameStarted;
		GameController.gameEnded += OnGameEnded;
	}

	void OnDisable()
	{
		GameController.gameActive -= OnGameActive;
		GameController.gameStarted -= OnGameStarted;
		GameController.gameEnded -= OnGameEnded;
	}

	void Start()
	{
		Debug.Log("TimeConn Start");
		_sceneController = FindObjectOfType<PreGameController>();
		if (_sceneController == null) Debug.LogError("PreGameController not found");
		else SetTimeLord();

		// Make sure that this script is executed before ParticleController.
		if (_player.Team == Constants.Team.Guardian)
		{
			_disController = _disGuardianController;
		}
		else _disController = _disMinerController;
		_disController.SetSubscriber(this);
		_tailManager.SetActive(true);
	}

	void Update() {
		// Local keys only affect client's player.
		if (_view.IsMine)
		{
			UpdateCooldowns();
			KeyControl();
		}
		if (_timeTravelEnabled)
		{
			UpdateTimeTravel();

			// If master client, synchronise everyone else every ten frames.
			if (PhotonNetwork.IsMasterClient && _syncTimer <= 0)
			{
				Dictionary<int, int[]> data = new Dictionary<int, int[]>();
				foreach (var reality in _timelord.GetRealities())
				{
					data.Add(reality.Key, reality.Value.GetData());
				}
				_view.RPC("RPC_synchronise", RpcTarget.All, data);
				_syncTimer = 10;
			}
			_syncTimer--;
		}
	}


	// ------------ ON EVENT METHODS ------------

	private void OnGameActive(GameController game)
	{
		_sceneController = game;
		SetTimeLord();
		_timeTravelEnabled = false;
		_forwardsJumpCooldown = 15;
		_backJumpCooldown = 15;
	}

	private void OnGameStarted()
	{
		_timeTravelEnabled = true;
	}

	private void OnGameEnded(Constants.Team winningTeam)
	{
		_timeTravelEnabled = false;
		Destroy(this);
	}

	public void NotifyStartedDissolving()
	{
		_isDissolving = true;
		if (_timelord.InYourReality(_view.ViewID))
		{
			_player.Show();
		}
	}

	public void NotifyStoppedDissolving(bool dissolvedOut)
	{
		_isDissolving = false;
		if (dissolvedOut)
		{
			_jumpDirection = Constants.JumpDirection.Static;
			_player.Hide();
		}
	}


	// ------------ PRIVATE METHODS ------------

	private void SetTimeLord()
	{
		if (_view.IsMine && _timelord != null) _debugPanel.UnRegister(_timelord);
		_timelord = _sceneController.GetTimeLord();
		_timelord.Connect(_view.ViewID, _view.IsMine);
		_timelord.EnterReality(_view.ViewID);
		if (_view.IsMine) _debugPanel.Register(_timelord);
	}

	// Returns true if you can jump in the given direction.
	private bool CanTimeTravel(Constants.JumpDirection direction)
	{
		if (direction == Constants.JumpDirection.Backward)
		{
			return _backJumpCooldown <= 0f && _timelord.CanJump(_view.ViewID, Constants.JumpDirection.Backward);
		}
		else if (direction == Constants.JumpDirection.Forward)
		{
			return _forwardsJumpCooldown <= 0f && _timelord.CanJump(_view.ViewID, Constants.JumpDirection.Forward);
		}
		else
		{
			Debug.LogError("Can't jump without a direction.");
			return false;
		}
	}

	private void TimeJump(Constants.JumpDirection direction, bool jumpOut)
	{
		if (!_view.IsMine) throw new InvalidOperationException("This function may not be called on an RPC-controlled Player.");

		if (_timeTravelEnabled)
		{
			if (jumpOut)
			{
				if (CanTimeTravel(direction))
				{
					_view.RPC("RPC_jumpOut", RpcTarget.All, direction);
					_tailManager.EnableParticles(false);
					_ppController?.TriggerPP(direction, jumpOut);
					
				}
			}
			else if (_isJumping)
			{
				int frame = _timelord.GetNearestReality(_view.ViewID);
				_view.RPC("RPC_jumpIn", RpcTarget.All, _view.ViewID, frame);
				_tailManager.EnableParticles(true);
				_ppController?.TriggerPP(direction, jumpOut);
				
			}
		}
	}

	private void UpdateCooldowns()
	{
		_forwardsJumpCooldown = (_forwardsJumpCooldown > 0) ? (_forwardsJumpCooldown - Time.deltaTime) : 0;
		_backJumpCooldown = (_backJumpCooldown > 0) ? (_backJumpCooldown - Time.deltaTime) : 0;
	}

	private void KeyControl()
	{
		if (Input.GetKeyDown(KeyCode.Q)) TimeJump(Constants.JumpDirection.Backward, true);
		if (Input.GetKeyDown(KeyCode.E)) TimeJump(Constants.JumpDirection.Forward, true);
		if (Input.GetKeyUp(KeyCode.Q)) TimeJump(Constants.JumpDirection.Backward, false);
		if (Input.GetKeyUp(KeyCode.E)) TimeJump(Constants.JumpDirection.Forward, false);
	}


	private void UpdateTimeTravel()
	{
		// Record your state in all realities you exist in.
		Vector3 pos = transform.position;
		Quaternion rot = transform.rotation;
		Constants.JumpDirection dir = Constants.JumpDirection.Static;

		// Only set the jump direction once, when the _setJumpState flag is active.
		if (_setJumpState)
		{
			dir = _jumpDirection;
			_setJumpState = false;
		}

		PlayerState ps = new PlayerState(_view.ViewID, pos, rot, dir, _isJumping);
		_timelord.RecordState(ps);

		if (_isJumping)
		{
			// Perform the time jump.
			if (_timelord.CanJump(_view.ViewID, _jumpDirection))
			{
				_timelord.TimeTravel(_view.ViewID, _jumpDirection);
			}
			// Force stop jumping.
			else if (_view.IsMine) TimeJump(_jumpDirection, false);
		}
		else _jumpDirection = Constants.JumpDirection.Static;
	}


	// ------------ RPC FUNCTIONS ------------

	[PunRPC]
	void RPC_jumpOut(Constants.JumpDirection direction)
	{
		_isJumping = true;
		_jumpDirection = direction;
		_setJumpState = true;
		_timelord.LeaveReality(_view.ViewID);
		if (direction == Constants.JumpDirection.Forward) _forwardsJumpCooldown = 15;
		else _backJumpCooldown = 15;

		if (_view.IsMine) _sceneController.HideAllPlayers();
		else if (!_view.IsMine && gameObject.layer == Constants.LayerPlayer)
		{
			_disController?.TriggerDissolve(_jumpDirection, true);
			_particles.StartParticles(_jumpDirection);
		}
	}

	[PunRPC]
	void RPC_jumpIn(int playerID, int frame)
	{
		_isJumping = false;
		_timelord.SetPerceivedFrame(playerID, frame);
		_timelord.EnterReality(_view.ViewID);
		
		if (_view.IsMine)
		{
			// TODO: The following line might be redundant?
			gameObject.layer = Constants.LayerPlayer;
			_sceneController.ShowPlayersInReality();
		}
		else if (_timelord.InYourReality(_view.ViewID))
		{
			_disController?.TriggerDissolve(_jumpDirection, false);
			_particles.StartParticles(_jumpDirection);
		}
	}

	[PunRPC]
	void RPC_synchronise(Dictionary<int, int[]> data)
	{
		Dictionary<int, Reality> realities = new Dictionary<int, Reality>();
		foreach (var item in data)
		{
			realities.Add(item.Key, new Reality(item.Value));
		}
		_timelord.SetRealities(realities);
	}


	// ------------ PUBLIC METHODS ------------

	public (float forward, float back) GetCooldowns() { return (_forwardsJumpCooldown, _backJumpCooldown); }

	public (bool forward, bool back) GetCanJump()
	{
		bool canJumpForward = _timeTravelEnabled && CanTimeTravel(Constants.JumpDirection.Forward);
		bool canJumpBack = _timeTravelEnabled && CanTimeTravel(Constants.JumpDirection.Backward);
		return (canJumpForward, canJumpBack);
	}

	public Constants.Team GetTeam()
	{
		return _player.Team;
	}
}
