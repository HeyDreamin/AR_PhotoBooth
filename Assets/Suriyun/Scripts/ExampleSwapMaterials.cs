using UnityEngine;
using System.Collections;

// Change renderer's material each changeInterval
// seconds from the material array defined in the inspector.
public class ExampleSwapMaterials : MonoBehaviour
{
    public Material[] materials;
    public float changeInterval = 0.33F;
    public Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.enabled = true;
    }

    void Update()
    {
        if (materials.Length == 0)
            return;

        // we want this material index now
        int index = Mathf.FloorToInt(Time.time / changeInterval);

        // take a modulo with materials count so that animation repeats
        index = index % materials.Length;

        // assign it to the renderer
        rend.sharedMaterial = materials[index];
    }
}