using UnityEngine;

namespace StateMachine
{
    public enum BattleStates
    {
        BEGINING,
        SET_ATTACK_ORDER,
        START,
        ATTACK,
        NEXT_ATTACKER,
        END_TURN,
        FINISHING_BATTLE,

    }

    public enum GameStates
    {
        GAMEPLAY,
        PAUSE,
        END
    }


    public class StateMachines : MonoBehaviour
    {
        public delegate BattleStates _onNextBattleState(BattleStates gameStates);
        public static _onNextBattleState OnNextBattleState;

        [SerializeField] BattleStates _battlePreviousState;
        [SerializeField] BattleStates _battleState;
        [SerializeField] BattleStates _battleNextState;

        public BattleStates BattlePreviousState
        { get { return _battlePreviousState; } set { _battlePreviousState = value; } }

        public BattleStates CurrentBattleState
        { get { return _battleState; } set { _battleState = value; } }

        public BattleStates BattleNextState
        { get { return _battleNextState; } set { _battleNextState = value; } }

        public BattleStates NextBattleState(BattleStates newState)
        {
            BattlePreviousState = CurrentBattleState;
            return BattleNextState = newState;
        }

        public BattleStates GetCurrentGameState()
        {
            return CurrentBattleState;
        }

        private void OnEnable()
        {
            OnNextBattleState = NextBattleState;
        }

        private void OnDisable()
        {
            OnNextBattleState -= NextBattleState;
        }
    }

    
}
