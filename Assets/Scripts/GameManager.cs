using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script that automatically creates a static gameobject that can hold references to other scripts.

public class GameManager
{
    private GameObject gameObject;

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new GameManager ();
                instance.gameObject = new GameObject ("Game Manager");
                instance.gameObject.AddComponent<InputHandler_Mattias> ();
            }
            return instance;
        }
    }

    private InputHandler_Mattias inputHandler;
    public InputHandler_Mattias InputHandler
    {
        get
        {
            if(inputHandler == null)
            {
                inputHandler = gameObject.GetComponent<InputHandler_Mattias> ();
            }
            return inputHandler;
        }
    }

    private AimingRay aimingRay;
    public AimingRay AimingRay
    {
        get
        {
            if(aimingRay == null)
            {
                aimingRay = gameObject.GetComponent<AimingRay> ();
            }
            return aimingRay;
        }
    }
}
