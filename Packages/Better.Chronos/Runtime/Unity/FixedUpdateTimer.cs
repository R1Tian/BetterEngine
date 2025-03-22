namespace Better.Chronos
{
    public class FixedUpdateTimer : Timer
    {
        static FixedUpdateTimer _default;
        public static FixedUpdateTimer Default
        {
            get
            {
                if (null == _default)
                {
                    _default = new FixedUpdateTimer();
                }

                return _default;
            }
        }

        private FixedUpdateTimer() : base(FixedUpdateTimeDriver.Instance.NewBeat(), FixedUpdateFrameDriver.Instance.NewBeat())
        {
        }
    }
}