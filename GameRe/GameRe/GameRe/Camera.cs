using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game
{
    public class Camera
    {
        public Vector2 offset;
        public Vector2 position;
        public Vector2 mouse;
        public Point oldpositionOnMap;
        public MouseState mousestate;
        public Point positionOnMap;
        public Point windowSize;
        public bool onposition;
        Point target;
        public int cameraSpeed;
        public int cameraToUnitSpeed;

        public Camera() { offset = Vector2.Zero; position = Vector2.Zero; }
                public Camera(int w,int h,int mw, int mh)
        {
            offset = Vector2.Zero;
            position = Vector2.Zero;
            mouse = Vector2.Zero;
            positionOnMap = Point.Zero;
            if (w-200 > mw * 64) offset.X = (w-200 - mw * 64) / 2;
            if (h > mh * 64) offset.Y = (h - mh * 64) / 2;
            position = new Vector2((mw * 64 - w + 200 > 0) ? (mw * 64 - w + 200) / 2 : 0, (mh * 64 - h > 0) ? (mh * 64 - h) / 2 : 0);
            windowSize = new Point(w, h);
            onposition = true;
            cameraSpeed = 5;
            cameraToUnitSpeed = 4;
        }

        public void Update(KeyboardState state,Map map,MouseState mstate,GameState gameState)
        {
            mousestate = mstate;
            Vector2 old = position;
            oldpositionOnMap = positionOnMap;
            mouse = new Vector2(mstate.X, mstate.Y);
            if (gameState == GameState.Editor || gameState == GameState.Game)
            {
                if (offset.X == 0) if (state.IsKeyDown(Keys.D) || Game.width - mouse.X < 2) position.X += cameraSpeed;
                if (offset.X == 0) if (state.IsKeyDown(Keys.A) || mouse.X < 2) position.X -= cameraSpeed;
                if (offset.Y == 0) if (state.IsKeyDown(Keys.W) || mouse.Y < 2) position.Y -= cameraSpeed;
                if (offset.Y == 0) if (state.IsKeyDown(Keys.S) || Game.height - mouse.Y < 2) position.Y += cameraSpeed;

                if (offset.X == 0) if (position.X < 0 || position.X / 64 > map.width - Game.width / 64.0f + 200 / 64.0f) position.X = old.X;
                if (offset.Y == 0) if (position.Y < 0 || position.Y / 64 > map.height - Game.height / 64.0f) position.Y = old.Y;
            }

            int i, j;
            j = (int)(mouse.X + position.X - offset.X) / 64;
            i = (int)(mouse.Y + position.Y - offset.Y) / 64;
            positionOnMap.X=j;
            positionOnMap.Y=i;

            #region UpdatePositionToTarget
            //Vector2 old = position;
            if(Game.gameState==GameState.Game)
            {
                if (!onposition)
                {
                    Vector2 v2 = new Vector2(target.X * 64 - Game.width / 2 + 100, target.Y * 64 - Game.height / 2) - offset;
                    bool stopx = false, stopy = false;
                    if (!onposition)
                        for (int k = 0; k < 4; k++)
                        {
                            Vector2 v = new Vector2(target.X * 64 - Game.width / 2 + 100, target.Y * 64 - Game.height / 2) - offset - position;
                            //v = v;
                            v.Normalize();
                            v = v * cameraToUnitSpeed;
                            v.X = (int)v.X;
                            v.Y = (int)v.Y;
                            //position += v;
                            if (offset.X == 0) position.X += v.X;
                            if (offset.Y == 0) position.Y += v.Y;

                            if (offset.X == 0) if (position.X < 0 || position.X / 64 > map.width - Game.width / 64.0f + 200 / 64.0f)
                                {
                                    position.X = old.X;
                                    stopx = true;
                                }
                            if (offset.Y == 0) if (position.Y < 0 || position.Y / 64 > map.height - Game.height / 64.0f)
                                {
                                    position.Y = old.Y;
                                    stopy = true;
                                }
                        }
                    if ((position - v2).Length() < 32 || (stopy || stopx)) onposition = true;
                }
                //else onposition = false;
            }
            #endregion
        }
        public void ToPositionOnMap(int x, int y)
        {
            target = new Point(x, y);
            onposition = false;
        }

        public float GetZ(float y,float level=0)
        {
            float z = 1-(y * 0.5f + level + Game.height / 4) / Game.height;
            if (z < 0) z = 0;
            if (z > 1) z = 1;
            return z;
        }
    }
}
