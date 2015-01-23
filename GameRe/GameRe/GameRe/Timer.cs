using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Game
{
    class Timer
    {
        static public float Time;
        static public void Update(GameTime gameTime)
        {
            Time=(float)gameTime.TotalGameTime.TotalSeconds;
        }

        static public bool GetTimerState(float time,float delta=0)
        {
            if (Math.Abs(Time + delta - (float)((int)((Time + delta) / time)) * time) < 0.01f)
                return true;
            return false;
        }
    }
}
