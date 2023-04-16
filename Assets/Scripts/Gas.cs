using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Gas : MonoBehaviour
{
    [FormerlySerializedAs("fireBallPrefab")] public GameObject projectilePrefab;
    private Bounds m_bounds;

    private void Awake()
    {
        m_bounds = GetComponent<BoxCollider>().bounds;
    }

    public bool IsInsideArea(float positionX, float positionZ)
    {
        return m_bounds.Contains(new Vector3(positionX, m_bounds.center.y, positionZ));
    }
}
