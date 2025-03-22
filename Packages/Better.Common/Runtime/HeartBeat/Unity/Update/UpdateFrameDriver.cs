using UnityEngine;

namespace Better
{
    public class UpdateFrameDriver : BeatDriver<UpdateFrameDriver>
    {
        const int DELTA_FRAME = 1;

        private void Update()
        {
            _executeBeats.Clear();
            foreach (var beat in _beats)
            {
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