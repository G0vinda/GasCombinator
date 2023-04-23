using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bean
{
    public class RedBean : Bean
    {
        public enum Effect
        {
            Spread,
            CriticalChance,
            CriticalMultiplier,
            InstantKill,
            KillFart
        }

        public List<Effect> effectOrder = new List<Effect>()
        {
            Effect.Spread, 
            Effect.CriticalChance,
            Effect.CriticalMultiplier,
            Effect.InstantKill,
            Effect.KillFart
        };
        
        public static class Attributes
        {
            public static int Collected = 0;
            public static int ExtraShotPerBean = 1;
            public static double CriticalChancePerBean = 0.05;
            public static float InstantKillChancePerBean = 0.025f;
            public static List<Effect> ActivatedEffects = new List<Effect>();

            public static void ToDefault()
            {
                Collected = 0;
                ActivatedEffects.Clear();
            }
        }
    }
}
