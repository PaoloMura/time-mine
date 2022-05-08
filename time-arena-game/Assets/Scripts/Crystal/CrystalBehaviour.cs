using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using static System.Math;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CrystalBehaviour : MonoBehaviour
{
  // Attributes relating to anim/aesthetics.
  [SerializeField] private Renderer _overlay;
  public float InitialWave = 0;
  public float T = 0;

  // Attributes for crystalmanager access.
  private CrystalManager _crystalManager;
  public int ID;
  protected SceneController _sceneController;
  protected TimeLord _timeLord;

  // Attributes defining crystal state.
  public Vector2 ExistanceRange = new Vector2(-1f, -1f);
  public float ExistanceLength = 60f;
  // If isCollected is true there is no instance of the crystal at any time.
  public bool IsCollected = false;
  private Vector3 _initialPos;


  // ------------ UNITY METHODS ------------

  protected virtual void Start()
  {
    InitialWave = Random.Range(5f, 10f);
    _sceneController = FindObjectOfType<SceneController>();
    _timeLord = _sceneController.GetTimeLord();
    _crystalManager = _sceneController.gameObject.GetComponent<CrystalManager>();
    _initialPos = gameObject.transform.position;

    // On instantiation, add self to crystal list in cm.
    // Give self id according to position in list (syncing doesnt matter, only uniqueness and size).
    ID = _crystalManager.Crystals.Count;
    _crystalManager.Crystals.Add(this);
    UpdateAnim();
  }

  void Update()
  {
    UpdateAnim();
  }


  // ------------ OTHER METHODS ------------

  // Update aesthetics.
  public void UpdateAnim()
  {
    T += Time.deltaTime;
    float offsetY = (float)(0.01 * Sin(T));
    gameObject.transform.Translate(new Vector3(0.0f, offsetY, 0.0f));
    transform.Rotate(0.0f, 30f*Time.deltaTime, 0.0f, Space.Self);
    _overlay.material.SetFloat("Wave_Incr", T);

    // Zoom into and out of existance.
    float percievedTime = (float)(_timeLord.GetYourPerceivedFrame()) / Constants.FrameRate;
    float closestBorderOFExistance = Min(Abs(percievedTime - ExistanceRange[0]), Abs(percievedTime - ExistanceRange[1]));
    float animLength = 2.0f;
    float size = Min(closestBorderOFExistance, T);
    if (size > animLength) setScale(1.0f);
    else setScale(size/animLength);
    gameObject.transform.position = _initialPos;
  }

  private void setScale(float a) { transform.localScale = new Vector3(a, a, a); }

  private void HalveExistanceLength() {
    ExistanceLength /= 2;
    ExistanceLength = Mathf.Max(ExistanceLength, 15f);
  }

  // ------------ RPC FUNCTIONS ------------

  // Called upon player collision.
  // Crystal will be set to inactive in following frame so coroutine outsourced to cm.
  [PunRPC]
  protected virtual void RPC_Collect()
  {
    ExistanceRange = new Vector2(-1f, -1f);
    IsCollected = true;
    if (PhotonNetwork.IsMasterClient)
    {
      _crystalManager.StartCoroutine(_crystalManager.Respawn(ID));
    }
  }

  // Set existance range (period of time when crystal exists in game).
  // i.e. spawn a new crystal (called by coroutine after x seconds).
  [PunRPC]
  void RPC_setExistanceRange(Vector2 newRange)
  {
    ExistanceRange = newRange;
    IsCollected = false;
    HalveExistanceLength();
  }
}
