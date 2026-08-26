
using UnityEngine;
using UdonSharp;

namespace ConstellationObservation
{
    /// <summary>
    /// Walks the player through the constellations one at a time, planetarium-style:
    /// only the current constellation is shown (which makes it stand out on its own),
    /// its narration plays, then it hides and the next one appears.
    ///
    /// Data is authored in a ConstellationLibrary asset for convenience (name / target /
    /// narration clip together on one row), then copied into the arrays below via the
    /// "Sync From Library" button in this component's inspector. UdonSharp cannot read
    /// fields off a plain ScriptableObject at runtime, so the arrays here are what actually
    /// get used in-game.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class ConstellationTourController : UdonSharpBehaviour
    {
        [Tooltip("Constellation root objects in presentation order. Each is toggled active/inactive.")]
        public GameObject[] constellations;

        public AudioSource audioSource;

        [Tooltip("One narration clip per constellation, same order/length as Constellations. Leave an entry empty to fall back to Fallback Duration for that constellation.")]
        public AudioClip[] narrationClips;

        [Tooltip("How long to show a constellation if it has no narration clip assigned.")]
        public float fallbackDuration = 8f;

        public ExperienceManager manager;

        private int currentIndex = -1;

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

            if (constellations == null || currentIndex >= constellations.Length)
            {
                if (manager != null) manager.OnConstellationTourComplete();
                return;
            }

            if (constellations[currentIndex] != null)
                constellations[currentIndex].SetActive(true);

            float wait = fallbackDuration;
            if (audioSource != null && narrationClips != null && currentIndex < narrationClips.Length && narrationClips[currentIndex] != null)
            {
                audioSource.clip = narrationClips[currentIndex];
                audioSource.Play();
                wait = narrationClips[currentIndex].length;
            }

            SendCustomEventDelayedSeconds(nameof(Advance), wait);
        }

        private void HideCurrent()
        {
            if (constellations == null) return;
            if (currentIndex >= 0 && currentIndex < constellations.Length && constellations[currentIndex] != null)
                constellations[currentIndex].SetActive(false);
        }
    }
}
