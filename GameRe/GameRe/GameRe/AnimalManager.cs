using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game
{
    enum BirdType
    { 
        Dark=1,
        White=0
    }
        
    class Bird
    {
        public Vector2 position;
        public BirdType type;
        public Vector2 direction;
        public int animationState;
        public int animationDirection;
        public Vector2 renderFrame;
        public Vector2 startrenderFrame;
        public Vector2 drawposition;
        public Vector2 oldupdateposition;
        public float height;
        public bool down;

        public Bird()
        {
            Random r = new Random();
            r.Next();
            switch (r.Next(4))
            {
                case 0: position = new Vector2(0, r.Next(Game.height)); direction = new Vector2(1, (float)r.NextDouble()/2 - 0.25f); break;
                case 1: position = new Vector2(Game.width - 200, r.Next(Game.height)); direction = new Vector2(-1, (float)r.NextDouble() / 2 - 0.25f); break;
                case 2: position = new Vector2(r.Next(Game.width - 200), 0); direction = new Vector2((float)r.NextDouble() / 2 - 0.25f, 1); break;
                case 3: position = new Vector2(r.Next(Game.width - 200), Game.height); direction = new Vector2((float)r.NextDouble() / 2 - 0.25f, -1); break;
                default: position = new Vector2(0.5f, 0.5f); break;
            }
            animationDirection = 1;
            //direction.Normalize();
            animationState = 0;
            switch (r.Next(2))
            {
                case 0: type = BirdType.Dark; renderFrame = new Vector2(40, 40); height = 60; startrenderFrame = new Vector2(0, 0); break;
                case 1: type = BirdType.White; renderFrame = new Vector2(32, 32); height = 50; startrenderFrame = new Vector2(120, 0); break;
                //default: renderFrame = new Vector2(40, 40);startrenderFrame=new Vector2(0,0);break;
            }
            drawposition = Vector2.Zero;
            oldupdateposition = position;
            down = false;
        }

        public void Update(Camera camera)
        {
            position += direction;
            if ((oldupdateposition - position).Length() > 25)
            {
                animationState += animationDirection;
                if (animationState == 0 || animationState == 2) animationDirection *= -1;
                oldupdateposition = position;
            }
            drawposition = position - camera.position;

            if (down) height-=1;
        }
    }

    class AnimalManager
    {
        public List<Bird> birds;

        public AnimalManager()
        {
            birds = new List<Bird>();
        }

        public void Update(Camera camera)
        {
            for (int i = 0; i < birds.Count; i++)
            {
                birds[i].Update(camera);
                if (birds[i].drawposition.X < -200 ||
                    birds[i].drawposition.X > Game.width - 200 + 200 ||
                    birds[i].drawposition.Y < -200 ||
                    birds[i].drawposition.Y > Game.height + 200||birds[i].height<=0) birds.RemoveAt(i);
            }
        }

        public void Draw(SpriteBatch spriteBatch,Texture2D set,Color light,Camera camera)
        {
            foreach (Bird b in birds)
            {
                int dir = 0;
                if (b.direction.X == 1) dir = 2;
                if (b.direction.Y == 1) dir = 0;
                if (b.direction.Y == -1) dir = 3;
                if (b.direction.X == -1) dir = 1;

                spriteBatch.Draw(set, b.drawposition, new Rectangle((int)(b.startrenderFrame.X + b.renderFrame.X * b.animationState), (int)(b.startrenderFrame.Y + b.renderFrame.Y * dir), (int)(b.renderFrame.X), (int)(b.renderFrame.Y)), new Color(0, 0, 0, 0.5f), 0, Vector2.Zero, 1, SpriteEffects.None, camera.GetZ(b.drawposition.Y, 100));
                spriteBatch.Draw(set, b.drawposition + new Vector2(0, -b.height), new Rectangle((int)(b.startrenderFrame.X + b.renderFrame.X * b.animationState), (int)(b.startrenderFrame.Y + b.renderFrame.Y * dir), (int)(b.renderFrame.X), (int)(b.renderFrame.Y)), light, 0, Vector2.Zero, 1, SpriteEffects.None, camera.GetZ(b.drawposition.Y - b.height, 100));
            }
        }
    }
}
