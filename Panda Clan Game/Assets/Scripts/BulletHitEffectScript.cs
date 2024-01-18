using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHitEffectScript : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 3);
    }
}
