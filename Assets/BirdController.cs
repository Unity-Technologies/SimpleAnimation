using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour {
    public float Power = 1.0f;

    void Update () {
		GetComponent<Rigidbody2D>().AddForce(Vector2.left * Power);
	}

    private void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
}
