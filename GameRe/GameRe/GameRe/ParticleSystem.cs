using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game
{
    public class Particle
    {
        public Vector2 position;
        public Vector2 offcet;
        public float lifetime;
        public float maxlifetime;
        public int segments;
        public bool isup;
        public int type;
        public float onetime;
        public Rectangle first;

        /* Types 
         * 0 smoke
         * 1 redspark
         * 2 spark
         * 3 dust
         * 4 big smoke
         * 5 lvlup
        */

        public Particle(int t,Vector2 p,float l,bool up)
        {
            type = t;
            position = p;
            lifetime = l;
            maxlifetime = l;
            //segments = s;
            offcet = Vector2.Zero;
            isup = up;
            switch (t)
            {
                case 0: segments = 4; first = new Rectangle(0, 128, 24, 20); break;
                case 1: segments = 6; first = new Rectangle(0, 64, 64, 64); break;
                case 2: segments = 6; first = new Rectangle(0, 0, 64, 64); break;
                case 3: segments = 4; first = new Rectangle(0, 208, 26, 20); break;
                case 4: segments = 4; first = new Rectangle(0, 148, 72, 60); break;
                case 5: segments = 0; first = new Rectangle(371, 128, 13, 16); break;
            }
            onetime = l / (segments+1);
        }

        public void Update(GameTime gameTime)
        {
            if (isup)
            {
                offcet.Y -= (float)gameTime.ElapsedGameTime.TotalSeconds * 20;
                if (type == 4) offcet.Y -= (float)gameTime.ElapsedGameTime.TotalSeconds * 40;
                if (type == 5) offcet.Y -= (float)gameTime.ElapsedGameTime.TotalSeconds * 10;
            }
            lifetime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        public Vector2 GetPosition(Camera camera)
        {
            return position - camera.position + offcet + camera.offset;
        }
    }

    public class TextParticle
    {
        public Vector2 position;
        public Vector2 offcet;
        public float lifetime;
        public float maxlifetime;
        public string text;
        public int type;
        public Color color;

        /* 0 Lvl up
         * 1 Damage
        */

        public TextParticle(int t,string text, Vector2 p, float l,Color c)
        {
            position = p;
            color = c;
            offcet = Vector2.Zero;
            lifetime = l;
            maxlifetime = l;
            type = t;
            this.text = text;
        }

        public void Update(GameTime gameTime)
        {
            if (type == 0)
            {
                offcet.Y -= (float)gameTime.ElapsedGameTime.TotalSeconds * 30;
            }
            if (type == 1)
            {
                if (lifetime > maxlifetime / 2)
                {
                    float y = 30*(float)Math.Sin((double)((Math.PI ) * (((maxlifetime / 2) - lifetime) / (maxlifetime / 2))));
                    offcet.Y = y;
                }
                else if (lifetime > maxlifetime / 4 && lifetime < maxlifetime / 2)
                {
                    float y = 15 * (float)Math.Sin((double)((Math.PI) * (((maxlifetime / 4) - lifetime) / (maxlifetime / 4))));
                    offcet.Y = y;
                }
                else
                    offcet.Y = 0;

            }
            lifetime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        public Vector2 GetPosition(Camera camera)
        {
            return position - camera.position + offcet + camera.offset;
        }
    }

    public class ParticleSystem
    {
        public List<Particle> particles;
        public List<TextParticle> textParticles;

        public ParticleSystem()
        {
            particles = new List<Particle>();
            textParticles = new List<TextParticle>();
        }

        public void AddNewParticle(Particle p)
        {
            particles.Add(p);
        }
        public void AddNewParticle(TextParticle p)
        {
            textParticles.Add(p);
        }

        public void Update(GameTime gameTime)
        {
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                particles[i].Update(gameTime);
                if (particles[i].lifetime <= 0) particles.Remove(particles[i]);
                if (particles.Count == 0) break;
            }
            for (int i = textParticles.Count - 1; i >= 0; i--)
            {
                textParticles[i].Update(gameTime);
                if (textParticles[i].lifetime <= 0) textParticles.Remove(textParticles[i]);
                if (textParticles.Count == 0) break;
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, Camera camera, Texture2D tex, Color lightColor)
        {
            foreach (Particle p in particles)
            {
                Rectangle r = new Rectangle(0, 0, 1, 1);

                r = new Rectangle(p.first.X+(p.segments - (int)(p.lifetime / p.onetime)) * p.first.Width, p.first.Y, p.first.Width, p.first.Height);

                spriteBatch.Draw(tex, p.GetPosition(camera), r, lightColor, 0, new Vector2(r.Width / 2, r.Height / 2), 1, SpriteEffects.None, 0.10f);
            }
            foreach (TextParticle p in textParticles)
            {
                Rectangle r = new Rectangle(0, 0, 1, 1);

                r = new Rectangle(0, 0, (int)font.MeasureString(p.text).X, (int)font.MeasureString(p.text).Y);

                spriteBatch.DrawString(font, p.text, p.position - camera.position + p.offcet + camera.offset - new Vector2(r.Width / 2 - 1, r.Height / 2), new Color(94, 51, 0), 0, Vector2.Zero, 1, SpriteEffects.None, 0.11f);
                spriteBatch.DrawString(font, p.text, p.position - camera.position + p.offcet + camera.offset - new Vector2(r.Width / 2 + 1, r.Height / 2), new Color(94, 51, 0), 0, Vector2.Zero, 1, SpriteEffects.None, 0.11f);
                spriteBatch.DrawString(font, p.text, p.position - camera.position + p.offcet + camera.offset - new Vector2(r.Width / 2, r.Height / 2 - 1), new Color(94, 51, 0), 0, Vector2.Zero, 1, SpriteEffects.None, 0.11f);
                spriteBatch.DrawString(font, p.text, p.position - camera.position + p.offcet + camera.offset - new Vector2(r.Width / 2, r.Height / 2 + 1), new Color(94, 51, 0), 0, Vector2.Zero, 1, SpriteEffects.None, 0.11f);

                spriteBatch.DrawString(font, p.text, p.position - camera.position + p.offcet + camera.offset - new Vector2(r.Width / 2, r.Height / 2), p.color, 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);
            }
        }

        public List<Particle> GetParticlesByPosition(Vector2 pos)
        {
            List<Particle> list = new List<Particle>();

            foreach (Particle p in particles)
            {
                if ((int)p.position.X/64 == (int)pos.X && (int)p.position.Y/64 == (int)pos.Y)
                    list.Add(p);
            }

            return list;
        }
        public bool IsLifeParticlesOfType(int t)
        {
            foreach (Particle p in particles)
            {
                if (p.type == t) return true;
            }
            return false;
        }
        public Vector2 GetPositionByType(int t)
        {
            foreach (Particle p in particles)
                if (p.type == t) return p.position / 64;
            return new Vector2(-1);
        }
    }
}