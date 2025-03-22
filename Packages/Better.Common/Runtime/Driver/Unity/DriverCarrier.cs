using System.Collections.Generic;
using UnityEngine;

namespace Better
{
    internal static class DriverCarrier
    {
        static readonly Dictionary<string, GameObject> _driverCarriers = new Dictionary<string, GameObject>();

        public static GameObject GetDriverCarrier(string driverName)
        {
            _driverCarriers.TryGetValue(driverName, out var driverCarrier);

            if (driverCarrier == null)
            {
                driverCarrier = new GameObject(driverName);
                Object.DontDestroyOnLoad(driverCarrier);
                
                _driverCarriers[driverName] = driverCarrier;
            }
            
            return driverCarrier;
        }

        public static void DestroyDriverCarrier(string driverName)
        {
            if (_driverCarriers.TryGetValue(driverName, out var driverCarrier))
            {
                Object.Destroy(driverCarrier);
                _driverCarriers.Remove(driverName);
            }
        }
    }
}