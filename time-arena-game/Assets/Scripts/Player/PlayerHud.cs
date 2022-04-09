using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class PlayerHud : MonoBehaviour
{
    private GameController _game;
    [SerializeField] HudStartTimer _startTimer;
    [SerializeField] HudTimeDisplay _timeDisplay;
    [SerializeField] HudMasterClientOptions _masterClientOptions;
    [SerializeField] HudTimeline _timeline;
	public PhotonView View;
    public Text TeamDispl;
    public CanvasGroup DebugCanvasGroup;
	public Text DebugPanelText;
	public Slider ForwardCooldownSlider;
	public Slider BackCooldownSlider;
    public Text WinningDispl;
    public Text ScoreDispl;
	public Image ForwardJumpIcon;
    public Image BackJumpIcon;
    public Sprite RedPressedSprite;
    public Sprite GreenPressedSprite;
    public Sprite GreenUnpressedSprite;
    public GameObject ArrowImage;
    public GameObject Tutorial;
    public GameObject PopUpText;
    public GameObject OptionsPopUpText;
    
    private float _secondsTillGame;
	private bool _isCountingTillGameStart;
    private Hashtable _debugItems;
    private float[] _cooldowns;
    private bool _debug;
    private bool _canJumpForward;
    private bool _canJumpBack;
    private string _message;
    private int score;
    private string _optionsMessage;


    private Dictionary<string, Vector2> _uiPositions;
    private Dictionary<string, Vector3> _uiRotations;
    


    void Start()
    {


        _uiPositions = new Dictionary<string, Vector2>();
        _uiPositions.Add("forwardJump",new Vector2(381f,-76f));
        _uiPositions.Add("backJump",new Vector2(-397f,-82f));
        _uiPositions.Add("timebar",new Vector2(-342f,-122f));
        _uiPositions.Add("timer",new Vector2(-290f,105f));
        _uiPositions.Add("team",new Vector2(284f,104f));


        _uiRotations = new Dictionary<string, Vector3>();
        _uiRotations.Add("forwardJump",new Vector3(-10.1f,-720.2f,18.5f));
        _uiRotations.Add("backJump",new Vector3(-10.1f,-535.5f,4.7f));
        _uiRotations.Add("timebar",new Vector3(-10.1f,-535.5f,4.7f));
        _uiRotations.Add("timer",new Vector3(-10.1f,-535.5f,-150f));
        _uiRotations.Add("team",new Vector3(-10.1f,-355.5f,-150f));


        if (View.IsMine)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                _masterClientOptions.SetActive(false);
            }
        }
        else
        {
            // TODO: After refactoring, remove this with a single command that sets the UI parent object inactive.
            // TODO: Actually, thinking about it, I'm pretty sure we already take care of this in PlayerController...
            // TODO: So the following lines are already redundant...?
            _startTimer.SetActive(false);
            _timeDisplay.SetActive(false);
            _masterClientOptions.SetActive(false);
            _timeline.SetActive(false);
        }

        _debugItems = new Hashtable();
        _cooldowns = new float[2];
        _debug = false;
        DebugCanvasGroup.alpha = 0.0f;
        _canJumpForward = false;
        _canJumpBack = false;
    }


    // ------------ LATE UPDATE HELPER FUNCTIONS ------------

    private void LateUpdateDebugPanel()
    {
        if (_debug)
        {
            System.String debugText = "";
            foreach (DictionaryEntry de in _debugItems)
            {
                debugText += $"{de.Key}: {de.Value}\n";
            }
            DebugPanelText.text = debugText;
            DebugCanvasGroup.alpha = 1.0f;
        }
        else
        {
            DebugCanvasGroup.alpha = 0.0f;
        }
    }

    private void LateUpdateCooldowns()
    {
        ForwardCooldownSlider.value = _cooldowns[0];
        BackCooldownSlider.value = _cooldowns[1];

        if (_canJumpForward) ForwardJumpIcon.sprite = GreenUnpressedSprite;
        else ForwardJumpIcon.sprite = RedPressedSprite;
        if (_canJumpBack) BackJumpIcon.sprite = GreenUnpressedSprite;
        else BackJumpIcon.sprite = RedPressedSprite;
    }

    private void LateUpdateWinningDisplay()
    {
        if (SceneManager.GetActiveScene().name == "GameScene" && _game.GameEnded)
        {
            WinningDispl.transform.parent.gameObject.SetActive(true);
            WinningDispl.text = (_game.WinningTeam == Constants.Team.Miner) ? "HIDERS WIN!" : "SEEKERS WIN!";
        }
    }

    private void LateUpdateTutorial()
    {
        if (SceneManager.GetActiveScene().name != "PreGameScene")
        {

            Tutorial.gameObject.SetActive(false);
            

        }
    }

    private void LateUpdateScore(){
      if (SceneManager.GetActiveScene().name == "GameScene")
      {
        ScoreDispl.transform.parent.gameObject.SetActive(true);

          ScoreDispl.text = score + "";
      } else if(SceneManager.GetActiveScene().name == "PreGameScene") {
        ScoreDispl.transform.parent.gameObject.SetActive(false);
      }
    }


    // ------------ UPDATE METHODS ------------

    void Update()
    {
        // TODO: Get this out of here!!! This kind of power belongs in GameController or PlayerController at the very least!
        // If counting, reduce timer.
        if (PhotonNetwork.IsMasterClient && _isCountingTillGameStart && View.IsMine) {
            _secondsTillGame -= Time.deltaTime;
            _masterClientOptions.SetSecondsTillGame(_secondsTillGame);
            if (_secondsTillGame <= 0) {
                PhotonNetwork.LoadLevel("GameScene");
                _isCountingTillGameStart = false;
                _masterClientOptions.SetIsCountingTillStart(_isCountingTillGameStart);
            }
        }
    }


    void LateUpdate()
    {
        if (!View.IsMine) return;

        LateUpdateDebugPanel();
        LateUpdateCooldowns();
        LateUpdateWinningDisplay();
        LateUpdateTutorial();
        LateUpdateScore();

    }


    // ------------ PUBLIC METHODS ------------

    public void setScore(int a){
      score = a;
    }
    public int getScore(){
      return score;
    }

    public void SetTeam(System.String teamName)
    {
        if (View.IsMine) TeamDispl.text = teamName;
    }


    public void StartCountingDown()
    {
        if (_isCountingTillGameStart) return;

        _isCountingTillGameStart = true;
        _masterClientOptions.SetIsCountingTillStart(_isCountingTillGameStart);
        _secondsTillGame = 5.0f;
        _masterClientOptions.SetSecondsTillGame(_secondsTillGame);
    }


    public void StopCountingDown()
    {
        _isCountingTillGameStart = false;
        _masterClientOptions.SetIsCountingTillStart(_isCountingTillGameStart);
        _secondsTillGame = 0.0f;
        _masterClientOptions.SetSecondsTillGame(_secondsTillGame);
    }


    public void SetDebugValues(Hashtable items)
    {
        _debugItems = items;
    }

    public void SetPlayerPositions(float clientPosition, List<float> playerPositions)
    {
        _timeline.SetPlayerPositions(clientPosition, playerPositions);
    }

    public void SetTimeBarPosition(float position)
    {
        _timeline.SetTimeBarPosition(position);
    }

    public void SetCooldownValues(float[] items)
    {
        // Each item should be a float between 0.0f (empty) and 1.0f (full).
        _cooldowns = items;
    }

    public void SetTime(int second) { _timeDisplay.SetTime(second); }

    public void ToggleDebug()
    {
        _debug = !_debug;
    }

    public void PressForwardJumpButton()
    {
        if (!View.IsMine) return;

        ForwardJumpIcon.sprite = GreenPressedSprite;
    }

    public void PressBackJumpButton()
    {
        if (!View.IsMine) return;
        BackJumpIcon.sprite = GreenPressedSprite;
    }

    public void SetCanJump(bool forward, bool back)
    {
        _canJumpForward = forward;
        _canJumpBack = back;
    }

    public void SetArrowPosition(string uiElement){

        if (!View.IsMine) return;

        ArrowImage.GetComponent<RectTransform>().anchoredPosition = _uiPositions[uiElement];
        ArrowImage.GetComponent<RectTransform>().eulerAngles = _uiRotations[uiElement];
    }

    public void SetMessage(string tutorialMessage){

        if (!View.IsMine) return;

        _message = tutorialMessage;
        PopUpText.GetComponent<TextMeshProUGUI>().text = _message;

    }

    public void SetArrowVisibility(bool arrowVisibility){

        if (!View.IsMine) return;

        ArrowImage.gameObject.SetActive(arrowVisibility);
    }
    public void SetTutorialVisibility(bool tutorialVisibility){

        if (!View.IsMine) return;
        
        Tutorial.gameObject.SetActive(tutorialVisibility);

    }
    public void SetOptionsText(string optionsMessage){

        if (!View.IsMine) return;

        _optionsMessage = optionsMessage;
        OptionsPopUpText.GetComponent<TextMeshProUGUI>().text = _optionsMessage;

    }
    

    public void SetGame(GameController game)
    {
        _game = game;
        if (_startTimer != null) _startTimer.SetGame(game);
        _timeDisplay.SetGame(game);
    }
}
