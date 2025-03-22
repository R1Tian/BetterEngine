using System;
using Better.Chronos;
using UnityEngine;

namespace DefaultNamespace
{
    public class ChronosTest : MonoBehaviour
    {
        private void Awake()
        {
            UpdateTimer.Default.LoopMs(3000, () => Debug.Log(123));
        }
    }
}