using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("体力")]
    public int hp;
    public GameObject deathEffectPrefab;

    public void OnDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Instantiate(deathEffectPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
//Random.Range(1, 3);
