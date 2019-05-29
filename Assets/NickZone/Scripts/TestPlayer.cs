using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    CharacterController characterController;

    [SerializeField]
    private GameObject attackBox, parryBox;
    //[SerializeField]
    private Material playerMat;

    public int health = 100;
    public int attackDamage = 10;

    public TestEnemy lockOnTarget;
    public GameObject lockOnReticule;

    public float speed;
    public float gravity;
    public float rotateSpeed;
    public float dashSpeedMultiplier = 3.0f;

    private Vector3 moveDirection = Vector3.zero;
    private Vector3 moveDirectionNoGravity = Vector3.zero;

    private bool isLockedOn = false;

    //Timers to determine how long actions are active, as well as how long of a cooldown they have before they can be used again.
    private float attackTimer, maxAttackTimer = 0.1f;
    private float attackCooldownTimer, maxAttackCooldownTimer = 0.05f;
    private float parryTimer, maxParryTimer = 0.2f;
    //If the player's parry doesn't deflect any attacks, briefly leave them in a vulnerable state.
    //TODO: Implement this
    private float parryWhiffTimer, maxParryWhiffTimer = 0.1f;
    private float parryCooldownTimer, maxParryCooldownTimer = 0.1f;
    private float dashTimer, maxDashTimer = 0.25f;
    private float dashCooldownTimer, maxDashCooldownTimer = 0.1f;
    private Vector3 dashDirection = Vector3.zero;

    private Color defaultPlayerMatColor;

    struct ReceivedAttack{
        public TestEnemy attacker;
        public int damage;
        public bool parryable;
    }

    private List<ReceivedAttack> receivedAttacks;

    // Start is called before the first frame update
    void Start()
    {
        receivedAttacks = new List<ReceivedAttack>();
        playerMat = GetComponent<Renderer>().material;
        characterController = GetComponent<CharacterController>();
        defaultPlayerMatColor = playerMat.color;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        CheckForInput();
        CheckReceivedAttacks();
        UpdateTimers();
    }

    void UpdateTimers()
    {
        if (IsParryCooldown())
        {
            parryCooldownTimer -= Time.deltaTime;
        }
        if (IsParrying())
        {
            parryTimer -= Time.deltaTime;
            if (parryTimer <= 0)
            {
                EndParry();
            }
        }
        if (IsAttackCooldown())
        {
            attackCooldownTimer -= Time.deltaTime;
        }
        if (IsAttacking())
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                EndAttack();
            }
        }
        if (IsDashCooldown()){
            dashCooldownTimer -= Time.deltaTime;
        }
        if (IsDashing())
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                EndDash();
            }
        }
    }

    void CheckForInput()
    {
        if(IsParrying() == false && IsAttacking() == false){
            if (Input.GetKeyDown(KeyCode.X) && IsParryCooldown() == false)
            {
                Parry();
            }
            else if (Input.GetKeyDown(KeyCode.Z) && IsAttackCooldown() == false)
            {
                Attack();
            }
            else if (Input.GetKeyDown(KeyCode.C) && IsDashCooldown() == false)
            {
                Dash();
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isLockedOn = !isLockedOn;
            lockOnReticule.SetActive(isLockedOn);
        }
    }

    bool IsParrying()
    {
        return parryTimer > 0;
    }

    bool IsAttacking()
    {
        return attackTimer > 0;
    }

    bool IsDashing()
    {
        return dashTimer > 0;
    }

    bool IsParryCooldown()
    {
        return parryCooldownTimer > 0;
    }

    bool IsAttackCooldown()
    {
        return attackCooldownTimer > 0;
    }

    bool IsDashCooldown()
    {
        return dashCooldownTimer > 0;
    }

    void Parry()
    {
        //The player can cancel out of a dash to parry
        if (IsDashing())
        {
            EndDash();
        }
        parryBox.SetActive(true);
        parryTimer = maxParryTimer;
    }

    void Attack()
    {
        //The player can cancel out of a dash to attack
        if (IsDashing())
        {
            EndDash();
        }
        attackBox.SetActive(true);
        attackTimer = maxAttackTimer;
        Collider boxCol = attackBox.GetComponent<BoxCollider>();
        Collider[] cols = Physics.OverlapBox(boxCol.bounds.center, boxCol.bounds.extents, boxCol.transform.rotation, LayerMask.GetMask("Enemy"));
        for (int i = 0; i < cols.Length; i++)
        {
            TestEnemy enemy = cols[i].GetComponent<TestEnemy>();
            if (enemy != null)
            {
                bool attackedOnBeat = enemy.TakeDamageAndCheckForOnBeatAttack(attackDamage);
                if (attackedOnBeat)
                {
                    //TODO increase attack multiplier meter
                    //print("GOOD! ON BEAT ATTACK!");
                }
                else
                {
                    //TODO decrease attack multiplier
                    //print("BAD! OFF BEAT ATTACK!");
                }
            }
        }
    }

    void Dash()
    {
        dashTimer = maxDashTimer;
        playerMat.color = Color.white;
        dashDirection = transform.forward;
    }

    void EndParry()
    {
        parryBox.SetActive(false);
        parryCooldownTimer = maxParryCooldownTimer;
    }

    void EndAttack()
    {
        attackBox.SetActive(false);
        attackCooldownTimer = maxAttackCooldownTimer;
    }

    void EndDash()
    {
        playerMat.color = defaultPlayerMatColor;
        dashCooldownTimer = maxDashCooldownTimer;
    }

    void Move()
    {
        //Make the player move/turn more slowly when parrying or attacking, and move quickly/turn slowly when dashing.
        float movementSpeedModifier = 1.0f;
        float turnSpeedModifier = 1.0f;
        //If this is anything other than 0, it overrides the player's speed.
        float movementSpeedOverride = 0.0f;
        if (IsDashing() == true)
        {
            movementSpeedOverride = speed * dashSpeedMultiplier;
            turnSpeedModifier = 0.05f;
        }
        else if (IsParrying() == true)
        {
            movementSpeedModifier = 0.5f;
            turnSpeedModifier = 0.05f;
        }
        else if (IsAttacking() == true)
        {
            movementSpeedModifier = 0.75f;
            turnSpeedModifier = 0.05f;
        }


        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        if (movementSpeedOverride != 0)
        {
            int inversionModifier = 1;
            if (moveDirection.magnitude == 0)
            {
                //If the player isn't moving when they dash, make them dash backwards.
                inversionModifier = -1;
            }
            moveDirection = dashDirection * inversionModifier * movementSpeedOverride;
        }
        else
        {
            moveDirection *= speed * movementSpeedModifier;
        }

        moveDirectionNoGravity = moveDirection;

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        //Only rotate player if they aren't dashing
        if (IsDashing() == false)
        {
            RotatePlayer(turnSpeedModifier);
        }
    }

    void RotatePlayer(float turnSpeedModifier)
    {
        //Rotate player to face movement direction
        if (moveDirectionNoGravity.magnitude > 0)
        {
            Vector3 targetPos = transform.position + moveDirectionNoGravity;
            Vector3 targetDir = targetPos - transform.position;
            //If locked on, ignore movement direction and always attempt to face enemy
            if (isLockedOn)
            {
                targetDir = lockOnTarget.transform.position - transform.position;
            }

            // The step size is equal to speed times frame time.
            float step = rotateSpeed * turnSpeedModifier * Time.deltaTime;

            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
            Debug.DrawRay(transform.position, newDir, Color.red);

            // Move our position a step closer to the target.
            transform.rotation = Quaternion.LookRotation(newDir);
        }
        //Failsafe to ensure that x and z are always zero.
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    public void ReceiveAttack(int damage, TestEnemy attacker, bool parryable = true)
    {
        //The player does not immediately receive damage upon taking an attack. They have until the attack ends to parry or dodge it.
        ReceivedAttack receivedAttack = new ReceivedAttack { damage = damage, attacker = attacker, parryable = parryable};
        receivedAttacks.Add(receivedAttack);
    }

    //The player does not immediately receive damage upon taking an attack. They have until the attack ends to parry or dodge it.
    void CheckReceivedAttacks()
    {
        for (int i = receivedAttacks.Count - 1; i >= 0; i--)
        {
            if (receivedAttacks[i].parryable && WasDamageParried(receivedAttacks[i].attacker.gameObject) == true)
            {
                print("SUCCESSFUL PARRY!");
                receivedAttacks[i].attacker.TakeDamage(receivedAttacks[i].damage);
                receivedAttacks.RemoveAt(i);
            }
            else if (IsDashing())
            {
                print("SUCCESSFUL LATE DODGE!");
                receivedAttacks.RemoveAt(i);
            }
            else if (receivedAttacks[i].attacker.IsAttacking() == false)
            {
                TakeDamage(receivedAttacks[i].damage);
                receivedAttacks.RemoveAt(i);
            }
        }
    }

    //TODO: Hook this up to something and make sure that it works.
    void TakeDamage(int damage)
    {
        print("Player takes " + damage + " damage!");
        health = Mathf.Max(0, health - damage);
        if (health <= 0)
        {
            Die();
        }
    }

    //TODO: Make sure that this works.
    bool WasDamageParried(GameObject source)
    {
        if (IsParrying())
        {
            //Calculate the angle of the absorbed attack by getting the angle between where the damage came from relative to the player, and the direction the player is facing.
            Vector3 sourceDirection = source.transform.position - transform.position;
            float damageAngle = Vector3.Angle(transform.forward, sourceDirection);
            //If the damage comes a direction within 60 degrees of where the player is facing, we consider it a successful parry.
            if (damageAngle <= 60)
            {
                return true;
            }
        }
        return false;
    }

    void Die()
    {
        print("Oh no I died :(");
    }
}
