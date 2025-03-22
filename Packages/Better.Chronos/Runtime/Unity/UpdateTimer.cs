namespace Better.Chronos
{
    public class UpdateTimer : Timer
    {
        static UpdateTimer _default;
        public static UpdateTimer Default
        {
            get
            {
                if (null == _default)
                {
                    _default = new UpdateTimer();
                }

                return _default;
            }
        }
        
        private UpdateTimer() : base(UpdateTimeDriver.Instance.NewBeat(), UpdateFrameDriver.Instance.NewBeat())
        {
        }
    }
}