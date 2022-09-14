using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Class Name", menuName = "Add New Class", order = 1)]
public class ClassAtributes : ScriptableObject
{
    [SerializeField] string _charName;
    public string CharName { get { return _charName; } }

    [SerializeField] Sprite _charPortrait;
    public Sprite CharPortrait { get { return _charPortrait; } }

    [SerializeField] float _life;
    public float Life { get { return _life; } }

    [SerializeField] float _energy;
    public float Energy { get { return _energy; } }

    [SerializeField] int _attack;
    public int Attack { get { return _attack; } }

    [SerializeField] int _defense;
    public int Defense { get { return _defense; } }
    [SerializeField] float _criticalAttack;
    public float CriticalAttack { get { return _criticalAttack; } }

    [SerializeField] float _speed;
    public float Speed { get { return _speed; } }

    [SerializeField] float _jumpSpeed;
    public float JumpSpeed { get { return _jumpSpeed; } }

}
