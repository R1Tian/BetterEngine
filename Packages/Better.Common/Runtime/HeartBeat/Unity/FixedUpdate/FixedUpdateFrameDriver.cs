using UnityEngine;

namespace Better
{
    public sealed class FixedUpdateFrameDriver : BeatDriver<FixedUpdateFrameDriver>
    {
        const int DELTA_FRAME = 1;

        void FixedUpdate()
        {
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
                    beat.Update(DELTA_FRAME);
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