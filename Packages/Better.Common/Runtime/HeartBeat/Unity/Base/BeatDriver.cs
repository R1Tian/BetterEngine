using System.Collections.Generic;

namespace Better
{
    public abstract class BeatDriver<T> : BaseDriver<T> where T : BeatDriver<T>
    {
        private HeartBeat _common;
        public HeartBeat Common
        {
            get
            {
                if (_common == null)
                {
                    _common = new HeartBeat();
                    _beats.Add(_common);
                }
                return _common;
            }
        }
        
        protected readonly List<HeartBeat> _beats = new List<HeartBeat>();
        protected readonly List<HeartBeat> _executeBeats = new List<HeartBeat>();

        public HeartBeat NewBeat()
        {
            var beat = new HeartBeat();
            _beats.Add(beat);
            return beat;
        }

        public void RemoveBeat(HeartBeat beat)
        {
            if (beat == null) return;

            _beats.Remove(beat);
        }
    }
}