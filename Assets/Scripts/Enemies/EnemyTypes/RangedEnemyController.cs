using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using FMODUnity;

public class RangedEnemyController : EnemyController
{
    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;

    public override void Attack() {
        RotateTowardsPlayer();

        // Calculate the current cooldown time. If cooldown is over, attack.
        SetCurrentAttackCooldown(GetCurrentAttackCooldown() - Time.deltaTime); 
        if (GetCurrentAttackCooldown() <= 0 && GetMusicTimeline().GetOnBeat()) {
            SetCurrentAttackCooldown(GetAttackCooldown());

            Debug.Log("Ranged Attack!");
            GetAnimator().SetTrigger(GetAttackAnimationTrigger());

            /*
             * NOTE: This event audio is triggered using a StudioEventEmitter component attached to this enemy.
             * The audio gets louder closer you are to the enemy, this is because it is using the FMOD Studio Listener Component
             * attached to the camera for Attenuation (basic top-down 3d directional audio falloff).
             * However the StudioEventEmitter "Override Attenuation" setting WONT WORK because the "Wooden Collision" FMOD audio track
             * is uses a spacializer that overrides this.
            */ 

            // Trigger Ranged Attack Audio Event
            StudioEventEmitter fire = GetComponent<FMODUnity.StudioEventEmitter>();
            fire.Play();
            //FMODUnity.RuntimeManager.AttachInstanceToGameObject(fire.EventInstance, gameObject, GetComponent<Rigidbody>());

            


            // Temporary until actual logic is implemented.
            // Eg: Could wait until the animation has finished playing.
            SetIsAttacking(true);

            // If the player is still alive.
            if (GetPlayerController()) SpawnProjectile();

            SetIsAttacking(false);
        }
    }

    protected override IEnumerator SlowTempo(float duration)
    {
        throw new System.NotImplementedException();
    }

    protected override IEnumerator FastTempo(float duration)
    {
        throw new System.NotImplementedException();
    }

    private void SpawnProjectile() {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        // Get the Projectile component from the projectile object.
        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        // Check if the Projectile component exists.
        if (projectileComponent != null) {
            var canAttackList = new List<string> { "Player" };
            projectileComponent.SendMessage("EditCanAttack", canAttackList);

            // Set the initial direction of the projectile.
            Vector3 direction = (GetPlayerController().transform.position - transform.position).normalized;
            projectileComponent.SetInitialDirection(new Vector3(direction.x, 0f, direction.z));
        }
    }
}
