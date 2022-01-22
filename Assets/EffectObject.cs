using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectObject : MonoBehaviour
{
    public float destoryTime = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, destoryTime);
    }
}
