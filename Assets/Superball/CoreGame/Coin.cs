using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Coin : MonoBehaviour
{
    public bool Collected { private set; get; }
    public bool Collect()
    {
        if (Collected) return false;

        EffectsManager.instance.PlayAddCoint(transform.position);
        Destroy(gameObject);

        return true;
    }
}
