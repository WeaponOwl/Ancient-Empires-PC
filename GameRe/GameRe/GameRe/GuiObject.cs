using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game
{
    class GuiObject
    {
        public Rectangle rect;
        public int id;
        public bool lpressed;
        public bool rpressed;
        public bool enable;
        public bool lclick;
        public bool rclick;
        public bool darktransparency;
        public bool lighttransparency;
        public GameState drawstate;
        public string name;
        public string text;
        public bool undercursor;

        public delegate void UpdateFunction(ref GuiObject me);
        public delegate void DrawFunction(SpriteBatch spriteBatch, Texture2D line, Texture2D darkbackground, Texture2D lightbackground, SpriteFont font,ref GuiObject me);
        public DrawFunction drawFunction;
        public UpdateFunction updateFunction;


        static public void Nothing(bool l, bool r) { ;}
        public GuiObject() { ;}
        public GuiObject(Rectangle rec, int i, bool dtr, bool ltr, GameState st, UpdateFunction f,DrawFunction f2, string name = "", string text = "") 
        {
            rect = rec;
            id = i;
            lpressed = false;
            rpressed = false;
            enable = true;
            lclick = false;
            rclick = false;
            darktransparency = dtr;
            lighttransparency = ltr;
            drawstate = st;
            this.name = name;
            this.text = text;
            updateFunction = f;
            drawFunction = f2;
        }

        public void Update(MouseState state)
        {
            lclick = false;
            rclick = false;
            if (rect.Contains(new Point(state.X, state.Y)))
            {
                if (state.LeftButton == ButtonState.Pressed)
                    if (!lpressed) { lclick = true; lpressed = true; }
                if (lpressed && state.LeftButton == ButtonState.Released)
                    lpressed = false;

                if (state.RightButton == ButtonState.Pressed)
                    if (!rpressed) { rclick = true; rpressed = true; }
                if (rpressed && state.RightButton == ButtonState.Released)
                    rpressed = false;
                undercursor = true;
            }
            else undercursor = false;
            //Function(lclick, rclick);
        }
        /*public void Draw(SpriteBatch spriteBatch,Texture2D line,Texture2D darkbackground,Texture2D lightbackground,SpriteFont font)
        {
            if(darktransparency)
                spriteBatch.Draw(darkbackground, new Vector2(rect.X, rect.Y), new Rectangle(0, 0, rect.Width, rect.Height), Color.White,0,Vector2.Zero,1,SpriteEffects.None,0);
            if(lighttransparency)
                spriteBatch.Draw(lightbackground, new Vector2(rect.X, rect.Y), new Rectangle(0, 0, rect.Width, rect.Height), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            spriteBatch.Draw(line, new Vector2(rect.X, rect.Y), new Rectangle(0, 0, 2, 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(line, new Vector2(rect.X + rect.Width - 2, rect.Y), new Rectangle(2, 0, 2, 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(line, new Vector2(rect.X, rect.Y + rect.Height - 2), new Rectangle(0, 2, 2, 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(line, new Vector2(rect.X + rect.Width - 2, rect.Y + rect.Height - 2), new Rectangle(2, 2, 2, 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            spriteBatch.Draw(line, new Rectangle(rect.X, rect.Y + 2, 2, rect.Height - 4), new Rectangle(0, 2, 2, 0), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(line, new Rectangle(rect.X + rect.Width - 2, rect.Y + 2, 2, rect.Height - 4), new Rectangle(2, 2, 2, 0), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(line, new Rectangle(rect.X + 2, rect.Y, rect.Width - 4, 2), new Rectangle(2, 0, 0, 2), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(line, new Rectangle(rect.X + 2, rect.Y + rect.Height - 2, rect.Width - 4, 2), new Rectangle(2, 2, 0, 2), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);

            if (text != "")
            {
                Vector2 size = font.MeasureString(text);
                spriteBatch.DrawString(font, text, new Vector2(rect.X+rect.Width/2-size.X/2, rect.Y+rect.Height/2-size.Y/2), Color.White);
            }
        }*/
    }
}