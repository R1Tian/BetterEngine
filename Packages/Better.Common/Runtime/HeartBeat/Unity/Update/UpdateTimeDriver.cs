using UnityEngine;

namespace Better
{
    public sealed class UpdateTimeDriver : BeatDriver<UpdateTimeDriver>
    {
        private int _deltaMs;
        
        void Update()
        {
            _deltaMs = Mathf.RoundToInt(1000 * Time.deltaTime); // 毫秒
            
            _executeBeats.Clear();
            foreach (var beat in _beats)
            {
                _executeBeats.Add(beat);
            }

            foreach (var beat in _executeBeats)
            {
                try
                {
                    beat.Update(_deltaMs);
                }
                catch (HeartBeatActionException e)
                {
                    Debug.LogException(e);
                }
            }
            _executeBeats.Clear();
        }
    }
}