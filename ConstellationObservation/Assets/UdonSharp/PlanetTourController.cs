
using UnityEngine;
using UdonSharp;

namespace ConstellationObservation
{
    /// <summary>
    /// Walks the player through the planets one at a time: the current planet is highlighted
    /// (a glow light gently pulses) while its narration plays, then advances to the next.
    ///
    /// Data is authored in a PlanetLibrary asset for convenience (name / target / highlight
    /// light / narration clip together on one row), then copied into the arrays below via the
    /// "Sync From Library" button in this component's inspector. UdonSharp cannot read fields
    /// off a plain ScriptableObject at runtime, so the arrays here are what actually get used
    /// in-game.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class PlanetTourController : UdonSharpBehaviour
    {
        [Tooltip("Planets in presentation order (e.g. Sun, Mercury, Venus, ... Neptune).")]
        public Transform[] targets;

        [Tooltip("One highlight Light per target, same order/length as Targets. Intensity is pulsed on/off.")]
        public Light[] highlightLights;

        public AudioSource audioSource;

        [Tooltip("One narration clip per target, same order/length as Targets. Leave an entry empty to fall back to Fallback Duration for that planet.")]
        public AudioClip[] narrationClips;

        [Tooltip("How long to show a planet if it has no narration clip assigned.")]
        public float fallbackDuration = 6f;

        [Header("Highlight Pulse")]
        public float highlightIntensityMin = 1.2f;
        public float highlightIntensityMax = 4f;
        public float pulseSpeed = 1.6f;

        public ExperienceManager manager;

        private int currentIndex = -1;
        private Light activeLight;

        public void BeginTour()
        {
            HideCurrent();
            currentIndex = -1;
            Advance();
        }

        public void Advance()
        {
            HideCurrent();

            currentIndex++;

            if (targets == null || currentIndex >= targets.Length)
            {
                if (manager != null) manager.OnPlanetTourComplete();
                return;
            }

            if (highlightLights != null && currentIndex < highlightLights.Length && highlightLights[currentIndex] != null)
            {
                activeLight = highlightLights[currentIndex];
                activeLight.intensity = highlightIntensityMax;
            }

            float wait = fallbackDuration;
            if (audioSource != null && narrationClips != null && currentIndex < narrationClips.Length && narrationClips[currentIndex] != null)
            {
                audioSource.clip = narrationClips[currentIndex];
                audioSource.Play();
                wait = narrationClips[currentIndex].length;
            }

            SendCustomEventDelayedSeconds(nameof(Advance), wait);
        }

        private void Update()
        {
            if (activeLight == null) return;
            float wave = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f; // 0..1
            activeLight.intensity = Mathf.Lerp(highlightIntensityMin, highlightIntensityMax, wave);
        }

        private void HideCurrent()
        {
            if (activeLight != null) activeLight.intensity = 0f;
            activeLight = null;
        }
    }
}
