using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animation))]
public class Block : MonoBehaviour
{
    public int health = 1;
    public int score = 0;
    public int scoreMutiply = 100;
    public AudioClip[] audioClips;
    public AudioMixerGroup audioGroup;
    public bool hit;
    [Range(0,1)]
    public float haveScoreRate;
    public AnimationCurve scoreRateCurve;

    AudioSource audio;
    BoxCollider2D collider2D;
    Animation animation;

    private void Awake()
    {
        gameObject.layer = 6;
        animation = GetComponent<Animation>();
        collider2D = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        float random = Random.value;
        if(random < haveScoreRate)
        {
            random = Random.value;
            score = (int)scoreRateCurve.Evaluate(random) * scoreMutiply;
        }
        
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            Mining();
        }
    }

    public void Mining()
    {
        health--;

        if(animation)
        {
            animation.Play();
        }

        if (!audio)
        {
            audio = gameObject.AddComponent<AudioSource>();
            audio.playOnAwake = false;
            audio.outputAudioMixerGroup = audioGroup;
        }

        if(audio)
        {
            audio.clip = audioClips[Random.Range(0, audioClips.Length - 1)];
            audio.pitch = Random.Range(0.5f, 1.5f);
            audio.Play();
        }

        if (health == 0)
        {
            ScoreManager.instance.AddScore(score);
            Destroy(gameObject);
        }
    }
}
