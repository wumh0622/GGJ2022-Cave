using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animation))]
public class Block : MonoBehaviour
{
    public int health = 1;
    public int score = 0;

    Collider2D collider2D;
    Animation animation;

    private void Awake()
    {
        gameObject.layer = 6;
        animation = GetComponent<Animation>();
        collider2D = GetComponent<Collider2D>();
    }

    public void Mining()
    {
        health--;

        if(animation)
        {
            animation.Play();
        }
        
        if (health == 0)
        {
            ScoreManager.instance.AddScore(score);
            Destroy(gameObject);
        }
    }
}
