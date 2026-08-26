
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

        private Vector3 planetCubeFullScale;
        private Vector3 constellationCubeFullScale;

        // 0 = hidden, 1 = shown, animating toward animTarget
        private float animT = 1f;
        private float animTarget = 1f;
        private bool animating = false;

        private void Start()
        {
            if (planetCube != null) planetCubeFullScale = planetCube.transform.localScale;
            if (constellationCube != null) constellationCubeFullScale = constellationCube.transform.localScale;

            SetCubesActiveImmediate(true);
            if (solarSystemRoot != null) solarSystemRoot.SetActive(true);
            if (constellationsRoot != null) constellationsRoot.SetActive(false);
        }

        private void Update()
        {
            if (!animating) return;

            float step = Time.deltaTime / Mathf.Max(cubeAnimDuration, 0.01f);
            animT = Mathf.MoveTowards(animT, animTarget, step);
            ApplyCubeScale(animT);

            if (Mathf.Approximately(animT, animTarget))
            {
                animating = false;
                if (animTarget <= 0f)
                {
                    if (planetCube != null) planetCube.SetActive(false);
                    if (constellationCube != null) constellationCube.SetActive(false);
                }
            }
        }

        private void ApplyCubeScale(float t)
        {
            if (planetCube != null) planetCube.transform.localScale = planetCubeFullScale * t;
            if (constellationCube != null) constellationCube.transform.localScale = constellationCubeFullScale * t;
        }

        private void SetCubesActiveImmediate(bool active)
        {
            animating = false;
            animT = active ? 1f : 0f;
            animTarget = animT;
            if (planetCube != null) { planetCube.SetActive(active); }
            if (constellationCube != null) { constellationCube.SetActive(active); }
            ApplyCubeScale(animT);
        }

        public void StartPlanetMode()
        {
            HideCubes();
            if (solarSystemRoot != null) solarSystemRoot.SetActive(true);
            if (constellationsRoot != null) constellationsRoot.SetActive(false);
            if (planetTour != null) planetTour.BeginTour();
        }

        public void StartConstellationMode()
        {
            HideCubes();
            if (solarSystemRoot != null) solarSystemRoot.SetActive(false);
            if (constellationsRoot != null) constellationsRoot.SetActive(true);
            if (constellationTour != null) constellationTour.BeginTour();
        }

        public void OnPlanetTourComplete()
        {
            ShowCubes();
        }

        public void OnConstellationTourComplete()
        {
            if (solarSystemRoot != null) solarSystemRoot.SetActive(true);
            if (constellationsRoot != null) constellationsRoot.SetActive(false);
            ShowCubes();
        }

        private void ShowCubes()
        {
            if (planetCube != null) planetCube.SetActive(true);
            if (constellationCube != null) constellationCube.SetActive(true);
            animT = 0f;
            animTarget = 1f;
            ApplyCubeScale(animT);
            animating = true;
        }

        private void HideCubes()
        {
            animTarget = 0f;
            animating = true;
        }
    }
}
