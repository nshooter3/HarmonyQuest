﻿namespace GameAI.AIStates.FrogKnight
{
    using GameAI.AIStateActions;
    using GameAI.Navigation;
    using GameAI.StateHandlers;
    using HarmonyQuest.Util;
    using UnityEngine;

    public class FrogKnightEngageState : AIState
    {
        //How frequently to check if our target is obstructed. If so, switch to the navigation state.
        private float checkForTargetObstructionTimer = 0.0f;

        //Used to track the player's distance from this enemy
        private float targetDistance;

        //The distance at which the enemy is available to attack the player
        private float attackRange = 5.0f;

        private bool shouldStrafe = false;

        private MoveAction moveAction = new MoveAction();
        private TargetDistanceAction targetDistanceAction = new TargetDistanceAction();
        private StrafeAction strafeAction;
        private DebugAction debugAction = new DebugAction();
        private GenerateAttackRNGCoefficientAction attackRNGCoefficientGenerator = new GenerateAttackRNGCoefficientAction();

        enum AttackOption { DoNothing, NormalAttack };
        AttackOption attackOption;
        private WeightedList<AttackOption> attackRandomizer = new WeightedList<AttackOption>();

        //Coefficient used to improve odds of attacking when the enemy is in favorable conditions.
        //This includes being the lock on target, being in front of the player, being close to the player, etc.
        private float attackOddsCoefficient = 0;

        //How many beats we've been in this state.
        private int beatCount = 0;

        public override void Init(AIStateUpdateData updateData)
        {
            strafeAction = new StrafeAction(updateData, GetFrogKnightStrafeRandomizer(), 4.0f, 1.0f);
            updateData.aiGameObjectFacade.data.isAggroed = true;
            updateData.aiGameObjectFacade.SetRigidBodyConstraintsToDefault();
            updateData.aiGameObjectFacade.data.individualCollisionAvoidanceModifier = 3.5f;
            updateData.aiGameObjectFacade.data.individualCollisionAvoidanceMaxDistance = 4.0f;
            targetDistanceAction.Init();
            attackRNGCoefficientGenerator.Init(updateData.player);
        }

        //Set up our Frog Knight strafe type odds before passing in the weighted list to the StrafeAction class.
        private WeightedList<StrafeAction.StrafeType> GetFrogKnightStrafeRandomizer()
        {
            WeightedList<StrafeAction.StrafeType> strafeRandomizer = new WeightedList<StrafeAction.StrafeType>();
            strafeRandomizer.Add(StrafeAction.StrafeType.Clockwise, 1);
            strafeRandomizer.Add(StrafeAction.StrafeType.Counterclockwise, 1);
            strafeRandomizer.Add(StrafeAction.StrafeType.Towards, 1);
            strafeRandomizer.Add(StrafeAction.StrafeType.Away, 1);
            strafeRandomizer.Add(StrafeAction.StrafeType.None, 3);
            return strafeRandomizer;
        }

        public override void OnUpdate(AIStateUpdateData updateData)
        {
            Think(updateData);
            Act(updateData);
        }

        public override void OnFixedUpdate(AIStateUpdateData updateData)
        {
            updateData.aiGameObjectFacade.ApplyVelocity();
            updateData.aiGameObjectFacade.ApplyGravity();
        }

        public override void OnBeatUpdate(AIStateUpdateData updateData)
        {
            //This particular enemy should only attack if they're within standard attacking distance.
            if (updateData.aiGameObjectFacade.isAvailableToAttack == true)
            {
                InitAttackRandomizerWithRNGCoefficient(updateData);
                RandomAttack(updateData);
            }

            beatCount++;

            //If we've been in the engage state for more than one beat, use RNG to attack instead of doing so ASAP.
            updateData.aiGameObjectFacade.shouldAttackAsSoonAsPossible = false;
        }

        private void InitAttackRandomizerWithRNGCoefficient(AIStateUpdateData updateData)
        {
            attackRandomizer.Clear();
            attackOddsCoefficient = attackRNGCoefficientGenerator.GetEnemyAttackRNGCoefficient(updateData);

            attackRandomizer.AddFloatWeightThenConvertToInt(AttackOption.DoNothing, 2f);
            //Since attackOddsCoefficient is a float, we multiply all our RNG odds by large number (AIStateConfig.floatToIntConversionScale) then convert it to an int.
            //This happens through the AddFloatWeightThenConvertToInt function.
            attackRandomizer.AddFloatWeightThenConvertToInt(AttackOption.NormalAttack, 1f * attackOddsCoefficient);
        }

        private void RandomAttack(AIStateUpdateData updateData)
        {
            attackOption = attackRandomizer.GetRandomWeightedEntry();
            if (attackOption == AttackOption.NormalAttack || updateData.aiGameObjectFacade.shouldAttackAsSoonAsPossible == true)
            {
                updateData.aiGameObjectFacade.requestingAttackPermission = true;
            }
        }

        private void UpdateAttackAvailability(AIStateUpdateData updateData)
        {
            updateData.aiGameObjectFacade.isAvailableToAttack = updateData.aiGameObjectFacade.GetDistanceFromAggroTarget() < AIStateConfig.standardAttackMaxDistance;
        }

        private void Think(AIStateUpdateData updateData)
        {
            //Update navpos graphic for debug. Shows where the agent is focusing.
            debugAction.NavPosTrackTarget(updateData);

            targetDistance = updateData.aiGameObjectFacade.GetDistanceFromAggroTarget();

            targetDistanceAction.Update(targetDistance);
            strafeAction.OnUpdate(updateData, targetDistance, targetDistanceAction.minDistanceFromPlayer, targetDistanceAction.maxDistanceFromPlayer);

            shouldStrafe = (targetDistanceAction.hitTargetDistance);

            UpdateAttackAvailability(updateData);
        }

        private void Act(AIStateUpdateData updateData)
        {
            if (updateData.aiGameObjectFacade.attackPermissionGranted == true)
            {
                updateData.aiGameObjectFacade.attackPermissionGranted = false;
                updateData.aiGameObjectFacade.attacking = true;
                updateData.stateHandler.RequestStateTransition(new FrogKnightWindup1State { }, updateData);
            }
            else if (shouldStrafe)
            {
                updateData.aiGameObjectFacade.SetRigidBodyConstraintsToDefault();
                Vector3 strafeDir = strafeAction.GetStrafeVector(updateData, updateData.aiGameObjectFacade.data.aggroTarget.transform.position);
                if (updateData.aiGameObjectFacade.data.debugEngage)
                {
                    Debug.Log("ENEMY STRAFE");
                }
                Debug.DrawRay(updateData.aiGameObjectFacade.transform.position, strafeDir * 1.0f, Color.blue);
                moveAction.SeekDirection(updateData.aiGameObjectFacade, strafeDir, true, 0.35f);
            }
            else if (targetDistanceAction.hitTargetDistance == false)
            {
                if (updateData.aiGameObjectFacade.data.debugEngage)
                {
                    Debug.Log("ENEMY APPROACH PLAYER");
                }
                Debug.DrawRay(updateData.aiGameObjectFacade.transform.position, (updateData.aiGameObjectFacade.data.aggroTarget.position - updateData.aiGameObjectFacade.transform.position) * 1.0f, Color.green);
                updateData.aiGameObjectFacade.SetRigidBodyConstraintsToDefault();
                moveAction.SeekDestination(updateData.aiGameObjectFacade, updateData.aiGameObjectFacade.data.aggroTarget.position);
            }
            else
            {
                if (updateData.aiGameObjectFacade.data.debugEngage)
                {
                    Debug.Log("ENEMY STAND STILL");
                }
                updateData.aiGameObjectFacade.SetRigidBodyConstraintsToLockAllButGravity();
            }
        }

        public override void CheckForStateChange(AIStateUpdateData updateData)
        {
            if (updateData.aiGameObjectFacade.IsDead() == true)
            {
                updateData.stateHandler.RequestStateTransition(new FrogKnightDeadState { }, updateData);
            }
            else if (ShouldDeAggro(updateData))
            {
                checkForTargetObstructionTimer = 0;
                updateData.stateHandler.RequestStateTransition(new FrogKnightLoseTargetState { }, updateData);
            }
            else
            {
                checkForTargetObstructionTimer += Time.deltaTime;
                if (checkForTargetObstructionTimer > NavigatorSettings.checkForTargetObstructionRate)
                {
                    checkForTargetObstructionTimer = 0;
                    if (NavMeshUtil.IsTargetObstructed(updateData.aiGameObjectFacade.data.aiAgentBottom, updateData.player.GetTransform()))
                    {
                        updateData.stateHandler.RequestStateTransition(new FrogKnightNavigateState { }, updateData);
                    }
                }
            }
        }

        public override void Abort(AIStateUpdateData updateData)
        {
            updateData.aiGameObjectFacade.data.individualCollisionAvoidanceModifier = 1.0f;
            updateData.aiGameObjectFacade.data.individualCollisionAvoidanceMaxDistance = NavigatorSettings.collisionAvoidanceDefaultMaxDistance;
            updateData.aiGameObjectFacade.ResetVelocity();
            updateData.aiGameObjectFacade.requestingAttackPermission = false;
            updateData.aiGameObjectFacade.attackPermissionGranted = false;
            updateData.aiGameObjectFacade.attacking = false;
            updateData.aiGameObjectFacade.isAvailableToAttack = false;
            aborted = true;
            readyForStateTransition = true;
        }

        private bool ShouldDeAggro(AIStateUpdateData updateData)
        {
            return updateData.aiGameObjectFacade.data.aiStats.disengageWithDistance && Vector3.Distance(updateData.aiGameObjectFacade.transform.position, updateData.player.GetTransform().position) > updateData.aiGameObjectFacade.data.aiStats.disengageDistance;
        }
    }
}
