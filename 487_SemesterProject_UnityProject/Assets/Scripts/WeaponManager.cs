using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : Singleton<WeaponManager>
{
    [System.Serializable]
    public class RarityMaterial
    {
        public Rarity rarity;
        public Material material;
    }

    [Header("Settings")]
    public float rangeHighlight = 5f;
    public float rangeInformation = 2f;
    public List<RarityMaterial> materials = new List<RarityMaterial>();

    public Material GetMaterialForRarity(Rarity rarity)
    {
        foreach (var i in materials)
        {
            if (i.rarity == rarity)
            {
                return i.material;
            }
        }
        Debug.LogError("No material found for rarity \'" + rarity.ToString() + "\'.");
        return null;
    }
}
