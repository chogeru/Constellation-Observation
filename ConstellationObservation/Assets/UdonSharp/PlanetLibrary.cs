
using UnityEngine;

namespace ConstellationObservation
{
    /// <summary>
    /// Data asset listing the planets to present, in order.
    /// Kept as plain parallel arrays so UdonSharp can read the fields directly at runtime.
    /// </summary>
    [CreateAssetMenu(fileName = "PlanetLibrary", menuName = "Constellation Observation/Planet Library")]
    public class PlanetLibrary : ScriptableObject
    {
        [Tooltip("Display name for each planet, in presentation order.")]
        public string[] planetNames;

        [Tooltip("Planet transform, same order as Planet Names.")]
        public Transform[] targets;

        [Tooltip("Highlight light (child of the planet) toggled on while presenting it, same order as Planet Names.")]
        public Light[] highlightLights;

        [Tooltip("Narration clip for each planet, same order as Planet Names. Leave empty to use the controller's fallback duration.")]
        public AudioClip[] narrationClips;
    }
}
