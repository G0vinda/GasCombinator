using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Gas;
using UnityEngine;
using Utils;

public class GasArea : MonoBehaviour
{
    [OnChangedCall("UpdateFloorColor")]
    [SerializeField] private GasAreaData data;
    [SerializeField] private MeshRenderer renderer;
    [SerializeField] private ParticleSystem fogParticleSystem;

    public Projectile.Projectile Projectile => data.projectilePrefab;
    
    private static List<GasArea> GasAreasInGame = new (); 
    
    private Bounds m_bounds;
    private ParticleSystem m_fogParticleSystem;

    public static Projectile.Projectile GetProjectileFromArea(float positionX, float positionZ)
    {
        var gasAreaOnPosition = GasAreasInGame.First(gasArea => gasArea.IsInsideArea(positionX, positionZ));
        return gasAreaOnPosition.Projectile;
    }
    
    public void UpdateColors()
    {
        renderer.material.color = data.floorColor;
    }

    private void Awake()
    {
        m_bounds = GetComponent<BoxCollider>().bounds;
        GasAreasInGame.Add(this);
        var fogParticles = fogParticleSystem.main;
        fogParticles.startColor = data.gasColor;
    }

    private bool IsInsideArea(float positionX, float positionZ)
    {
        return m_bounds.Contains(new Vector3(positionX, m_bounds.center.y, positionZ));
    }
}
