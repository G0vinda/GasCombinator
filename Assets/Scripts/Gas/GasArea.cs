using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Gas;
using UnityEngine;
using Utils;

public class GasArea : MonoBehaviour
{
    [SerializeField] private Projectile.Projectile projectile;

    public Projectile.Projectile Projectile => projectile;
    
    private static List<GasArea> GasAreasInGame = new (); 
    
    private Bounds m_bounds;

    public static Projectile.Projectile GetProjectileFromArea(float positionX, float positionZ)
    {
        var gasAreaOnPosition = GasAreasInGame.First(gasArea => gasArea.IsInsideArea(positionX, positionZ));
        return gasAreaOnPosition.Projectile;
    }

    private void Awake()
    {
        m_bounds = GetComponent<BoxCollider>().bounds;
        GasAreasInGame.Add(this); ;
    }

    private bool IsInsideArea(float positionX, float positionZ)
    {
        return m_bounds.Contains(new Vector3(positionX, m_bounds.center.y, positionZ));
    }
}
