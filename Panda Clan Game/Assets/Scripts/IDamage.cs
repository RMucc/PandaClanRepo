using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{
    // will also take int for Critical damage.
    void TakeDamage(int amount);
}
