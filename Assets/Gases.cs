using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Gases : MonoBehaviour
{
    [SerializeField] private List<Gas> gasList;
    
    /*private void Awake()
    {
        foreach (var gas in GetComponentsInChildren<Gas>())
        {
            if (gas == null)
                print("AAA");
            gasList.Add(gas);
        }
    }*/

    public GameObject GetProjectile(float positionX, float positionZ)
    {
        foreach (var gas in gasList)
        {
            if (gas.IsInsideArea(positionX, positionZ))
                return gas.projectilePrefab;
        }
        return null;
    }
}
