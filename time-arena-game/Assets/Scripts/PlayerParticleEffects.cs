using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class PlayerParticleEffects : MonoBehaviour
{

    // public ParticleSystem fireCircle;
    // public ParticleSystem splash;

    // public Material material;

    // Color ORANGE = new Color(1.0f, 0.46f, 0.19f, 1.0f);
    // Color BLUE = new Color(0.19f, 0.38f, 1.0f, 1.0f);
    // Color WHITE = new Color(1.0f, 1.0f, 1.0f, 1.0f);

    // bool dissolve;
    // float height;

    // void start()
    // {
    //     dissolve = false;
    //     height = 4.0f;
    //     material.SetFloat("_CutoffHeight", height);
    // }

    // // TODO: this is for testing purposes only - remove later
    // void Update()
    // {
    //     if ( Input.GetKeyDown( KeyCode.J ) )
    //     {
    //         // JumpForward();
    //         startJumping();
    //     }
    //     else if ( Input.GetKeyDown( KeyCode.K ) )
    //     {
    //         JumpBackward();
    //     }

    //     if ( dissolve ) {
    //         height -= 0.01f;
    //         if ( height < -3.0f) {
    //             height = 4.0f;
    //             dissolve = false;
    //             stopJumping();
    //         } 
    //     } else {
    //         height = 4.0f;
    //     }

    //     material.SetFloat("_CutoffHeight", height);
    // }

    // void JumpForward()
    // {
    //     var fcm = fireCircle.main;
    //     fcm.startColor = BLUE;

    //     var fct = fireCircle.trails;
    //     fct.colorOverTrail = BLUE;

    //     var sm = splash.main;
    //     sm.startColor = WHITE;

    //     var st = splash.trails;
    //     st.colorOverTrail = BLUE;

    //     fireCircle.Play();
    //     splash.Play();

    //     dissolve = true;
    // }

    // void JumpBackward()
    // {
    //     var fcm = fireCircle.main;
    //     fcm.startColor = ORANGE;

    //     var fct = fireCircle.trails;
    //     fct.colorOverTrail = ORANGE;

    //     var sm = splash.main;
    //     sm.startColor = WHITE;

    //     var st = splash.trails;
    //     st.colorOverTrail = ORANGE;

    //     fireCircle.Play();
    //     splash.Play();

    //     dissolve = true;
    // }
}
