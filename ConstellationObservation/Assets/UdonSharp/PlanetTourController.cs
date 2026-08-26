
using UnityEngine;
using UdonSharp;
using VRC.SDKBase;

namespace ConstellationObservation
{
    /// <summary>
    /// Walks the player through the planets one at a time: the current planet is pulled in
    /// front of the player, highlighted (a glow light gently pulses), and named on a floating
    /// label while its narration plays. When it's done it returns to its real orbital position
    /// and the next planet takes its place, before advancing to the next.
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

        [Tooltip("Display name per target, same order/length as Targets.")]
        public string[] planetNames;

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

        [Header("Name Label")]
        [Tooltip("Single shared label shown only above whichever planet is currently being explained.")]
        public UnityEngine.UI.Text nameLabel;
        public Vector3 nameLabelOffset = new Vector3(0f, 1.5f, 0f);

        [Header("Bring Planet In Front")]
        [Tooltip("Move the current planet in front of the player while it's being presented.")]
        public bool bringToFront = true;
        public float presentationDistanceMultiplier = 1.8f;
        public float presentationMinDistance = 4f;
        public float presentationMaxDistance = 15f;

        public ExperienceManager manager;

        private int currentIndex = -1;
        private Light activeLight;
        private Transform activeTarget;
        private Vector3[] originalLocalPositions;

        public void BeginTour()
        {
            HideCurrent();
            currentIndex = -1;
            CaptureOriginalPositions();
            Advance();
        }

        private void CaptureOriginalPositions()
        {
            if (targets == null) return;
            if (originalLocalPositions != null && originalLocalPositions.Length == targets.Length) return;
            originalLocalPositions = new Vector3[targets.Length];
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] != null) originalLocalPositions[i] = targets[i].localPosition;
            }
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

            Transform current = targets[currentIndex];
            activeTarget = current;

            if (bringToFront)
            {
                VRCPlayerApi local = Networking.LocalPlayer;
                if (Utilities.IsValid(local))
                {
                    VRCPlayerApi.TrackingData head = local.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
                    float diameter = current.localScale.x;
                    float distance = Mathf.Clamp(diameter * presentationDistanceMultiplier, presentationMinDistance, presentationMaxDistance);
                    Vector3 forward = head.rotation * Vector3.forward;
                    forward.y = 0f;
                    if (forward.sqrMagnitude < 0.0001f) forward = Vector3.forward;
                    forward.Normalize();
                    current.position = head.position + forward * distance;
                }
            }

            if (highlightLights != null && currentIndex < highlightLights.Length && highlightLights[currentIndex] != null)
            {
                activeLight = highlightLights[currentIndex];
                activeLight.intensity = highlightIntensityMax;
            }

            if (nameLabel != null)
            {
                if (planetNames != null && currentIndex < planetNames.Length) nameLabel.text = planetNames[currentIndex];
                float radius = current.localScale.x * 0.5f;
                nameLabel.transform.position = current.position + Vector3.up * (radius + 1.2f);
                nameLabel.gameObject.SetActive(true);
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

            if (nameLabel != null) nameLabel.gameObject.SetActive(false);

            if (bringToFront && activeTarget != null && originalLocalPositions != null
                && currentIndex >= 0 && currentIndex < originalLocalPositions.Length)
            {
                activeTarget.localPosition = originalLocalPositions[currentIndex];
            }
            activeTarget = null;
        }
    }
}
