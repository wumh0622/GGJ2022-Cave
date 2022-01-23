using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animation))]
public class Block : MonoBehaviour
{
    public float health = 1;
    public int score = 0;
    public int scoreMutiply = 100;
    public AudioClip[] audioClips;
    public AudioMixerGroup audioGroup;
    public bool hit;
    [Range(0,1)]
    public float haveScoreRate;
    public AnimationCurve scoreRateCurve;
    public GameObject destoryBlockEffect;
    public GameObject goldBurstEffect;
    public GameObject treatureBlockEffect;
    public Vector3 mChangeScale = Vector3.one;
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
        //float random = Random.value;
        //if(random < haveScoreRate)
        //{
        //    random = Random.value;
        //    score = (int)scoreRateCurve.Evaluate(random) * scoreMutiply;
        if (score > 0)
        {
            score *= scoreMutiply;
            Instantiate(treatureBlockEffect, transform).transform.parent = transform;
        }

        //}

    }

    /*private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            Mining(1.0f);
        }
    }*/

    public void Mining(float damage)
    {
        health-= damage;

        if (animation)
        {
            animation.Play();
        }

        transform.DOScale(mChangeScale, 0.1f).SetLoops(2, LoopType.Yoyo);
        GameObject sound = new GameObject("BlockSound");
        AudioSource source = sound.gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.outputAudioMixerGroup = audioGroup;
        source.clip = audioClips[Random.Range(0, audioClips.Length - 1)];

        if (health <= 0)
        {
            source.pitch = Random.Range(0.4f, 0.6f);
        }
        else
        {
            source.pitch = Random.Range(0.5f, 1.5f);
        }

        source.Play();
        Destroy(sound, source.clip.length);

        if (health <= 0)
        {
            if (score > 0)
            {
                ScoreManager.instance.AddScore(score);
                Instantiate(goldBurstEffect, transform.position, Quaternion.identity);
            }

            Instantiate(destoryBlockEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
