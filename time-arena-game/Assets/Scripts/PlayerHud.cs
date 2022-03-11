using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour
{
    public GameController game;
	public PhotonView view;
    public Text teamDispl;
    public CanvasGroup debugCanvasGroup;
	public Text debugPanelText;
	public Text masterClientOpts;
	public Slider forwardCooldownSlider;
	public Slider backCooldownSlider;
	public Text timeDispl;
	public Text startTimeDispl;
	public Text winningDispl;
	private float secondsTillGame;
	private bool isCountingTillGameStart;
    public CanvasGroup timelineCanvasGroup;
    public Slider elapsedTimeSlider;
    public Slider playerIcon0;
    public Slider playerIcon1;
    public Slider playerIcon2;
    public Slider playerIcon3;
    public Slider playerIcon4;
	private Slider[] playerIcons;
	public Image forwardJumpIcon;
    public Image backJumpIcon;
    public Sprite redPressedSprite;
    public Sprite greenPressedSprite;
    public Sprite greenUnpressedSprite;

    private Hashtable debugItems;
    private float[] cooldowns;
    private bool debug;
    private bool canJumpForward;
    private bool canJumpBack;


    void Start()
    {
        if (view.IsMine)
        {
            // The first Slider in the array corresponds to this player
            playerIcons = new Slider[]{playerIcon0, playerIcon1, playerIcon2, playerIcon3, playerIcon4};

            // Link SceneChange event to OnSceneChange
            SceneManager.activeSceneChanged += OnSceneChange;
        }

        debugItems = new Hashtable();
        cooldowns = new float[2];
        debug = false;
        debugCanvasGroup.alpha = 0.0f;
        canJumpForward = false;
        canJumpBack = false;
    }

    void OnSceneChange(Scene current, Scene next)
    {
        if (next.name == "GameScene")
        {
            game = FindObjectOfType<GameController>();
        }
    }

    // ------------ LATE UPDATE HELPER FUNCTIONS ------------

    private void LateUpdateMasterClientOptions()
    {
        // If master client, show 'press e to start' text or 'starting in' text
        masterClientOpts.transform.parent.gameObject.SetActive(
            SceneManager.GetActiveScene().name == "PreGameScene" && PhotonNetwork.IsMasterClient
        );

        if (isCountingTillGameStart)
        {
            masterClientOpts.text = "Starting in " + System.Math.Round (secondsTillGame, 0) + "s";
            if (System.Math.Round (secondsTillGame, 0) <= 0.0f)
            {
                masterClientOpts.text = "Loading...";
            }
        }
    }


    private void LateUpdateStartTimeDisplay()
    {
        startTimeDispl.transform.parent.gameObject.SetActive(
            SceneManager.GetActiveScene().name != "PreGameScene"
        );

        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            startTimeDispl.transform.parent.gameObject.SetActive(!game.gameStarted);
            if (!game.gameStarted && !game.gameEnded)
            {
                startTimeDispl.text = "" + (5-(int)(game.timeElapsedInGame+0.9f));
            }
        }
    }


    private void LateUpdateTimeDisplay()
    {
        timeDispl.transform.parent.gameObject.SetActive(
            SceneManager.GetActiveScene().name != "PreGameScene"
        );

        if (SceneManager.GetActiveScene().name == "GameScene" && !game.gameEnded)
        {
            if (game.gameStarted)
            {
                float t = game.gameLength - game.timeElapsedInGame;
                timeDispl.text = (int)(t/60) + ":" + ((int)(t%60)).ToString().PadLeft(2, '0') + ":" 
                + (((int)(((t%60)-(int)(t%60))*100))*60/100).ToString().PadLeft(2, '0');
            } else {
                timeDispl.text = "0:00:00";
            }
        }
    }


    private void LateUpdateTimeline()
    {
        // Set visibility of timeline, player icons and jump cooldowns
        timelineCanvasGroup.alpha = (SceneManager.GetActiveScene().name != "PreGameScene") ? 1.0f: 0.0f;
        elapsedTimeSlider.gameObject.SetActive(SceneManager.GetActiveScene().name != "PreGameScene");
        playerIcons[0].gameObject.SetActive(SceneManager.GetActiveScene().name != "PreGameScene");
        for (int i=1; i < 5; i++)
        {
            playerIcons[i].gameObject.SetActive(
                SceneManager.GetActiveScene().name != "PreGameScene" && 
                game.otherPlayersElapsedTime.Count >= i + 1
            );
        }

        // Set player icon positions
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            if (game.gameStarted && !game.gameEnded)
            {
                elapsedTimeSlider.value = game.timeElapsedInGame / game.gameLength;
                int n = 0;
                List<int> keys = new List<int>(game.otherPlayersElapsedTime.Keys);
                foreach(int key in keys)
                {
                    playerIcons[n].value = game.otherPlayersElapsedTime[key];
                    n++;
                }
            } else if (!game.gameStarted && !game.gameEnded) {
                playerIcons[0].value = 0;
                int n = 0;
                List<int> keys = new List<int>(game.otherPlayersElapsedTime.Keys);
                foreach(int key in keys){
                    playerIcons[n].value = 0;
                    n++;
                }
            }
        }
    }


    private void LateUpdateDebugPanel()
    {
        if (debug)
        {
            System.String debugText = "";
            foreach (DictionaryEntry de in debugItems)
            {
                debugText += $"{de.Key}: {de.Value}\n";
            }
            debugPanelText.text = debugText;
            debugCanvasGroup.alpha = 1.0f;
        }
        else
        {
            debugCanvasGroup.alpha = 0.0f;
        }
    }

    private void LateUpdateCooldowns()
    {
        forwardCooldownSlider.value = cooldowns[0];
        backCooldownSlider.value = cooldowns[1];

        if (canJumpForward) forwardJumpIcon.sprite = greenUnpressedSprite;
        else forwardJumpIcon.sprite = redPressedSprite;
        if (canJumpBack) backJumpIcon.sprite = greenUnpressedSprite;
        else backJumpIcon.sprite = redPressedSprite;
    }

    private void LateUpdateWinningDisplay()
    {
        if (SceneManager.GetActiveScene().name == "GameScene" && game.gameEnded)
        {
            winningDispl.transform.parent.gameObject.SetActive(true);
            winningDispl.text = (game.winningTeam == (int) GameController.Teams.Hider) ? "HIDERS WIN!" : "SEEKERS WIN!";
        }
    }


    // ------------ UPDATE METHODS ------------

    void Update()
    {
        // if counting, reduce timer
        if (PhotonNetwork.IsMasterClient && isCountingTillGameStart && view.IsMine) {
            secondsTillGame -= Time.deltaTime;
            if (secondsTillGame <= 0) {
                PhotonNetwork.LoadLevel("GameScene");
                isCountingTillGameStart = false;
            }
        }
    }


    void LateUpdate()
    {
        if (!view.IsMine) return;

        LateUpdateMasterClientOptions();
        LateUpdateStartTimeDisplay();
        LateUpdateTimeDisplay();
        LateUpdateTimeline();
        LateUpdateDebugPanel();
        LateUpdateCooldowns();
        LateUpdateWinningDisplay();
    }


    // ------------ PUBLIC METHODS ------------


    public void SetTeam(System.String teamName)
    {
        if (view.IsMine) teamDispl.text = teamName;
    }


    public void StartCountingDown()
    {
        if (isCountingTillGameStart) return;

        isCountingTillGameStart = true;
        secondsTillGame = 5.0f;
    }


    public void StopCountingDown()
    {
        isCountingTillGameStart = false;
        secondsTillGame = 0.0f;
    }


    public void SetDebugValues(Hashtable items)
    {
        debugItems = items;
    }

    public void SetCooldownValues(float[] items)
    {
        // each item should be a float between 0.0f (empty) and 1.0f (full)
        cooldowns = items;
    }

    public void ToggleDebug()
    {
        debug = !debug;
    }

    public void PressForwardJumpButton()
    {
        forwardJumpIcon.sprite = greenPressedSprite;
    }

    public void PressBackJumpButton()
    {
        backJumpIcon.sprite = greenPressedSprite;
    }

    public void SetCanJump(bool forward, bool back)
    {
        canJumpForward = forward;
        canJumpBack = back;
    }
}
