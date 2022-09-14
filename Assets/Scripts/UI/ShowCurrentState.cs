using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using StateMachine;

public class ShowCurrentState : MonoBehaviour
{
    TMP_Text txt;

    private void Awake()
    {
        txt = this.GetComponent<TMP_Text>();
    }

    void Start()
    {
        BattleBehaviour.OnNextBattleState += ShowText;    
    }

    BattleStates ShowText(BattleStates state)
    {
        txt.text = "Game State = " + state;

        return state;
    }
}
