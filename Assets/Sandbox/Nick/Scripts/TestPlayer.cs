using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HarmonyQuest.Audio;
using GamePhysics;
using System;

public class TestPlayer : MonoBehaviour
{
    CharacterController characterController;

    [SerializeField]
    private DamageHitbox attackHitbox;

    [SerializeField]
    private GameObject parryBox;

    [SerializeField]
    private ParticleSystem parryParticles, getHitParticles, healParticles;

    //References to the scripts we'll be using to play musical sounds.
    [SerializeField]
    private HarmonyQuest.Audio.FmodEventHandler attackConnectSounds, harmonyMeterSounds, attackSwingSound, healSound, tonalAttackSound, tonalParrySound;

    public static TestPlayer instance;

    //Used to tell attackConnectSounds what happened when passing in fmod param values.
    public enum AttackFmodParamValues
    {
        None = 0,
        MissedAttack = 1,
        GoodHit = 2,
        GreatHit = 3,
        Parry = 4,
    };

    //Used to tell harmonyMeterSounds what happened when passing in fmod param values.
    public enum HarmonyModeFmodParamValues
    {
        None = 0,
        ComboUp1 = 1,
        ComboUp2 = 2,
        ComboUp3 = 3,
        ComboUp4 = 4,
        HarmonyModeActivated = 5,
    };

    //Used to tell healSound what happened when passing in fmod param values.
    public enum HealFmodParamValues
    {
        None = 0,
        SmallHeal = 1,
        MediumHeal = 2,
        BigHeal = 3,
    };

    [SerializeField]
    private TestPlayerUI playerUI;

    //[SerializeField]
    private Material playerMat;

    public int attackDamage = 10;

    [HideInInspector]
    public int health;
    public int maxHealth;

    public int healingItems;
    private int healingAmount;

    //[HideInInspector]
    public float harmonyCharge;
    public float maxHarmonyCharge;
    public float harmonyChargeDropSpeed = 10.0f;

    public TestEnemy lockOnTarget;
    public GameObject lockOnReticule;

    public float speed;
    public float gravity;
    public float rotateSpeed;
    public float dashSpeedMultiplier = 3.0f;

    private Vector3 moveDirection = Vector3.zero;
    private Vector3 moveDirectionNoGravity = Vector3.zero;

    private bool isLockedOn = false;

    private bool isInHarmonyMode = false;
    private int harmonyModeMultilpier = 1;

    //Timers to determine how long actions are active, as well as how long of a cooldown they have before they can be used again.
    private float attackTimer, maxAttackTimer = 0.1f;
    private float attackCooldownTimer, maxAttackCooldownTimer = 0.05f;
    private float parryTimer, maxParryTimer = 0.2f;
    private float healTimer, maxHealTimer = 0.2f;
    //If the player's parry doesn't deflect any attacks, briefly leave them in a vulnerable state.
    //TODO: Implement this
    private float parryWhiffTimer, maxParryWhiffTimer = 0.1f;
    private float parryCooldownTimer, maxParryCooldownTimer = 0.1f;
    private float dashTimer, maxDashTimer = 0.2f;
    private float dashCooldownTimer, maxDashCooldownTimer = 0.1f;
    private Vector3 dashDirection = Vector3.zero;

    private Color defaultPlayerMatColor;

    private int attackMultiplier = 1, nextMultiplierProgress = 0;

    private bool isDead = false;

    struct ReceivedAttack{
        public TestEnemy attacker;
        public int damage;
        public bool parryable;
    }

    private List<ReceivedAttack> receivedAttacks;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        healingAmount = Mathf.CeilToInt(maxHealth * 0.4f);
        receivedAttacks = new List<ReceivedAttack>();
        playerMat = GetComponent<Renderer>().material;
        characterController = GetComponent<CharacterController>();
        defaultPlayerMatColor = playerMat.color;
        playerUI.SetHealthBar(health, maxHealth);
        playerUI.SetHarmonyChargeBar(harmonyCharge, maxHarmonyCharge);
        playerUI.SetMultiplierProgress(attackMultiplier * harmonyModeMultilpier, nextMultiplierProgress);
        playerUI.SetHealingItems(healingItems);
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        attackHitbox.UpdateHitbox();
        if (isLockedOn && lockOnTarget == null)
        {
            isLockedOn = false;
        }
        if (isDead == false)
        {
            Move();
            CheckForInput();
        }

        CheckReceivedAttacks();
        UpdateTimers();
    }

    void UpdateTimers()
    {
        if (IsHealing())
        {
            healTimer -= Time.deltaTime;
        }
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
        if (isInHarmonyMode)
        {
            harmonyCharge = Mathf.Max(0, harmonyCharge - Time.deltaTime * harmonyChargeDropSpeed);
            playerUI.SetHarmonyChargeBar(harmonyCharge, maxHarmonyCharge);
            if (harmonyCharge <= 0)
            {
                isInHarmonyMode = false;
                harmonyModeMultilpier = 1;
                playerUI.ToggleHarmonyMode(false);
                playerUI.SetMultiplierProgress(attackMultiplier * harmonyModeMultilpier, nextMultiplierProgress);
            }
        }
    }

    void CheckForInput()
    {
        if(IsParrying() == false && IsAttacking() == false && IsHealing() == false){
            if (TestPlayerInputManager.instance.HealButtonDown() && healingItems > 0 && !isInHarmonyMode)
            {
                Heal();
            }
            else if (TestPlayerInputManager.instance.ParryButtonDown() && IsParryCooldown() == false)
            {
                Parry();
            }
            else if (TestPlayerInputManager.instance.AttackButtonDown() && IsAttackCooldown() == false)
            {
                Attack();
            }
            else if (TestPlayerInputManager.instance.DodgeButtonDown() && IsDashCooldown() == false)
            {
                Dash();
            }
        }
        if (TestPlayerInputManager.instance.LockonButtonDown() && lockOnTarget != null)
        {
            isLockedOn = !isLockedOn;
            lockOnReticule.SetActive(isLockedOn);
        }
        if (TestPlayerInputManager.instance.HarmonyModeButtonDown() && harmonyCharge >= maxHarmonyCharge / 2.0f && isInHarmonyMode == false)
        {
            FmodParamData[] harmonyModeParamData = { new FmodParamData("global_melody_harmony_mode", (float) HarmonyModeFmodParamValues.HarmonyModeActivated) };
            harmonyMeterSounds.Play(harmonyModeParamData);
            playerUI.ToggleHarmonyMode(true);
            isInHarmonyMode = true;
            harmonyModeMultilpier = 2;
            health = maxHealth;
            playerUI.SetMultiplierProgress(attackMultiplier * harmonyModeMultilpier, nextMultiplierProgress);
            playerUI.SetHealthBar(health, maxHealth);
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

    bool IsHealing()
    {
        return healTimer > 0;
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
        FmodParamData[] swingParamData = { new FmodParamData("global_melody_attack_swing", 1.0f)};
        attackSwingSound.Play(swingParamData);
        //The player can cancel out of a dash to attack
        if (IsDashing())
        {
            EndDash();
        }
        attackHitbox.ActivateHitbox(0.0f, 0.1f, 20, Guid.NewGuid());
        attackTimer = maxAttackTimer;
        /*Collider boxCol = attackBox.GetComponent<BoxCollider>();
        Collider[] cols = Physics.OverlapBox(boxCol.bounds.center, boxCol.bounds.extents, boxCol.transform.rotation, LayerMask.GetMask("Enemy"));
        for (int i = 0; i < cols.Length; i++)
        {
            TestEnemy enemy = cols[i].GetComponent<TestEnemy>();
            if (enemy != null)
            {
                TestBeatTracker.OnBeatAccuracy attackedOnBeatAccuracy = enemy.TakeDamageAndCheckForOnBeatAttack(attackDamage * attackMultiplier);
                if (attackedOnBeatAccuracy == TestBeatTracker.OnBeatAccuracy.Great)
                {
                    //print("1. GREAT! ON BEAT ATTACK!");
                    tonalAttackSound.Play();
                    FmodParamData[] attackParamData = { new FmodParamData("global_melody_attack_hit", (float)AttackFmodParamValues.GreatHit) };
                    attackConnectSounds.Play(attackParamData);
                    AddToMultiplierProgress(1);
                    harmonyCharge = Mathf.Min(maxHarmonyCharge, harmonyCharge + 2);
                    playerUI.SetHarmonyChargeBar(harmonyCharge, maxHarmonyCharge);
                }
                else if (attackedOnBeatAccuracy == TestBeatTracker.OnBeatAccuracy.Good)
                {
                    //print("2. GOOD! ALMOST ON BEAT ATTACK!");
                    FmodParamData[] attackParamData = { new FmodParamData("global_melody_attack_hit", (float)AttackFmodParamValues.GoodHit) };
                    attackConnectSounds.Play(attackParamData);
                    harmonyCharge = Mathf.Min(maxHarmonyCharge, harmonyCharge + 1);
                    playerUI.SetHarmonyChargeBar(harmonyCharge, maxHarmonyCharge);
                }
                else
                {
                    FmodParamData[] attackParamData = { new FmodParamData("global_melody_attack_hit", (float) AttackFmodParamValues.MissedAttack) };
                    attackConnectSounds.Play(attackParamData);
                    nextMultiplierProgress = 0;
                    if (attackMultiplier == 4)
                    {
                        attackMultiplier = 3;
                    }
                    playerUI.SetMultiplierProgress(attackMultiplier * harmonyModeMultilpier, nextMultiplierProgress);
                }
            }
        }*/
    }

    void Dash()
    {
        dashTimer = maxDashTimer;
        playerMat.color = Color.white;
        if (characterController.velocity.magnitude > 0)
        {
            dashDirection = characterController.velocity.normalized;
        }
        else
        {
            //If the player isn't moving when they dash, make them dash backwards.
            dashDirection = transform.forward * -1;
        }
    }

    void Heal()
    {
        FmodParamData[] healParamData = { new FmodParamData("global_melody_heal", (float) HealFmodParamValues.SmallHeal)};
        healSound.Play(healParamData);
        health = Mathf.Min(health + healingAmount, maxHealth);
        healingItems--;
        playerUI.SetHealthBar(health, maxHealth);
        playerUI.SetHealingItems(healingItems);
        healTimer = maxHealTimer;
        healParticles.Play();
    }

    void EndParry()
    {
        parryBox.SetActive(false);
        parryCooldownTimer = maxParryCooldownTimer;
    }

    void EndAttack()
    {
        //attackBox.SetActive(false);
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


        moveDirection = new Vector3(TestPlayerInputManager.instance.GetHorizontalMovement(), 0.0f, TestPlayerInputManager.instance.GetVerticalMovement());
        if (movementSpeedOverride != 0)
        {
            moveDirection = dashDirection * movementSpeedOverride;
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

        RotatePlayer(turnSpeedModifier);
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
                //print("SUCCESSFUL PARRY!");
                tonalParrySound.Play();
                AddToMultiplierProgress(2);
                harmonyCharge = Mathf.Min(maxHarmonyCharge, harmonyCharge + 6);
                playerUI.SetHarmonyChargeBar(harmonyCharge, maxHarmonyCharge);
                parryParticles.Play();
                FmodParamData[] parryParamData = { new FmodParamData("global_melody_attack_hit", (float)AttackFmodParamValues.Parry) };
                attackConnectSounds.Play(parryParamData);
                receivedAttacks[i].attacker.TakeDamage(attackDamage * 4 * attackMultiplier);
                receivedAttacks.RemoveAt(i);
            }
            else if (IsDashing())
            {
                //print("SUCCESSFUL LATE DODGE!");
                receivedAttacks.RemoveAt(i);
            }
            else if (receivedAttacks[i].attacker.IsAttacking() == false)
            {
                receivedAttacks[i].attacker.PlayAttackConnectSFX(receivedAttacks[i].parryable);
                TakeDamage(receivedAttacks[i].damage);
                receivedAttacks.RemoveAt(i);
            }
        }
    }

    void AddToMultiplierProgress(int nodesToAdd = 1)
    {
        for (int i = 0; i < nodesToAdd; i++)
        {
            nextMultiplierProgress = Mathf.Min(10, nextMultiplierProgress + 1);
            if (nextMultiplierProgress >= 10 && attackMultiplier < 4)
            {
                attackMultiplier++;
                HarmonyModeFmodParamValues paramVal = HarmonyModeFmodParamValues.None;
                switch (attackMultiplier) {
                    case 1:
                        paramVal = HarmonyModeFmodParamValues.ComboUp1;
                        break;
                    case 2:
                        paramVal = HarmonyModeFmodParamValues.ComboUp2;
                        break;
                    case 3:
                        paramVal = HarmonyModeFmodParamValues.ComboUp3;
                        break;
                    case 4:
                        paramVal = HarmonyModeFmodParamValues.ComboUp4;
                        break;
                }
                FmodParamData[] harmonyMeterParamData = { new FmodParamData("global_melody_harmony_mode", (float) paramVal) };
                harmonyMeterSounds.Play(harmonyMeterParamData);
                if (attackMultiplier < 4)
                {
                    nextMultiplierProgress = 0;
                }
            }
        }
        playerUI.SetMultiplierProgress(attackMultiplier * harmonyModeMultilpier, nextMultiplierProgress);
    }

    void SubtractFromMultiplierProgress(int nodesToSubtract = 1)
    {
        for (int i = 0; i < nodesToSubtract; i++)
        {
            nextMultiplierProgress = nextMultiplierProgress - 1;
            if (nextMultiplierProgress < 0)
            {
                if (attackMultiplier > 1)
                {
                    attackMultiplier--;
                    nextMultiplierProgress = 10;
                }
                else
                {
                    nextMultiplierProgress = 0;
                }
            }
        }
        playerUI.SetMultiplierProgress(attackMultiplier * harmonyModeMultilpier, nextMultiplierProgress);
    }

    //TODO: Hook this up to something and make sure that it works.
    void TakeDamage(int damage)
    {
        //print("Player takes " + damage + " damage!");

        getHitParticles.Play();

        LoseAttackMultiplierLevel();

        if (isInHarmonyMode)
        {
            harmonyCharge = Mathf.Max(0, harmonyCharge - damage * 2);
            playerUI.SetHarmonyChargeBar(health, maxHealth);
        }
        else
        {
            health = Mathf.Max(0, health - damage);
            playerUI.SetHealthBar(health, maxHealth);

            if (health <= 0)
            {
                Die();
            }
        }
    }

    void LoseAttackMultiplierLevel()
    {
        attackMultiplier = Mathf.Max(1, attackMultiplier - 1);
        nextMultiplierProgress = 0;

        playerUI.SetMultiplierProgress(attackMultiplier * harmonyModeMultilpier, nextMultiplierProgress);
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
        isDead = true;
        TestGameState.instance.Lose();
    }
}
