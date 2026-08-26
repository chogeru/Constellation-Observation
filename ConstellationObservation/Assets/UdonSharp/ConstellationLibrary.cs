
using UnityEngine;

namespace ConstellationObservation
{
    /// <summary>
    /// Data asset listing the constellations to present, in order.
    /// Kept as plain parallel arrays (rather than a wrapper class per entry) so that
    /// UdonSharp can read the fields directly at runtime.
    /// </summary>
    [CreateAssetMenu(fileName = "ConstellationLibrary", menuName = "Constellation Observation/Constellation Library")]
    public class ConstellationLibrary : ScriptableObject
    {
        [Tooltip("Display name for each constellation, in presentation order.")]
        public string[] constellationNames;

        [Tooltip("Root GameObject for each constellation's stars/lines, same order as Constellation Names.")]
        public GameObject[] targets;

        [Tooltip("Narration clip for each constellation, same order as Constellation Names. Leave empty to use the controller's fallback duration.")]
        public AudioClip[] narrationClips;
    }
}
