using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float backScale;
    [SerializeField] private float breakingScale;

    [SerializeField] private AnimationCurve speedCurve;
    private float _speedCurveTimer = 0;

    private bool back = false;
    public bool noMovement { get; private set; } = true;

    private Vector2 turnInterruption = Vector2.zero;

    public float realTime_speed { get; private set; }

    private float turningSpeed 
    {
        get
        {
            return playerSM.GetStat(EStatName.turiningSpeed);
        }
    }
    private float movementSpeed
    {
        get
        {
            return playerSM.GetStat(EStatName.movementSpeed) - playerSM.GetStat(EStatName.weight) / 100;
        }
    }
    private float rotateSpeed
    {
        get
        {
            return playerSM.GetStat(EStatName.rotateSpeed);
        }
    }
    private float acceleration
    {
        get
        {
            return playerSM.GetStat(EStatName.acceleration);
        }
    }

    private GameObject PartTop => transform.GetChild(0).gameObject;

    private Transform Path => transform.GetChild(transform.childCount - 1);

    private Transform BarrelTransform => PartTop.transform.GetChild(0);
    private Transform BarrelPath => PartTop.transform.GetChild(0).GetChild(0);
    private readonly float maxBackBarrelLenght = 0.02f;
    private readonly float maxTimer = 0.04f;
    private float _timer;
    private bool IsShooting
    {
        set
        {
            _isShooting = value;
            if(_isShooting)
            {
                BarrelTransform.localPosition = playerSM.PlayerItemManager.TopMain.barrelPosition;
            }
        }
    }
    private bool _isShooting = false;

    private PlayerScriptsManager playerSM => GetComponent<PlayerScriptsManager>();

    void Setup()
    {
        playerSM.PlayerTurret.ShootEvent += SetShootAnim;
    }

    void Start()
    {
        Setup();
    }
    void Update()
    {
        if (playerSM.DisablePlayer)
            return;

        Movement();
        MouseMovement();
        BarrelMovement();
    }
    private void MouseMovement()
    {
        Vector2 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 diff = (Vector2)PartTop.transform.position - mousePos;
        float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg + 90;

        Quaternion nextRotation = Quaternion.Lerp(PartTop.transform.rotation, Quaternion.Euler(0, 0, angle), rotateSpeed * Time.deltaTime);
        PartTop.transform.rotation = nextRotation;
    }
    private void Movement()
    {
        if(Input.GetKey(KeyCode.W))
        {
            DriveMechanics(true);
            CheckForTurnInput();
            return;
        }
        if(Input.GetKey(KeyCode.S))
        {
            DriveMechanics(false);
            CheckForTurnInput();
            return;
        }
        CheckForTurnInput();
        SlowDown(back);
    }
    private void DriveMechanics(bool forward)
    {
        if (back == forward && !noMovement)
        {
            SlowDown(back);
            return;
        }
        noMovement = false;
        back = !forward;

        realTime_speed = movementSpeed * Accelerate() * (forward ? 1 : -1 / backScale);
        transform.position = Vector2.MoveTowards(transform.position, Path.position, realTime_speed * Time.deltaTime);
    }
    private void CheckForTurnInput()
    {
        Vector2 input;
        input = new Vector2((Input.GetKey(KeyCode.A) ? 1 : 0) * (turnInterruption.y == 1 ? 0 : 1), (Input.GetKey(KeyCode.D) ? 1 : 0) * (turnInterruption.x == 1 ? 0 : 1));

        if (Input.GetKeyDown(KeyCode.D))
            turnInterruption = new Vector2(0, 1);
        if (Input.GetKeyDown(KeyCode.A))
            turnInterruption = new Vector2(1, 0);

        if (Input.GetKeyUp(KeyCode.D))
            turnInterruption.y = 0;
        if (Input.GetKeyUp(KeyCode.A))
            turnInterruption.x = 0;

        if (input == Vector2.zero)
            return;

        Quaternion childPosition = PartTop.transform.rotation;
        transform.Rotate(0, 0, (input.x == 1 ? turningSpeed : -turningSpeed) * Time.deltaTime);
        PartTop.transform.rotation = childPosition;
    }
    private void SlowDown(bool back)
    {
        _speedCurveTimer -= Time.deltaTime * breakingScale;
        if (_speedCurveTimer < 0.1f)
        {
            _speedCurveTimer = 0;
            noMovement = true;
        }
        float evaluation = _speedCurveTimer / acceleration;

        realTime_speed = movementSpeed * speedCurve.Evaluate(evaluation);
        realTime_speed *= back ? -1 / backScale : 1;
        transform.position = Vector2.MoveTowards(transform.position, Path.position, realTime_speed * Time.deltaTime);
    }
    private float Accelerate()
    {
        float speed;
        _speedCurveTimer += Time.deltaTime;
        if(_speedCurveTimer > acceleration)
        {
            _speedCurveTimer = acceleration;
        }
        float evaluation = _speedCurveTimer / acceleration;
        speed = speedCurve.Evaluate(evaluation);
        return speed;
    }
    private void SetShootAnim()
    {
        IsShooting = true;
    }
    private void BarrelMovement()
    {
        if(_isShooting)
        {
            _timer += Time.deltaTime;
        }
        Vector2 diff = BarrelTransform.localPosition - BarrelPath.localPosition;
        Vector2 dir = diff.normalized;
        float distance = Mathf.Min(diff.magnitude, maxBackBarrelLenght);
        Vector2 dest = _isShooting ? ((Vector2)BarrelTransform.localPosition -distance * dir) : (playerSM.PlayerItemManager.TopMain.barrelPosition);
        BarrelTransform.localPosition = Vector2.MoveTowards(BarrelTransform.localPosition, dest, Time.deltaTime * (_isShooting ? 3.5f : 0.5f));
        if(_isShooting && _timer >= maxTimer)
        {
            _timer = 0;
            _isShooting = false;
        }
    }
    public bool IsMaxSpeed()
    {
        return Mathf.Abs(realTime_speed) == (back ? movementSpeed / backScale : movementSpeed);
    }

    private void OnDestroy()
    {
        playerSM.PlayerTurret.ShootEvent -= SetShootAnim;
    }
}
