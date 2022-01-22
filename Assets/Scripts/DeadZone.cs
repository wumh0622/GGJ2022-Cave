using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{

    public AnimationCurve speedCurve;
    bool begin;
    float timer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void GameStart()
    {
        begin = true;
    }

    public void GameStop()
    {
        begin = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(begin)
        {
            timer += Time.deltaTime;
            transform.Translate(new Vector3(speedCurve.Evaluate(timer), 0, 0));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            GameFlow.instance.Dead();
        }
    }
}
