using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

public class SetTurnTypeFromPlayerPref : MonoBehaviour
{
    public SnapTurnProvider snapTurn;
    public ContinuousTurnProvider continuousTurn;

    // Start is called before the first frame update
    void Start()
    {
        ApplyPlayerPref();
    }

    public void ApplyPlayerPref()
    {
        if(PlayerPrefs.HasKey("turn"))
        {
            int value = PlayerPrefs.GetInt("turn");
            if (value == 0)
            {
                snapTurn.enabled = true;
                continuousTurn.enabled = false;
            }
            else if (value == 1)
            {
                snapTurn.enabled = false;
                continuousTurn.enabled = true;
            }
        }
    }
}
