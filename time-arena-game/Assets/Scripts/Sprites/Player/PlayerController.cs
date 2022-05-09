using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class PlayerController : MonoBehaviour, Debuggable
{
	[SerializeField] protected GameObject _camera;
	[SerializeField] protected GameObject _UI;
	[SerializeField] protected GameObject _me;
	[SerializeField] protected PhotonView _view;
	[SerializeField] protected GameObject _mesh;
	[SerializeField] protected PlayerMovement _playerMovement;
	private string _userID;
	private Vector3 _spawnpoint;
	private Dictionary<int, string> _viewIDtoUserID;
	protected SceneController _sceneController;
	public Constants.Team Team;
	public int ID;
	public int Score;
	public string Nickname;
	

	// ------------ UNITY METHODS ------------

	void Awake()
	{
		ID = _view.ViewID;
		Nickname = _view.Controller.NickName;
		SetActive();
        SetTeam();
	}

	void OnEnable() { GameController.gameActive += OnGameActive; }

	void OnDisable() { GameController.gameActive -= OnGameActive; }

	void Start()
	{
		DontDestroyOnLoad(gameObject);

		gameObject.layer = Constants.LayerPlayer;

		FindObjectOfType<PreGameController>().Register(this);

		FindObjectOfType<HudDebugPanel>().Register(this);

		if (!_view.IsMine)
		{
			Destroy(_camera);
			Destroy(_UI);
			Destroy(_me);
		}

		// Allow master client to move players from one scene to another.
        PhotonNetwork.AutomaticallySyncScene = true;

		// Lock players cursor to center screen.
        Cursor.lockState = CursorLockMode.Locked;
	}


	// ------------ PRIVATE METHODS ------------

	private void OnGameActive(GameController game)
	{
		_sceneController = game;
		_sceneController.Register(this);
		Show();
		Score = 0;
	}

	protected abstract void SetActive();

    protected abstract void SetTeam();

    public abstract void SetSceneController(SceneController sceneController);

	public abstract void IncrementScore();


	// ------------ PUBLIC METHODS ------------

	public Dictionary<int, string> GetViewIDTranslation() {
		_viewIDtoUserID = new Dictionary<int, string>();
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach (var player in players) {
			PhotonView playerView = player.GetComponent<PhotonView>();
			string playerRealtimeID = playerView.Owner.UserId;
			_viewIDtoUserID.Add(playerView.ViewID, playerRealtimeID);
		} _userID = _viewIDtoUserID[ID];
		return _viewIDtoUserID;
	}

	public void Show()
	{
		if (!_view.IsMine)
		{
			gameObject.layer = Constants.LayerPlayer;
			_mesh.SetActive(true);
		}
	}

	public void Hide()
	{
		if (!_view.IsMine)
		{
			gameObject.layer = Constants.LayerOutsideReality;
			_mesh.SetActive(false);
		}
	}

	public Hashtable GetDebugValues()
	{
		Hashtable debugValues = new Hashtable();
		debugValues.Add($"{_view.ViewID} score", Score);
		return debugValues;
	}

	public void SetSpawnpoint(Vector3 spawnpoint)
	{
		_spawnpoint = spawnpoint;
		_playerMovement.MoveToSpawnPoint();

	}

	public Vector3 GetSpawnpoint()
	{
		return _spawnpoint;
	}


	// ------------ RPC METHODS ------------

	[PunRPC]
	public void RPC_incrementScore() { Score++; }
}
