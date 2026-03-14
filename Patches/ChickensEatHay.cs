using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using Objects.Items;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Items;
using Assets.Scripts.Objects.Entities;
using Assets.Scripts.GridSystem;

namespace ChickensEatHay.Patches
{
    public static class HayFoodRegistry
    {
        public static Dictionary<Hay, HayAnimalFood> Registry = new();
    }

    public class HayAnimalFood : IAnimalFood
    {
        private Hay _hay;

        public Vector3 Position => _hay.ThingTransformPosition;
        public Grid3 GridPosition => _hay.GridPosition;

        public HayAnimalFood(Hay hay)
        {
            // Debug.Log($"HayAnimalFood({hay}) created ");
            _hay = hay;
        }


        public float GetNutritionValue()
        {
            return 1f;
        }

        public bool OnUseItem(float quantity, Thing onUseThing)
        {
            return _hay.OnUseItem(quantity, onUseThing);
        }
    }

    [HarmonyPatch(typeof(Thing), "Awake")]
    public static class RegisterHayFood
    {
        static void Postfix(Thing __instance)
        {
            if (__instance is Hay hay)
            {
                // Debug.Log($"Hay.Awake({hay})");
                if (!HayFoodRegistry.Registry.ContainsKey(hay))
                {
                    var food = new HayAnimalFood(hay);
                    HayFoodRegistry.Registry[hay] = food;
                    Plant.AllEdibles.Add(food);
                    // Debug.Log($"Added {food} to {hay}");
                }
            }
        }
    }

    [HarmonyPatch(typeof(Thing), "OnDestroy")]
    public static class OnDestroyrHayFood
    {
        static void Postfix(IReferencable __instance)
        {
            // Debug.Log($"Thing.OnDestroy({__instance})");
            if (__instance is Hay hay)
            {
                // Debug.Log($"Hay.OnDestroy({hay})");
                if (HayFoodRegistry.Registry.TryGetValue(hay, out var food))
                {
                    Plant.AllEdibles.Remove(food);
                    HayFoodRegistry.Registry.Remove(hay);
                    // Debug.Log($"removed {food} from {hay}");
                    food = null;
                }
            }
        }
    }

    [HarmonyPatch(typeof(Animal), "Eat")]
    public static class AnimalEatPatch
    {
        static readonly AccessTools.FieldRef<Animal, IAnimalFood> targetFood =
            AccessTools.FieldRefAccess<Animal, IAnimalFood>("_targetFood");

        static bool Prefix(Animal __instance)
        {
            var food = targetFood(__instance);

            if (food is Hay hay)
            {
                float nutritionValue = HayFoodRegistry.Registry[hay].GetNutritionValue();
                float useAmount = 1f;

                if (hay.Quantity < useAmount) return false;

                __instance.Nutrition += nutritionValue * useAmount;

                food.OnUseItem(useAmount, __instance);

                return false;
            }

            return true;
        }
    }
}