using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
	[SerializeField] private GameObject _pauseMenuUI;
	[SerializeField] private PhotonView _view;
	private bool _paused;
	public static event Action<bool> paused;

	void Awake()
	{
		if (!_view.IsMine) Destroy(this);
	}

	void OnEnable()
	{
		GameController.gameEnded += OnGameEnded;
	}

	void OnDisable()
	{
		GameController.gameEnded -= OnGameEnded;
	}

	void Start()
	{
		_paused = false;
	}

	void Update()
	{	
		if (Input.GetKeyDown(KeyCode.Escape)) SetPause(!_paused);
	}

	private void OnGameEnded(Constants.Team team) { SetPause(true); }

	// This gets called on the Resume button press.
	public void OnResume() { SetPause(false); }

	// This gets called on the Leave button press.
	public void OnLeave()
	{
		PhotonNetwork.LeaveRoom();
		SceneManager.LoadScene("MenuScene");
		Destroy(gameObject);
	}

	private void SetPause(bool pause)
	{
		_paused = pause;
		_pauseMenuUI.SetActive(_paused);
		Cursor.lockState = _paused ? CursorLockMode.None : CursorLockMode.Locked;
		paused?.Invoke(_paused);
	}
}
