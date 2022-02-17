using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticleEffects : MonoBehaviour
{

    public ParticleSystem fireCircle;
    public ParticleSystem splash;

    Color ORANGE = new Color(1.0f, 0.46f, 0.19f, 1.0f);
    Color BLUE = new Color(0.19f, 0.38f, 1.0f, 1.0f);
    Color WHITE = new Color(1.0f, 1.0f, 1.0f, 1.0f);

    // TODO: this is for testing purposes only - remove later
    void Update()
    {
        if ( Input.GetKeyDown( KeyCode.J ) )
        {
            JumpForward();
        }
        else if ( Input.GetKeyDown( KeyCode.K ) )
        {
            JumpBackward();
        }
    }

    void JumpForward()
    {
        var fcm = fireCircle.main;
        fcm.startColor = BLUE;

        var fct = fireCircle.trails;
        fct.colorOverTrail = BLUE;

        var sm = splash.main;
        sm.startColor = WHITE;

        var st = splash.trails;
        st.colorOverTrail = BLUE;

        fireCircle.Play();
        splash.Play();
    }

    void JumpBackward()
    {
        var fcm = fireCircle.main;
        fcm.startColor = ORANGE;

        var fct = fireCircle.trails;
        fct.colorOverTrail = ORANGE;

        var sm = splash.main;
        sm.startColor = WHITE;

        var st = splash.trails;
        st.colorOverTrail = ORANGE;

        fireCircle.Play();
        splash.Play();
    }
}
