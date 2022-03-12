using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurret : MonoBehaviour
{
    [Header("Particles")]
    [SerializeField] GameObject shootParticles;
    private PlayerScriptsManager PlayerSM => GetComponent<PlayerScriptsManager>();
    private Transform ShootPos => transform.GetChild(0).GetChild(3);
    private Transform AimPos => transform.GetChild(0).GetChild(1);
    private Transform GunSight => transform.GetChild(0).GetChild(2);
    [SerializeField] private GameObject bulletPrefab;

    public delegate void DShoot();
    public event DShoot ShootEvent;

    private float AttackSpeed
    {
        get { return PlayerSM.GetStat(EStatName.attackSpeed); }
    }
    private float _attackSpeedTimer = 0;
    void Update()
    {
        if (PlayerSM.DisablePlayer)
            return;

        CheckForInput(); // shoot is inside!
        GunSightMovement();
    }
    private void  GunSightMovement()
    {
        GunSight.position = Vector2.MoveTowards(GunSight.position, GetDestinationPos(), PlayerSM.GetStat(EStatName.rotateSpeed) * 2 * Time.deltaTime);
        GunSight.rotation = Quaternion.Euler(0, 0, 0);
    }
    private void CheckForInput()
    {
        _attackSpeedTimer += Time.deltaTime;
        if (_attackSpeedTimer > AttackSpeed)
            _attackSpeedTimer = AttackSpeed;

        Vector2 input = new Vector2(Input.GetMouseButton(0) ? 1 : 0, Input.GetMouseButton(1) ? 1 : 0);
        if (input == Vector2.zero || _attackSpeedTimer < AttackSpeed)
            return;

        Shoot(input.x == 1);
        _attackSpeedTimer = 0;
    }
    private void Shoot(bool mainWeapon)
    {
        if(mainWeapon)
        {
            PlayerBullet pb = Instantiate(bulletPrefab, ShootPos.position, ShootPos.rotation).GetComponent<PlayerBullet>();
            pb.DestinationPos = GunSight.position;
            pb.BulletSpeed = 16;
            ShootEvent?.Invoke();
            GameObject go = Instantiate(shootParticles, ShootPos.position, ShootPos.rotation);
            Destroy(go, 2f);
        }
    }
    private Vector2 GetDestinationPos()
    {
        Vector2 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector2 difference = mousePos - (Vector2)transform.position;
        Vector2 realDir = AimPos.position - transform.position;
        Vector2 direction = realDir.normalized;
        float distance = Mathf.Min(PlayerSM.GetStat(EStatName.range), difference.magnitude);
        distance = Mathf.Max(PlayerSM.GetStat(EStatName.range) / 3, distance);
        Vector2 destPos = (Vector2)transform.position + distance * direction;

        return destPos;
    }
}
