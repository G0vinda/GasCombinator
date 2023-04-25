using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GasArea : MonoBehaviour
{
    [SerializeField] private List<Projectile.Projectile> ProjectilePrefabs;

    private static List<Color> Colors = new List<Color>()
    {
        new Color(1f, 0.2945083f, 0.004716992f),
        new Color(0.5801887f, 1f, 0.9878315f),
        new Color(0.4528302f, 0.754717f, 0f),
    };

    [SerializeField] private Projectile.Projectile projectile;

    public Projectile.Projectile Projectile => projectile;

    public static bool randomizeQueued = true;
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
        GasAreasInGame.Add(this);
    }
    
    private void Update()
    {
        if (randomizeQueued)
        {
            randomizeQueued = false;
            foreach (var gasArea in GasAreasInGame)
            {
                var chosen = Random.Range(0, ProjectilePrefabs.Count);
                gasArea.projectile = ProjectilePrefabs[chosen];
                var temp = gasArea.GetComponentInChildren<ParticleSystem>().main;
                temp.startColor = Colors[chosen];
            }
        }
    }

    private bool IsInsideArea(float positionX, float positionZ)
    {
        return m_bounds.Contains(new Vector3(positionX, m_bounds.center.y, positionZ));
    }

    private void OnDestroy()
    {
        randomizeQueued = true;
        GasAreasInGame.Clear();
    }
}