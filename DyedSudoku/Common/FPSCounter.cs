using System;

namespace Common
{
    public class FPSCounter
    {
        private DateTime lastTime;
        private short lastFPS;
        private short ticks;

        public FPSCounter()
        {
            lastTime = DateTime.Now;
            lastFPS = 0;
            ticks = 0;
        }

        public void Tick()
        {
            ticks++;

            if (DateTime.Now.Second == lastTime.Second)
                return;

            lastTime = DateTime.Now;
            lastFPS = ticks;
            ticks = 0;
        }

        public short GetFPS()
        {
            return lastFPS;
        }
    }
}

