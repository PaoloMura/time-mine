using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    [SerializeField] private Animator PlayerAnim;
    [SerializeField] private PlayerGrab _grab;
    [SerializeField] private  PhotonView _view;
    
    void Awake() {

        if(!_view.IsMine){
            Destroy(this);
            return;
        }
        
    }
    
    // Update is called once per frame
    void Update()
    {
        AnimationKeyControl();
 
    }
    public void AnimationKeyControl(){
        
        if(Input.GetKeyDown(KeyCode.W)){
            StartRunningForwards();
            Debug.Log(PlayerAnim);
        } 
        if(Input.GetKeyDown(KeyCode.S)) StartRunningBackwards();
        if(Input.GetKeyUp(KeyCode.W)) StopRunningForwards();
        if(Input.GetKeyDown(KeyCode.A)) StartRunningForwards();
        if(Input.GetKeyUp(KeyCode.A)) StopRunningForwards();
        if(Input.GetKeyDown(KeyCode.D)) StartRunningForwards();
        if(Input.GetKeyUp(KeyCode.D)) StopRunningForwards();
        if(Input.GetKeyUp(KeyCode.S)) StopRunningBackwards();
        if(Input.GetKeyDown(KeyCode.Space))StartJumping();
        if(Input.GetKeyUp(KeyCode.Space))StopJumping();
        if(Input.GetMouseButtonDown(0)) _grab.Grab();
    }
    public void StartRunningForwards(){
        PlayerAnim.SetBool("isRunningForwards",true);
    }
    public void StartRunningBackwards(){
        PlayerAnim.SetBool("isRunningBackwards",true);
    }
    public void StopRunningForwards(){
        PlayerAnim.SetBool("isRunningForwards",false);
    }
    public void StopRunningBackwards(){
        PlayerAnim.SetBool("isRunningBackwards",false);
    }
    public void StartJumping(){
        PlayerAnim.SetBool("isJumping",true);
    }
    public void StopJumping(){
        PlayerAnim.SetBool("isJumping",false);
    }
}
