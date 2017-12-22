using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallReset : MonoBehaviour
{

  private void OnTriggerEnter2D(Collider2D other)
    {
        this.transform.localPosition = Vector3.zero;
        this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}
