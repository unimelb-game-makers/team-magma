using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using UnityEditor.Callbacks;
using Damage;

public class AssassinEnemyController : EnemyController
{
    [Header("Dash")]
    // [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 2f;
    private bool isDashing = false;
    private float currentDashTime;
    private Vector3 dashDirection;

    // Temp variables for collision checking - if all enemy types are using this, should declare in the base class
    [Header("Knockback")]
    // [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackDuration = 1f;
    private bool hasCollidedWithPlayer = false;
    public bool isKnockback = false;
    private float currentknockbackTime;

    // May include this in base class if all enemy types are using this.
    [SerializeField] private string knockbackAnimationTrigger = "isKnockback";

    public override void Attack() {
        if (isKnockback) {
            currentknockbackTime -= Time.deltaTime;
            if (currentknockbackTime <= 0) {
                isKnockback = false;
                hasCollidedWithPlayer = false;
            }
        }
        else if (isDashing) {
            UpdateDash();
        }
        else {
            transform.LookAt(Player.transform.position);

            if (!IsAttacking())
            {
                // Trigger animation
                // GetAnimator().SetTrigger(GetAttackAnimationTrigger());

                SetIsAttacking(true);
                StartDash();
            }
        }     
    }

    private void StartDash() {
        isDashing = true;
        GetNavMeshAgent().enabled = false;
        // The dash cannot change direction midway.
        // Calculate the direction without y, so enemy stays on the ground.
        Vector3 dashDirectionWithY = Player.transform.position - transform.position;
        dashDirection = new Vector3(dashDirectionWithY.x, 0f, dashDirectionWithY.z).normalized;
        currentDashTime = dashDuration;
    }

    private void UpdateDash() {
        currentDashTime -= Time.deltaTime;

        // If the dash's duration has ended or the player was hit, end the dash.
        if (currentDashTime <= 0 || hasCollidedWithPlayer) {
            isDashing = false;
            SetIsAttacking(false);
            GetNavMeshAgent().enabled = true;

            if (hasCollidedWithPlayer) {
                isKnockback = true;
                currentknockbackTime = knockbackDuration;
                // GetAnimator().SetTrigger(knockbackAnimationTrigger);

                // Apply a knockback:
                // Calculate the direction without y, so enemy stays on the ground.
                Vector3 knockbackDirectionWithY = transform.position - Player.transform.position;
                Vector3 knockbackDirection = new Vector3(knockbackDirectionWithY.x, 0f, knockbackDirectionWithY.z).normalized;
                //GetRigidbody().AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);

                // Player has taken damage
                // GetPlayerController().GetComponent<Damageable>().TakeDamage(GetDamage());
            }
        } else {
            //GetRigidbody().velocity = dashDirection * dashSpeed;
        }
    }

    // Temporary methods for checking for collisions with the player.
    // Checks if the enemy has collided with the player.
    // Could implement this for all enemy types and apply a knockback to all of them, interrupting their attack.
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            Debug.Log("Assassin Enemy has collided with the player");
            hasCollidedWithPlayer = true;
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
}