using System;
using UnityEngine;

namespace Vertigo.Core
{
    [Serializable]
    public sealed class ZoneTierEntry
    {
        [SerializeField, Min(1)] private int minZone = 1;
        [SerializeField] private WheelConfig wheel;

        public int MinZone => minZone;
        public WheelConfig Wheel => wheel;
    }
}