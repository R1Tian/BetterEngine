using UnityEngine;

namespace Better
{
    public abstract class BaseDriver<T> : MonoBehaviour where T : BaseDriver<T>
    {
        private static T _instance;
        
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    var carrier = DriverCarrier.GetDriverCarrier(GetCarrierName());
                    _instance = carrier.AddComponent<T>();
                }
                return _instance;
            }
        }

        public static void Destroy()
        {
            if (_instance == null) return;
            
            DriverCarrier.DestroyDriverCarrier(GetCarrierName());

            _instance = null;
        }
        
        static string GetCarrierName()
        {
            var type = typeof(T);

            var parentType = type.BaseType;
            if (null == parentType) return GetFormatName(type.Name);

            string parentName = parentType.Name;
            if (parentName == "BaseDriver`1") return GetFormatName(type.Name);

            return GetFormatName(parentType.Name);
        }

        static string GetFormatName(string name)
        {
            int splitIndex = name.IndexOf('`');
            if (splitIndex > 0) name = name.Substring(0, splitIndex);

            return $"[{name}]";
        }
    }
}