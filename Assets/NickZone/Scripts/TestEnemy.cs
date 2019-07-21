using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : BeatTrackerObject
{
    public int maxHealth;
    public int health;
    public int attackDamage = 10;

    public float speed;
    public float gravity;
    public float rotateSpeed;

    public bool pursuePlayer = true;
    public bool attackPlayer = true;

    public float aggroRange = 10.0f;

    [SerializeField]
    private CharacterController characterController;
    [SerializeField]
    private TestPlayer player;
    [SerializeField]
    private GameObject attackBox;
    [SerializeField]
    private ParticleSystem getHitParticles;
    [SerializeField]
    private AudioSource getHitSound, charge1Sound, charge2Sound, attack1Sound, attack2Sound;

    [SerializeField]
    private TestEnemyHealthbar healthBar;

    //[SerializeField]
    private Material enemyMat;

    private float attackTimer, maxAttackTimer = 0.4f;
    //Which attack this enemy will perform. 1 means parryable attack, 2 means unparryable attack that must be dodged.
    private float attackType = 1;

    //Change color to telegraph attacks for now
    public Color windUpColor1, windUpColor2, preAttackColor;
    public Color defaultColor;

    private Vector3 moveDirection = Vector3.zero;
    private Vector3 moveDirectionNoGravity = Vector3.zero;

    public enum EnemyState
    {
        Idle,
        Phase1,
        Phase2,
        Dead
    };

    public EnemyState enemyState;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        enemyMat = GetComponent<Renderer>().material;
        enemyState = EnemyState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimers();

        if (pursuePlayer && IsAggroed())
        {
            PursuePlayer();
        }

        CheckForPlayerEnteringAggroRange();
    }

    void UpdateTimers()
    {
        if (IsAttacking())
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                EndAttack();
            }
        }
    }

    void CheckForPlayerEnteringAggroRange()
    {
        if (IsAggroed() == false)
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= aggroRange)
            {
                enemyState = EnemyState.Phase1;
                FmodFacade.instance.SetMusicEventParameterValue("global_phase1", 1.0f);
            }
        }
    }

    bool IsAggroed()
    {
        return enemyState == EnemyState.Phase1 || enemyState == EnemyState.Phase2;
    }

    void PursuePlayer()
    {
        moveDirection = (player.transform.position - transform.position).normalized * speed;

        moveDirectionNoGravity = moveDirection;

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        RotateEnemy(1.0f);
    }

    void RotateEnemy(float turnSpeedModifier)
    {
        //Rotate player to face movement direction
        if (moveDirectionNoGravity.magnitude > 0)
        {
            Vector3 targetPos = transform.position + moveDirectionNoGravity;
            Vector3 targetDir = targetPos - transform.position;

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

    public bool IsAttacking()
    {
        return attackTimer > 0;
    }

    void EndAttack()
    {
        attackBox.SetActive(false);
    }

    public override void SixteenthNoteUpdate()
    {
        if (attackPlayer && IsAggroed())
        {
            int count = 1 + TestBeatTracker.instance.sixteenthNoteCount + (TestBeatTracker.instance.beatCount - 1) * 4;
            //print("ENEMY COUNT = " + count);

            //Giant switch statements, the mark of a true prototype. We gotta come up with a more eloquent way to do beat based actions.
            switch (count)
            {
                case 5:
                    //Start telegraphing attack on the second beat of the measure
                    attackType = 1;

                    //Add the chance to do an unparryable attack once phase 2 activates
                    if (enemyState == EnemyState.Phase2)
                    {
                        int ran = Random.Range(0, 8);
                        if (ran < 3)
                        {
                            attackType = 2;
                        }
                    }

                    if (attackType == 1)
                    {
                        enemyMat.color = windUpColor1;
                        charge1Sound.Play();
                    }
                    else
                    {
                        enemyMat.color = windUpColor2;
                        charge2Sound.Play();
                    }
                    break;
                case 8:
                    //Flash red RIGHT before the moment of attack. One 16th note away, to be precise.
                    enemyMat.color = preAttackColor;
                    break;
                case 9:
                    enemyMat.color = defaultColor;
                    //Attack right on the third beat of the measure
                    if (attackType == 1)
                    {
                        Attack(true);
                        attack1Sound.Play();
                    }
                    else
                    {
                        Attack(false);
                        attack2Sound.Play();
                    }
                    break;
            }
        }
    }

    void Attack(bool parryable = true)
    {
        attackBox.SetActive(true);
        attackTimer = maxAttackTimer;
        Collider boxCol = attackBox.GetComponent<BoxCollider>();
        Collider[] cols = Physics.OverlapBox(boxCol.bounds.center, boxCol.bounds.extents, boxCol.transform.rotation, LayerMask.GetMask("Player"));
        for (int i = 0; i < cols.Length; i++)
        {
            TestPlayer player = cols[i].GetComponent<TestPlayer>();
            if (player != null)
            {
                //print("ATTACK HIT PLAYER!");
                player.ReceiveAttack(attackDamage, this, parryable);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health = Mathf.Max(0, health - damage);
        //print("Enemy " + damage + " damage taken! Health is currently " + health);

        healthBar.SetHealthBarSize(health, maxHealth);

        getHitParticles.Play();
        getHitSound.Play();

        if (enemyState != EnemyState.Dead && health <= 0)
        {
            Die();
        }
        else if (enemyState != EnemyState.Phase2 && enemyState != EnemyState.Dead && health <= maxHealth / 2.0f)
        {
            TransitionToPhase2();
        }
    }

    /// <summary>
    /// Make this enemy take damage
    /// </summary>
    /// <param name="damage"> How much damage to take </param>
    /// <returns> Whether or not the player attacked the enemy on beat </returns>
    public TestBeatTracker.OnBeatAccuracy TakeDamageAndCheckForOnBeatAttack(int damage)
    {
        TestBeatTracker.OnBeatAccuracy attackedOnBeatAccuracy = WasAttackedOnBeat();
        if (attackedOnBeatAccuracy == TestBeatTracker.OnBeatAccuracy.Great)
        {
            TakeDamage(damage);
        }
        else if(attackedOnBeatAccuracy == TestBeatTracker.OnBeatAccuracy.Good)
        {
            TakeDamage(Mathf.Max(damage/2, 1));
        }
        return attackedOnBeatAccuracy;
    }

    void TransitionToPhase2()
    {
        FmodFacade.instance.SetMusicEventParameterValue("global_phase1_idle", 1.0f);
        FmodFacade.instance.SetMusicEventParameterValue("global_dissonance", 1.0f);
        enemyState = EnemyState.Phase2;
    }

    void Die()
    {
        FmodFacade.instance.SetMusicEventParameterValue("global_dissonance_idle", 1.0f);
        FmodFacade.instance.SetMusicEventParameterValue("global_fight_outro", 1.0f);
        TestGameState.instance.Win();
        TestBeatTracker.instance.RemoveBeatTrackerAtIndex(beatTrackerIndex);
        enemyState = EnemyState.Dead;
        Destroy(healthBar.gameObject);
        Destroy(gameObject);
    }

    //Allow a little bit of wiggle room both before and after the beat for determing whether or not the enemy was attacked on beat.
    TestBeatTracker.OnBeatAccuracy WasAttackedOnBeat()
    {
        return TestBeatTracker.instance.WasActionOnBeat();
    }
}
