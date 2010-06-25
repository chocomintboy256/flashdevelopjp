using System;
using System.Windows.Forms;

namespace AutoBuildPlugin
{
    public delegate void DelayExecuterHandler();
   

    public class DelayExecuter
    {

        private Timer timer;
        private int numDelay;

        DelayExecuterHandler delayExHandler;

        public DelayExecuter(int delay, DelayExecuterHandler callback)
        {
            numDelay = delay;
            delayExHandler = callback;

        }

        public void TimerStart()
        {
            if (timer != null)
            {
                return;
            }
            timer = new Timer();
            
            timer.Interval = numDelay;
            timer.Tick += new EventHandler(timerCompleted);

            timer.Start();

            delayExHandler();
        }

        private void timerCompleted(System.Object source, EventArgs e)
        {
            Console.WriteLine("timerComp");
            timer.Stop();
            timer.Dispose();
            timer = null;
             
        }

        public void Dispose()
        {
            if (timer == null) return;
            timer.Stop();
            timer.Dispose();
            timer = null;
            delayExHandler = null;
        }
    }
}
