using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemiesList : MonoBehaviour
{
    [SerializeField] GameObject _btnEnemy;
    [SerializeField] Animator _anim;
    public void CallEnemyList()
    {
        _anim.Play("CallEnemyMenu");

        GameObject gameBehaviour = GameObject.FindGameObjectWithTag("GameController");

        foreach (var item in gameBehaviour.GetComponent<BattleBehaviour>().Enemies)
        {
            GameObject temp = Instantiate(_btnEnemy, this.gameObject.transform);
            temp.GetComponentInChildren<TMP_Text>().text = item.name;

            temp.GetComponent<Button>().onClick.AddListener(
                                delegate { gameBehaviour.GetComponent<BattleBehaviour>().Attacker(item); });
            temp.GetComponent<Button>().onClick.AddListener(delegate { ExitEnemyList(); });
        }
    }

    public void ExitEnemyList()
    {
        _anim.Play("ExitMenuEnemyMenu");

        GameObject[] temp = GameObject.FindGameObjectsWithTag("EnemyButtons");

        foreach (var item in temp)
        {
            Destroy(item);
        }
    }
}
