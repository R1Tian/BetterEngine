using UnityEngine;

namespace Better
{
    public sealed class FixedUpdateTimeDriver : BeatDriver<FixedUpdateTimeDriver>
    {
        void FixedUpdate()
        {
            int deltaMs = Mathf.RoundToInt(1000 * Time.fixedDeltaTime);

            _executeBeats.Clear();
            foreach (var beat in _beats)
            {
                if (null == beat) continue;

                _executeBeats.Add(beat);
            }

            foreach (var beat in _executeBeats)
            {
                try
                {
                    beat.Update(deltaMs);
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