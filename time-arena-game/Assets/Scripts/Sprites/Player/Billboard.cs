using Photon.Pun;
using UnityEngine;


// This script is used to control the rotation of an object so that it always faces the main camera
public class Billboard : MonoBehaviour
{
    private Transform _mainCameraTransform;

    void OnEnable()
    {
        PlayerController.newPlayer += OnNewPlayer;
    }

    void OnDisable()
    {
        PlayerController.newPlayer -= OnNewPlayer;
    }

    void Start()
    {
        _mainCameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (transform != null)
        {
            if (_mainCameraTransform == null) Debug.LogError("mainCameraTransform is null");
            else transform.LookAt(transform.position + _mainCameraTransform.rotation * Vector3.forward, _mainCameraTransform.rotation * Vector3.up);
        }
    }

    private void OnNewPlayer(PlayerController pc)
    {
        _mainCameraTransform = Camera.main.transform;
    }
}
