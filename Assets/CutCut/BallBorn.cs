using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBorn : MonoBehaviour
{

    public GameObject ball;
    private GameObject temp;
    public int nums;
    void Start()
    {
        ball = Resources.Load<GameObject>("Ball");
        StartCoroutine(BornBall());
    }

    private IEnumerator BornBall()
    {
        while (nums++<10)
        {
            temp = Instantiate<GameObject>(ball, this.transform);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
