
using UnityEngine;
using UdonSharp;
using VRC.SDKBase;

namespace ConstellationObservation
{
    /// <summary>
    /// Gently rotates the object around its local up axis. Uses server time so every
    /// client sees the same rotation without needing to network-sync a variable.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class SlowRotate : UdonSharpBehaviour
    {
        [Tooltip("Degrees per second.")]
        public float degreesPerSecond = 3f;

        private void Update()
        {
            float t = (float)Networking.GetServerTimeInSeconds();
            transform.localRotation = Quaternion.Euler(0f, t * degreesPerSecond, 0f);
        }
    }
}
