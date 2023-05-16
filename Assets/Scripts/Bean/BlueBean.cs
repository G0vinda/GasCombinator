using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bean
{
    public class BlueBean : Bean
    {
        public enum Effect
        {
            AvoidDamage,
            DoubleFartHeal,
            PermanentFreeze,
            Spread,
            FreezeFart
        }

        public List<Effect> effectOrder = new List<Effect>()
        {
          Effect.AvoidDamage,
          Effect.DoubleFartHeal,
          Effect.PermanentFreeze,
          Effect.Spread,
          Effect.FreezeFart
        };
        
        public static class Attributes
        {
            public static int Collected = 0;
            public static float AvoidDamageChancePerBean = 0.05f;
            public static float PermanentSlowMultiplierPerBean = 0.5f;
            public static float FartFreezeTime = 5f;
            public static float ExtraShotPerBean = 0.5f;
            
            public static Dictionary<int, string> EffectInfo = new Dictionary<int, string>() {{1, "Damage Avoidance Chance"}, 
                {2, "Farts Heal More"}, {3, "Slow Enemies After Freeze"}, {4, "Triple Blue Shots"}, {5, "Super Freeze Fart"}};

            public static List<Effect> ActivatedEffects = new List<Effect>();

            public static void ToDefault()
            {
                Collected = 0;
                ActivatedEffects.Clear();
            }
        }
    }
}
