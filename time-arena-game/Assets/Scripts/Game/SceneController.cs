using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class SceneController: MonoBehaviour
{
    protected Dictionary<int, PlayerMinerController> _miners;
	protected Dictionary<int, PlayerGuardianController> _guardians;
	protected TimeLord _timeLord;
  	protected int _minerScore;
	protected Dictionary<int, string> _iconAssignments;
	protected Dictionary<int, string> _viewIDTranslations;
    public static event Action<int> scoreChange;

	protected void CreateTimeLord(int sceneLength, bool logging=false, bool diagnostics=false)
    {
        if (logging && PhotonNetwork.CurrentRoom.PlayerCount != 1)
        {
            throw new InvalidOperationException("Logging is only allowed in single player tests.");
        }
        else if (diagnostics && PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            throw new InvalidOperationException("Diagnostics is only allowed in multiplayer tests.");
        }
        else
        {
			// Beware that if running diagnostics, non-master clients must be run in the Unity Editor.
            int totalFrames = sceneLength * Constants.FrameRate;
            _timeLord = new ProxyTimeLord(totalFrames, logging, diagnostics && !(PhotonNetwork.IsMasterClient));
        }
    }

	public void Register(PlayerController pc)
	{
		pc.SetSceneController(this);
		_viewIDTranslations = pc.GetViewIDTranslation();
		_iconAssignments = GetIconAssignments();
	}

	public void Register(PlayerMinerController pmc)
	{
		_miners.Add(pmc.ID, pmc);
	}

	public void Register(PlayerGuardianController pgc)
	{
		_guardians.Add(pgc.ID, pgc);
	}

    public Constants.Team GetTeam(int playerID)
	{
		if (_miners.ContainsKey(playerID)) return Constants.Team.Miner;
		else if (_guardians.ContainsKey(playerID)) return Constants.Team.Guardian;
		else throw new KeyNotFoundException("No team associated with the given playerID.");
	}

	public string GetIconString(int playerID) {
		return _iconAssignments[playerID];
	}

    public void HideAllPlayers()
	{
		foreach (var guardian in _guardians)
		{
			guardian.Value.Hide();
		}
		foreach (var miner in _miners)
		{
			miner.Value.Hide();
		}
	}

    public void ShowPlayersInReality()
	{
		HashSet<int> playerIDs = _timeLord.GetPlayersInReality();
		foreach (var id in playerIDs)
		{
			if (_guardians.ContainsKey(id)) _guardians[id].Show();
			else if (_miners.ContainsKey(id)) _miners[id].Show();
		}
	}

	public void OffsetScore(int offset)
	{
		_minerScore += offset;
		scoreChange?.Invoke(_minerScore);
	}

	public TimeLord GetTimeLord() { return _timeLord;}

	public int GetMinerScore() { return _minerScore; }

	protected Dictionary<int, string> GetIconAssignments() {
		Dictionary<int, string> icons = new Dictionary<int, string>();
		foreach (var pair in _viewIDTranslations) {
			icons.Add(pair.Key, PlayerPrefs.GetString(pair.Value));
		} return icons;
	}

}
