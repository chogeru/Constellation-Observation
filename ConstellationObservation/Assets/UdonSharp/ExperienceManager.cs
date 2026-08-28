
using UnityEngine;
using UdonSharp;

namespace ConstellationObservation
{
    /// <summary>
    /// Central hub for the two observation scenarios. Owns the selection cubes and
    /// the root objects for the planets / constellations, and hands control off to
    /// the two tour controllers. Cubes grow in / shrink out instead of popping instantly.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class ExperienceManager : UdonSharpBehaviour
    {
        [Header("Selection Cubes")]
        public GameObject planetCube;
        public GameObject constellationCube;

        [Header("Scene Roots")]
        public GameObject solarSystemRoot;
        public GameObject constellationsRoot;

        [Header("Tour Controllers")]
        public PlanetTourController planetTour;
        public ConstellationTourController constellationTour;

        [Tooltip("How long the cube grow/shrink animation takes, in seconds.")]
        public float cubeAnimDuration = 0.4f;

        [Tooltip("How strongly the cubes overshoot and settle as they appear (0 = no overshoot).")]
        public float cubeShowOvershoot = 1.4f;

        [Tooltip("The cubes' normal (fully shown) scale. Fixed here instead of being read from the "
            + "transform at runtime, since that reads whatever scale happened to be set at the moment "
            + "Start() ran and could race against an early StartPlanetMode/StartConstellationMode call.")]
        public Vector3 cubeFullScale = new Vector3(0.6f, 0.6f, 0.6f);

        // 0 = hidden, 1 = shown; currentFraction is the last displayed value, used as the tween's
        // start point so interrupting a show/hide mid-flight restarts smoothly from where it is.
        private float currentFraction = 1f;
        private float animStart = 1f;
        private float animEnd = 1f;
        private float animElapsed = 0f;
        private bool animating = false;

        // Guards against a cube being interacted with twice (e.g. double-click while it's
        // still shrinking) from starting a second, overlapping tour.
        private bool tourInProgress = false;

        private void Start()
        {
            SetCubesActiveImmediate(true);
            if (solarSystemRoot != null) solarSystemRoot.SetActive(true);
            if (constellationsRoot != null) constellationsRoot.SetActive(false);
        }

        private void Update()
        {
            if (!animating) return;

            animElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(animElapsed / Mathf.Max(cubeAnimDuration, 0.01f));
            bool showing = animEnd > animStart;
            float e = showing ? TweenEase.OutBack(t, cubeShowOvershoot) : TweenEase.InCubic(t);
            currentFraction = Mathf.LerpUnclamped(animStart, animEnd, e);
            ApplyCubeScale(currentFraction);

            if (t >= 1f)
            {
                animating = false;
                currentFraction = animEnd;
                ApplyCubeScale(currentFraction);
                if (animEnd <= 0f)
                {
                    if (planetCube != null) planetCube.SetActive(false);
                    if (constellationCube != null) constellationCube.SetActive(false);
                }
            }
        }

        private void ApplyCubeScale(float t)
        {
            if (planetCube != null) planetCube.transform.localScale = cubeFullScale * t;
            if (constellationCube != null) constellationCube.transform.localScale = cubeFullScale * t;
        }

        private void SetCubesActiveImmediate(bool active)
        {
            animating = false;
            currentFraction = active ? 1f : 0f;
            if (planetCube != null) { planetCube.SetActive(active); }
            if (constellationCube != null) { constellationCube.SetActive(active); }
            ApplyCubeScale(currentFraction);
        }

        public void StartPlanetMode()
        {
            if (tourInProgress) return;
            tourInProgress = true;

            HideCubes();
            if (solarSystemRoot != null) solarSystemRoot.SetActive(true);
            if (constellationsRoot != null) constellationsRoot.SetActive(false);
            if (planetTour != null) planetTour.BeginTour();
        }

        public void StartConstellationMode()
        {
            if (tourInProgress) return;
            tourInProgress = true;

            HideCubes();
            if (solarSystemRoot != null) solarSystemRoot.SetActive(false);
            if (constellationsRoot != null) constellationsRoot.SetActive(true);
            if (constellationTour != null) constellationTour.BeginTour();
        }

        public void OnPlanetTourComplete()
        {
            tourInProgress = false;
            ShowCubes();
        }

        public void OnConstellationTourComplete()
        {
            tourInProgress = false;
            if (solarSystemRoot != null) solarSystemRoot.SetActive(true);
            if (constellationsRoot != null) constellationsRoot.SetActive(false);
            ShowCubes();
        }

        private void ShowCubes()
        {
            if (planetCube != null) planetCube.SetActive(true);
            if (constellationCube != null) constellationCube.SetActive(true);
            currentFraction = 0f;
            ApplyCubeScale(currentFraction);
            animStart = 0f;
            animEnd = 1f;
            animElapsed = 0f;
            animating = true;
        }

        private void HideCubes()
        {
            animStart = currentFraction;
            animEnd = 0f;
            animElapsed = 0f;
            animating = true;
        }
    }
}
