namespace Better
{
    public class HeartTime
    {
        public int DeltaMs { get; private set; }
        public long NowMs { get; private set; }
        public int DeltaFrame { get; private set; }
        public int NowFrame { get; private set; }

        public HeartTime(HeartBeat timeBeat, HeartBeat frameBeat)
        {
            timeBeat.Add(TimeTick);
            frameBeat.Add(FrameTick);
        }

        void TimeTick(int deltaMs)
        {
            DeltaMs = deltaMs;
            NowMs += deltaMs;
        }

        void FrameTick(int deltaFrame)
        {
            DeltaFrame = deltaFrame;
            NowFrame += deltaFrame;
        }
    }
}