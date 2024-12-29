using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealthManager
{
    void TakeDamage(float damage);

    bool IsDead();
}
