using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    class State
    {
        public string Message;
        public string ElementToPointTo;
        public KeyCode InputTrigger;
        public bool VisibilityOfArrow;
        public bool NeedKey;
        //public bool CrystalVisibility;

        public State(string message, string elementToPointTo, KeyCode inputTrigger ,bool visibilityOfArrow,bool needKeyPress/*,bool visibilityOfCrystal*/)
        {
            Message = message;
            ElementToPointTo = elementToPointTo;
            InputTrigger = inputTrigger;
            VisibilityOfArrow = visibilityOfArrow;
            NeedKey = needKeyPress;
            //CrystalVisibility = visibilityOfCrystal;
        }
    }
    
    [SerializeField] private PlayerController _player;
    [SerializeField] private HudTutorial _tutorialHud;
    [SerializeField] private PhotonView _view;
    [SerializeField] private GameObject _masterClientOptions;

    private bool _hasMovedOn = true;
    private List<State> _guardianStates;
    private List<State> _minerStates;
    private List<State> _states;
    private int _currentState;
  

    // ------------ UNITY FUNCTIONS ------------

    void Awake()
    {
        _masterClientOptions.SetActive(false);
        if (!_view.IsMine) Destroy(this);
        CreateStatesGuardian();
        CreateStatesMiner();
    }

    void OnEnable() { GameController.gameActive += OnGameActive; }

    void OnDisable() { GameController.gameActive -= OnGameActive; }

    void Start()
    {
        if (_player.Team == Constants.Team.Guardian) _states = _guardianStates;
        else _states = _minerStates;
        StartTutorial();
    }

    void Update()
    {
        if (_currentState == _states.Count - 1 && Input.GetKeyDown(KeyCode.Alpha1)) MoveToState(0);
        if (_currentState < _states.Count - 1 && Input.GetKeyDown(KeyCode.Alpha2)) MoveToState(_states.Count - 1);
        if (_currentState < _states.Count)
        {
            if (_states[_currentState].NeedKey)
            {
                if (Input.GetKeyDown(_states[_currentState].InputTrigger)) MoveToState(_currentState + 1);
            }
            else if (_hasMovedOn)
            {
                StartCoroutine(DelayPopup());
                _hasMovedOn = false;     
            }
        }
    }


    // ------------ ON EVENT METHODS ------------

    private void OnGameActive(GameController game) { Destroy(this); }


    // ------------ PRIVATE METHODS ------------

    private void CreateStatesGuardian()
    {
        _guardianStates = new List<State>();
        _guardianStates.Add(new State("Welcome to tutorial Guardian!\n\nPlease use <sprite=9> keys to move around.","backJump", KeyCode.S,false,true,false,false));
        _guardianStates.Add(new State("Welcome to tutorial Guardian!\n\nPlease use <sprite=26> keys to move around.","backJump", KeyCode.S,false,false,false,false));
        _guardianStates.Add(new State("Press <sprite=12> + <sprite=2> to sprint. ","backJump", KeyCode.W,false,true,false,false));
        _guardianStates.Add(new State("Press <sprite=29> + <sprite=21> to sprint. ","backJump", KeyCode.W,false,false,false,false));
        _guardianStates.Add(new State("Use <sprite=15> to jump.","backJump", KeyCode.Space,false,true,false,false));
        _guardianStates.Add(new State("Use <sprite=31> to jump.","backJump", KeyCode.Space,false,false,false,false));
        _guardianStates.Add(new State("Click left <sprite=13> to grab miners and drop their crystals!.","backJump", KeyCode.Mouse0,false,true,false,false));
        _guardianStates.Add(new State("Click left <sprite=23> to grab miners and drop their crystals!.","backJump", KeyCode.Space,false,false,false,false));
        _guardianStates.Add(new State("Now,let's have a look at game features!!","backJump", KeyCode.A,false,false,false,false));
        _guardianStates.Add(new State("This is the timer which shows the game time.\nYou have 5 minutes!!","timer", KeyCode.Return,true,false,false,false));
        _guardianStates.Add(new State("This shows the team you are in!!","team", KeyCode.Return,true,false,false,false));
        _guardianStates.Add(new State("This is the timebar which helps you to see where you are at in time.","timebar", KeyCode.Return,true,false,false,false));
        _guardianStates.Add(new State("IT'S TIME TO TIME TRAVEL!!","timebar", KeyCode.Return,false,false,false,false));
        _guardianStates.Add(new State("This icon shows the ability of time travelling backwards.\nOnce it starts spinning you can travel back in time.", "backJump",KeyCode.Return,true,false,false,false));
        _guardianStates.Add(new State("Now, you are ready to go to the past!!\n\nPlease hold <sprite=7>.", "backJump",KeyCode.Q,true,true,false,false));
        _guardianStates.Add(new State("Now, you are ready to go to the past!!\n\nPlease hold <sprite=24>.", "backJump",KeyCode.Q,true,false,false,false));
        _guardianStates.Add(new State("Well done! You just traveled back in time!", "timebar",KeyCode.Return,false,false,false,false));
        _guardianStates.Add(new State("Let's travel forwards now!!\nThis icon shows the ability of time travelling forwards.\nOnce it starts spinning you can travel forward in time.", "forwardJump",KeyCode.Return,true,false,false,false));
        _guardianStates.Add(new State("Please hold <sprite=14> to travel forwards!", "forwardJump",KeyCode.E,true,true,false,false));
        _guardianStates.Add(new State("Please hold <sprite=30> to travel forwards!", "forwardJump",KeyCode.E,true,false,false,false));
        _guardianStates.Add(new State("Awesome!!It's the end of the tutorial.You are ready to play!!", "forwardJump",KeyCode.E,false,true,false,false));
    }

    private void CreateStatesMiner()
    {
        _minerStates = new List<State>();
        _minerStates.Add(new State("Welcome to tutorial Miner!\n\nPlease use <sprite=9> keys to move around.","backJump", KeyCode.S,false,true,false,false));
        _minerStates.Add(new State("Welcome to tutorial Miner!\n\nPlease use <sprite=26> keys to move around.","backJump", KeyCode.S,false,false,false,false));
        _minerStates.Add(new State("Press <sprite=12> + <sprite=2> to sprint. ","backJump", KeyCode.W,false,true,false,false));
        _minerStates.Add(new State("Press <sprite=29> + <sprite=21> to sprint. ","backJump", KeyCode.W,false,false,false,false));
        _minerStates.Add(new State("Use <sprite=15> to jump.","backJump", KeyCode.Space,false,true,false,false));
        _minerStates.Add(new State("Use <sprite=31> to jump.","backJump", KeyCode.Space,false,false,false,false));
        _minerStates.Add(new State("Now,let's have a look at game features!!","backJump", KeyCode.A,false,false,false,false));
        _minerStates.Add(new State("Time crystals only appear in certain times!\n Run through the crystal to collect it!","backJump", KeyCode.W,false,true,true,false));
        _minerStates.Add(new State("Time crystals only appear in certain times!\n Run through the crystal to collect it!","backJump", KeyCode.W,false,false,true,false));
        _minerStates.Add(new State("REMINDER: You can check your tracker device to find the nearest crystal.","backJump", KeyCode.A,false,false,false,true));
        _minerStates.Add(new State("Don't get caught to guardians! Otherwise you will drop your crystals.","backJump", KeyCode.A,false,false,false,true));
        _minerStates.Add(new State("This is the timer which shows the game time.\nYou have 5 minutes!!","timer", KeyCode.Return,true,false,false,true));
        _minerStates.Add(new State("This shows the team you are in!!","team", KeyCode.Return,true,false,false,true));
        _minerStates.Add(new State("This is the timebar which helps you to see where you are at in time.","timebar", KeyCode.Return,true,false,false,true));
        _minerStates.Add(new State("IT'S TIME TO TIME TRAVEL!!","timebar", KeyCode.Return,false,false,false,true));
        _minerStates.Add(new State("This icon shows the ability of time travelling backwards.\nOnce it starts spinning you can travel back in time.", "backJump",KeyCode.Return,true,false,false,true));
        _minerStates.Add(new State("Now, you are ready to go to the past!!\n\nPlease hold <sprite=7>.", "backJump",KeyCode.Q,true,true,false,true));
        _minerStates.Add(new State("Now, you are ready to go to the past!!\n\nPlease hold <sprite=24>.", "backJump",KeyCode.Q,true,false,false,true));
        _minerStates.Add(new State("Well done! You just traveled back in time!", "timebar",KeyCode.Return,false,false,false,true));
        _minerStates.Add(new State("Let's travel forwards now!!\nThis icon shows the ability of time travelling forwards.\nOnce it starts spinning you can travel forward in time.", "forwardJump",KeyCode.Return,true,false,false,true));
        _minerStates.Add(new State("Please hold <sprite=14> to travel forwards!", "forwardJump",KeyCode.E,true,true,false,true));
        _minerStates.Add(new State("Please hold <sprite=30> to travel forwards!", "forwardJump",KeyCode.E,true,false,false,true));
        _minerStates.Add(new State("Awesome!!It's the end of the tutorial.You are ready to play!!", "forwardJump",KeyCode.E,false,true,false,true));
 
    }

    private void StartTutorial()
    {
        _currentState = 0;
        _tutorialHud.SetMessage(_states[_currentState].Message);
        //_tutorialHud.SetCrystalVisibility(_states[_currentState].CrystalVisibility);
        _tutorialHud.SetArrowVisibility(
            _states[_currentState].ElementToPointTo,
            _states[_currentState].VisibilityOfArrow
        );
    }

    private void MoveToState(int state)
    {
        if (state >= _states.Count) return;
        
        // Deactivate old arrow.
        _tutorialHud.SetArrowVisibility(_states[_currentState].ElementToPointTo, false);

        // Set the new state.
        _currentState = state;
        _tutorialHud.SetMessage(_states[_currentState].Message);
        //_tutorialHud.SetCrystalVisibility(_states[_currentState].CrystalVisibility);
        
        // Activate new arrow (if there is one for the current state).
        _tutorialHud.SetArrowVisibility(
            _states[_currentState].ElementToPointTo, 
            _states[_currentState].VisibilityOfArrow
        );
        
        // Set the options text.
        if (state == _states.Count - 1){
            _tutorialHud.SetOptionsText("Go back to tutorial <sprite=1>");
            if (PhotonNetwork.IsMasterClient) _masterClientOptions.SetActive(true);
        } 
        else _tutorialHud.SetOptionsText("Skip tutorial <sprite=3>");

    }

    IEnumerator DelayPopup() {
        yield return new WaitForSeconds(4);
        MoveToState(_currentState + 1);
        _hasMovedOn = true;
    }
 }

