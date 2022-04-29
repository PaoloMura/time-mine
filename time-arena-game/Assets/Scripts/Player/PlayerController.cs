using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Realtime;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private GameObject _camera;
	[SerializeField] private GameObject _UI;
	[SerializeField] private PhotonView _view;
	private string _userID;
	private Dictionary<int, string> _viewIDtoUserID;
	private Dictionary<int, string> _iconAssignments;

	public Constants.Team Team;
	public int ID;


	// ------------ UNITY METHODS ------------

	void Awake()
	{
		_viewIDtoUserID = new Dictionary<int, string>();
		_iconAssignments = new Dictionary<int, string>();
		ID = _view.ViewID;
		
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach (var player in players) {
			string playerRealtimeID = player.GetComponent<PhotonView>().Owner.UserId;
			PhotonView playerView = player.GetComponent<PhotonView>();
			_viewIDtoUserID.Add(playerView.ViewID, playerRealtimeID);
		} _userID = _viewIDtoUserID[ID];

		foreach (KeyValuePair<int, string> pair in _viewIDtoUserID) {
			Debug.Log($"Added {pair.Key}: {PlayerPrefs.GetString(pair.Value)}");
			_iconAssignments.Add(pair.Key, PlayerPrefs.GetString(pair.Value));
		}

		SetTeam(GetIconName());

		SceneController sceneController = FindObjectOfType<PreGameController>();
		if (sceneController == null) Debug.LogError("PreGameController not found");
		else sceneController.Register(this);
	}

	void OnEnable() { GameController.gameActive += OnGameActive; }

	void OnDisable() { GameController.gameActive -= OnGameActive; }

	void Start()
	{	
		DontDestroyOnLoad(gameObject);

		gameObject.layer = Constants.LayerPlayer;
		
		if (_view.IsMine) gameObject.tag = "Client";
		else
		{
			Destroy(_camera);
			Destroy(_UI);
		}

		// Allow master client to move players from one scene to another.
        PhotonNetwork.AutomaticallySyncScene = true;

		// Lock players cursor to center screen.
        Cursor.lockState = CursorLockMode.Locked;
	}


	// ------------ PRIVATE METHODS ------------

	private void OnGameActive(GameController game)
	{
		game.Register(this);
		gameObject.layer = Constants.LayerPlayer;
	}

	private void SetTeam(string iconName) {
		string teamName = iconName.Split('_')[0]; 
		if (teamName == "miner") Team = Constants.Team.Miner;
		else if (teamName == "guardian") Team = Constants.Team.Guardian;
	}

	private string GetIconName() {
		return _iconAssignments[ID];
	}

	// ------------ PUBLIC METHODS ------------

	public void Show() { gameObject.layer = Constants.LayerPlayer; }

	public void Hide() { gameObject.layer = Constants.LayerOutsideReality; }

	public Dictionary<int, string> GetIconAssignments() { return _iconAssignments; }

	// ------------ RPC ------------

	// This RPC call is only called on the Master Client, so it needs to send out RPCs to everyone
	// but themselves.
	[PunRPC] void RPC_getIcons() {
		foreach (Player player in PhotonNetwork.PlayerList) {
			if (player.UserId != _userID) {
				Debug.Log($"Sending _iconAssignment to {player.NickName}");
				_view.RPC("RPC_sendIcons", player, _iconAssignments);
			}
		}
		// _view.RPC("RPC_sendIcons", RpcTarget.All, _iconAssignments);
	}

	[PunRPC] void RPC_sendIcons(Dictionary<string, string> iconAssignments) {
		/*_iconAssignments = iconAssignments;
		string userID = _viewIDtoUserID[ID];
		string iconAssignment = _iconAssignments[userID];
		foreach (var icon in _iconAssignments) {
			Debug.Log($"{icon.Key} {icon.Value}");
		}

		// e.g. "miner_icon_red" returns "miner"
		string teamName = iconAssignment.Split('_')[0]; 
		if (teamName == "miner") Team = Constants.Team.Miner;
		else if (teamName == "guardian") Team = Constants.Team.Guardian;
		*/
	}
}
