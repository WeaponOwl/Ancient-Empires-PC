using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game
{
    class Gui
    {
        public GuiObject[] elements;
        public List<string> names;
        public string selectedName;
        public int selectedNameId;
        public float floatMessageTime;
        public float floatMessageTimeMax;
        public string floatMessage;
        public Rectangle floatMessageRectangle;
        int stoppedmessage;
        float stoppedtime;
        float waitmessagetime;
        public string dialogString;
        public int dialogSayer;
        public bool dialogShowed;
        Rectangle dialogrectangle;
        static Rectangle dialogHead = new Rectangle(15, Game.height - 128 - 15, 128, 128);
        static Rectangle dialogHeadOutline = new Rectangle(13, Game.height - 128 - 17, 132, 132);

        public Gui(GraphicsDevice gr) 
        { 
            elements = new GuiObject[65]; 
            names = new List<string>(); 
            selectedName = ""; 
            selectedNameId = -1;
            stoppedmessage = 0;
            stoppedtime = 0;
            waitmessagetime = 0.5f;
            dialogShowed = false;
            dialogrectangle = new Rectangle(0, Game.height - 150, Game.width - 200, 150);
        }

        public void Update(MouseState mstate,GameState state,GameTime gameTime)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i].drawstate == state&&elements[i].enable)
                {
                    elements[i].Update(mstate);
                    elements[i].updateFunction(ref elements[i]);
                }
            }

            if (floatMessageTime != -1)
            {
                if (floatMessageTime < floatMessageTimeMax / 2&&stoppedmessage!=2)
                {
                    if (stoppedmessage==0)
                    {
                        stoppedtime = (float)gameTime.TotalGameTime.TotalSeconds;
                        stoppedmessage = 1;
                    }
                    if (stoppedmessage==1&&stoppedtime+waitmessagetime<gameTime.TotalGameTime.TotalSeconds)
                    {
                        stoppedmessage = 2;
                    }
                }
                else floatMessageTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (floatMessageTime > 0)
            {
                    floatMessageRectangle.Y = -floatMessageRectangle.Height + (int)(((Game.height) + 2 * floatMessageRectangle.Height) * (1 - floatMessageTime / floatMessageTimeMax));
            }
            if (mstate.LeftButton==ButtonState.Pressed&&dialogShowed && dialogrectangle.Contains(mstate.X, mstate.Y)&&(Game.gameInfo.state==GameState2.None||dialogSayer!=-1))
            {
                dialogString = "";
                dialogShowed = false;
            }
        }

        public GuiObject GetElement(int id)
        {
            GuiObject r = new GuiObject();
            foreach (GuiObject o in elements)
            {
                if (o.id == id)
                    r = o;
            }
            return r;
        }

        public GuiObject GetElement(string name)
        {
            GuiObject r = new GuiObject();
            foreach (GuiObject o in elements)
            {
                if (o.name == name)
                    r = o;
            }
            return r;
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, Texture2D line, Texture2D darkbackground, Texture2D lightbackground,Texture2D portraits, GameState state)
        {
            if (state == GameState.Game)
            {
                if (floatMessageTime > 0 || floatMessageTime == -1)
                {
                    spriteBatch.Draw(lightbackground, new Vector2(floatMessageRectangle.X, floatMessageRectangle.Y), new Rectangle(0, 0, floatMessageRectangle.Width, floatMessageRectangle.Height), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                    int lw = line.Width, lh = line.Height;
                    spriteBatch.Draw(line, new Vector2(floatMessageRectangle.X, floatMessageRectangle.Y), new Rectangle(0, 0, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                    spriteBatch.Draw(line, new Vector2(floatMessageRectangle.X + floatMessageRectangle.Width - lw / 2, floatMessageRectangle.Y), new Rectangle(lw / 2, 0, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                    spriteBatch.Draw(line, new Vector2(floatMessageRectangle.X, floatMessageRectangle.Y + floatMessageRectangle.Height - lh / 2), new Rectangle(0, lh / 2, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                    spriteBatch.Draw(line, new Vector2(floatMessageRectangle.X + floatMessageRectangle.Width - lw / 2, floatMessageRectangle.Y + floatMessageRectangle.Height - lh / 2), new Rectangle(lw / 2, lh / 2, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                    spriteBatch.Draw(line, new Rectangle(floatMessageRectangle.X, floatMessageRectangle.Y + lh / 2, lw / 2, floatMessageRectangle.Height - lh), new Rectangle(0, lh / 2, lw / 2, 0), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                    spriteBatch.Draw(line, new Rectangle(floatMessageRectangle.X + floatMessageRectangle.Width - lw / 2, floatMessageRectangle.Y + lh / 2, lw / 2, floatMessageRectangle.Height - lh), new Rectangle(lw / 2, lh / 2, lw / 2, 0), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                    spriteBatch.Draw(line, new Rectangle(floatMessageRectangle.X + lw / 2, floatMessageRectangle.Y, floatMessageRectangle.Width - lw, lh / 2), new Rectangle(lw / 2, 0, 0, lh / 2), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                    spriteBatch.Draw(line, new Rectangle(floatMessageRectangle.X + lw / 2, floatMessageRectangle.Y + floatMessageRectangle.Height - lh / 2, floatMessageRectangle.Width - lw, lh / 2), new Rectangle(lw / 2, lh / 2, 0, lh / 2), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);

                    spriteBatch.DrawString(font, floatMessage, new Vector2(floatMessageRectangle.X + 15, floatMessageRectangle.Y + 15), Color.White);
                }
                if (dialogShowed)
                {
                    if (dialogSayer == -1)
                    {
                        spriteBatch.Draw(lightbackground, new Vector2(dialogrectangle.X, dialogrectangle.Y), new Rectangle(0, 0, dialogrectangle.Width, dialogrectangle.Height), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                        int lw = line.Width, lh = line.Height;
                        spriteBatch.Draw(line, new Vector2(dialogrectangle.X, dialogrectangle.Y), new Rectangle(0, 0, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Vector2(dialogrectangle.X + dialogrectangle.Width - lw / 2, dialogrectangle.Y), new Rectangle(lw / 2, 0, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Vector2(dialogrectangle.X, dialogrectangle.Y + dialogrectangle.Height - lh / 2), new Rectangle(0, lh / 2, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Vector2(dialogrectangle.X + dialogrectangle.Width - lw / 2, dialogrectangle.Y + dialogrectangle.Height - lh / 2), new Rectangle(lw / 2, lh / 2, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                        spriteBatch.Draw(line, new Rectangle(dialogrectangle.X, dialogrectangle.Y + lh / 2, lw / 2, dialogrectangle.Height - lh), new Rectangle(0, lh / 2, lw / 2, 0), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Rectangle(dialogrectangle.X + dialogrectangle.Width - lw / 2, dialogrectangle.Y + lh / 2, lw / 2, dialogrectangle.Height - lh), new Rectangle(lw / 2, lh / 2, lw / 2, 0), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Rectangle(dialogrectangle.X + lw / 2, dialogrectangle.Y, dialogrectangle.Width - lw, lh / 2), new Rectangle(lw / 2, 0, 0, lh / 2), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Rectangle(dialogrectangle.X + lw / 2, dialogrectangle.Y + dialogrectangle.Height - lh / 2, dialogrectangle.Width - lw, lh / 2), new Rectangle(lw / 2, lh / 2, 0, lh / 2), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);

                        spriteBatch.DrawString(font, dialogString, new Vector2(dialogrectangle.X + 15, dialogrectangle.Y + 15), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(lightbackground, new Vector2(dialogrectangle.X, dialogrectangle.Y), new Rectangle(0, 0, dialogrectangle.Width, dialogrectangle.Height), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                        int lw = line.Width, lh = line.Height;
                        spriteBatch.Draw(line, new Vector2(dialogrectangle.X, dialogrectangle.Y), new Rectangle(0, 0, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Vector2(dialogrectangle.X + dialogrectangle.Width - lw / 2, dialogrectangle.Y), new Rectangle(lw / 2, 0, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Vector2(dialogrectangle.X, dialogrectangle.Y + dialogrectangle.Height - lh / 2), new Rectangle(0, lh / 2, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Vector2(dialogrectangle.X + dialogrectangle.Width - lw / 2, dialogrectangle.Y + dialogrectangle.Height - lh / 2), new Rectangle(lw / 2, lh / 2, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                        spriteBatch.Draw(line, new Rectangle(dialogrectangle.X, dialogrectangle.Y + lh / 2, lw / 2, dialogrectangle.Height - lh), new Rectangle(0, lh / 2, lw / 2, 0), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Rectangle(dialogrectangle.X + dialogrectangle.Width - lw / 2, dialogrectangle.Y + lh / 2, lw / 2, dialogrectangle.Height - lh), new Rectangle(lw / 2, lh / 2, lw / 2, 0), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Rectangle(dialogrectangle.X + lw / 2, dialogrectangle.Y, dialogrectangle.Width - lw, lh / 2), new Rectangle(lw / 2, 0, 0, lh / 2), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Rectangle(dialogrectangle.X + lw / 2, dialogrectangle.Y + dialogrectangle.Height - lh / 2, dialogrectangle.Width - lw, lh / 2), new Rectangle(lw / 2, lh / 2, 0, lh / 2), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);

                        spriteBatch.Draw(line, new Vector2(dialogHeadOutline.X, dialogHeadOutline.Y), new Rectangle(0, 0, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Vector2(dialogHeadOutline.X + dialogHeadOutline.Width - lw / 2, dialogHeadOutline.Y), new Rectangle(lw / 2, 0, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Vector2(dialogHeadOutline.X, dialogHeadOutline.Y + dialogHeadOutline.Height - lh / 2), new Rectangle(0, lh / 2, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Vector2(dialogHeadOutline.X + dialogHeadOutline.Width - lw / 2, dialogHeadOutline.Y + dialogHeadOutline.Height - lh / 2), new Rectangle(lw / 2, lh / 2, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                        spriteBatch.Draw(line, new Rectangle(dialogHeadOutline.X, dialogHeadOutline.Y + lh / 2, lw / 2, dialogHeadOutline.Height - lh), new Rectangle(0, lh / 2, lw / 2, 0), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Rectangle(dialogHeadOutline.X + dialogHeadOutline.Width - lw / 2, dialogHeadOutline.Y + lh / 2, lw / 2, dialogHeadOutline.Height - lh), new Rectangle(lw / 2, lh / 2, lw / 2, 0), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Rectangle(dialogHeadOutline.X + lw / 2, dialogHeadOutline.Y, dialogHeadOutline.Width - lw, lh / 2), new Rectangle(lw / 2, 0, 0, lh / 2), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Rectangle(dialogHeadOutline.X + lw / 2, dialogHeadOutline.Y + dialogHeadOutline.Height - lh / 2, dialogHeadOutline.Width - lw, lh / 2), new Rectangle(lw / 2, lh / 2, 0, lh / 2), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);

                        spriteBatch.Draw(darkbackground, dialogHead, new Rectangle(0, 0, 128, 128), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                        spriteBatch.Draw(portraits, new Vector2(15, Game.height - 128-15), new Rectangle(128 * dialogSayer, 0, 128, 128), Color.White);
                        spriteBatch.DrawString(font, dialogString, new Vector2(dialogrectangle.X + 15+150, dialogrectangle.Y + 15), Color.White);
                    }
                }
            }
            for (int i = 0; i < elements.Length; i++)
            {
                if ((elements[i].drawstate == GameState.Any || elements[i].drawstate == state)&&elements[i].enable)
                    elements[i].drawFunction(spriteBatch, line, darkbackground, lightbackground, font,ref elements[i]);
            }
        }

        public bool IsFloatMessageLive()
        {
            return floatMessageTime > 0 || floatMessageTime == -1;
        }
        public void AddFloatMessage(string text, float time,float waittime, SpriteFont font)
        {
            floatMessageTime = time;
            floatMessageTimeMax = time;
            floatMessage = text;
            Vector2 v = font.MeasureString(text);
            v.X += 30; v.Y += 30;
            if (time != -1)
                floatMessageRectangle = new Rectangle((int)(Game.width / 2 - 100 - v.X / 2), 0, (int)v.X, (int)v.Y);
            else
                floatMessageRectangle = new Rectangle((int)(Game.width / 2 - 100 - v.X / 2), Game.height / 2 - (int)v.Y / 2, (int)v.X, (int)v.Y);

            stoppedmessage = 0;
            waitmessagetime = waittime;
        }
        public void AddFloatMessage(string text, float time, SpriteFont font)
        {
            floatMessageTime = time;
            floatMessageTimeMax = time;
            floatMessage = text;
            Vector2 v = font.MeasureString(text);
            v.X += 30; v.Y += 30;
            if (time != -1)
                floatMessageRectangle = new Rectangle((int)(Game.width / 2 - 100 - v.X / 2), 0, (int)v.X, (int)v.Y);
            else
                floatMessageRectangle = new Rectangle((int)(Game.width / 2 - 100 - v.X / 2), Game.height / 2 - (int)v.Y / 2, (int)v.X, (int)v.Y);

            stoppedmessage = 0;
            waitmessagetime = 0.5f;
        }
        public void AddDialog(int sayer, string words, SpriteFont font)
        {
            dialogShowed = true;
            dialogSayer = sayer;
            dialogString = words;
            dialogrectangle.Height = (int)(font.MeasureString(dialogString).Y + 30);
            //if (dialogrectangle.Height < 128 + 30) dialogrectangle.Height = 128 + 30;
            dialogrectangle.Y = Game.height - dialogrectangle.Height;
        }
        /*public void AddDialog(int sayer, int words, SpriteFont font)
        {
            dialogShowed = true;
            dialogSayer = sayer;
            dialogString = Game.langManager.dialogs[words];
            dialogrectangle.Height = (int)(font.MeasureString(dialogString).Y + 30);
            //if (dialogrectangle.Height < 128 + 30) dialogrectangle.Height = 128 + 30;
            dialogrectangle.Y = Game.height - dialogrectangle.Height;
        }*/
    }
}
