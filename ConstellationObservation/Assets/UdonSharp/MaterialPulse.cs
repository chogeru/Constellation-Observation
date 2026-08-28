
using UnityEngine;
using UdonSharp;
using VRC.SDKBase;

namespace ConstellationObservation
{
    /// <summary>
    /// Gently pulses a material's emission color between two levels, giving everything that
    /// shares that material (e.g. all constellation stars, which share one material) a slow
    /// "breathing" glow. Uses server time so the pulse stays in sync across every client.
    /// Operates on the material asset directly (not a per-renderer instance) so one of these
    /// components can animate every object using that shared material at once.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class MaterialPulse : UdonSharpBehaviour
    {
        public Material targetMaterial;
        public Color baseEmission = Color.white;

        public float minMultiplier = 0.7f;
        public float maxMultiplier = 1.3f;
        public float pulseSpeed = 0.8f;

        [Tooltip("How long a triggered flash (see TriggerFlash) takes to decay back into the normal pulse.")]
        public float flashDecay = 0.5f;

        private float flashTimer;
        private float flashPeakMultiplier;

        private void Start()
        {
            if (targetMaterial != null) targetMaterial.EnableKeyword("_EMISSION");
        }

        /// Briefly boosts the emission above the normal pulse range, then eases back in - a one-shot
        /// "sparkle" cue callable from other scripts (e.g. when a new constellation reveals).
        public void TriggerFlash(float peakMultiplier)
        {
            flashPeakMultiplier = peakMultiplier;
            flashTimer = flashDecay;
        }

        private void Update()
        {
            if (targetMaterial == null) return;
            float t = (float)Networking.GetServerTimeInSeconds();
            float wave = (Mathf.Sin(t * pulseSpeed) + 1f) * 0.5f;
            float mult = Mathf.Lerp(minMultiplier, maxMultiplier, wave);

            if (flashTimer > 0f)
            {
                flashTimer -= Time.deltaTime;
                float flashT = Mathf.Clamp01(flashTimer / flashDecay);
                mult = Mathf.Lerp(mult, flashPeakMultiplier, flashT);
            }

            targetMaterial.SetColor("_EmissionColor", baseEmission * mult);
        }
    }
}
