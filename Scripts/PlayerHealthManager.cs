using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour, IDamagable
{
    private HitMechanics HitMechanics => GameManager.Instance.Player.PlayerHitMechanics;

    public int Health
    {
        get { return _health; }
        set
        {
            _health = value;
            if(_health <= 0)
            {
                Die();
            }
        }
    }
    private int _health;
    void Start()
    {
        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            GetDamaged(transform.position, 0);
    }
    private void Die()
    {
        Destroy(gameObject, 0.1f);
    }
    public void GetDamaged(Vector2 hitPosition, int damage)
    {
        HitMechanics.FlashMask();
    }
}
