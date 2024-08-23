using TMPro;
using UnityEngine;

public class DamageOutput : MonoBehaviour
{
    [SerializeField] TMP_Text _damageText;

    public void SetDamage(int damage)
    {
        _damageText.text = damage.ToString();
    }

    public void DestroyOnEvent()
    {
        Destroy(this.gameObject);
    }
}
