using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bean
{
    public class GreenBean : Bean
    {
        public enum Effect
        {
            SpreadChance,
            PoisonDamage,
            PoisonDuration,
            PoisonSpeed,
            PoisonFart
        }
        
        public List<Effect> effectOrder = new List<Effect>()
        {
            Effect.SpreadChance,
            Effect.PoisonDamage,
            Effect.PoisonDuration,
            Effect.PoisonSpeed,
            Effect.PoisonFart
        };
        
        public static class Attributes
        {
            public static int Collected = 0;
            public static float SpreadChancePerBean = 0.16f;
            public static float PoisonDamagePerBean = 2.5f;
            public static int ExtraPoisonTickPerBean = 1;
            public static float PoisonTickSpeedPerBean = 0.1f;

            public static List<Effect> ActivatedEffects = new List<Effect>();

            public static Dictionary<int, string> EffectInfo = new Dictionary<int, string>() {{1, "Chance to shoot extra green shots"}, 
                {2, "Poison damage increased"}, {3, "Poison duration increased"}, {4, "Poison damage speed increased"}, {5, "Super Poison Fart"}};
            
            public static void ToDefault()
            {
                Collected = 0;
                ActivatedEffects.Clear();
            }
        }
    }
}
