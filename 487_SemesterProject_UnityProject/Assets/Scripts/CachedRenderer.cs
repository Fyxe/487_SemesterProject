using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CachedRenderer
{
    public Renderer renderer;
    List<Material> materials = new List<Material>();

    public CachedRenderer(Renderer newRenderer)
    {
        renderer = newRenderer;
        materials = renderer.materials.ToList();
    }

    public void SetMaterial(Material material)
    {
        renderer.materials = new Material[1] { material };
    }

    public void ResetMaterials()
    {
        renderer.materials = materials.ToArray();
    }
}
