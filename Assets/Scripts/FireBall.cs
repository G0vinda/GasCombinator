using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] private float speed;

    private void Awake()
    {
        Destroy(gameObject, 10f);
    }

    void Update()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
    }
}
