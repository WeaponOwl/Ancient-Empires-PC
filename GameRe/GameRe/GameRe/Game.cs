using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;

namespace Game
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        static public int width = 800;
        public static int height = 600;
        public static bool fullscreen = false;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        #region Graphics
        Texture2D tileset;
        Texture2D miniset;
        Texture2D unitset;
        Texture2D guiset;
        Texture2D particlesset;
        Texture2D aroundset;
        Texture2D fractionset;
        Texture2D alphaset;
        Texture2D tombset;
        Texture2D pathset;

        Texture2D backgroundlight;
        Texture2D backgrounddark;
        Texture2D cursor;
        Texture2D mapcursor;
        Texture2D outline;
        Texture2D fractionsemblemsset;
        Texture2D logo;
        Texture2D empry;
        Texture2D shield;
        Texture2D portraitsset;

        Texture2D birdset;

        SpriteFont font;
        SpriteFont attackfont;

        public static Effect standartEffect;
        #endregion

        #region Sounds
        Song main1Sound;
        Song main2Sound;
        Song winSound;
        Song introSound;
        #endregion

        Camera camera;
        Map map;
        UnitManager unitManager;
        ParticleSystem particalManager;
        PlayerManager playerManager;
        ScriptManager scriptManager;
        public static LangManager langManager;
        AnimalManager animalManger;

        List<Building> tombs;

        Gui gui;
        public static GameState gameState;
        GameState lastgameState;
        EditorInfo editorInfo;
        public static GameInfo gameInfo;
        bool saveMapWhenBackToMenu;
        bool cursorActive;
        bool updateGame;
        static Color lightColor = Color.White;
        int nowtileset;
        bool cheatmode;
        int nowsound;
        bool music;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IniParser.FileIniDataParser parser = new IniParser.FileIniDataParser();
            IniParser.IniData data = parser.LoadFile("config.ini");
            width = Convert.ToInt32(data["Graphics"]["Width"]);
            height = Convert.ToInt32(data["Graphics"]["Height"]);
            fullscreen = Convert.ToBoolean(data["Graphics"]["Fullscreen"]);
            cheatmode = Convert.ToBoolean(data["GamePlay"]["Cheats"]);
            music = Convert.ToBoolean(data["Music"]["Sounds"]);
            string lang = data["GamePlay"]["Lang"];
            switch (lang)
            {
                case "Eng": langManager = new LangManager("Content/Languages/eng.lang"); 
                            langManager.LoadDialogs("Content/Languages/eng_dialogs.lang"); break;
                case "Rus": langManager = new LangManager("Content/Languages/rus.lang");
                            langManager.LoadDialogs("Content/Languages/rus_dialogs.lang"); break;
                case "Ukr": langManager = new LangManager("Content/Languages/ukr.lang");
                            langManager.LoadDialogs("Content/Languages/ukr_dialogs.lang"); break;
                default: langManager = new LangManager("Content/Languages/eng.lang");
                            langManager.LoadDialogs("Content/Languages/eng_dialogs.lang"); break;
            }

            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            graphics.IsFullScreen = fullscreen;
            graphics.ApplyChanges();
            editorInfo = new EditorInfo();
            gameInfo = new GameInfo();
            saveMapWhenBackToMenu = false;
            playerManager = new PlayerManager();

            map = new Map();
            camera = new Camera(width, height, map.width, map.height);
            unitManager = new UnitManager();
            particalManager = new ParticleSystem();
            scriptManager = new ScriptManager();
            scriptManager.UpdateFunction = UpdateScript;
            gui = new Gui(graphics.GraphicsDevice);
            #region create gui
            gui.elements[0] = (new GuiObject(new Rectangle(width - 200, 199, 200, height - 280), 0, false, false, GameState.Editor, Nothing, StandartGuiDraw));
            gui.elements[1] = (new GuiObject(new Rectangle(width - 200, height - 82, 200, 81), 2, false, false, GameState.Editor, Nothing, StandartGuiDraw));
            gui.elements[2] = (new GuiObject(new Rectangle(width - 200, 0, 200, 200), 1, false, false, GameState.Editor, Nothing, StandartGuiDraw));
            gui.elements[3] = (new GuiObject(new Rectangle(width - 200 + 8, 8, 184, 184), 1, true, false, GameState.Editor, Nothing, DrawMiniMap));
            gui.elements[4] = (new GuiObject(new Rectangle(width - 194, height - 75, 68, 68), 2, true, false, GameState.Editor, Nothing, DrawSelectedId));

            gui.elements[5] = (new GuiObject(new Rectangle(width - 200, 199, 200, height - 280), 0, false, false, GameState.Game, Nothing, StandartGuiDraw));
            gui.elements[6] = (new GuiObject(new Rectangle(width - 200, height - 82, 200, 81), 2, false, false, GameState.Game, Nothing, DrawSelectedPlayerAndTile));
            gui.elements[7] = (new GuiObject(new Rectangle(width - 200, 0, 200, 200), 1, false, false, GameState.Game, Nothing, StandartGuiDraw));
            gui.elements[8] = (new GuiObject(new Rectangle(width - 200 + 8, 8, 184, 184), 1, true, false, GameState.Game, Nothing, DrawMiniMap));
            gui.elements[9] = (new GuiObject(new Rectangle(width - 194, height - 75, 68, 68), 2, true, false, GameState.Game, Nothing, DrawSelectedTile));

            gui.elements[10] = (new GuiObject(new Rectangle(width/2 - 75, height - 38 * 3 - height / 8, 150, 30), -1, false, true, GameState.MainMenu, SimpleGameButton, StandartGuiDraw, "SimpleGameButton", langManager[GameString.SimpleGame]));
            gui.elements[11] = (new GuiObject(new Rectangle(width/2 - 75, height - 38 * 2 - height / 8, 150, 30), -1, false, true, GameState.MainMenu, EditorButton, StandartGuiDraw, "EditorButton", langManager[GameString.Editor]));
            gui.elements[12] = (new GuiObject(new Rectangle(width/2 - 75, height - 38 - height / 8, 150, 30), -1, false, true, GameState.MainMenu, ExitButton, StandartGuiDraw, "ExitButton", langManager[GameString.Quit]));

            gui.elements[13] = (new GuiObject(new Rectangle(width - 8 - 150, height - 38, 150, 30), -1, false, true, GameState.NewLoadList, BackToMenuButton, StandartGuiDraw, "BackButton", langManager[GameString.ToMenu]));
            gui.elements[14] = (new GuiObject(new Rectangle(8, 8, width - 24 - 150, height - 16), -1, false, false, GameState.NewLoadList, LoadList, DrawLoadList, "NewLoadList"));
            gui.elements[15] = (new GuiObject(new Rectangle(width - 8 - 150, 8, 150, 30), -1, false, true, GameState.NewLoadList, CreateMapButton, StandartGuiDraw, "CreateMapButton", langManager[GameString.Create]));
            gui.elements[16] = (new GuiObject(new Rectangle(width - 8 - 150, 46, 70, 30), -1, false, true, GameState.NewLoadList, NewWidthButton, StandartGuiDraw, "NewMapWidth", ""));
            gui.elements[17] = (new GuiObject(new Rectangle(width - 8 - 70, 46, 70, 30), -1, false, true, GameState.NewLoadList, NewHeightButton, StandartGuiDraw, "NewMapHeight", ""));

            gui.elements[18] = (new GuiObject(new Rectangle(width - 200 + 25, 208, 150, 30), 1, true, false, GameState.Editor, SaveButton, StandartGuiDraw, "EditorSaveButton", langManager[GameString.Save]));
            gui.elements[19] = (new GuiObject(new Rectangle(width - 200 + 25, height - 75 - 30 - 12, 150, 30), 1, true, false, GameState.Editor, EditorToMenuButton, StandartGuiDraw, "EditorToMenu", langManager[GameString.ToMenu]));

            gui.elements[20] = (new GuiObject(new Rectangle(width/2 - 75, height - 38 * 5 - height / 8, 150, 30), -1, false, true, GameState.MainMenu, ContinueButton, StandartGuiDraw, "ContinueButton", langManager[GameString.ContinueLast]));

            gui.elements[21] = (new GuiObject(new Rectangle(width - 200 + 25, 246, 70, 30), -1, false, false, GameState.Editor, EditorSetUnitsButton, StandartGuiDraw, "EditorUnits", langManager[GameString.Units]));
            gui.elements[22] = (new GuiObject(new Rectangle(width - 200 + 105, 246, 70, 30), -1, true, false, GameState.Editor, EditorSetTilesButton, StandartGuiDraw, "EditorTiles", langManager[GameString.Tiles]));

            gui.elements[23] = (new GuiObject(new Rectangle(width - 200 + 8, 284, 184, height - 284 - 75 - 48 - 38), -1, true, false, GameState.Editor, EditorIdSeter, DrawIdSeter, "EditorIDSeter"));

            gui.elements[24] = (new GuiObject(new Rectangle(width - 200 + 25, height - 75 - 30 - 12 - 38, 70, 30), -1, true, false, GameState.Editor, EditorBackward, StandartGuiDraw, "EditorIDSeter", langManager[GameString.LeftArrow]));
            gui.elements[25] = (new GuiObject(new Rectangle(width - 200 + 105, height - 75 - 30 - 12 - 38, 70, 30), -1, true, false, GameState.Editor, EditorForward, StandartGuiDraw, "EditorIDSeter", langManager[GameString.RightArrow]));

            gui.elements[26] = (new GuiObject(new Rectangle(0, 0, width - 200, height), -1, true, false, GameState.Editor, EditorSeter, DrawNothing, "EditorIDSeter"));

            gui.elements[27] = (new GuiObject(new Rectangle(0, 0, width - 200, height), -1, true, false, GameState.Game, GameMain, DrawNothing));

            gui.elements[28] = (new GuiObject(new Rectangle(width - 8 - 150, height - 38, 150, 30), -1, false, true, GameState.LoadList, BackToMenuButton, StandartGuiDraw, "BackButton", langManager[GameString.ToMenu]));
            gui.elements[29] = (new GuiObject(new Rectangle(8, 8, width - 24 - 150, height - 16), -1, false, false, GameState.LoadList, GameLoadList, DrawLoadList, "LoadList"));
            gui.elements[30] = (new GuiObject(new Rectangle(width - 8 - 150, 8, 150, 30), -1, false, true, GameState.LoadList, LoadMapButton, StandartGuiDraw, "LoadMapButton", langManager[GameString.Load]));

            gui.elements[31] = (new GuiObject(new Rectangle(0, 0, 0, 0), -1, false, true, GameState.Game, SubMenuUpdate, DrawSubMenu));

            gui.elements[32] = (new GuiObject(new Rectangle(width - 200 + 25, height - 75 - 30 - 12 - 30, 150, 30), 1, true, false, GameState.Game, GameToMenuButton, StandartGuiDraw, "GameToMenu", langManager[GameString.ToMenu]));
            gui.elements[33] = (new GuiObject(new Rectangle(width - 200 + 25, height - 75 - 30 - 12 - 64, 150, 30), 1, true, false, GameState.Game, EndTurnButton, StandartGuiDraw, "GameEndTurn", langManager[GameString.EndTurn]));

            gui.elements[34] = (new GuiObject(new Rectangle(width - 192, 208, 68, 68), 2, true, false, GameState.Game, Nothing, DrawSelectedUnit));

            gui.elements[35] = (new GuiObject(new Rectangle(10, 10, width - 220, height - 20), 2, false, true, GameState.Game, BuyMenu, DrawBuyMenu, "BuyMenu"));
            gui.elements[36] = (new GuiObject(new Rectangle(width - 220 - 158, height - 192, 150, 30), 2, true, false, GameState.Game, BuyMenuButtonForward, DrawBuyMenuButton, "BuyForward", langManager[GameString.RightArrow]));
            gui.elements[37] = (new GuiObject(new Rectangle(18, height - 192, 150, 30), 2, true, false, GameState.Game, BuyMenuButtonBackward, DrawBuyMenuButton, "BuyBackward", langManager[GameString.LeftArrow]));
            gui.elements[38] = (new GuiObject(new Rectangle(width - 220 - 158, height - 58, 150, 30), 2, true, false, GameState.Game, BuyMenuButtonOk, DrawBuyMenuButton, "BuyOk", langManager[GameString.Buy]));
            gui.elements[39] = (new GuiObject(new Rectangle(18, height - 58, 150, 30), 2, true, false, GameState.Game, BuyMenuButtonCansel, DrawBuyMenuButton, "BuyCancel", langManager[GameString.Cansel]));

            gui.elements[40] = (new GuiObject(new Rectangle(0, 208, 68, 68), 2, true, false, GameState.Game, Nothing, DrawSelectedUnit2));

            gui.elements[41] = (new GuiObject(new Rectangle(width - 8 - 150, 50 + 8, 150, 30), -1, false, true, GameState.LoadList, Player1AIChanche, StandartGuiDraw));
            gui.elements[42] = (new GuiObject(new Rectangle(width - 8 - 150, 50 + 46, 150, 30), -1, false, true, GameState.LoadList, Player1TeamChanche, StandartGuiDraw));
            gui.elements[43] = (new GuiObject(new Rectangle(width - 8 - 150, 50 + 46 + 38, 150, 30), -1, false, true, GameState.LoadList, Player2AIChanche, StandartGuiDraw));
            gui.elements[44] = (new GuiObject(new Rectangle(width - 8 - 150, 50 + 46 + 38 * 2, 150, 30), -1, false, true, GameState.LoadList, Player2TeamChanche, StandartGuiDraw));
            gui.elements[45] = (new GuiObject(new Rectangle(width - 8 - 150, 50 + 46 + 38 * 3, 150, 30), -1, false, true, GameState.LoadList, Player3AIChanche, StandartGuiDraw));
            gui.elements[46] = (new GuiObject(new Rectangle(width - 8 - 150, 50 + 46 + 38 * 4, 150, 30), -1, false, true, GameState.LoadList, Player3TeamChanche, StandartGuiDraw));
            gui.elements[47] = (new GuiObject(new Rectangle(width - 8 - 150, 50 + 46 + 38 * 5, 150, 30), -1, false, true, GameState.LoadList, Player4AIChanche, StandartGuiDraw));
            gui.elements[48] = (new GuiObject(new Rectangle(width - 8 - 150, 50 + 46 + 38 * 6, 150, 30), -1, false, true, GameState.LoadList, Player4TeamChanche, StandartGuiDraw));

            gui.elements[49] = (new GuiObject(new Rectangle(width - 8 - 150, 50 + 46 + 38 * 8, 150, 30), -1, false, true, GameState.LoadList, ChancheMoney, StandartGuiDraw));

            gui.elements[50] = (new GuiObject(new Rectangle(width - 8 - 150, height - 38 * 2, 70, 30), -1, false, true, GameState.NewLoadList, LoadListBackward, StandartGuiDraw, "", langManager[GameString.LeftArrow]));
            gui.elements[51] = (new GuiObject(new Rectangle(width - 8 - 70, height - 38 * 2, 70, 30), -1, false, true, GameState.NewLoadList, LoadListForward, StandartGuiDraw, "", langManager[GameString.RightArrow]));
            gui.elements[52] = (new GuiObject(new Rectangle(width - 8 - 150, height - 38 * 2, 70, 30), -1, false, true, GameState.LoadList, LoadListBackward, StandartGuiDraw, "", langManager[GameString.LeftArrow]));
            gui.elements[53] = (new GuiObject(new Rectangle(width - 8 - 70, height - 38 * 2, 70, 30), -1, false, true, GameState.LoadList, LoadListForward, StandartGuiDraw, "", langManager[GameString.RightArrow]));

            gui.elements[54] = (new GuiObject(new Rectangle(width/2 - 75, height - 38 * 4 - height / 8, 150, 30), -1, false, true, GameState.MainMenu, LoadSavedGameButton, StandartGuiDraw, "SimpleLoadButton", langManager[GameString.LoadGame]));
            gui.elements[55] = (new GuiObject(new Rectangle(width - 200 + 25, height - 75 - 30 - 12 - 64 - 40, 150, 30), 1, true, false, GameState.Game, SaveGame, StandartGuiDraw, "GameSaveButton", langManager[GameString.SaveGame]));
            gui.elements[56] = (new GuiObject(new Rectangle(8, 8, width - 24 - 150, height - 16), 1, true, false, GameState.LoadSavesList, LoadSavedList, DrawLoadList));

            gui.elements[57] = (new GuiObject(new Rectangle(width - 8 - 150, height - 38 * 2, 70, 30), -1, false, true, GameState.LoadSavesList, LoadListBackward, StandartGuiDraw, "", langManager[GameString.LeftArrow]));
            gui.elements[58] = (new GuiObject(new Rectangle(width - 8 - 70, height - 38 * 2, 70, 30), -1, false, true, GameState.LoadSavesList, LoadListForward, StandartGuiDraw, "", langManager[GameString.RightArrow]));
            gui.elements[59] = (new GuiObject(new Rectangle(width - 8 - 150, height - 38, 150, 30), -1, false, true, GameState.LoadSavesList, BackToMenuButton, StandartGuiDraw, "BackButton", langManager[GameString.ToMenu]));

            gui.elements[60] = (new GuiObject(new Rectangle(width - 8 - 150, 50 + 46 + 38 * 9, 150, 30), -1, false, true, GameState.LoadList, ChancheTileset, StandartGuiDraw));
            gui.elements[61] = (new GuiObject(new Rectangle(width - 8 - 150, 50 + 46 + 38 * 9, 150, 30), -1, false, true, GameState.NewLoadList, ChancheTileset, StandartGuiDraw));

            gui.elements[62] = (new GuiObject(new Rectangle(width / 2 - 75 - 154, height - 38 * 4 - height / 8, 150, 30), -1, false, true, GameState.MainMenu, StoryOneButton, StandartGuiDraw, "SimpleGameButton", langManager[GameString.Story1]));
            gui.elements[63] = (new GuiObject(new Rectangle(width / 2 - 75 - 154, height - 38 * 3 - height / 8, 150, 30), -1, false, true, GameState.MainMenu, StoryTwoButton, StandartGuiDraw, "SimpleGameButton", langManager[GameString.Story2]));

            gui.elements[64] = (new GuiObject(new Rectangle(width - 30, height-30, 22, 22), -1, false, true, GameState.MainMenu, Nothing, DrawAbout, "", "?"));
            #endregion
            gameState = GameState.MainMenu;
            lastgameState = GameState.MainMenu;

            tombs = new List<Building>();

            nowtileset = 0;
            nowsound = 0;

            animalManger = new AnimalManager();

            //soundInstance = new SoundEffectInstance();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            tileset = Content.Load<Texture2D>("Textures/tileset");
            miniset = Content.Load<Texture2D>("Textures/minitileset");
            unitset = Content.Load<Texture2D>("Textures/human units");
            guiset = Content.Load<Texture2D>("Gui/gui");
            aroundset = Content.Load<Texture2D>("Gui/mapoutline");
            alphaset = Content.Load<Texture2D>("Gui/alpha");
            particlesset = Content.Load<Texture2D>("Textures/particles");
            fractionset = Content.Load<Texture2D>("Textures/fractions");
            font = Content.Load<SpriteFont>("Fonts/debugfont");
            attackfont = Content.Load<SpriteFont>("Fonts/attackfont");
            standartEffect = Content.Load<Effect>("Effects/standart");
            mapcursor = Content.Load<Texture2D>("Gui/mapcursor");
            pathset = Content.Load<Texture2D>("Gui/path");
            logo = Content.Load<Texture2D>("Gui/logo");
            tombset = Content.Load<Texture2D>("Textures/tomb");
            birdset = Content.Load<Texture2D>("Textures/birds");
            portraitsset = Content.Load<Texture2D>("Textures/portraits");
            empry = new Texture2D(graphics.GraphicsDevice, 1, 1);
            empry.SetData<Color>(new Color[] { Color.White });
            Texture2D temp = Content.Load<Texture2D>("Textures/fractionsemblems");
            fractionsemblemsset = new Texture2D(graphics.GraphicsDevice, 1024, 256);
            Color[] a = new Color[temp.Width * temp.Height];
            temp.GetData<Color>(a);
            fractionsemblemsset.SetData<Color>(0, new Rectangle(0, 0, temp.Width, temp.Height), a, 0, temp.Width * temp.Height);

            backgroundlight = new Texture2D(graphics.GraphicsDevice, 16, 16);
            backgrounddark = new Texture2D(graphics.GraphicsDevice, 16, 16);
            cursor = new Texture2D(graphics.GraphicsDevice, 16, 16);
            outline = new Texture2D(graphics.GraphicsDevice, 16, 16);
            Color[] array = new Color[16 * 16];
            guiset.GetData<Color>(0, new Rectangle(0, 0, 16, 16), array, 0, 16 * 16);
            backgroundlight.SetData<Color>(array);
            guiset.GetData<Color>(0, new Rectangle(0, 16, 16, 16), array, 0, 16*16);
            backgrounddark.SetData<Color>(array);
            guiset.GetData<Color>(0, new Rectangle(16, 0, 16, 16), array, 0, 16*16);
            outline.SetData<Color>(array);
            array = new Color[16 * 16];
            guiset.GetData<Color>(0, new Rectangle(35, 19, 16, 16), array, 0, 16 * 16);
            cursor.SetData<Color>(array);
            array = new Color[16 * 16];
            particlesset.GetData<Color>(0, new Rectangle(342, 128, 16, 16), array, 0, 16 * 16);
            shield = new Texture2D(graphics.GraphicsDevice, 16, 16);
            shield.SetData<Color>(array);

            if (music)
            {
                winSound = Content.Load<Song>("Music/win");
                main1Sound = Content.Load<Song>("Music/main1");
                main2Sound = Content.Load<Song>("Music/main2");
                introSound = Content.Load<Song>("Music/intro");

                MediaPlayer.Play(introSound);
            }

            gui.names = Directory.GetFiles(Content.RootDirectory + "/Maps/", "*.am").ToList();
            gui.selectedNameId = new Random().Next(gui.names.Count-1);
            gui.selectedName = gui.names[gui.selectedNameId];
            LoadGame(gui.selectedName);
            lightColor = new Color(25,25,25);
            nowtileset = new Random().Next(2);
            LoadTileset();
            this.Window.Title = "Ancient empires" + (cheatmode ? " : cheatmode" : "");
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            camera.Update(Keyboard.GetState(), map, Mouse.GetState(), gameState);
            gui.Update(camera.mousestate, gameState,gameTime);
            particalManager.Update(gameTime);
            Timer.Update(gameTime);

            updateGame = scriptManager.Update(gameTime,ref scriptManager);

            animalManger.Update(camera);
            if (new Random().Next(2500) == 0) animalManger.birds.Add(new Bird());

            #region SoundUpdate
            if (music && MediaPlayer.State == MediaState.Stopped)
            {
                nowsound++;
                nowsound %= 2;

                switch (nowsound)
                {
                    case 1: MediaPlayer.Play(main2Sound); break;
                    default: MediaPlayer.Play(main1Sound); break;

                }
            }
            #endregion

            System.GC.Collect(1);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone,standartEffect);

            if (gameState == GameState.Game || gameState == GameState.Editor)
            {
                spriteBatch.Draw(backgroundlight, new Vector2(width - 200, 0), new Rectangle(0, 0, 200, height), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.0000000000001f);
                spriteBatch.Draw(backgrounddark, Vector2.Zero, new Rectangle(0, 0, width - 200, height), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1.0f);
                if (gameState == GameState.Editor)
                {
                    spriteBatch.DrawString(font, Game.langManager[GameString.Position] + camera.positionOnMap.ToString(), new Vector2(1, 1), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                }
            }
            else
                spriteBatch.Draw(backgrounddark, Vector2.Zero, new Rectangle(0, 0, width, height), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1.0f);
            gui.Draw(spriteBatch, font, outline, backgrounddark, backgroundlight,portraitsset, gameState);

            if (gameState == GameState.MainMenu)
            {
                spriteBatch.DrawString(font, gameInfo.halloWorld, new Vector2(width/2 +logo.Width/2, (height / 2 - 110 + 50 + logo.Height) / 2-50+height/8), Color.White, 0.2f, font.MeasureString(gameInfo.halloWorld) / 2, 1 + (float)Math.Abs(Math.Sin(Timer.Time*2)+1)/2, SpriteEffects.None, 0.0001f);
            }

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone,standartEffect);

            if (gameState == GameState.Game || gameState == GameState.Editor)
            {
                unitManager.Draw(spriteBatch, attackfont, camera, unitset, fractionset, map,lightColor);
                map.Draw(spriteBatch, camera, tileset, fractionset, alphaset, ref particalManager,lightColor);
                map.DrawOutLine(spriteBatch, camera, aroundset);

                /*for (int i = 0; i < map.height; i++)
                    for (int j = 0; j < map.width; j++)
                    {
                        spriteBatch.DrawString(font, map.alphaData[i, j].ToString(), new Vector2(j, i)*64 - camera.position + camera.offset, Color.White,0,Vector2.Zero,1,SpriteEffects.None,0);
                    }*/


                particalManager.Draw(spriteBatch, attackfont, camera, particlesset, lightColor);
                foreach (Building t in tombs)
                {
                    bool inwater = Map.TILES_IN_TERRAIN[map.mapData[(int)t.y, (int)t.x]] == 5;
                    spriteBatch.Draw(tombset, new Vector2(t.x * 64, t.y * 64) - camera.position + camera.offset, new Rectangle(0, 0, 64, 64), lightColor, 0, Vector2.Zero, 1, SpriteEffects.None, camera.GetZ(t.y * 64 - camera.position.Y + camera.offset.Y, 4));
                }
                if (camera.positionOnMap.X >= 0 && camera.positionOnMap.Y >= 0 && camera.positionOnMap.X < map.width && camera.positionOnMap.Y < map.height&&gameInfo.mapcursorVisible)
                {
                    if (Timer.GetTimerState(0.5f)) cursorActive = !cursorActive;
                    spriteBatch.Draw(mapcursor, new Vector2((float)camera.positionOnMap.X * 64 - camera.position.X + camera.offset.X - 2, (float)camera.positionOnMap.Y * 64 - camera.position.Y + camera.offset.Y - 2), new Rectangle(68*(cursorActive?0:1),0,68,68), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.0001f);
                }
                #region DrawMovePath
                if (gameInfo.state == GameState2.Move && playerManager.players[gameInfo.nowplayer].state==PlayerState.Player&&gameInfo.path.Count>=1&&!gameInfo.wait&&gameInfo.mapcursorVisible)
                {
                    for (int i = 0; i < gameInfo.path.Count; i++)
                    {
                        int next = 5;
                        int previous = 5;
                        Rectangle r = new Rectangle(0, 0, 0, 0);
                        try
                        {
                            if (gameInfo.path[i - 1].X > gameInfo.path[i].X) previous = 6;
                            if (gameInfo.path[i - 1].X < gameInfo.path[i].X) previous = 4;
                            if (gameInfo.path[i - 1].Y > gameInfo.path[i].Y) previous = 8;
                            if (gameInfo.path[i - 1].Y < gameInfo.path[i].Y) previous = 2;
                        }
                        catch { ;}

                        try
                        {
                            if (gameInfo.path[i + 1].X > gameInfo.path[i].X) next = 6;
                            if (gameInfo.path[i + 1].X < gameInfo.path[i].X) next = 4;
                            if (gameInfo.path[i + 1].Y > gameInfo.path[i].Y) next = 8;
                            if (gameInfo.path[i + 1].Y < gameInfo.path[i].Y) next = 2;
                        }
                        catch { ;}

                        if (previous == 4) r = new Rectangle(128, 192, 64, 64);
                        if (previous == 6) r = new Rectangle(192, 192, 64, 64);
                        if (previous == 8) r = new Rectangle(64, 192, 64, 64);
                        if (previous == 2) r = new Rectangle(0, 192, 64, 64);
                        spriteBatch.Draw(pathset, gameInfo.path[i] * 64 - camera.position + camera.offset, r, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.0001f);

                        if (next == 4) r = new Rectangle(128, 192, 64, 64);
                        if (next == 6) r = new Rectangle(192, 192, 64, 64);
                        if (next == 8) r = new Rectangle(64, 192, 64, 64);
                        if (next == 2) r = new Rectangle(0, 192, 64, 64);
                        spriteBatch.Draw(pathset, gameInfo.path[i] * 64 - camera.position + camera.offset, r, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.0001f);
                    }

                    int prev = 5;
                    Rectangle rec = new Rectangle(0, 0, 0, 0);
                    if(gameInfo.path.Count>0&&gameInfo.selectedUnitId>=0)
                    {
                        if (unitManager[gameInfo.selectedUnitId].position.X > gameInfo.path[0].X) prev = 4;
                        if (unitManager[gameInfo.selectedUnitId].position.X < gameInfo.path[0].X) prev = 6;
                        if (unitManager[gameInfo.selectedUnitId].position.Y > gameInfo.path[0].Y) prev = 2;
                        if (unitManager[gameInfo.selectedUnitId].position.Y < gameInfo.path[0].Y) prev = 8;
                    }
                    if (prev == 4) rec = new Rectangle(128, 192, 64, 64);
                    if (prev == 6) rec = new Rectangle(192, 192, 64, 64);
                    if (prev == 8) rec = new Rectangle(64, 192, 64, 64);
                    if (prev == 2) rec = new Rectangle(0, 192, 64, 64);
                    spriteBatch.Draw(pathset, unitManager[gameInfo.selectedUnitId].position * 64 - camera.position + camera.offset, rec, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.0001f);
                    if (prev == 6) rec = new Rectangle(128, 192, 64, 64);
                    if (prev == 4) rec = new Rectangle(192, 192, 64, 64);
                    if (prev == 2) rec = new Rectangle(64, 192, 64, 64);
                    if (prev == 8) rec = new Rectangle(0, 192, 64, 64);
                    spriteBatch.Draw(pathset, gameInfo.path[0] * 64 - camera.position + camera.offset, rec, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.0001f);
                    spriteBatch.Draw(pathset, new Vector2(camera.positionOnMap.X, camera.positionOnMap.Y) * 64 - camera.position + camera.offset, new Rectangle(0, 64, 64, 64), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.00001f);
                }
                #endregion

                animalManger.Draw(spriteBatch,birdset,lightColor,camera);
            }

            if (gameState == GameState.MainMenu)
            {
                map.Draw(spriteBatch, camera, tileset, fractionset, alphaset, ref particalManager, lightColor,true);
                map.DrawOutLine(spriteBatch, camera, aroundset,true);
                spriteBatch.Draw(logo, new Vector2((width - logo.Width) / 2, (height / 2 - 110 + 50-logo.Height)/2+height/8),null,Color.White,0,Vector2.Zero,1,SpriteEffects.None,0.002f);

            }
            if(gameInfo.cursorVisible)
            spriteBatch.Draw(guiset, camera.mouse, new Rectangle(35, 19, 15, 15), Color.White);

            spriteBatch.End();

            System.GC.Collect(1);

            base.Draw(gameTime);
        }

        #region Gui
        void Nothing(ref GuiObject me) { ;}
        void SimpleGameButton(ref GuiObject me)
        {
            if (me.lclick)
            {
                gui.names = Directory.GetFiles(Content.RootDirectory + "/Maps/", "*.am").ToList();
                if (gui.names.Count > 0)
                {
                    gui.selectedNameId = 0;
                    gui.selectedName = gui.names[gui.selectedNameId];
                }
                gameState = GameState.LoadList;
                //gameInfo = new GameInfo();
            }
        }
        void EditorButton(ref GuiObject me)
        {
            if (me.lclick)
            {
                gui.names = Directory.GetFiles(Content.RootDirectory + "/Maps/", "*.am").ToList();
                if (gui.names.Count > 0)
                {
                    gui.selectedNameId = 0;
                    gui.selectedName = gui.names[gui.selectedNameId];
                }
                gameState = GameState.NewLoadList;
                //gameInfo = new GameInfo();
            }
        }
        void BackToMenuButton(ref GuiObject me)
        {
            if (me.lclick)
            {
                gui.selectedNameId = -1;
                gameState = GameState.MainMenu;
                lightColor = new Color(25, 25, 25);
            }
        }
        void ExitButton(ref GuiObject me) { if (me.lclick)this.Exit(); }
        void CreateMapButton(ref GuiObject me)
        {
            if (me.lclick)
            {
                map = new Map(editorInfo.nw, editorInfo.nh);
                this.Window.Title = map.name;
                camera = new Camera(width, height, editorInfo.nw, editorInfo.nh);
                unitManager.units.Clear();
                tombs.Clear();
                gameState = GameState.Editor;
                lastgameState = gameState;
                LoadTileset();
                lightColor = Color.White;
            }
        }
        void NewWidthButton(ref GuiObject me)
        {
            if (me.lclick)
                editorInfo.nw++;
            if (me.rclick)
                editorInfo.nw--;
            if (editorInfo.nw < 5) editorInfo.nw = 5;
            me.text = editorInfo.nw.ToString();
        }
        void NewHeightButton(ref GuiObject me)
        {
            if (me.lclick)
                editorInfo.nh++;
            if (me.rclick)
                editorInfo.nh--;
            if (editorInfo.nh < 5) editorInfo.nh = 5;
            me.text = editorInfo.nh.ToString();
        }
        void SaveButton(ref GuiObject me)
        {
            if (me.lclick)
            {
                FileStream fs = new FileStream("Content/Maps/" + map.name + ".am", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                BinaryWriter writer = new BinaryWriter(fs);
                writer.Write((float)1.1);//version
                writer.Write(map.width);
                writer.Write(map.height);
                for (int i = 0; i < map.height; i++)
                {
                    for (int j = 0; j < map.width; j++)
                    {
                        writer.Write(map.mapData[i, j]);
                    }
                }
                writer.Write(map.buidingData.Count);
                for (int i = 0; i < map.buidingData.Count; i++)
                {
                    writer.Write(map.buidingData[i].x);
                    writer.Write(map.buidingData[i].y);
                    writer.Write(map.buidingData[i].fraction);
                }
                writer.Write(map.castleData.Count);
                for (int i = 0; i < map.castleData.Count; i++)
                {
                    writer.Write(map.castleData[i].x);
                    writer.Write(map.castleData[i].y);
                    writer.Write(map.castleData[i].fraction);
                }
                writer.Write(unitManager.units.Count);
                if (unitManager.units.Count > 0)
                    for (int i = 0; i < unitManager.units.Count; i++)
                    {
                        writer.Write(unitManager.units[i].id);
                        writer.Write(unitManager.units[i].name);
                        writer.Write(unitManager.units[i].male);
                        writer.Write(unitManager.units[i].lvl);
                        writer.Write(unitManager.units[i].experience);
                        writer.Write(unitManager.units[i].position.X);
                        writer.Write(unitManager.units[i].position.Y);
                        writer.Write(unitManager.units[i].team);
                        writer.Write(unitManager.units[i].baseAttackMin);
                        writer.Write(unitManager.units[i].baseAttackMax);
                        writer.Write(unitManager.units[i].baseDefence);
                        writer.Write(unitManager.units[i].health);
                        writer.Write(unitManager.units[i].baseMoveRange);
                        writer.Write(unitManager.units[i].baseAttackRangeMin);
                        writer.Write(unitManager.units[i].baseAttackRangeMax);
                        writer.Write(unitManager.units[i].bonusAttack);
                        writer.Write(unitManager.units[i].bonusDefence);
                        writer.Write(unitManager.units[i].cost);
                        writer.Write(unitManager.units[i].animationActive);
                        writer.Write(unitManager.units[i].moveEnd);
                        writer.Write(unitManager.units[i].effects.Length);
                        foreach (UnitEffect u in unitManager.units[i].effects) writer.Write((int)u);
                        writer.Write(unitManager.units[i].properties.Length);
                        foreach (UnitProperty u in unitManager.units[i].properties) writer.Write((int)u);
                    }
                writer.Write(nowtileset);
                writer.Close();
            }
        }
        void LoadList(ref GuiObject me)
        {
            if (me.lclick)
            {
                if (gui.names.Count > 0)
                {
                    int h = (int)font.MeasureString(gui.names[0]).Y + 2;
                    int id = (int)(camera.mouse.Y - 11) / h;
                    if (gui.selectedNameId == id)
                    {
                        LoadGame(gui.selectedName);
                        gameState = GameState.Editor;
                    }
                    else
                    {
                        gui.selectedNameId = id;
                        if (gui.selectedNameId < gui.names.Count)
                            gui.selectedName = gui.names[gui.selectedNameId+gameInfo.firstMapName];
                        else gui.selectedNameId = 0;
                    }
                }
            }
        }
        void EditorToMenuButton(ref GuiObject me)
        {
            if (me.lclick)
            {
                saveMapWhenBackToMenu = true;
                gameState = GameState.MainMenu;
                lightColor = new Color(25, 25, 25);
            }
        }
        void GameToMenuButton(ref GuiObject me)
        {
            if (me.lclick)
            {
                saveMapWhenBackToMenu = true;
                lastgameState = gameState;
                gameState = GameState.MainMenu;
                lightColor = new Color(25, 25, 25);
            }
        }
        void ContinueButton(ref GuiObject me)
        {
            if (me.lclick && saveMapWhenBackToMenu)
            {
                gameState = lastgameState;
                lightColor = Color.White;
            }
        }
        void EditorSetUnitsButton(ref GuiObject me)
        {
            if (me.lclick)
            {
                gui.elements[21].darktransparency = true;
                gui.elements[22].darktransparency = false;
                editorInfo.unitMode = true;
            }
        }
        void EditorSetTilesButton(ref GuiObject me)
        {
            if (me.lclick)
            {
                gui.elements[21].darktransparency = false;
                gui.elements[22].darktransparency = true;
                editorInfo.unitMode = false;
            }
        }
        void EditorIdSeter(ref GuiObject me)
        {
            if (me.lclick)
            {
                int x = (int)(camera.mouse.X - me.rect.X), y = (int)(camera.mouse.Y - me.rect.Y);
                if (editorInfo.unitMode)
                {
                    int oldid = editorInfo.id;
                    editorInfo.id = (y / 35) * 5 + (x / 35);
                    if (editorInfo.id > 15) editorInfo.id = oldid;
                    editorInfo.id %= 128;
                }
                else
                {
                    editorInfo.id = (y / 35) * 5 + (x / 35) + editorInfo.startIndexOfTiles;
                    editorInfo.id %= 128;
                }
            }
        }
        void EditorForward(ref GuiObject me)
        {
            if (me.lclick) editorInfo.startIndexOfTiles += 5;
        }
        void EditorBackward(ref GuiObject me)
        {
            if (me.lclick)
            {
                editorInfo.startIndexOfTiles -= 5;
                if (editorInfo.startIndexOfTiles < 0)
                    editorInfo.startIndexOfTiles = 0;
            }
        }
        void EditorSeter(ref GuiObject me)
        {
            if (me.lpressed)
            {
                if (!editorInfo.unitMode && camera.positionOnMap.X < map.width && camera.positionOnMap.Y < map.height && camera.positionOnMap.X >= 0 && camera.positionOnMap.Y >= 0)
                {
                    if (map.GetBuilding(camera.positionOnMap.X, camera.positionOnMap.Y) != null) map.DestroyBuilding(camera.positionOnMap.X, camera.positionOnMap.Y);
                    if (map.GetCastle(camera.positionOnMap.X, camera.positionOnMap.Y) != null) map.DestroyCastle(camera.positionOnMap.X, camera.positionOnMap.Y);

                    if (editorInfo.id == (63+64) || editorInfo.id == (62+64))
                        map.buidingData.Add(new Building(camera.positionOnMap.X, camera.positionOnMap.Y, editorInfo.nowFraction));
                    else if (editorInfo.id == (61+64))
                        map.castleData.Add(new Building(camera.positionOnMap.X, camera.positionOnMap.Y, editorInfo.nowFraction));
                    map.mapData[camera.positionOnMap.Y, camera.positionOnMap.X] = editorInfo.id;
                }
            }
            if (me.lclick)
            {
                if (editorInfo.unitMode && camera.positionOnMap.X < map.width && camera.positionOnMap.Y < map.height && camera.positionOnMap.X >= 0 && camera.positionOnMap.Y >= 0)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        int i = unitManager.units.IndexOf(unitManager.GetByPosition(camera.positionOnMap.X, camera.positionOnMap.Y));
                        if (i >= 0) unitManager.units.RemoveAt(i);
                    }
                    else
                    {
                        int i = unitManager.units.IndexOf(unitManager.GetByPosition(camera.positionOnMap.X, camera.positionOnMap.Y));
                        if (i >= 0) unitManager.units.RemoveAt(i);
                        unitManager.AddUnit(new Unit(editorInfo.id % 16, editorInfo.nowFraction % 5, camera.positionOnMap.X, camera.positionOnMap.Y));
                    }
                }
            }
            if (me.rclick)
            {
                if (!editorInfo.unitMode)
                {
                    int ic, ib;
                    ic = map.castleData.IndexOf(map.GetCastle(camera.positionOnMap.X, camera.positionOnMap.Y));
                    ib = map.buidingData.IndexOf(map.GetVillage(camera.positionOnMap.X, camera.positionOnMap.Y));
                    if (ic >= 0)
                    {
                        map.castleData[ic].fraction++; map.castleData[ic].fraction %= 5;
                    }
                    if (ib >= 0)
                    {
                        map.buidingData[ib].fraction++; map.buidingData[ib].fraction %= 5;
                    }
                }
                else
                {
                    int i = unitManager.units.IndexOf(unitManager.GetByPosition(camera.positionOnMap.X, camera.positionOnMap.Y));
                    if (i >= 0)
                    {
                        unitManager.units[i].team++;
                        unitManager.units[i].team %= 5;
                    }
                }
            }
        }

        void StoryOneButton(ref GuiObject me)
        {
            if (me.lclick)
            {
                LoadGame("Content/Maps/ae1mission1_Regroup.am");
                gameState = GameState.Game;
            }
        }
        void StoryTwoButton(ref GuiObject me)
        {
            if (me.lclick)
            {
                LoadGame("Content/Maps/ae2mission1_Temple raiders.am");
                gameState = GameState.Game;
            }
        }

        void LoadSavedGameButton(ref GuiObject me)
        {
            if (me.lclick)
            {
                gui.names = Directory.GetFiles(Content.RootDirectory + "/Saves/", "*.as").ToList();
                if (gui.names.Count > 0)
                {
                    gui.selectedNameId = 0;
                    gui.selectedName = gui.names[gui.selectedNameId];
                }
                gameState = GameState.LoadSavesList;
                //gameInfo = new GameInfo();
            }
        }
        void LoadSavedList(ref GuiObject me)
        {
            if (me.lclick)
            {
                if (gui.names.Count > 0)
                {
                    int h = (int)font.MeasureString(gui.names[0]).Y + 2;
                    int id = (int)(camera.mouse.Y - 11) / h;
                    if (gui.selectedNameId == id)
                    {
                        LoadSavedGame(gui.selectedName);
                        gameState = GameState.Game;
                    }
                    else
                    {
                        gui.selectedNameId = id;
                        if (gui.selectedNameId < gui.names.Count)
                            gui.selectedName = gui.names[gui.selectedNameId + gameInfo.firstMapName];
                        else gui.selectedNameId = 0;
                    }
                }
            }
        }
        void SaveGame(ref GuiObject me)
        {
            if (me.lclick&&gameInfo.state==GameState2.None&&playerManager.players[gameInfo.nowplayer].state==PlayerState.Player&&gameInfo.submenu.Count==0&&map.counterOfMoveCells()==0)
            {
                string time = DateTime.Now.ToString();
                time = time.Replace('.', '_');
                time = time.Replace(':', '_');
                time = time.Replace(' ', '_');
                FileStream fs = new FileStream("Content/Saves/" + map.name + " " + time.ToString() + ".as", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                BinaryWriter writer = new BinaryWriter(fs);
                writer.Write((float)1.1);//version
                writer.Write(map.name);

                writer.Write(map.buidingData.Count);
                foreach (Building b in map.buidingData)
                {
                    writer.Write(b.fraction);
                    writer.Write(b.x);
                    writer.Write(b.y);
                    writer.Write(map.mapData[b.y, b.x]);
                }
                writer.Write(map.castleData.Count);
                foreach (Building b in map.castleData)
                {
                    writer.Write(b.fraction);
                    writer.Write(b.x);
                    writer.Write(b.y);
                }

                writer.Write(unitManager.units.Count);
                if (unitManager.units.Count > 0)
                    for (int i = 0; i < unitManager.units.Count; i++)
                    {
                        writer.Write(unitManager.units[i].id);
                        writer.Write(unitManager.units[i].name);
                        writer.Write(unitManager.units[i].male);
                        writer.Write(unitManager.units[i].lvl);
                        writer.Write(unitManager.units[i].experience);
                        writer.Write(unitManager.units[i].position.X);
                        writer.Write(unitManager.units[i].position.Y);
                        writer.Write(unitManager.units[i].team);
                        writer.Write(unitManager.units[i].baseAttackMin);
                        writer.Write(unitManager.units[i].baseAttackMax);
                        writer.Write(unitManager.units[i].baseDefence);
                        writer.Write(unitManager.units[i].health);
                        writer.Write(unitManager.units[i].baseMoveRange);
                        writer.Write(unitManager.units[i].baseAttackRangeMin);
                        writer.Write(unitManager.units[i].baseAttackRangeMax);
                        writer.Write(unitManager.units[i].bonusAttack);
                        writer.Write(unitManager.units[i].bonusDefence);
                        writer.Write(unitManager.units[i].cost);
                        writer.Write(unitManager.units[i].animationActive);
                        writer.Write(unitManager.units[i].moveEnd);
                        writer.Write(unitManager.units[i].effects.Length);
                        foreach (UnitEffect u in unitManager.units[i].effects) writer.Write((int)u);
                        writer.Write(unitManager.units[i].properties.Length);
                        foreach (UnitProperty u in unitManager.units[i].properties) writer.Write((int)u);
                    }

                writer.Write(scriptManager.gameStarted);
                writer.Write(scriptManager.nextstring);
                writer.Write(scriptManager.nowcase);
                writer.Write(scriptManager.nowcasestartstring);
                writer.Write(scriptManager.nowstring);
                writer.Write(scriptManager.waitTime);

                writer.Write(gameInfo.alooweUnit);
                writer.Write(gameInfo.cursorVisible);
                writer.Write(gameInfo.mapcursorVisible);
                writer.Write(gameInfo.nowplayer);
                writer.Write(gameInfo.nowturn);
                writer.Write(gameInfo.onlyUpdateUnits);
                writer.Write(gameInfo.mainFraction);

                writer.Write(lightColor.ToVector4().W);
                writer.Write(lightColor.ToVector4().X);
                writer.Write(lightColor.ToVector4().Y);
                writer.Write(lightColor.ToVector4().Z);

                writer.Write((int)gameInfo.playersStates[0]);
                writer.Write((int)gameInfo.playersStates[1]);
                writer.Write((int)gameInfo.playersStates[2]);
                writer.Write((int)gameInfo.playersStates[3]);
                writer.Write((int)gameInfo.playersStates[4]);
                writer.Write(gameInfo.playersTeams[0]);
                writer.Write(gameInfo.playersTeams[1]);
                writer.Write(gameInfo.playersTeams[2]);
                writer.Write(gameInfo.playersTeams[3]);
                writer.Write(gameInfo.playersTeams[4]);
                writer.Write(gameInfo.wait);

                for (int i = 0; i < 5; i++)
                {
                    writer.Write(playerManager.players[0].castlenum);
                    writer.Write(playerManager.players[0].fraction);
                    writer.Write(playerManager.players[0].gold);
                    writer.Write((int)playerManager.players[0].state);
                    writer.Write(playerManager.players[0].team);
                    writer.Write(playerManager.players[0].unitnum);
                    writer.Write(playerManager.players[0].villagenum);
                }
                writer.Write(nowtileset);
                writer.Close();
                gui.AddFloatMessage(Game.langManager[GameString.GameSaved], 0.5f, 0.5f, font);
            }
        }
        void LoadSavedGame(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader reader = new BinaryReader(fs);
            float version = reader.ReadSingle();
            if (version == 1.1f)
            {
                map = new Map();
                map.name = reader.ReadString();
                LoadGame("Content/Maps/"+map.name+".am");

                int bc = reader.ReadInt32();
                map.buidingData.Clear();
                for (int i = 0; i < bc; i++)
                {
                    int fr = reader.ReadInt32();
                    int x = reader.ReadInt32();
                    int y = reader.ReadInt32();
                    map.buidingData.Add(new Building(x, y, fr));
                    map.mapData[y, x] = reader.ReadInt32();
                }

                int cc = reader.ReadInt32();
                map.castleData.Clear();
                for (int i = 0; i < cc; i++)
                {
                    int fr = reader.ReadInt32();
                    int x = reader.ReadInt32();
                    int y = reader.ReadInt32();
                    map.castleData.Add(new Building(x, y, fr));
                }

                int uc = reader.ReadInt32();
                unitManager.units.Clear();
                if (uc > 0)
                {
                    for (int i = 0; i < uc; i++)
                    {
                        unitManager.AddUnit(new Unit());
                        unitManager.units[i].id = reader.ReadInt32();
                        unitManager.units[i].name = reader.ReadString();
                        unitManager.units[i].male = reader.ReadBoolean();
                        unitManager.units[i].lvl = reader.ReadInt32();
                        unitManager.units[i].experience = reader.ReadInt32();
                        unitManager.units[i].position.X = reader.ReadSingle();
                        unitManager.units[i].position.Y = reader.ReadSingle();
                        unitManager.units[i].team = reader.ReadInt32();
                        unitManager.units[i].baseAttackMin = reader.ReadInt32();
                        unitManager.units[i].baseAttackMax = reader.ReadInt32();
                        unitManager.units[i].baseDefence = reader.ReadInt32();
                        unitManager.units[i].health = reader.ReadInt32();
                        unitManager.units[i].baseMoveRange = reader.ReadInt32();
                        unitManager.units[i].baseAttackRangeMin = reader.ReadInt32();
                        unitManager.units[i].baseAttackRangeMax = reader.ReadInt32();
                        unitManager.units[i].bonusAttack = reader.ReadInt32();
                        unitManager.units[i].bonusDefence = reader.ReadInt32();
                        unitManager.units[i].cost = reader.ReadInt32();
                        unitManager.units[i].animationActive = reader.ReadBoolean();
                        unitManager.units[i].moveEnd = reader.ReadBoolean();
                        int ec = reader.ReadInt32();
                        unitManager.units[i].effects = new UnitEffect[ec];
                        for (int j = 0; j < ec; j++)
                            unitManager.units[i].effects[j] = (UnitEffect)reader.ReadInt32();//writer.Write((int)u);
                        int pc = reader.ReadInt32();
                        unitManager.units[i].properties = new UnitProperty[pc];
                        for (int j = 0; j < pc; j++)
                            unitManager.units[i].properties[j] = (UnitProperty)reader.ReadInt32();//writer.Write((int)u);
                        if (unitManager.units[i].team == 1) camera.ToPositionOnMap((int)unitManager.units[i].position.X, (int)unitManager.units[i].position.Y);

                    }
                }


                scriptManager.gameStarted = reader.ReadBoolean();
                scriptManager.nextstring = reader.ReadInt32();
                scriptManager.nowcase = reader.ReadInt32();
                scriptManager.nowcasestartstring = reader.ReadInt32();
                scriptManager.nowstring = reader.ReadInt32();
                scriptManager.waitTime = reader.ReadSingle();

                gameInfo.alooweUnit = reader.ReadInt32();
                gameInfo.cursorVisible = reader.ReadBoolean();
                gameInfo.mapcursorVisible = reader.ReadBoolean();
                gameInfo.nowplayer = reader.ReadInt32();
                gameInfo.nowturn = reader.ReadInt32();
                gameInfo.onlyUpdateUnits = reader.ReadBoolean();
                gameInfo.mainFraction = reader.ReadInt32();

                Vector4 v = new Vector4();
                v.W = reader.ReadSingle();
                v.X = reader.ReadSingle();
                v.Y = reader.ReadSingle();
                v.Z = reader.ReadSingle();
                lightColor = new Color(v);

                gameInfo.playersStates[0] = (PlayerState)reader.ReadInt32();
                gameInfo.playersStates[1] = (PlayerState)reader.ReadInt32();
                gameInfo.playersStates[2] = (PlayerState)reader.ReadInt32();
                gameInfo.playersStates[3] = (PlayerState)reader.ReadInt32();
                gameInfo.playersStates[4] = (PlayerState)reader.ReadInt32();
                gameInfo.playersTeams[0] = reader.ReadInt32();
                gameInfo.playersTeams[1] = reader.ReadInt32();
                gameInfo.playersTeams[2] = reader.ReadInt32();
                gameInfo.playersTeams[3] = reader.ReadInt32();
                gameInfo.playersTeams[4] = reader.ReadInt32();
                gameInfo.wait = reader.ReadBoolean();

                playerManager.SetPlayers(map, unitManager, 0, gameInfo.playersTeams, gameInfo.playersStates);

                for (int i = 0; i < 5; i++)
                {
                    playerManager.players[0].castlenum = reader.ReadInt32();
                    playerManager.players[0].fraction = reader.ReadInt32();
                    playerManager.players[0].gold = reader.ReadInt32();
                    playerManager.players[0].state = (PlayerState)reader.ReadInt32();
                    playerManager.players[0].team = reader.ReadInt32();
                    playerManager.players[0].unitnum = reader.ReadInt32();
                    playerManager.players[0].villagenum = reader.ReadInt32();
                }
            }

            //nowtileset = 0;
            try
            {
                nowtileset = reader.ReadInt32();
            }
            catch { ;}
            LoadTileset();

            reader.Close();

            if(music)
            MediaPlayer.Play(main1Sound);
        }

        void StandartGuiDraw(SpriteBatch spriteBatch, Texture2D line, Texture2D darkbackground, Texture2D lightbackground, SpriteFont font, ref GuiObject me)
        {
            if (me.darktransparency)
                spriteBatch.Draw(darkbackground, new Vector2(me.rect.X, me.rect.Y), new Rectangle(0, 0, me.rect.Width, me.rect.Height), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            if (me.lighttransparency)
                spriteBatch.Draw(lightbackground, new Vector2(me.rect.X, me.rect.Y), new Rectangle(0, 0, me.rect.Width, me.rect.Height), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            int lw = line.Width, lh = line.Height;
            spriteBatch.Draw(line, new Vector2(me.rect.X, me.rect.Y), new Rectangle(0, 0, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(line, new Vector2(me.rect.X + me.rect.Width - lw / 2, me.rect.Y), new Rectangle(lw / 2, 0, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(line, new Vector2(me.rect.X, me.rect.Y + me.rect.Height - lh / 2), new Rectangle(0, lh / 2, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(line, new Vector2(me.rect.X + me.rect.Width - lw / 2, me.rect.Y + me.rect.Height - lh / 2), new Rectangle(lw / 2, lh / 2, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            spriteBatch.Draw(line, new Rectangle(me.rect.X, me.rect.Y + lh / 2, lw / 2, me.rect.Height - lh), new Rectangle(0, lh / 2, lw / 2, 0), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(line, new Rectangle(me.rect.X + me.rect.Width - lw / 2, me.rect.Y + lh / 2, lw / 2, me.rect.Height - lh), new Rectangle(lw / 2, lh / 2, lw / 2, 0), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(line, new Rectangle(me.rect.X + lw / 2, me.rect.Y, me.rect.Width - lw, lh / 2), new Rectangle(lw / 2, 0, 0, lh / 2), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(line, new Rectangle(me.rect.X + lw / 2, me.rect.Y + me.rect.Height - lh / 2, me.rect.Width - lw, lh / 2), new Rectangle(lw / 2, lh / 2, 0, lh / 2), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);

            if (me.text != "")
            {
                Vector2 size = font.MeasureString(me.text);
                spriteBatch.DrawString(font, me.text, new Vector2((int)(me.rect.X + me.rect.Width / 2 - size.X / 2), (int)(me.rect.Y + me.rect.Height / 2 - size.Y / 2)), Color.White);
            }
        }
        void DrawAbout(SpriteBatch spriteBatch, Texture2D line, Texture2D darkbackground, Texture2D lightbackground, SpriteFont font, ref GuiObject me)
        {

            if (me.darktransparency)
                spriteBatch.Draw(darkbackground, new Vector2(me.rect.X, me.rect.Y), new Rectangle(0, 0, me.rect.Width, me.rect.Height), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            if (me.lighttransparency)
                spriteBatch.Draw(lightbackground, new Vector2(me.rect.X, me.rect.Y), new Rectangle(0, 0, me.rect.Width, me.rect.Height), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            int lw = line.Width, lh = line.Height;
            spriteBatch.Draw(line, new Vector2(me.rect.X, me.rect.Y), new Rectangle(0, 0, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(line, new Vector2(me.rect.X + me.rect.Width - lw / 2, me.rect.Y), new Rectangle(lw / 2, 0, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(line, new Vector2(me.rect.X, me.rect.Y + me.rect.Height - lh / 2), new Rectangle(0, lh / 2, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(line, new Vector2(me.rect.X + me.rect.Width - lw / 2, me.rect.Y + me.rect.Height - lh / 2), new Rectangle(lw / 2, lh / 2, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            spriteBatch.Draw(line, new Rectangle(me.rect.X, me.rect.Y + lh / 2, lw / 2, me.rect.Height - lh), new Rectangle(0, lh / 2, lw / 2, 0), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(line, new Rectangle(me.rect.X + me.rect.Width - lw / 2, me.rect.Y + lh / 2, lw / 2, me.rect.Height - lh), new Rectangle(lw / 2, lh / 2, lw / 2, 0), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(line, new Rectangle(me.rect.X + lw / 2, me.rect.Y, me.rect.Width - lw, lh / 2), new Rectangle(lw / 2, 0, 0, lh / 2), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(line, new Rectangle(me.rect.X + lw / 2, me.rect.Y + me.rect.Height - lh / 2, me.rect.Width - lw, lh / 2), new Rectangle(lw / 2, lh / 2, 0, lh / 2), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);

            if (me.text != "")
            {
                Vector2 size = font.MeasureString(me.text);
                spriteBatch.DrawString(font, me.text, new Vector2((int)(me.rect.X + me.rect.Width / 2 - size.X / 2), (int)(me.rect.Y + me.rect.Height / 2 - size.Y / 2)), Color.White);
            }
            if (me.undercursor)
            {
                string about = AlignText(langManager.GetString(GameString.About), 200);
                Vector2 v = font.MeasureString(about);
                Rectangle rect = new Rectangle(width - (int)v.X - 8-8, height - 38 - (int)v.Y-8, (int)v.X+16, (int)v.Y+16);
                spriteBatch.Draw(darkbackground, new Vector2(rect.X, rect.Y), new Rectangle(0, 0, rect.Width, rect.Height), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                lw = line.Width; lh = line.Height;
                spriteBatch.Draw(line, new Vector2(rect.X, rect.Y), new Rectangle(0, 0, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                spriteBatch.Draw(line, new Vector2(rect.X + rect.Width - lw / 2, rect.Y), new Rectangle(lw / 2, 0, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                spriteBatch.Draw(line, new Vector2(rect.X, rect.Y + rect.Height - lh / 2), new Rectangle(0, lh / 2, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                spriteBatch.Draw(line, new Vector2(rect.X + rect.Width - lw / 2, rect.Y + rect.Height - lh / 2), new Rectangle(lw / 2, lh / 2, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                spriteBatch.Draw(line, new Rectangle(rect.X, rect.Y + lh / 2, lw / 2, rect.Height - lh), new Rectangle(0, lh / 2, lw / 2, 0), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                spriteBatch.Draw(line, new Rectangle(rect.X + rect.Width - lw / 2, rect.Y + lh / 2, lw / 2, rect.Height - lh), new Rectangle(lw / 2, lh / 2, lw / 2, 0), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                spriteBatch.Draw(line, new Rectangle(rect.X + lw / 2, rect.Y, rect.Width - lw, lh / 2), new Rectangle(lw / 2, 0, 0, lh / 2), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                spriteBatch.Draw(line, new Rectangle(rect.X + lw / 2, rect.Y + rect.Height - lh / 2, rect.Width - lw, lh / 2), new Rectangle(lw / 2, lh / 2, 0, lh / 2), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, about, new Vector2(width - v.X-8, height - 38 - v.Y), Color.White);
            }
        }
        void StandartGuiPanel(SpriteBatch spriteBatch, Texture2D line, Texture2D darkbackground, Texture2D lightbackground, SpriteFont font, ref GuiObject me)
        {
            if (me.darktransparency)
                spriteBatch.Draw(darkbackground, new Vector2(me.rect.X, me.rect.Y), new Rectangle(0, 0, me.rect.Width, me.rect.Height), new Color(255,255,255,100), 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            if (me.lighttransparency)
                spriteBatch.Draw(lightbackground, new Vector2(me.rect.X, me.rect.Y), new Rectangle(0, 0, me.rect.Width, me.rect.Height), new Color(255, 255, 255, 100), 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            int lw = line.Width, lh = line.Height;
            spriteBatch.Draw(line, new Vector2(me.rect.X, me.rect.Y), new Rectangle(0, 0, lw / 2, lh / 2), new Color(255, 255, 255, 100), 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(line, new Vector2(me.rect.X + me.rect.Width - lw / 2, me.rect.Y), new Rectangle(lw / 2, 0, lw / 2, lh / 2), new Color(255, 255, 255, 100), 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(line, new Vector2(me.rect.X, me.rect.Y + me.rect.Height - lh / 2), new Rectangle(0, lh / 2, lw / 2, lh / 2), new Color(255, 255, 255, 100), 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(line, new Vector2(me.rect.X + me.rect.Width - lw / 2, me.rect.Y + me.rect.Height - lh / 2), new Rectangle(lw / 2, lh / 2, lw / 2, lh / 2), new Color(255, 255, 255, 100), 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            spriteBatch.Draw(line, new Rectangle(me.rect.X, me.rect.Y + lh / 2, lw / 2, me.rect.Height - lh), new Rectangle(0, lh / 2, lw / 2, 0), new Color(255, 255, 255, 100), 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(line, new Rectangle(me.rect.X + me.rect.Width - lw / 2, me.rect.Y + lh / 2, lw / 2, me.rect.Height - lh), new Rectangle(lw / 2, lh / 2, lw / 2, 0), new Color(255, 255, 255, 100), 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(line, new Rectangle(me.rect.X + lw / 2, me.rect.Y, me.rect.Width - lw, lh / 2), new Rectangle(lw / 2, 0, 0, lh / 2), new Color(255, 255, 255, 100), 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(line, new Rectangle(me.rect.X + lw / 2, me.rect.Y + me.rect.Height - lh / 2, me.rect.Width - lw, lh / 2), new Rectangle(lw / 2, lh / 2, 0, lh / 2), new Color(255, 255, 255, 100), 0, Vector2.Zero, SpriteEffects.None, 0);
        }
        void DrawLoadList(SpriteBatch spriteBatch, Texture2D line, Texture2D darkbackground, Texture2D lightbackground, SpriteFont font, ref GuiObject me)
        {
            StandartGuiDraw(spriteBatch, line, darkbackground, lightbackground, font, ref me);
            if (gui.names.Count > 0 && (gameState == GameState.NewLoadList || gameState == GameState.LoadList||gameState==GameState.LoadSavesList))
            {
                Vector2 size = font.MeasureString(gui.names[0]);
                float h = size.Y + 2;
                float allh = 0;
                for (int i = gameInfo.firstMapName; i < gui.names.Count; i++)
                {
                    if (allh + h < me.rect.Height)
                    {
                        if (gui.names[i] == gui.selectedName && gui.selectedNameId >= 0)
                            spriteBatch.Draw(lightbackground, new Rectangle(8, (int)((i - gameInfo.firstMapName) * h) + 8 + 2, Game.width - 24 - 150, (int)h), new Rectangle(0, 0, (Game.width - 174), (int)h), Color.White);
                        spriteBatch.DrawString(font, gui.names[i], new Vector2(12, 10 + (i - gameInfo.firstMapName) * h), Color.White);
                        allh += h;
                    }
                }
            }
            //StandartGuiDraw(spriteBatch, line, darkbackground, lightbackground, font, ref me);
        }
        void DrawIdSeter(SpriteBatch spriteBatch, Texture2D line, Texture2D darkbackground, Texture2D lightbackground, SpriteFont font, ref GuiObject me)
        {
            StandartGuiDraw(spriteBatch, line, darkbackground, lightbackground, font, ref me);
            if (editorInfo.unitMode)
            {
                int offcetx = 6, offcety = 6;
                for (int i = 0; i < 16; i++)
                {
                    spriteBatch.Draw(unitset, new Rectangle(35 * (i % 5) + offcetx + me.rect.X, 35 * (i / 5) + offcety + me.rect.Y, 32, 32), new Rectangle(64 * (i % 16), 0, 64, 64), Color.White);
                    if (i > (((me.rect.Height - offcety) / 35) * 5) - 2) break;
                }
            }
            else
            {
                int offcetx = 6, offcety = 6;
                for (int i = 0; i < 64; i++)
                {
                    int id = editorInfo.startIndexOfTiles + i;
                    spriteBatch.Draw(tileset, new Rectangle(35 * (id % 5 - editorInfo.startIndexOfTiles % 5) + offcetx + me.rect.X, 35 * (id / 5 - editorInfo.startIndexOfTiles / 5) + offcety + me.rect.Y, 32, 32), new Rectangle(64 * (id % 8), 64 * (id / 8), 64, 64), Color.White);
                    if (i > (((me.rect.Height - offcety) / 35) * 5) - 2) break;
                }
            }
        }
        void DrawSelectedId(SpriteBatch spriteBatch, Texture2D line, Texture2D darkbackground, Texture2D lightbackground, SpriteFont font, ref GuiObject me)
        {
            StandartGuiDraw(spriteBatch, line, lightbackground, darkbackground, font, ref me);
            if (!editorInfo.unitMode)
            {
                spriteBatch.Draw(tileset, new Rectangle(width - 194 + 2, height - 75 + 2, 64, 64), new Rectangle(64 * (editorInfo.id % 8), 64 * (editorInfo.id / 8), 64, 64), Color.White);
            }
            else
            {
                //int i = editorInfo.id / 8 > 0 ? editorInfo.id + 8 : editorInfo.id;
                spriteBatch.Draw(unitset, new Rectangle(width - 194 + 2, height - 75 + 2, 64, 64), new Rectangle(64 * (editorInfo.id % 16), 0, 64, 64), Color.White); ;
            }
        }
        void DrawNothing(SpriteBatch spriteBatch, Texture2D line, Texture2D darkbackground, Texture2D lightbackground, SpriteFont font, ref GuiObject me) { ;}
        void DrawSubMenu(SpriteBatch spriteBatch, Texture2D line, Texture2D darkbackground, Texture2D lightbackground, SpriteFont font, ref GuiObject me)
        {
            if (gameInfo.submenu.Count > 0)
            {
                StandartGuiDraw(spriteBatch, line, darkbackground, lightbackground, font, ref me);

                Vector2 size = font.MeasureString(gui.names[0]);
                float h = size.Y + 2;

                for (int i = 0; i < gameInfo.submenu.Count; i++)
                {
                    spriteBatch.DrawString(font, gameInfo.submenu[i], new Vector2(gui.elements[31].rect.X + 2, gui.elements[31].rect.Y + i * h + 2), Color.White);
                }
            }
        }
        void DrawSelectedUnit(SpriteBatch spriteBatch, Texture2D line, Texture2D darkbackground, Texture2D lightbackground, SpriteFont font, ref GuiObject me)
        {
            spriteBatch.Draw(fractionsemblemsset, new Vector2(width - 200 + 8, 200 + ((height - 200 - 82) - 184) / 2 - 50), new Rectangle((gameInfo.nowplayer - 1) * 184, 0, 184, 184), new Color(255, 255, 255, 25), 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            if (gameInfo.selectedUnitId >= 0)
            {
                StandartGuiDraw(spriteBatch, line, darkbackground, lightbackground, font, ref me);
                Vector2 v = font.MeasureString(unitManager[gameInfo.selectedUnitId].name);
                v.Y += 2;

                Rectangle rect = new Rectangle(me.rect.X + 68 + 8, me.rect.Y, 108, (int)(v.Y * 7.5f));
                spriteBatch.Draw(darkbackground, new Vector2(rect.X, rect.Y), new Rectangle(0, 0, rect.Width, rect.Height), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                int lw = line.Width, lh = line.Height;
                spriteBatch.Draw(line, new Vector2(rect.X, rect.Y), new Rectangle(0, 0, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                spriteBatch.Draw(line, new Vector2(rect.X + rect.Width - lw / 2, rect.Y), new Rectangle(lw / 2, 0, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                spriteBatch.Draw(line, new Vector2(rect.X, rect.Y + rect.Height - lh / 2), new Rectangle(0, lh / 2, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                spriteBatch.Draw(line, new Vector2(rect.X + rect.Width - lw / 2, rect.Y + rect.Height - lh / 2), new Rectangle(lw / 2, lh / 2, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                spriteBatch.Draw(line, new Rectangle(rect.X, rect.Y + lh / 2, lw / 2, rect.Height - lh), new Rectangle(0, lh / 2, lw / 2, 0), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                spriteBatch.Draw(line, new Rectangle(rect.X + rect.Width - lw / 2, rect.Y + lh / 2, lw / 2, rect.Height - lh), new Rectangle(lw / 2, lh / 2, lw / 2, 0), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                spriteBatch.Draw(line, new Rectangle(rect.X + lw / 2, rect.Y, rect.Width - lw, lh / 2), new Rectangle(lw / 2, 0, 0, lh / 2), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                spriteBatch.Draw(line, new Rectangle(rect.X + lw / 2, rect.Y + rect.Height - lh / 2, rect.Width - lw, lh / 2), new Rectangle(lw / 2, lh / 2, 0, lh / 2), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);

                spriteBatch.Draw(unitset, new Vector2(me.rect.X, me.rect.Y), new Rectangle(unitManager[gameInfo.selectedUnitId].id * 64, (unitManager[gameInfo.selectedUnitId].team * 128), 64, 64), !unitManager[gameInfo.selectedUnitId].moveEnd ? Color.White : Color.Gray, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, unitManager[gameInfo.selectedUnitId].name, new Vector2(me.rect.X + 68 + 12, me.rect.Y), Color.White);
                spriteBatch.DrawString(font, Game.langManager[GameString.Lvl] + " " + unitManager[gameInfo.selectedUnitId].lvl.ToString(), new Vector2(me.rect.X + 68 + 12, me.rect.Y + v.Y), Color.White);
                spriteBatch.DrawString(font, Game.langManager[GameString.Exp] + " " + (unitManager[gameInfo.selectedUnitId].experience / 100).ToString() + "/" + (unitManager[gameInfo.selectedUnitId].GetNextLvlExp() / 100).ToString(), new Vector2(me.rect.X + 68 + 12, me.rect.Y + v.Y * 2), Color.White);
                spriteBatch.DrawString(font, Game.langManager[GameString.Hp] + " " + unitManager[gameInfo.selectedUnitId].health.ToString() + "/100", new Vector2(me.rect.X + 68 + 12, me.rect.Y + v.Y * 3), Color.White);
                spriteBatch.DrawString(font, Game.langManager[GameString.Att] + " " + unitManager[gameInfo.selectedUnitId].baseAttackMin.ToString() + "-" + unitManager[gameInfo.selectedUnitId].baseAttackMax.ToString() + (unitManager[gameInfo.selectedUnitId].bonusAttack == 0 ? "" : "+" + (unitManager[gameInfo.selectedUnitId].bonusAttack.ToString())), new Vector2(me.rect.X + 68 + 12, me.rect.Y + v.Y * 4.5f), Color.White);
                spriteBatch.DrawString(font, Game.langManager[GameString.Def] + " " + unitManager[gameInfo.selectedUnitId].baseDefence.ToString() + (unitManager[gameInfo.selectedUnitId].bonusDefence == 0 ? "" : "+" + (unitManager[gameInfo.selectedUnitId].bonusDefence.ToString())), new Vector2(me.rect.X + 68 + 12, me.rect.Y + v.Y * 5.5f), Color.White);
                spriteBatch.DrawString(font, Game.langManager[GameString.Steps] + " " + unitManager[gameInfo.selectedUnitId].baseMoveRange.ToString(), new Vector2(me.rect.X + 68 + 12, me.rect.Y + v.Y * 6.5f), Color.White);
            }
        }

        void DrawSelectedUnit2(SpriteBatch spriteBatch, Texture2D line, Texture2D darkbackground, Texture2D lightbackground, SpriteFont font, ref GuiObject me)
        {
            if (gameState == GameState.Game)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.X)&&cheatmode&&!gameInfo.wait)
                {
                    int id = unitManager.units.IndexOf(unitManager.GetByPosition(camera.positionOnMap.X, camera.positionOnMap.Y));
                    if (id >= 0 && playerManager.players[gameInfo.nowplayer].state == PlayerState.Player)
                    {
                        playerManager.players[unitManager[id].team].unitnum--;
                        unitManager.units.RemoveAt(id);
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Z) && cheatmode)
                {
                    foreach (Bird b in animalManger.birds)
                    {
                        Rectangle r = new Rectangle((int)b.drawposition.X, (int)(b.drawposition.Y - b.height), (int)b.renderFrame.X, (int)b.renderFrame.Y);
                        if (r.Contains((int)camera.mouse.X, (int)camera.mouse.Y)) b.down = true;
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.I))
                {
                    int id = unitManager.units.IndexOf(unitManager.GetByPosition(camera.positionOnMap.X, camera.positionOnMap.Y));
                    if (id >= 0)
                    {
                        StandartGuiDraw(spriteBatch, line, darkbackground, lightbackground, font, ref me);
                        Vector2 v = font.MeasureString(unitManager[id].name);
                        v.Y += 2;

                        Rectangle rect = new Rectangle(me.rect.X + 68 + 8, me.rect.Y, 108, (int)(v.Y * 7.5f));
                        spriteBatch.Draw(darkbackground, new Vector2(rect.X, rect.Y), new Rectangle(0, 0, rect.Width, rect.Height), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                        int lw = line.Width, lh = line.Height;
                        spriteBatch.Draw(line, new Vector2(rect.X, rect.Y), new Rectangle(0, 0, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Vector2(rect.X + rect.Width - lw / 2, rect.Y), new Rectangle(lw / 2, 0, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Vector2(rect.X, rect.Y + rect.Height - lh / 2), new Rectangle(0, lh / 2, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Vector2(rect.X + rect.Width - lw / 2, rect.Y + rect.Height - lh / 2), new Rectangle(lw / 2, lh / 2, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                        spriteBatch.Draw(line, new Rectangle(rect.X, rect.Y + lh / 2, lw / 2, rect.Height - lh), new Rectangle(0, lh / 2, lw / 2, 0), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Rectangle(rect.X + rect.Width - lw / 2, rect.Y + lh / 2, lw / 2, rect.Height - lh), new Rectangle(lw / 2, lh / 2, lw / 2, 0), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Rectangle(rect.X + lw / 2, rect.Y, rect.Width - lw, lh / 2), new Rectangle(lw / 2, 0, 0, lh / 2), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Rectangle(rect.X + lw / 2, rect.Y + rect.Height - lh / 2, rect.Width - lw, lh / 2), new Rectangle(lw / 2, lh / 2, 0, lh / 2), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);

                        spriteBatch.Draw(unitset, new Vector2(me.rect.X, me.rect.Y), new Rectangle(unitManager[id].id * 64, unitManager[id].team * 128, 64, 64), !unitManager[id].moveEnd ? Color.White : Color.Gray, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                        spriteBatch.DrawString(font, unitManager[id].name, new Vector2(me.rect.X + 68 + 12, me.rect.Y), Color.White);
                        spriteBatch.DrawString(font, Game.langManager[GameString.Lvl] + " " + unitManager[id].lvl.ToString(), new Vector2(me.rect.X + 68 + 12, me.rect.Y + v.Y), Color.White);
                        spriteBatch.DrawString(font, Game.langManager[GameString.Exp] + " " + (unitManager[id].experience / 100).ToString() + "/" + (unitManager[id].GetNextLvlExp() / 100).ToString(), new Vector2(me.rect.X + 68 + 12, me.rect.Y + v.Y * 2), Color.White);
                        spriteBatch.DrawString(font, Game.langManager[GameString.Hp] + " " + unitManager[id].health.ToString() + "/100", new Vector2(me.rect.X + 68 + 12, me.rect.Y + v.Y * 3), Color.White);
                        spriteBatch.DrawString(font, Game.langManager[GameString.Att] + " " + unitManager[id].baseAttackMin.ToString() + "-" + unitManager[id].baseAttackMax.ToString() + (unitManager[id].bonusAttack == 0 ? "" : "+" + (unitManager[id].bonusAttack.ToString())), new Vector2(me.rect.X + 68 + 12, me.rect.Y + v.Y * 4.5f), Color.White);
                        spriteBatch.DrawString(font, Game.langManager[GameString.Def] + " " + unitManager[id].baseDefence.ToString() + (unitManager[id].bonusDefence == 0 ? "" : "+" + (unitManager[id].bonusDefence.ToString())), new Vector2(me.rect.X + 68 + 12, me.rect.Y + v.Y * 5.5f), Color.White);
                        spriteBatch.DrawString(font, Game.langManager[GameString.Steps] + " " + unitManager[id].baseMoveRange.ToString(), new Vector2(me.rect.X + 68 + 12, me.rect.Y + v.Y * 6.5f), Color.White);

                        string deck=AlignText(unitManager[id].GetDescription(),400);
                        v = font.MeasureString(deck);
                        rect = new Rectangle(me.rect.X + 192-4, me.rect.Y, (int)v.X+12, (int)v.Y+8);
                        spriteBatch.Draw(darkbackground, new Vector2(rect.X, rect.Y), new Rectangle(0, 0, rect.Width, rect.Height), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                        lw = line.Width; lh = line.Height;
                        spriteBatch.Draw(line, new Vector2(rect.X, rect.Y), new Rectangle(0, 0, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Vector2(rect.X + rect.Width - lw / 2, rect.Y), new Rectangle(lw / 2, 0, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Vector2(rect.X, rect.Y + rect.Height - lh / 2), new Rectangle(0, lh / 2, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Vector2(rect.X + rect.Width - lw / 2, rect.Y + rect.Height - lh / 2), new Rectangle(lw / 2, lh / 2, lw / 2, lh / 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                        spriteBatch.Draw(line, new Rectangle(rect.X, rect.Y + lh / 2, lw / 2, rect.Height - lh), new Rectangle(0, lh / 2, lw / 2, 0), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Rectangle(rect.X + rect.Width - lw / 2, rect.Y + lh / 2, lw / 2, rect.Height - lh), new Rectangle(lw / 2, lh / 2, lw / 2, 0), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Rectangle(rect.X + lw / 2, rect.Y, rect.Width - lw, lh / 2), new Rectangle(lw / 2, 0, 0, lh / 2), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                        spriteBatch.Draw(line, new Rectangle(rect.X + lw / 2, rect.Y + rect.Height - lh / 2, rect.Width - lw, lh / 2), new Rectangle(lw / 2, lh / 2, 0, lh / 2), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font,deck,new Vector2(me.rect.X+192+4,me.rect.Y+4),Color.White);
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.P))
                {
                    int id = unitManager.units.IndexOf(unitManager.GetByPosition(camera.positionOnMap.X, camera.positionOnMap.Y));
                    if (id >= 0 && gameInfo.state == GameState2.None)
                    {
                        unitManager[id].FillAttackRange(ref map, ref unitManager, ref playerManager);
                        gameInfo.state = GameState2.AttackPre;
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.C) && cheatmode)
                {
                    int id = unitManager.units.IndexOf(unitManager.GetByPosition(camera.positionOnMap.X, camera.positionOnMap.Y));
                    if (id >= 0 && gameInfo.state == GameState2.None)
                    {
                        playerManager.players[unitManager[id].team].gold += 5;
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.O))
                {
                    Unit king = unitManager.GetKingOfFraction(gameInfo.nowplayer);
                    if (king != null)
                        camera.ToPositionOnMap((int)king.position.X, (int)king.position.Y);
                }
            }
        }

        void DrawBuyMenuButton(SpriteBatch spriteBatch, Texture2D line, Texture2D darkbackground, Texture2D lightbackground, SpriteFont font, ref GuiObject me)
        {
            if (gameInfo.state == GameState2.Buy)
            {
                StandartGuiDraw(spriteBatch, line, darkbackground, lightbackground, font, ref me);
            }
        }
        void DrawBuyMenu(SpriteBatch spriteBatch, Texture2D line, Texture2D darkbackground, Texture2D lightbackground, SpriteFont font, ref GuiObject me)
        {
            if (gameInfo.state == GameState2.Buy)
            {
                bool liveking = unitManager.IsLiveKingOfFraction(gameInfo.nowplayer);
                StandartGuiDraw(spriteBatch, line, darkbackground, lightbackground, font, ref me);

                spriteBatch.Draw(darkbackground, new Vector2(me.rect.X, me.rect.Y + me.rect.Height - 144), new Rectangle(0, 0, me.rect.Width, 68), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                List<int> id = new List<int>();
                if (!liveking) id.Add(9);
                for (int i = 0; i < 9; i++) if (i <= gameInfo.alooweUnit) id.Add(i);
                int offcetx = (me.rect.Width - id.Count * 64) / 2;
                spriteBatch.Draw(lightbackground, new Vector2(offcetx + me.rect.X + (gameInfo.buyid - gameInfo.buystartid) * 64 + 2, 2 + me.rect.Y + me.rect.Height - 144), new Rectangle(0, 0, 64, 64), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                for (int i = gameInfo.buystartid; i < id.Count && i - gameInfo.buystartid < (width - 220 + gameInfo.buystartid * 64) / 64; i++)
                {
                    try
                    {
                        if (id[i] == 2 || id[i] == 3 || id[i] == 5)
                        {
                            if (id[i] == 2)
                                spriteBatch.Draw(unitset, new Vector2(offcetx + me.rect.X + (i - gameInfo.buystartid) * 64 + 7, me.rect.Y + me.rect.Height - 144 + 7), new Rectangle(13 * 64, 0, 64, 64), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                            if (id[i] == 3)
                                spriteBatch.Draw(unitset, new Vector2(offcetx + me.rect.X + (i - gameInfo.buystartid) * 64 + 7, me.rect.Y + me.rect.Height - 144 + 7), new Rectangle(14 * 64, 0, 64, 64), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                            if (id[i] == 5)
                                spriteBatch.Draw(unitset, new Vector2(offcetx + me.rect.X + (i - gameInfo.buystartid) * 64 + 7, me.rect.Y + me.rect.Height - 144 + 7), new Rectangle(15 * 64, 0, 64, 64), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                            spriteBatch.Draw(unitset, new Vector2(offcetx + me.rect.X + (i - gameInfo.buystartid) * 64 - 7, me.rect.Y + me.rect.Height - 144 - 7), new Rectangle(id[i] * 64, 0, 64, 64), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                        }
                        else
                            spriteBatch.Draw(unitset, new Vector2(offcetx + me.rect.X + (i - gameInfo.buystartid) * 64, me.rect.Y + me.rect.Height - 144), new Rectangle(id[i] * 64, 0, 64, 64), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                    }
                    catch { break; }
                }

                Rectangle r = new Rectangle(me.rect.X + 20, me.rect.Y + 20, me.rect.Width - 40, me.rect.Height - 212);
                spriteBatch.Draw(darkbackground, new Vector2(r.X, r.Y), new Rectangle(0, 0, r.Width, r.Height), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                Vector2 v = font.MeasureString(gameInfo.BuySelectionUnit.name);
                v.Y += 2;
                int offx = 0, offy = (int)(r.Height - v.Y * 5.5f - 20) / 2;
                //id[gameInfo.buyid]
                spriteBatch.Draw(unitset, new Vector2(offx + r.X + 20, offy + r.Y + 20 + (v.Y * 5.5f - 64) / 2), new Rectangle(gameInfo.BuySelectionUnit.id * 64, 0, 64, 64), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, gameInfo.BuySelectionUnit.name, new Vector2(offx + r.X + 104, offy + r.Y + 20), Color.White);
                spriteBatch.DrawString(font, Game.langManager[GameString.Price] + " : " + gameInfo.BuySelectionUnit.cost.ToString() + " gold", new Vector2(offx + r.X + 104, offy + r.Y + 20 + v.Y), Color.White);
                spriteBatch.DrawString(font, Game.langManager[GameString.Attack] + " : " + gameInfo.BuySelectionUnit.baseAttackMin.ToString() + "-" + gameInfo.BuySelectionUnit.baseAttackMax.ToString(), new Vector2(offx + r.X + 104, offy + r.Y + 20 + v.Y * 2.5f), Color.White);
                spriteBatch.DrawString(font, Game.langManager[GameString.Deffence] + " : " + gameInfo.BuySelectionUnit.baseDefence.ToString(), new Vector2(offx + r.X + 104, offy + r.Y + 20 + v.Y * 3.5f), Color.White);
                spriteBatch.DrawString(font, Game.langManager[GameString.Steps] + " : " + gameInfo.BuySelectionUnit.baseMoveRange.ToString(), new Vector2(offx + r.X + 104, offy + r.Y + 20 + v.Y * 4.5f), Color.White);

                string about = AlignText(gameInfo.BuySelectionUnit.GetDescription(),r.Width-370);
                spriteBatch.DrawString(font, about, new Vector2(r.X + 250, (int)(r.Height-font.MeasureString(about).Y)/2), Color.White);
            }
        }
        void DrawSelectedPlayerAndTile(SpriteBatch spriteBatch, Texture2D line, Texture2D darkbackground, Texture2D lightbackground, SpriteFont font, ref GuiObject me)
        {
            StandartGuiDraw(spriteBatch, line, darkbackground, lightbackground, font, ref me);

            Vector2 v = font.MeasureString(playerManager.players[gameInfo.nowplayer].ToString());
            spriteBatch.DrawString(font, langManager.strings[(int)GameString.GoldNum] + ":" + (playerManager.players[gameInfo.nowplayer].state == PlayerState.Player ? playerManager.players[gameInfo.nowplayer].gold.ToString() : Game.langManager[GameString.Unknown]), new Vector2(width - 194 + 68 + 8, height - 75), Color.White);
            spriteBatch.DrawString(font, langManager.strings[(int)GameString.CastlesNum] + ":" + playerManager.players[gameInfo.nowplayer].castlenum.ToString(), new Vector2(width - 194 + 68 + 8, height - 75 + v.Y), Color.White);
            spriteBatch.DrawString(font, langManager.strings[(int)GameString.VillagesNum] + ":" + playerManager.players[gameInfo.nowplayer].villagenum.ToString(), new Vector2(width - 194 + 68 + 8, height - 75 + v.Y * 2), Color.White);
            spriteBatch.DrawString(font, langManager.strings[(int)GameString.UnitsNum] + ":" + playerManager.players[gameInfo.nowplayer].unitnum.ToString(), new Vector2(width - 194 + 68 + 8, height - 75 + v.Y * 3), Color.White);

        }
        void DrawSelectedTile(SpriteBatch spriteBatch, Texture2D line, Texture2D darkbackground, Texture2D lightbackground, SpriteFont font, ref GuiObject me)
        {
            StandartGuiDraw(spriteBatch, line, darkbackground, lightbackground, font, ref me);

            if (camera.positionOnMap.Y > 0 && camera.positionOnMap.X > 0 && camera.positionOnMap.Y < map.height && camera.positionOnMap.X<map.width)
            {
                spriteBatch.Draw(tileset, new Vector2(width - 192, height - 73), new Rectangle(map.mapData[camera.positionOnMap.Y, camera.positionOnMap.X] % 8 * 64, map.mapData[camera.positionOnMap.Y, camera.positionOnMap.X] / 8 * 64, 64, 64), Color.White);
                if (Map.TILES_IN_TERRAIN[map.mapData[camera.positionOnMap.Y, camera.positionOnMap.X]] == 9)
                    spriteBatch.Draw(tileset, new Vector2(width - 192, height - 73 - 64), new Rectangle(map.mapData[camera.positionOnMap.Y, camera.positionOnMap.X] % 8 * 64, map.mapData[camera.positionOnMap.Y, camera.positionOnMap.X] / 8 * 64 - 64, 64, 64), Color.White);
                string t = Map.TERRAIN_DEFENCE_BONUS[Map.TILES_IN_TERRAIN[map.mapData[camera.positionOnMap.Y, camera.positionOnMap.X]]].ToString();
                Vector2 offcet = attackfont.MeasureString(t);
                spriteBatch.DrawString(attackfont, t, new Vector2(width - 192 + 64 - offcet.X - 4 + 1, height - 73 + 64 - offcet.Y), Color.Black);
                spriteBatch.DrawString(attackfont, t, new Vector2(width - 192 + 64 - offcet.X - 4 - 1, height - 73 + 64 - offcet.Y), Color.Black);
                spriteBatch.DrawString(attackfont, t, new Vector2(width - 192 + 64 - offcet.X - 4, height - 73 + 64 - offcet.Y + 1), Color.Black);
                spriteBatch.DrawString(attackfont, t, new Vector2(width - 192 + 64 - offcet.X - 4, height - 73 + 64 - offcet.Y-1), Color.Black);
                spriteBatch.DrawString(attackfont, t, new Vector2(width - 192 + 64 - offcet.X-4, height - 73 + 64 - offcet.Y), Color.White);
                spriteBatch.Draw(shield, new Vector2(width - 192 + 64 - offcet.X - 16-4, height - 73 + 64 - offcet.Y), Color.White);
            }
        }
        void DrawWinnerScreen()
        {
            Rectangle rect = new Rectangle((width - 200) / 2 - 95, height / 2 - 40, 190, 80);
            spriteBatch.Draw(backgrounddark, new Vector2(rect.X, rect.Y), new Rectangle(0, 0, rect.Width, rect.Height), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            spriteBatch.Draw(outline, new Vector2(rect.X, rect.Y), new Rectangle(0, 0, 2, 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(outline, new Vector2(rect.X + rect.Width - 2, rect.Y), new Rectangle(2, 0, 2, 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(outline, new Vector2(rect.X, rect.Y + rect.Height - 2), new Rectangle(0, 2, 2, 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(outline, new Vector2(rect.X + rect.Width - 2, rect.Y + rect.Height - 2), new Rectangle(2, 2, 2, 2), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            spriteBatch.Draw(outline, new Rectangle(rect.X, rect.Y + 2, 2, rect.Height - 4), new Rectangle(0, 2, 2, 0), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(outline, new Rectangle(rect.X + rect.Width - 2, rect.Y + 2, 2, rect.Height - 4), new Rectangle(2, 2, 2, 0), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(outline, new Rectangle(rect.X + 2, rect.Y, rect.Width - 4, 2), new Rectangle(2, 0, 0, 2), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(outline, new Rectangle(rect.X + 2, rect.Y + rect.Height - 2, rect.Width - 4, 2), new Rectangle(2, 2, 0, 2), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);

            string text = (playerManager.players[1].unitnum <= 0 && playerManager.players[1].castlenum <= 0) ? Game.langManager[GameString.YouLose] : Game.langManager[GameString.YouWin];
            Vector2 v = font.MeasureString(text);

            spriteBatch.DrawString(font, text, new Vector2(rect.X + rect.Width / 2 - (int)v.X / 2, rect.Y + rect.Height / 2 - (int)v.Y / 2), Color.White);
        }
        void DrawMiniMap(SpriteBatch spriteBatch, Texture2D line, Texture2D darkbackground, Texture2D lightbackground, SpriteFont font, ref GuiObject me)
        {
            StandartGuiDraw(spriteBatch, line, darkbackground, lightbackground, font, ref me);

            float sx = (float)(me.rect.Width - 6) / map.width, sy = (float)(me.rect.Height - 6) / map.height;
            float s = Math.Min(sx, sy);
            if ((int)Math.Floor(s + 1) > 16) s = 15;
            int offcetx = (int)((me.rect.Width - 4 - s * map.width) / 2), offcety = (int)((me.rect.Height - 4 - s * map.height) / 2);
            for (int i = 0; i < map.height; i++)
                for (int j = 0; j < map.width; j++)
                {
                    Rectangle r = new Rectangle(offcetx+me.rect.X + 2 + (int)(j * s), offcety+me.rect.Y + 2 + (int)(i * s), (int)Math.Floor(s+1), (int)Math.Floor(s+1));
                    Rectangle r2 = new Rectangle(Map.TILES_IN_TERRAIN[map.mapData[i, j]] % 4*16, Map.TILES_IN_TERRAIN[map.mapData[i, j]] / 4*16, 16, 16);
                    spriteBatch.Draw(miniset, r, r2, Color.White);
                }
            //if(gameState==GameState.Game)
            foreach (Unit u in unitManager.units)
            {
                    Rectangle r = new Rectangle(offcetx + me.rect.X + 3 + (int)(u.position.X * s), offcety + me.rect.Y + 3 + (int)(u.position.Y * s), (int)Math.Floor(s + 1)-2, (int)Math.Floor(s + 1)-2);
                    Color c = Color.DarkGray;
                    switch (u.team)
                    {
                        case 1: c = Color.BlueViolet; break;
                        case 2: c = Color.Firebrick; break;
                        case 3: c = Color.DarkGreen; break;
                        case 4: c = Color.DarkGray; break;
                    }
                    spriteBatch.Draw(empry, r, c);

                    r = new Rectangle(offcetx + me.rect.X + 4 + (int)(u.position.X * s), offcety + me.rect.Y + 4 + (int)(u.position.Y * s), (int)Math.Floor(s + 1)-4, (int)Math.Floor(s + 1)-4);
                    c = Color.Gray;
                    switch (u.team)
                    {
                        case 1: c = Color.Blue; break;
                        case 2: c = Color.Red; break;
                        case 3: c = Color.Green; break;
                        case 4: c = Color.Black; break;
                    }
                    spriteBatch.Draw(empry, r, c);
            }

            //StandartGuiDraw(spriteBatch, line, darkbackground, lightbackground, font, ref me);
        }

        void GameMain(ref GuiObject me)
        {
            if (!gui.IsFloatMessageLive()&&updateGame)
            {
                #region Players
                if (playerManager.players[gameInfo.nowplayer].state==PlayerState.Player)
                {
                    if (me.lclick && gameInfo.state != GameState2.Buy&&!gameInfo.onlyUpdateUnits)
                    {
                        int unitOnClick = unitManager.units.IndexOf(unitManager.GetByPosition(camera.positionOnMap.X, camera.positionOnMap.Y));
                        bool castle = map.IsCastleOnPosition(camera.positionOnMap.X, camera.positionOnMap.Y);
                        bool canmove = unitOnClick != -1 ? !unitManager[unitOnClick].HasProperty(UnitProperty.CanNotAttackAndMoveAtOnce) : false;
                        bool doSomesing = !(gameInfo.submenu.Count == 0);

                        #region SetSelectedUnitAndCreateMoveRange
                        if (gameInfo.state == GameState2.None &&
                            !gameInfo.wait &&
                            unitOnClick != -1 &&
                            !doSomesing)
                        {
                            if (gameInfo.selectedUnitId == -1 &&
                                unitManager[unitOnClick].team == gameInfo.nowplayer &&
                                !unitManager[unitOnClick].moveEnd)
                            {
                                gameInfo.selectedUnitId = unitOnClick;
                                gameInfo.oldposition = unitManager[gameInfo.selectedUnitId].position;
                            }
                            if (gameInfo.selectedUnitId >= 0)
                            {
                                if (castle || !canmove)
                                    CreateSubMenu(true);
                                if ((!castle && canmove) || (gameInfo.submenu.Count == 1 && gameInfo.submenu[0] == Game.langManager[GameString.Move]))
                                {
                                    gameInfo.submenu.Clear();
                                    if (map.alphaData[camera.positionOnMap.Y, camera.positionOnMap.X] == 0)
                                    {
                                        gameInfo.oldposition = unitManager[gameInfo.selectedUnitId].position;
                                        map.ClearAlphaData();
                                        unitManager.units[gameInfo.selectedUnitId].FillMoveRange(ref map, ref unitManager,ref playerManager);
                                        gameInfo.state = GameState2.Move;
                                        doSomesing = true;
                                    }
                                }
                                //gameInfo.selectedUnitId = -1;
                                doSomesing = true;
                                //unitManager[gameInfo.selectedUnitId].GetAiTarget(ref map, ref unitManager, tombs);
                            }
                        }
                        #endregion

                        #region SetMovePathToSelectedUnit
                        if (gameInfo.selectedUnitId >= 0 &&
                            !gameInfo.wait &&
                            gameInfo.state == GameState2.Move &&
                            map.IsInMoveRange(camera.positionOnMap.X, camera.positionOnMap.Y) &&
                            (unitOnClick == -1 || unitOnClick == gameInfo.selectedUnitId) &&
                            !doSomesing)
                        {
                            gameInfo.wait = true;
                            //List<Point> p = new List<Point>();
                            //p = unitManager.units[gameInfo.selectedUnitId].GetPathFromPointToPoint(ref map, new Point((int)unitManager.units[gameInfo.selectedUnitId].position.X, (int)unitManager.units[gameInfo.selectedUnitId].position.Y), camera.positionOnMap);
                            //gameInfo.path = new List<Vector2>();
                            //for (int i = 0; i < p.Count; i++)
                            //    gameInfo.path.Add(new Vector2(p[i].X, p[i].Y));
                            //gameInfo.path.RemoveAt(0);
                            SetPath(new Point((int)unitManager.units[gameInfo.selectedUnitId].position.X, (int)unitManager.units[gameInfo.selectedUnitId].position.Y), camera.positionOnMap);

                            map.ClearAlphaData();
                            doSomesing = true;
                        }
                        #endregion

                        #region Attack
                        if (gameInfo.state == GameState2.Attack &&
                            unitOnClick != gameInfo.selectedUnitId &&
                            map.IsInAttackRange(camera.positionOnMap.X, camera.positionOnMap.Y) &&
                            !doSomesing)
                        {
                            Unit s = unitManager[gameInfo.selectedUnitId];
                            bool attackdone = false;
                            if (unitOnClick >= 0 && playerManager.players[unitManager[unitOnClick].team].team != playerManager.players[gameInfo.nowplayer].team)
                            {
                                attackdone = AtackUnits(ref gameInfo.selectedUnitId, ref unitOnClick);
                            }
                            else if (unitManager[gameInfo.selectedUnitId].HasProperty(UnitProperty.CanDestroyVillages) &&
                                map.IsVillageOnPosition(camera.positionOnMap.X, camera.positionOnMap.Y) ?playerManager.players[map.buidingData[map.buidingData.IndexOf(map.GetVillage(camera.positionOnMap.X, camera.positionOnMap.Y))].fraction].team !=playerManager.players[gameInfo.nowplayer].team : false && unitOnClick == -1)
                            {
                                if (map.buidingData[map.buidingData.IndexOf(map.GetVillage(camera.positionOnMap.X, camera.positionOnMap.Y))].fraction != 0)
                                    playerManager.players[map.buidingData[map.buidingData.IndexOf(map.GetVillage(camera.positionOnMap.X, camera.positionOnMap.Y))].fraction].villagenum--;
                                map.buidingData[map.buidingData.IndexOf(map.GetVillage(camera.positionOnMap.X, camera.positionOnMap.Y))].fraction = 0;
                                map.mapData[camera.positionOnMap.Y, camera.positionOnMap.X] = 62+64;
                                particalManager.AddNewParticle(new Particle(1, new Vector2(camera.positionOnMap.X * 64 + 32, camera.positionOnMap.Y * 64 + 32), 0.3f, false));
                                attackdone = true;
                            }

                            if (attackdone)
                            {
                                if (gameInfo.state == GameState2.Attack)
                                {
                                    gameInfo.state = GameState2.None;
                                    unitManager.units[gameInfo.selectedUnitId].moveEnd = true;
                                    gameInfo.selectedUnitId = -1;
                                }
                                map.ClearAlphaData();
                                doSomesing = true;
                            }
                        }
                        #endregion

                        #region BuyMenuWhenNoUnits
                        try
                        {
                            if (castle && (unitOnClick == -1 || (unitManager[unitOnClick].moveEnd)) && !doSomesing && map.GetCastle(camera.positionOnMap.X, camera.positionOnMap.Y).fraction == gameInfo.nowplayer)
                            {
                                gui.elements[31].rect = new Rectangle((int)camera.mouse.X + 1, (int)camera.mouse.Y, 0, 0);
                                List<int> id = new List<int>();
                                if (!unitManager.IsLiveKingOfFraction(gameInfo.nowplayer)) id.Add(9);
                                for (int k = 0; k < 9; k++) id.Add(k);

                                gameInfo.submenu = new List<string>();
                                gameInfo.submenu.Add(Game.langManager[GameString.Buy]);
                                gameInfo.buyid = 0;
                                gameInfo.castlePosition = new Vector2(camera.positionOnMap.X, camera.positionOnMap.Y);
                                gameInfo.BuySelectionUnit = Unit.GetByType(id[gameInfo.buyid], 0, 0, 0);
                                //gui.elements[31].lclick = false;

                                SetSubMenuSize();
                                doSomesing = true;
                            }
                        }
                        catch { ;}
                        #endregion

                        #region Raise
                        if (gameInfo.state == GameState2.Raise &&
                            unitOnClick == -1 &&
                            !doSomesing)
                        {
                            if (map.IsInAttackRange(camera.positionOnMap.X, camera.positionOnMap.Y))
                            {
                                for (int i = tombs.Count - 1; i >= 0; i--)
                                {
                                    if (tombs[i].x == camera.positionOnMap.X && tombs[i].y == camera.positionOnMap.Y)
                                    {
                                        particalManager.AddNewParticle(new Particle(2, new Vector2(tombs[i].x * 64 + 16, tombs[i].y * 64 + 16), 0.5f, false));
                                        particalManager.AddNewParticle(new Particle(2, new Vector2(tombs[i].x * 64 + 48, tombs[i].y * 64 + 16), 0.5f, false));
                                        particalManager.AddNewParticle(new Particle(2, new Vector2(tombs[i].x * 64 + 48, tombs[i].y * 64 + 48), 0.5f, false));
                                        particalManager.AddNewParticle(new Particle(2, new Vector2(tombs[i].x * 64 + 16, tombs[i].y * 64 + 48), 0.5f, false));
                                        unitManager.AddUnit(new Unit(10, gameInfo.nowplayer, camera.positionOnMap.X, camera.positionOnMap.Y));
                                        unitManager.units[unitManager.units.IndexOf(unitManager.GetByPosition(camera.positionOnMap.X, camera.positionOnMap.Y))].moveEnd = true;
                                        tombs.RemoveAt(i);
                                        particalManager.AddNewParticle(new Particle(1, new Vector2(camera.positionOnMap.X * 64 + 32, camera.positionOnMap.Y * 64 + 32), 0.3f, false));

                                        unitManager.units[gameInfo.selectedUnitId].moveEnd = true;
                                        gameInfo.selectedUnitId = -1;
                                        map.ClearAlphaData();
                                        doSomesing = true;
                                        gameInfo.state = GameState2.None;

                                        break;
                                    }
                                }
                            }
                        }
                        #endregion
                    }

                    #region RCLick
                    if (me.rclick && !gameInfo.wait && !gameInfo.onlyUpdateUnits)
                    {
                        if (gameInfo.submenu.Count == 0)
                        {
                            if (gameInfo.selectedUnitId >=0)
                                if (gameInfo.state == GameState2.Move && unitManager.GetNumberOfUnitsOnPoition(unitManager[gameInfo.selectedUnitId].position) <= 1)
                                {
                                    gameInfo.state = GameState2.None;
                                    gameInfo.selectedUnitId = -1;
                                    map.ClearAlphaData();
                                }
                                else 
                                {

                                    //if (gameInfo.selectedUnitId >= 0)
                                        unitManager.units[gameInfo.selectedUnitId].position = gameInfo.oldposition;
                                    //gameInfo.selectedUnitId = -1;
                                    gameInfo.submenu.Clear();
                                    map.ClearAlphaData();
                                    //map.ClearAlphaData();
                                    //if (gameInfo.selectedUnitId >= 0)
                                        unitManager.units[gameInfo.selectedUnitId].FillMoveRange(ref map, ref unitManager,ref playerManager);
                                    gameInfo.state = GameState2.Move;
                                }
                            if (gameInfo.state == GameState2.AttackPre)
                            {
                                gameInfo.state = GameState2.None;
                                gameInfo.selectedUnitId = -1;
                                map.ClearAlphaData();
                            }
                        }
                        if (gameInfo.submenu.Count != 0)
                        {
                                if (gameInfo.selectedUnitId >= 0)
                                    unitManager.units[gameInfo.selectedUnitId].position = gameInfo.oldposition;
                                //gameInfo.selectedUnitId = -1;
                                gameInfo.submenu.Clear();
                                map.ClearAlphaData();
                                //map.ClearAlphaData();
                                if (gameInfo.selectedUnitId >= 0)
                                unitManager.units[gameInfo.selectedUnitId].FillMoveRange(ref map, ref unitManager, ref playerManager);
                                gameInfo.state = GameState2.Move;
                        }
                    }
                    #endregion

                    #region UpdateMove
                    if (gameInfo.wait && gameInfo.state == GameState2.Move)
                    {
                        if (gameInfo.path.Count > 0)
                        {
                            unitManager.units[gameInfo.selectedUnitId].Move(new Vector2(gameInfo.path[0].X, gameInfo.path[0].Y));
                            if (Timer.GetTimerState(0.2f)) particalManager.AddNewParticle(new Particle(3, unitManager[gameInfo.selectedUnitId].position * 64 + new Vector2(32, 64), 1, false));
                            //particalManager.AddNewParticle(new Particle(0, unitManager[gameInfo.selectedUnitId].position * 64 + new Vector2(32,62), 1, 4, false));
                            if ((unitManager.units[gameInfo.selectedUnitId].position - new Vector2(gameInfo.path[0].X, gameInfo.path[0].Y)).Length() < 0.01f)
                            {
                                unitManager.units[gameInfo.selectedUnitId].position = new Vector2(gameInfo.path[0].X, gameInfo.path[0].Y);
                                gameInfo.path.RemoveAt(0);
                            }
                            if (gameInfo.path.Count == 0)
                            {
                                gameInfo.wait = false;
                                if (!gameInfo.onlyUpdateUnits)
                                    CreateSubMenu(false);
                            }
                        }
                        if (gameInfo.path.Count == 0)
                        {
                            gameInfo.wait = false;
                            if (!gameInfo.onlyUpdateUnits)
                                CreateSubMenu(false);
                        }
                    }
                    #endregion

                    #region Reattack
                    if (gameInfo.state == GameState2.Reattack && !particalManager.IsLifeParticlesOfType(1))
                    {
                        Reattack(ref gameInfo.attackUnitId, ref gameInfo.selectedUnitId);
                    }
                    #endregion

                    #region Update path
                    if (gameInfo.state == GameState2.Move)
                    {
                        if (map.IsInMoveRange(camera.positionOnMap.X, camera.positionOnMap.Y) && camera.positionOnMap != camera.oldpositionOnMap)
                        {
                            SetPath(new Point((int)unitManager.units[gameInfo.selectedUnitId].position.X, (int)unitManager.units[gameInfo.selectedUnitId].position.Y), camera.positionOnMap);
                        }
                    }
                    #endregion
                }
                #endregion
                #region Ai
                if (playerManager.players[gameInfo.nowplayer].state == PlayerState.Ai || playerManager.players[gameInfo.nowplayer].state == PlayerState.Ai2)
                {
                    bool dosomesing = false;
                    int nowunitid = -1;
                    for (int i = 0; i < unitManager.units.Count; i++)
                    {
                        if (unitManager.units[i].team == gameInfo.nowplayer && !unitManager.units[i].moveEnd) { nowunitid = i; break; }
                    }


                    #region Buy

                    if (nowunitid == -1 && !gameInfo.onlyUpdateUnits)
                    {
                        List<Building> bl = new List<Building>();
                        Unit u = null;
                        foreach (Building b in map.castleData)
                        {
                            if (b.fraction == gameInfo.nowplayer)
                            {
                                int id = unitManager.GetIdByPosition(b.x, b.y);
                                if ((id != -1 && playerManager.players[unitManager[id].team].team == playerManager.players[gameInfo.nowplayer].team) || id == -1)
                                    bl.Add(b);
                            }
                        }
                        if (bl.Count > 0)
                        {
                            int i = new Random().Next(bl.Count - 1);
                            if (!unitManager.IsLiveKingOfFraction(gameInfo.nowplayer))
                            {
                                Unit un = Unit.GetByType(9, gameInfo.nowplayer, bl[i].x, bl[i].y);
                                if (playerManager.players[gameInfo.nowplayer].gold >= un.cost)
                                    u = un;
                            }
                            else
                            {
                                //for (int k = 0; k < 9; k++)
                                //{
                                //    Unit un = Unit.GetByType(k, gameInfo.nowplayer, bl[i].x, bl[i].y);
                                //    if (playerManager.players[gameInfo.nowplayer].gold >= un.cost * (new Random().Next(10, 25) / 10.0f))
                                //        u = un;
                                //    else
                                //    {
                                //        break;
                                //    }
                                //}
                                if(unitManager.countUnits(0,gameInfo.nowplayer)<2&&CanBuyUnit(0,bl[i].x,bl[i].y,ref playerManager))
                                    u=Unit.GetByType(0,gameInfo.nowplayer,bl[i].x,bl[i].y);
                                else if (unitManager.countUnits(1, gameInfo.nowplayer) < 2 && CanBuyUnit(1, bl[i].x, bl[i].y, ref playerManager))
                                    u = Unit.GetByType(1, gameInfo.nowplayer, bl[i].x, bl[i].y);
                                else
                                {
                                    int countOfMe = 0, countOfEnemies = 0;
                                    for (int k = 0; k < playerManager.players.Count; k++)
                                    {
                                        if (k == gameInfo.nowplayer) countOfMe += unitManager.countUnits(-1, k);
                                        else countOfEnemies += unitManager.countUnits(-1, k);
                                    }
                                    if (playerManager.players[gameInfo.nowplayer].gold >= 1000 || unitManager.countUnits(-1, gameInfo.nowplayer) < 8 || countOfMe < countOfEnemies)
                                    {
                                        int countofEnableUnits=0;
                                        int[] arraysWhhithCountOfUnits = new int[11];

                                        int q = 0;
                                        for (q = 1; q < 11; q++)
                                        { 
                                            Unit un=Unit.GetByType(q,gameInfo.nowplayer,bl[i].x,bl[i].y);
                                            if ((unitManager.countUnits(q, gameInfo.nowplayer) < 1 || un.cost >= 600) && CanBuyUnit(q, bl[i].x, bl[i].y, ref playerManager))
                                            {
                                                arraysWhhithCountOfUnits[countofEnableUnits] = q;
                                                countofEnableUnits++;
                                            }
                                        }
                                        if (countofEnableUnits > 0)
                                        { 
                                            q=arraysWhhithCountOfUnits[new Random().Next(0,countofEnableUnits)];
                                            u = Unit.GetByType(q, gameInfo.nowplayer, bl[i].x, bl[i].y);
                                        }
                                    }
                                }
                            }
                            if (u != null)
                            {
                                playerManager.players[gameInfo.nowplayer].gold -= u.cost;
                                playerManager.players[gameInfo.nowplayer].unitnum++;
                                unitManager.AddUnit(u);
                                nowunitid = unitManager.units.Count - 1;
                            }
                            //dosomesing = true;
                        }
                    }

                    #endregion
                    if (nowunitid == -1 && !gameInfo.wait) { EndTurn(); dosomesing = true; }
                    #region Logic
                    if (Timer.GetTimerState(0.9f) && !gameInfo.wait && !gameInfo.onlyUpdateUnits)
                    {

                        if (nowunitid != -1 &&
                            !gameInfo.wait &&
                            gameInfo.state == GameState2.None &&
                            !dosomesing)
                        {
                            map.UpdateFractionData(unitManager);
                            Point target;
                            if (playerManager.players[gameInfo.nowplayer].state == PlayerState.Ai2)
                                target = unitManager[nowunitid].GetAiTargetTest(ref map, ref unitManager, tombs, ref playerManager);
                            else
                                target = unitManager[nowunitid].GetAiTarget(ref map, ref unitManager, tombs, ref playerManager);
                            //unitManager[nowunitid].FillMoveAllMap(ref map, ref unitManager, target.X, target.Y);
                            gameInfo.selectedUnitId = nowunitid;
                            gameInfo.oldposition = unitManager[nowunitid].position;
                            gameInfo.state = GameState2.Move;
                            gameInfo.wait = true;
                            if (unitManager[nowunitid].position == new Vector2(target.X, target.Y))
                                gameInfo.wait = true;

                            SetPath(new Point((int)unitManager.units[gameInfo.selectedUnitId].position.X, (int)unitManager.units[gameInfo.selectedUnitId].position.Y), target);
                            //map.ClearAlphaData();
                            dosomesing = true;
                            map.ClearAlphaData();
                        }
                    }
                    else dosomesing = true;
                    #endregion

                    #region Move
                    if (gameInfo.wait && gameInfo.state == GameState2.Move)
                    {
                        if (gameInfo.path.Count > 0)
                        {
                            unitManager.units[gameInfo.selectedUnitId].Move(new Vector2(gameInfo.path[0].X, gameInfo.path[0].Y));
                            if (Timer.GetTimerState(0.2f)) particalManager.AddNewParticle(new Particle(3, unitManager[gameInfo.selectedUnitId].position * 64 + new Vector2(32, 64), 1, false));
                            //particalManager.AddNewParticle(new Particle(0, unitManager[gameInfo.selectedUnitId].position * 64 + new Vector2(32,62), 1, 4, false));
                            if ((unitManager.units[gameInfo.selectedUnitId].position - new Vector2(gameInfo.path[0].X, gameInfo.path[0].Y)).Length() < 0.01f)
                            {
                                unitManager.units[gameInfo.selectedUnitId].position = new Vector2(gameInfo.path[0].X, gameInfo.path[0].Y);
                                gameInfo.path.RemoveAt(0);
                                map.UpdateFractionData(unitManager);
                            }
                            if (gameInfo.path.Count == 0)
                            {
                                gameInfo.wait = false;
                                //map.UpdateFractionData(unitManager);
                                if (!gameInfo.onlyUpdateUnits)
                                    AfterMove();
                            }
                        }
                        else if (gameInfo.path.Count == 0)
                        {
                            gameInfo.wait = false;
                            //map.UpdateFractionData(unitManager);
                            if (!gameInfo.onlyUpdateUnits)
                                AfterMove();
                        }
                        dosomesing = true;
                    }
                    #endregion

                    #region Reattack
                    if (gameInfo.wait && gameInfo.state == GameState2.Reattack && !particalManager.IsLifeParticlesOfType(1))
                    {
                        Reattack(ref gameInfo.attackUnitId, ref gameInfo.selectedUnitId);
                        gameInfo.wait = false;
                        dosomesing = true;
                    }
                    #endregion

                    if (!dosomesing && !gameInfo.wait) { EndTurn(); }
                }
                #endregion

                if (playerManager.players[gameInfo.nowplayer].state == PlayerState.Net)
                {
                    EndTurn();
                }
            }

            #region updatesubmenuposition
            if (gameInfo.selectedUnitId >= 0)
            {
                if (((gameInfo.submenu.Count == 0 && map.counterOfMoveCells() == 0) && playerManager.players[gameInfo.nowplayer].state == PlayerState.Player) || playerManager.players[gameInfo.nowplayer].state == PlayerState.Ai)
                    camera.ToPositionOnMap((int)unitManager[gameInfo.selectedUnitId].position.X, (int)unitManager[gameInfo.selectedUnitId].position.Y);

                gui.elements[31].rect.X = (int)(unitManager[gameInfo.selectedUnitId].position.X * 64 - camera.position.X + camera.offset.X + 64);
                gui.elements[31].rect.Y = (int)(unitManager[gameInfo.selectedUnitId].position.Y * 64 - camera.position.Y + camera.offset.Y);

                if (gui.elements[31].rect.X + gui.elements[31].rect.Width > width - 200)
                    gui.elements[31].rect.X = gui.elements[31].rect.X - gui.elements[31].rect.Width-64;
                if (gui.elements[31].rect.Y + gui.elements[31].rect.Height > height)
                    gui.elements[31].rect.Y = gui.elements[31].rect.Y - gui.elements[31].rect.Height-64;

                if (gui.elements[31].rect.X + gui.elements[31].rect.Width > width - 200) gui.elements[31].enable = false;
                else gui.elements[31].enable = true;
            }
            #endregion

            #region vibrate
            if (Timer.GetTimerState(0.05f))
            {
                foreach (Unit u in unitManager.units)
                    u.vibrate = false;
                if (particalManager.IsLifeParticlesOfType(1))
                {
                    Unit.vibrateOffcet = new Vector2(new Random().Next(-5, 5), new Random().Next(-5, 5));
                    Vector2 p = particalManager.GetPositionByType(1);
                    int id = unitManager.GetIdByPosition((int)p.X, (int)p.Y);
                    if (id != -1)
                        unitManager[id].vibrate = true;
                }
                else Unit.vibrateOffcet = Vector2.Zero;
            }
            #endregion


        }
        void SubMenuUpdate(ref GuiObject me)
        {
            if (gameInfo.submenu.Count > 0 && me.lclick)
            {
                int i = (int)(camera.mouse.Y - gui.elements[31].rect.Y - 1) / (int)(font.MeasureString(gameInfo.submenu[0]).Y + 2);
                if (i >= gameInfo.submenu.Count) i = gameInfo.submenu.Count - 1;
                if (gameInfo.submenu[i] == Game.langManager[GameString.EndTurn])
                {
                    unitManager.units[gameInfo.selectedUnitId].moveEnd = true;
                    camera.onposition = true;
                    if (unitManager.units[gameInfo.selectedUnitId].HasProperty(UnitProperty.Lighting))
                    {
                        for (int k = -2; k < 3; k++)
                            for (int j = -2; j < 3; j++)
                            {
                                if (Math.Abs(k + j) <= 2)
                                {
                                    int ui = unitManager.units.IndexOf(unitManager.GetByPosition(k + (int)unitManager[gameInfo.selectedUnitId].position.X, j + (int)unitManager[gameInfo.selectedUnitId].position.Y));
                                    if (ui != -1 && unitManager[ui].team == unitManager.units[gameInfo.selectedUnitId].team)
                                    {
                                        unitManager.units[ui].bonusDefence += 10;
                                        particalManager.AddNewParticle(new Particle(2, new Vector2(unitManager[ui].position.X * 64 + 32, unitManager[ui].position.Y * 64 + 32), 0.3f,  false));
                                        List<UnitEffect> up = new List<UnitEffect>();
                                        foreach (UnitEffect upr in unitManager[ui].effects)
                                            up.Add(upr);
                                        up.Add(UnitEffect.Light);
                                        unitManager.units[ui].effects = up.ToArray();

                                    }
                                }
                            }
                    }
                    gameInfo.selectedUnitId = -1;
                    gameInfo.state = GameState2.None;
                }
                if (gameInfo.submenu[i] == Game.langManager[GameString.Attack])
                {
                    unitManager.units[gameInfo.selectedUnitId].FillAttackRangeEx(ref map, (int)unitManager.units[gameInfo.selectedUnitId].position.X, (int)unitManager.units[gameInfo.selectedUnitId].position.Y, ref unitManager);
                    gameInfo.state = GameState2.Attack;
                }
                if (gameInfo.submenu[i] == Game.langManager[GameString.Move])
                {
                    if (map.alphaData[(int)unitManager.units[gameInfo.selectedUnitId].position.Y, (int)unitManager.units[gameInfo.selectedUnitId].position.X] == 0)
                    {
                        map.ClearAlphaData();
                        unitManager.units[gameInfo.selectedUnitId].FillMoveRange(ref map, ref unitManager, ref playerManager);
                        gameInfo.state = GameState2.Move;
                    }
                }
                if (gameInfo.submenu[i] == Game.langManager[GameString.Occupy])
                {
                    Occupy();
                }
                if (gameInfo.submenu[i] == Game.langManager[GameString.Repair])
                {
                    Unit u = unitManager[gameInfo.selectedUnitId];
                    if(u!=null)
                    {
                        map.mapData[(int)u.position.Y, (int)u.position.X] = (63 + 64);
                        //map.buidingData[map.buidingData.IndexOf(map.GetVillage((int)u.position.X, (int)u.position.Y))].fraction = gameInfo.nowplayer;
                        map.buidingData[map.buidingData.IndexOf(map.GetVillage((int)u.position.X, (int)u.position.Y))].fraction = 0;
                    }
                    unitManager[gameInfo.selectedUnitId].moveEnd = true;
                    gameInfo.selectedUnitId = -1;
                    gameInfo.state = GameState2.None;
                }
                if (gameInfo.submenu[i] == Game.langManager[GameString.Raise])
                {
                    map.ClearAlphaData();
                    gameInfo.state = GameState2.Raise;
                    unitManager[gameInfo.selectedUnitId].FillAttackRangeEx(ref map, (int)unitManager[gameInfo.selectedUnitId].position.X, (int)unitManager[gameInfo.selectedUnitId].position.Y, ref unitManager);
                    playerManager.players[gameInfo.nowplayer].unitnum++;                    
                }
                if (gameInfo.submenu[i] == Game.langManager[GameString.Buy])
                {
                    gameInfo.state = GameState2.Buy;
                    //gameInfo.BuySelectionUnit = unitManager.IsLiveKingOfFraction(gameInfo.nowplayer) ? Unit.GetByType(9, 0, 0, 0) : Unit.GetByType(0, 0, 0, 0);
                }
                gameInfo.submenu.Clear();
            }
        }
        void GameLoadList(ref GuiObject me)
        {
            if (me.lclick)
            {
                if (gui.names.Count > 0)
                {
                    int h = (int)font.MeasureString(gui.names[0]).Y + 2;
                    int id = (int)(camera.mouse.Y - 11) / h;
                    if (gui.selectedNameId == id)
                    {
                        LoadGame(gui.selectedName);
                        gameState = GameState.Game;
                        gameInfo = new GameInfo();
                        //playerManager.SetPlayers(map, unitManager, 1000);
                    }
                    else
                    {
                        gui.selectedNameId = id;
                        if (gui.selectedNameId < gui.names.Count)
                            gui.selectedName = gui.names[gui.selectedNameId+gameInfo.firstMapName];
                        else gui.selectedNameId = 0;
                    }
                }
            }
        }
        void LoadMapButton(ref GuiObject me)
        {
            if (me.lclick)
            {
                if (gui.names.Count > 0)
                {
                    LoadGame(gui.selectedName);
                    gameState = GameState.Game;
                }
            }
        }
        void EndTurnButton(ref GuiObject me)
        {
            if (me.lclick && gameInfo.state == GameState2.None && gameInfo.submenu.Count == 0 && !gui.IsFloatMessageLive() && playerManager.players[gameInfo.nowplayer].state == PlayerState.Player)
            {
                EndTurn();
            }
        }

        void LoadListBackward(ref GuiObject me)
        {
            if (me.lclick)
            {
                gameInfo.firstMapName--;
                if (gameInfo.firstMapName < 0) gameInfo.firstMapName = 0;
                gui.selectedName = gui.names[gui.selectedNameId + gameInfo.firstMapName];
            }
        }
        void LoadListForward(ref GuiObject me)
        {
            if (me.lclick)
            {
                try
                {
                    gameInfo.firstMapName++;
                    int max = (int)Math.Floor((gui.names.Count * ((int)font.MeasureString(gui.names[0]).Y + 2) - me.rect.Height) / (font.MeasureString(gui.names[0]).Y + 2));
                    if (gameInfo.firstMapName > max) gameInfo.firstMapName = max;
                    gui.selectedName = gui.names[gui.selectedNameId + gameInfo.firstMapName];
                }
                catch { ;}
            }
        }

        void BuyMenuButtonForward(ref GuiObject me)
        {
            if (me.lclick && gameInfo.state == GameState2.Buy)
            {
                List<int> id = new List<int>();
                if (!unitManager.IsLiveKingOfFraction(gameInfo.nowplayer)) id.Add(9);
                for (int i = 0; i < 9; i++) id.Add(i);
                if (id.Count > (width - 220 + gameInfo.buystartid * 64) / 64)
                {
                    gameInfo.buystartid++;
                    gameInfo.buyid++;
                }
            }
        }
        void BuyMenuButtonBackward(ref GuiObject me)
        {
            if (me.lclick && gameInfo.state == GameState2.Buy)
            {
                gameInfo.buystartid--;
                gameInfo.buyid--;
                if (gameInfo.buystartid < 0) { gameInfo.buystartid = 0; gameInfo.buyid = 0; }
                //if(unitManager.IsLiveKingOfFraction(gameInfo.nowplayer))
            }
        }
        void BuyMenuButtonOk(ref GuiObject me)
        {
            if (me.lclick && gameInfo.state == GameState2.Buy && playerManager.players[gameInfo.nowplayer].gold - gameInfo.BuySelectionUnit.cost >= 0)
            {
                playerManager.players[gameInfo.nowplayer].gold -= gameInfo.BuySelectionUnit.cost;
                unitManager.AddUnit(gameInfo.BuySelectionUnit);
                playerManager.players[gameInfo.nowplayer].unitnum++;
                unitManager.units[unitManager.units.Count - 1].team = gameInfo.nowplayer;
                unitManager.units[unitManager.units.Count - 1].position = gameInfo.castlePosition;
                gameInfo.selectedUnitId = unitManager.units.Count - 1;
                gameInfo.oldposition = unitManager[gameInfo.selectedUnitId].position;
                map.ClearAlphaData();
                unitManager.units[gameInfo.selectedUnitId].FillMoveRange(ref map, ref unitManager, ref playerManager);
                gameInfo.state = GameState2.Move;
            }
        }
        void BuyMenuButtonCansel(ref GuiObject me)
        {
            if (me.lclick && gameInfo.state == GameState2.Buy)
            {
                gameInfo.state = GameState2.None;
                gameInfo.selectedUnitId = -1;
                map.ClearAlphaData();
            }
        }
        void BuyMenu(ref GuiObject me)
        {
            if (me.lclick && gameInfo.state == GameState2.Buy)
            {
                Rectangle r = new Rectangle(me.rect.X, me.rect.Y + me.rect.Height - 144, me.rect.Width, 68);
                if (r.Contains((int)camera.mouse.X, (int)camera.mouse.Y))
                {
                    List<int> id = new List<int>();
                    if (!unitManager.IsLiveKingOfFraction(gameInfo.nowplayer)) id.Add(9);
                    for (int k = 0; k < 9; k++) if(k<=gameInfo.alooweUnit)id.Add(k);
                    int offcetx = (me.rect.Width - id.Count * 64) / 2;

                    int i = ((int)camera.mouse.X - me.rect.X - 2 - offcetx) / 64;
                    if (i < 0) i = 0;
                    if (i > gameInfo.alooweUnit + (!unitManager.IsLiveKingOfFraction(gameInfo.nowplayer) ? 1 : 0)) i = gameInfo.alooweUnit + (!unitManager.IsLiveKingOfFraction(gameInfo.nowplayer) ? 1 : 0);
                    if ((id[i] == 2 || id[i] == 3 || id[i] == 5) && gameInfo.BuySelectionUnit.id == id[i])
                    {
                        if (id[i] == 2) gameInfo.BuySelectionUnit = Unit.GetByType(13, 0, 0, 0);
                        else if (id[i] == 3) gameInfo.BuySelectionUnit = Unit.GetByType(14, 0, 0, 0);
                        else if (id[i] == 5) gameInfo.BuySelectionUnit = Unit.GetByType(15, 0, 0, 0);
                    }
                    else gameInfo.BuySelectionUnit = Unit.GetByType(id[i], 0, 0, 0);
                    gameInfo.buyid = i + gameInfo.buystartid;
                }
            }
        }

        void LoadGame(string path)
        {
            lightColor = Color.White;
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader reader = new BinaryReader(fs);
            float version = reader.ReadSingle();
            if (version == 1.1f)
            {
                int wid = reader.ReadInt32();
                int hei = reader.ReadInt32();
                map = new Map(wid, hei);
                camera = new Camera(width, height, wid, hei);
                map.name = path.Remove(0, 13);
                map.name = map.name.Remove(map.name.Length - 3, 3);
                //for (int j = 0; j < gui.selectedName.Length - 13 - 3; j++)
                //{
                //    map.name += gui.selectedName[j + 13];
                //}
                this.Window.Title = map.name;
                for (int i = 0; i < map.height; i++)
                    for (int j = 0; j < map.width; j++)
                        map.mapData[i, j] = reader.ReadInt32();
                int bc = reader.ReadInt32();
                for (int i = 0; i < bc; i++)
                {
                    int x = reader.ReadInt32(), y = reader.ReadInt32(), team = reader.ReadInt32();
                    map.buidingData.Add(new Building(x, y, team));
                }
                int cc = reader.ReadInt32();
                for (int i = 0; i < cc; i++)
                {
                    int x = reader.ReadInt32(), y = reader.ReadInt32(), team = reader.ReadInt32();
                    map.castleData.Add(new Building(x, y, team));
                }
                int uc = reader.ReadInt32();
                unitManager = new UnitManager();
                for (int i = 0; i < uc; i++)
                {
                    unitManager.AddUnit(new Unit());
                    unitManager.units[i].id = reader.ReadInt32();
                    unitManager.units[i].name = reader.ReadString();
                    unitManager.units[i].male = reader.ReadBoolean();
                    unitManager.units[i].lvl = reader.ReadInt32();
                    unitManager.units[i].experience = reader.ReadInt32();
                    unitManager.units[i].position.X = reader.ReadSingle();
                    unitManager.units[i].position.Y = reader.ReadSingle();
                    unitManager.units[i].team = reader.ReadInt32();
                    unitManager.units[i].baseAttackMin = reader.ReadInt32();
                    unitManager.units[i].baseAttackMax = reader.ReadInt32();
                    unitManager.units[i].baseDefence = reader.ReadInt32();
                    unitManager.units[i].health = reader.ReadInt32();
                    unitManager.units[i].baseMoveRange = reader.ReadInt32();
                    unitManager.units[i].baseAttackRangeMin = reader.ReadInt32();
                    unitManager.units[i].baseAttackRangeMax = reader.ReadInt32();
                    unitManager.units[i].bonusAttack = reader.ReadInt32();
                    unitManager.units[i].bonusDefence = reader.ReadInt32();
                    unitManager.units[i].cost = reader.ReadInt32();
                    unitManager.units[i].animationActive = reader.ReadBoolean();
                    unitManager.units[i].moveEnd = reader.ReadBoolean();
                    int ec = reader.ReadInt32();
                    unitManager.units[i].effects = new UnitEffect[ec];
                    for (int j = 0; j < ec; j++)
                        unitManager.units[i].effects[j] = (UnitEffect)reader.ReadInt32();//writer.Write((int)u);
                    int pc = reader.ReadInt32();
                    unitManager.units[i].properties = new UnitProperty[pc];
                    for (int j = 0; j < pc; j++)
                        unitManager.units[i].properties[j] = (UnitProperty)reader.ReadInt32();//writer.Write((int)u);
                    if (unitManager.units[i].team == 1) camera.ToPositionOnMap((int)unitManager.units[i].position.X,(int) unitManager.units[i].position.Y);
                }

                //nowtileset = 0;
                //try
                //{
                //    nowtileset = reader.ReadInt32();
                //}
                //catch { ;}
                LoadTileset();
            }
            reader.Close();
            map.ClearAlphaData();

            tombs.Clear();
            //gameInfo = new GameInfo();
            playerManager.SetPlayers(map, unitManager, gameInfo.startmoney,gameInfo.playersTeams,gameInfo.playersStates);
            //gui.AddFloatMessage(map.name, 1, font);

            //gameInfo = new GameInfo();
            int id = map.name.IndexOf('_');
            scriptManager.LoadScript(map.name.Substring(0,id)+".aescript");
            //map.name = map.name.Remove(0, id + 1);

            //LoadTileset();

            if(music)MediaPlayer.Play(main1Sound);
        }

        void LoadTileset()
        {
            switch (nowtileset)
            {
                case 0: tileset = Content.Load<Texture2D>("Textures/tileset"); 
                        miniset = Content.Load<Texture2D>("Textures/minitileset");break;
                case 1: tileset = Content.Load<Texture2D>("Textures/tileset_winter");
                        miniset = Content.Load<Texture2D>("Textures/minitileset_winter"); break;
                case 2: tileset = Content.Load<Texture2D>("Textures/tileset_hotsummer");
                        miniset = Content.Load<Texture2D>("Textures/minitileset_hot"); break;
                case 3: tileset = Content.Load<Texture2D>("Textures/tileset_lava");
                        miniset = Content.Load<Texture2D>("Textures/minitileset_lava"); break;
                //default: tileset = Content.Load<Texture2D>("Textures/tileset"); 
                //         miniset = Content.Load<Texture2D>("Textures/minitileset");break;
            }
        }

        void CreateSubMenu(bool startmove)
        {
            if (gameInfo.selectedUnitId >= 0)
            {
                Unit u = unitManager.units[gameInfo.selectedUnitId];
                bool castle = map.IsCastleOnPosition(camera.positionOnMap.X, camera.positionOnMap.Y) ? map.castleData[map.castleData.IndexOf(map.GetCastle(camera.positionOnMap.X, camera.positionOnMap.Y))].fraction == gameInfo.nowplayer : false;
                bool canmove = false;
                if (u != null) { canmove = gameInfo.selectedUnitId != -1 ? !unitManager[gameInfo.selectedUnitId].HasProperty(UnitProperty.CanNotAttackAndMoveAtOnce) : false; }
                gui.elements[31].rect = new Rectangle(0, 0, 0, 0);

                gameInfo.submenu = new List<string>();
                if (startmove)
                    gameInfo.submenu.Add(Game.langManager[GameString.Move]);
                else
                    gameInfo.submenu.Add(Game.langManager[GameString.EndTurn]);

                if (castle && startmove)
                {
                    gui.elements[31].rect = new Rectangle((int)camera.mouse.X + 1, (int)camera.mouse.Y, 0, 0);
                    List<int> id = new List<int>();
                    if (!unitManager.IsLiveKingOfFraction(gameInfo.nowplayer)) id.Add(9);
                    for (int k = 0; k < 9; k++) id.Add(k);

                    gameInfo.submenu.Add(Game.langManager[GameString.Buy]);
                    gameInfo.buyid = 0;
                    gameInfo.castlePosition = new Vector2(u.position.X, u.position.Y);
                    gameInfo.BuySelectionUnit = Unit.GetByType(id[gameInfo.buyid], 0, 0, 0);
                }

                #region Attack after move
                unitManager.units[gameInfo.selectedUnitId].FillAttackRangeEx(ref map, (int)u.position.X, (int)u.position.Y, ref unitManager);
                for (int i = 0; i < map.height; i++)
                    for (int j = 0; j < map.width; j++)
                        if (map.alphaData[i, j] == 127)
                        {
                            Unit un = unitManager.GetByPosition(j, i);
                            if (un != null && playerManager.players[un.team].team != playerManager.players[gameInfo.nowplayer].team && !gameInfo.submenu.Contains(Game.langManager[GameString.Attack]) && canmove)
                            {
                                gameInfo.submenu.Add(Game.langManager[GameString.Attack]);
                                break;
                            }
                        }
                map.ClearAlphaData();
                #endregion

                #region Attack before move
                if (!canmove && startmove)
                {
                    u.FillAttackRangeEx(ref map, (int)u.position.X, (int)u.position.Y, ref unitManager);
                    for (int i = 0; i < map.height; i++)
                        for (int j = 0; j < map.width; j++)
                            if (map.alphaData[i, j] == 127)
                            {
                                Unit un = unitManager.GetByPosition(j, i);
                                if (un != null && playerManager.players[un.team].team != playerManager.players[gameInfo.nowplayer].team && !gameInfo.submenu.Contains(Game.langManager[GameString.Attack]))
                                {
                                    gameInfo.submenu.Add(Game.langManager[GameString.Attack]);
                                    break;
                                }
                            }
                    map.ClearAlphaData();

                    List<int> ids = map.GetVillagesOfNotFractions((int)unitManager[gameInfo.selectedUnitId].position.X, (int)unitManager[gameInfo.selectedUnitId].position.Y, unitManager[gameInfo.selectedUnitId].baseAttackRangeMin, unitManager[gameInfo.selectedUnitId].baseAttackRangeMax, unitManager[gameInfo.selectedUnitId].team, playerManager);
                    if (ids.Count > 0 && !gameInfo.submenu.Contains(Game.langManager[GameString.Attack])) gameInfo.submenu.Add(Game.langManager[GameString.Attack]);
                }
                #endregion

                #region Occupy
                if ((u.HasProperty(UnitProperty.CanCaptureVillages) && !gameInfo.submenu.Contains(Game.langManager[GameString.Occupy]) && map.IsVillageOnPosition((int)u.position.X, (int)u.position.Y) ? (playerManager.players[map.GetVillage((int)u.position.X, (int)u.position.Y).fraction].team != playerManager.players[gameInfo.nowplayer].team && map.mapData[(int)u.position.Y, (int)u.position.X] == (63 + 64)) : false) ||
                    (u.HasProperty(UnitProperty.CanCaptureCastles) && !gameInfo.submenu.Contains(Game.langManager[GameString.Occupy]) && map.IsCastleOnPosition((int)u.position.X, (int)u.position.Y) ? playerManager.players[map.GetCastle((int)u.position.X, (int)u.position.Y).fraction].team != playerManager.players[gameInfo.nowplayer].team : false))
                {
                    gameInfo.submenu.Add(Game.langManager[GameString.Occupy]);
                }
                #endregion

                #region Repair
                if (u.HasProperty(UnitProperty.CanCaptureVillages) && !gameInfo.submenu.Contains(Game.langManager[GameString.Occupy]) && map.IsVillageOnPosition((int)u.position.X, (int)u.position.Y) ? (map.GetVillage((int)u.position.X, (int)u.position.Y).fraction != gameInfo.nowplayer && map.mapData[(int)u.position.Y, (int)u.position.X] == (62 + 64)) : false)
                {
                    gameInfo.submenu.Add(Game.langManager[GameString.Repair]);
                }
                #endregion

                #region Raise
                if (u.HasProperty(UnitProperty.CanRaise))
                {
                    u.FillAttackRangeEx(ref map, (int)u.position.X, (int)u.position.Y, ref unitManager);
                    bool n = true;
                    for (int i = 0; i < map.height; i++)
                        for (int j = 0; j < map.width; j++)
                            if (map.alphaData[i, j] == 127 && n)
                            {
                                if (unitManager.GetByPosition(j, i) == null && n)
                                {
                                    foreach (Building t in tombs)
                                    {
                                        if (t.x == j && t.y == i && n)
                                        {
                                            if (!gameInfo.submenu.Contains(Game.langManager[GameString.Raise]) && n)
                                            {
                                                gameInfo.submenu.Add(Game.langManager[GameString.Raise]);
                                                n = false;
                                            }
                                        }
                                    }
                                }
                            }
                    map.ClearAlphaData();
                }
                #endregion

                gui.elements[31].rect.X = (int)(u.position.X * 64 - camera.position.X + camera.offset.X + 64);
                gui.elements[31].rect.Y = (int)(u.position.Y * 64 - camera.position.Y + camera.offset.Y);
                SetSubMenuSize();
            }
        }

        void SetSubMenuSize()
        {
            foreach (string s in gameInfo.submenu)
            {
                Vector2 v = font.MeasureString(s);
                v.Y += 2;
                v.X += 2;
                if (gui.elements[31].rect.Width < v.X)
                    gui.elements[31].rect.Width = (int)v.X;
                gui.elements[31].rect.Height += (int)v.Y;
                gui.elements[31].rect.Width += 2;
                gui.elements[31].rect.Height += 2;
            }

            if (gui.elements[31].rect.X + gui.elements[31].rect.Width > width - 200)
                gui.elements[31].rect.X = gui.elements[31].rect.X - gui.elements[31].rect.Width;
            if (gui.elements[31].rect.Y + gui.elements[31].rect.Height > height)
                gui.elements[31].rect.Y = gui.elements[31].rect.Y - gui.elements[31].rect.Height;
        }

        void Player1AIChanche(ref GuiObject me)
        {
            if (me.lclick)
            {
                switch (gameInfo.playersStates[1])
                {
                    case PlayerState.Net: gameInfo.playersStates[1] = PlayerState.None; break;
                    case PlayerState.Ai2: gameInfo.playersStates[1] = PlayerState.Net; break;
                    case PlayerState.Ai: gameInfo.playersStates[1] = PlayerState.Ai2; break;
                    case PlayerState.Player: gameInfo.playersStates[1] = PlayerState.Ai; break;
                    case PlayerState.None: gameInfo.playersStates[1] = PlayerState.Player; break;
                }
            }
            if (me.rclick)
            {
                switch (gameInfo.playersStates[1])
                {
                    case PlayerState.Ai2: gameInfo.playersStates[1] = PlayerState.Ai; break;
                    case PlayerState.Ai: gameInfo.playersStates[1] = PlayerState.Player; break;
                    case PlayerState.Player: gameInfo.playersStates[1] = PlayerState.None; break;
                    case PlayerState.None: gameInfo.playersStates[1] = PlayerState.Net; break;
                    case PlayerState.Net: gameInfo.playersStates[1] = PlayerState.Ai2; break;
                }
            }
            me.text = Game.langManager[GameString.Player1]+" - " + gameInfo.playersStates[1].ToString();
        }
        void Player2AIChanche(ref GuiObject me)
        {
            if (me.lclick)
            {
                switch (gameInfo.playersStates[2])
                {
                    case PlayerState.Net: gameInfo.playersStates[2] = PlayerState.None; break;
                    case PlayerState.Ai2: gameInfo.playersStates[2] = PlayerState.Net; break;
                    case PlayerState.Ai: gameInfo.playersStates[2] = PlayerState.Ai2; break;
                    case PlayerState.Player: gameInfo.playersStates[2] = PlayerState.Ai; break;
                    case PlayerState.None: gameInfo.playersStates[2] = PlayerState.Player; break;
                }
            }
            if (me.rclick)
            {
                switch (gameInfo.playersStates[2])
                {
                    case PlayerState.Ai2: gameInfo.playersStates[2] = PlayerState.Ai; break;
                    case PlayerState.Ai: gameInfo.playersStates[2] = PlayerState.Player; break;
                    case PlayerState.Player: gameInfo.playersStates[2] = PlayerState.None; break;
                    case PlayerState.None: gameInfo.playersStates[2] = PlayerState.Net; break;
                    case PlayerState.Net: gameInfo.playersStates[2] = PlayerState.Ai2; break;
                }
            }
            me.text = Game.langManager[GameString.Player2] + " - " + gameInfo.playersStates[2].ToString();
        }
        void Player3AIChanche(ref GuiObject me)
        {
            if (me.lclick)
            {
                switch (gameInfo.playersStates[3])
                {
                    case PlayerState.Net: gameInfo.playersStates[3] = PlayerState.None; break;
                    case PlayerState.Ai2: gameInfo.playersStates[3] = PlayerState.Net; break;
                    case PlayerState.Ai: gameInfo.playersStates[3] = PlayerState.Ai2; break;
                    case PlayerState.Player: gameInfo.playersStates[3] = PlayerState.Ai; break;
                    case PlayerState.None: gameInfo.playersStates[3] = PlayerState.Player; break;
                }
            }
            if (me.rclick)
            {
                switch (gameInfo.playersStates[3])
                {
                    case PlayerState.Ai2: gameInfo.playersStates[3] = PlayerState.Ai; break;
                    case PlayerState.Ai: gameInfo.playersStates[3] = PlayerState.Player; break;
                    case PlayerState.Player: gameInfo.playersStates[3] = PlayerState.None; break;
                    case PlayerState.None: gameInfo.playersStates[3] = PlayerState.Net; break;
                    case PlayerState.Net: gameInfo.playersStates[3] = PlayerState.Ai2; break;
                }
            }
            me.text = Game.langManager[GameString.Player3] + " - " + gameInfo.playersStates[3].ToString();
        }
        void Player4AIChanche(ref GuiObject me)
        {
            if (me.lclick)
            {
                switch (gameInfo.playersStates[4])
                {
                    case PlayerState.Net: gameInfo.playersStates[4] = PlayerState.None; break;
                    case PlayerState.Ai2: gameInfo.playersStates[4] = PlayerState.Net; break;
                    case PlayerState.Ai: gameInfo.playersStates[4] = PlayerState.Ai2; break;
                    case PlayerState.Player: gameInfo.playersStates[4] = PlayerState.Ai; break;
                    case PlayerState.None: gameInfo.playersStates[4] = PlayerState.Player; break;
                }
            }
            if (me.rclick)
            {
                switch (gameInfo.playersStates[4])
                {
                    case PlayerState.Ai2: gameInfo.playersStates[4] = PlayerState.Ai; break;
                    case PlayerState.Ai: gameInfo.playersStates[4] = PlayerState.Player; break;
                    case PlayerState.Player: gameInfo.playersStates[4] = PlayerState.None; break;
                    case PlayerState.None: gameInfo.playersStates[4] = PlayerState.Net; break;
                    case PlayerState.Net: gameInfo.playersStates[4] = PlayerState.Ai2; break;
                }
            }
            me.text = Game.langManager[GameString.Player4] + " - " + gameInfo.playersStates[4].ToString();
        }
        void Player1TeamChanche(ref GuiObject me)
        {
            if (me.lclick)
            {
                gameInfo.playersTeams[1]++;
                gameInfo.playersTeams[1] %= 5;
                if (gameInfo.playersTeams[1] == 0) gameInfo.playersTeams[1]++;
            }
            if (me.rclick)
            {
                gameInfo.playersTeams[1]--;
                if (gameInfo.playersTeams[1] == 0) gameInfo.playersTeams[1]=4;
            }
            me.text = Game.langManager[GameString.Player1Team] + " :" + gameInfo.playersTeams[1].ToString();
        }
        void Player2TeamChanche(ref GuiObject me)
        {
            if (me.lclick)
            {
                gameInfo.playersTeams[2]++;
                gameInfo.playersTeams[2] %= 5;
                if (gameInfo.playersTeams[2] == 0) gameInfo.playersTeams[2]++;
            }
            if (me.rclick)
            {
                gameInfo.playersTeams[2]--;
                if (gameInfo.playersTeams[2] == 0) gameInfo.playersTeams[2] = 4;
            }
            me.text = Game.langManager[GameString.Player2Team] + " :" + gameInfo.playersTeams[2].ToString();
        }
        void Player3TeamChanche(ref GuiObject me)
        {
            if (me.lclick)
            {
                gameInfo.playersTeams[3]++;
                gameInfo.playersTeams[3] %= 5;
                if (gameInfo.playersTeams[3] == 0) gameInfo.playersTeams[3]++;
            }
            if (me.rclick)
            {
                gameInfo.playersTeams[3]--;
                if (gameInfo.playersTeams[3] == 0) gameInfo.playersTeams[3] = 4;
            }
            me.text = Game.langManager[GameString.Player3Team] + " :" + gameInfo.playersTeams[3].ToString();
        }
        void Player4TeamChanche(ref GuiObject me)
        {
            if (me.lclick)
            {
                gameInfo.playersTeams[4]++;
                gameInfo.playersTeams[4] %= 5;
                if (gameInfo.playersTeams[4] == 0) gameInfo.playersTeams[4]++;
            }
            if (me.rclick)
            {
                gameInfo.playersTeams[4]--;
                if (gameInfo.playersTeams[4] == 0) gameInfo.playersTeams[4] = 4;
            }
            me.text = Game.langManager[GameString.Player4Team] + " :" + gameInfo.playersTeams[4].ToString();
        }
        void ChancheMoney(ref GuiObject me)
        {
            if (me.lclick)
            {
                gameInfo.startmoney += 50;
            }
            if (me.rclick)
            {
                gameInfo.startmoney = (gameInfo.startmoney - 50 < 0 ? 0 : (gameInfo.startmoney - 50));
            }
            me.text = Game.langManager[GameString.StartMoney] + ": " + gameInfo.startmoney.ToString();
        }

        void ChancheTileset(ref GuiObject me)
        {
            if (me.lclick)
            {
                nowtileset++;
                nowtileset %= 4;
            }
            if (me.rclick)
            {
                nowtileset--;
                nowtileset += 4;
                nowtileset %= 4;
            }
            switch (nowtileset)
            {
                case 0: me.text = Game.langManager[GameString.GreenTileset]; break;
                case 1: me.text = Game.langManager[GameString.WhileTileset]; break;
                case 2: me.text = Game.langManager[GameString.Green2Tileset]; break;
                case 3: me.text = Game.langManager[GameString.LavaTileset]; break;
                default: me.text = Game.langManager[GameString.Error]; break;

            }
        }

        #region Logic
        void EndTurn()
        {
            playerManager.players[gameInfo.nowplayer].Update(map, unitManager);
            int nextteam = gameInfo.nowplayer;
            do
            {
                nextteam ++;
                nextteam %= playerManager.players.Count;
                if (nextteam == 0) gameInfo.nowturn++;
            } while (nextteam == 0 || (playerManager.players[nextteam].unitnum <= 0 && playerManager.players[nextteam].castlenum <= 0) || playerManager.players[nextteam].state == PlayerState.None);

            foreach (Unit u in unitManager.units)
            {
                if (u.team == gameInfo.nowplayer) u.moveEnd = false;
                Building b = map.GetVillage((int)u.position.X, (int)u.position.Y);
                Building c = map.GetCastle((int)u.position.X, (int)u.position.Y);
                if (b != null && playerManager.players[b.fraction].team == playerManager.players[u.team].team && u.team == nextteam)
                {
                    int dh = u.health;
                    u.health += 20;
                    if (u.health > 100) u.health = 100;
                    dh = u.health - dh;
                    if (dh > 0)
                        particalManager.AddNewParticle(new TextParticle(1, dh.ToString(), new Vector2(u.position.X * 64 + 32, u.position.Y * 64 + 54), 1, new Color(143, 255, 117)));
                }
                if (c != null && playerManager.players[c.fraction].team == playerManager.players[u.team].team && u.team == nextteam)
                {
                    int dh = u.health;
                    u.health += 20;
                    if (u.health > 100) u.health = 100;
                    dh = u.health - dh;
                    if (dh > 0)
                        particalManager.AddNewParticle(new TextParticle(1, dh.ToString(), new Vector2(u.position.X * 64 + 32, u.position.Y * 64 + 54), 1, new Color(143, 255, 117)));
                }
                if (Map.TILES_IN_TERRAIN[map.mapData[(int)u.position.Y, (int)u.position.X]] == 7 && u.team == nextteam)
                {
                    int dh = u.health;
                    u.health += 20;
                    if (u.health > 100) u.health = 100;
                    dh = u.health - dh;
                    if (dh > 0)
                        particalManager.AddNewParticle(new TextParticle(1, dh.ToString(), new Vector2(u.position.X * 64 + 32, u.position.Y * 64 + 54), 1, new Color(143, 255, 117)));
                }
                u.onceReattaked = false;
            }

            gameInfo.nowplayer = nextteam;
            //if (gameInfo.nowplayer == 0) gameInfo.nowplayer++;

            for (int i = tombs.Count - 1; i >= 0; i--)
            {
                if (tombs[i].fraction == gameInfo.nowplayer) tombs.RemoveAt(i);
                else 
                if (tombs[i].fraction > 5 && tombs[i].fraction - 5 == gameInfo.nowplayer) tombs[i].fraction -= 5;
            }
            foreach (Unit u in unitManager.units)
            {
                if (u.team == gameInfo.nowplayer)
                {
                    u.effects = new UnitEffect[0];
                    u.bonusAttack = 0;
                    u.bonusDefence = 0;
                    camera.ToPositionOnMap((int)u.position.X, (int)u.position.Y);
                }
                if (u.HasEffect(UnitEffect.Light))
                    particalManager.AddNewParticle(new Particle(2, new Vector2(u.position.X * 64 + 32, u.position.Y * 64 + 32), 0.3f, false));
            }
            switch (gameInfo.nowplayer)
            {
                case 1: gui.AddFloatMessage(Game.langManager[GameString.BluePlayer], 1, font); break;
                case 2: gui.AddFloatMessage(Game.langManager[GameString.RedPlayer], 1, font); break;
                case 3: gui.AddFloatMessage(Game.langManager[GameString.GreenPlayer], 1, font); break;
                case 4: gui.AddFloatMessage(Game.langManager[GameString.BlackPlayer], 1, font); break;
            }
            Unit king = unitManager.GetKingOfFraction(gameInfo.nowplayer);
            if(king!=null)
                camera.ToPositionOnMap((int)king.position.X, (int)king.position.Y);
            //gameInfo.selectedUnitId = -1;

        }
        void AfterMove()
        {
            bool dosomething = false;
            if (gameInfo.selectedUnitId != -1)
            {
                Unit selu =unitManager[gameInfo.selectedUnitId];
                //unitManager.units[gameInfo.selectedUnitId].moveEnd = true;
                if (unitManager[gameInfo.selectedUnitId].HasProperty(UnitProperty.CanCaptureVillages) || unitManager[gameInfo.selectedUnitId].HasProperty(UnitProperty.CanCaptureCastles))
                {
                    dosomething = Repair();
                    if (!dosomething)
                        dosomething = Occupy();
                }

                #region Stay and light
                //if (!dosomething && !unitManager[gameInfo.selectedUnitId].moveEnd)
                //{
                if (gameInfo.selectedUnitId != -1)
                    if (unitManager.units[gameInfo.selectedUnitId].HasProperty(UnitProperty.Lighting))
                    {
                        for (int k = -2; k < 3; k++)
                            for (int j = -2; j < 3; j++)
                            {
                                if (Math.Abs(k + j) <= 2)
                                {
                                    int ui = unitManager.units.IndexOf(unitManager.GetByPosition(k + (int)unitManager[gameInfo.selectedUnitId].position.X, j + (int)unitManager[gameInfo.selectedUnitId].position.Y));
                                    if (ui != -1 && unitManager[ui].team == unitManager.units[gameInfo.selectedUnitId].team)
                                    {
                                        unitManager.units[ui].bonusDefence += 10;
                                        particalManager.AddNewParticle(new Particle(2, new Vector2(unitManager[ui].position.X * 64 + 32, unitManager[ui].position.Y * 64 + 32), 0.3f, false));
                                        List<UnitEffect> up = new List<UnitEffect>();
                                        foreach (UnitEffect upr in unitManager[ui].effects)
                                            up.Add(upr);
                                        up.Add(UnitEffect.Light);
                                        unitManager.units[ui].effects = up.ToArray();
                                    }
                                }
                            }
                    }

                #endregion

                #region Raise
                if (!dosomething && unitManager[gameInfo.selectedUnitId].HasProperty(UnitProperty.CanRaise))
                {
                    for (int i = tombs.Count - 1; i >= 0; i--)
                    {
                        if (Math.Abs(tombs[i].x - (int)selu.position.X) + Math.Abs(tombs[i].y - (int)selu.position.Y) == 1)
                        {
                            particalManager.AddNewParticle(new Particle(2, new Vector2(tombs[i].x * 64 + 16, tombs[i].y * 64 + 16), 0.5f,  false));
                            particalManager.AddNewParticle(new Particle(2, new Vector2(tombs[i].x * 64 + 48, tombs[i].y * 64 + 16), 0.5f,  false));
                            particalManager.AddNewParticle(new Particle(2, new Vector2(tombs[i].x * 64 + 48, tombs[i].y * 64 + 48), 0.5f,  false));
                            particalManager.AddNewParticle(new Particle(2, new Vector2(tombs[i].x * 64 + 16, tombs[i].y * 64 + 48), 0.5f,  false));
                            unitManager.AddUnit(new Unit(10, gameInfo.nowplayer, tombs[i].x, tombs[i].y));
                            unitManager.units[unitManager.units.IndexOf(unitManager.GetByPosition(tombs[i].x, tombs[i].y))].moveEnd = true;
                            particalManager.AddNewParticle(new Particle(1, new Vector2(tombs[i].x * 64 + 32, tombs[i].y * 64 + 32), 0.3f, false));
                            tombs.RemoveAt(i);
                            //particalManager.AddNewParticle(new Particle(1, new Vector2(tombs[i].x * 64 + 32, tombs[i].y*64 + 32), 0.3f,  false));

                            unitManager.units[gameInfo.selectedUnitId].moveEnd = true;
                            gameInfo.selectedUnitId = -1;
                            map.ClearAlphaData();
                            dosomething = true;
                            gameInfo.state = GameState2.None;

                            break;
                        }
                    }
                }
                #endregion

                #region Attack
                if (!dosomething && !unitManager[gameInfo.selectedUnitId].moveEnd)
                {
                    unitManager[gameInfo.selectedUnitId].FillAttackRangeEx(ref map, (int)unitManager[gameInfo.selectedUnitId].position.X, (int)unitManager[gameInfo.selectedUnitId].position.Y, ref unitManager);
                    bool attacked = false;
                    foreach (Unit u in unitManager.units)
                    {
                        if (map.IsInAttackRange((int)u.position.X, (int)u.position.Y) && playerManager.players[u.team].team != playerManager.players[unitManager[gameInfo.selectedUnitId].team].team && (unitManager[gameInfo.selectedUnitId].HasProperty(UnitProperty.CanNotAttackAndMoveAtOnce) ? (gameInfo.oldposition == unitManager[gameInfo.selectedUnitId].position) : true))
                        {
                            gameInfo.attackUnitId = unitManager.units.IndexOf(u);
                            AtackUnits(ref gameInfo.selectedUnitId, ref gameInfo.attackUnitId);
                            gameInfo.wait = true;
                            dosomething = true;
                            attacked = true;
                            break;
                        }
                    }

                    if (gameInfo.state != GameState2.Reattack&&attacked)
                        unitManager.units[gameInfo.selectedUnitId].moveEnd = true;
                    map.ClearAlphaData();

                    if (!dosomething&&unitManager[gameInfo.selectedUnitId].HasProperty(UnitProperty.CanDestroyVillages))
                    {
                        List<int> ids = map.GetVillagesOfNotFractions((int)unitManager[gameInfo.selectedUnitId].position.X, (int)unitManager[gameInfo.selectedUnitId].position.Y, unitManager[gameInfo.selectedUnitId].baseAttackRangeMin, unitManager[gameInfo.selectedUnitId].baseAttackRangeMax, unitManager[gameInfo.selectedUnitId].team, playerManager);
                        if (ids.Count > 0)
                        {
                            int i = new Random().Next(ids.Count - 1);
                            //if (map.buidingData[map.buidingData.IndexOf(map.GetVillage(camera.positionOnMap.X, camera.positionOnMap.Y))].fraction != 0)
                                playerManager.players[map.buidingData[i].fraction].villagenum--;
                            map.buidingData[i].fraction = 0;
                            map.mapData[map.buidingData[i].y, map.buidingData[i].x] = 62+64;
                            particalManager.AddNewParticle(new Particle(1, new Vector2(map.buidingData[i].x * 64 + 32, map.buidingData[i].y * 64 + 32), 0.3f, false));
                        }
                    }
                }
                #endregion


                if (!dosomething)
                {
                    unitManager.units[gameInfo.selectedUnitId].moveEnd = true;
                    gameInfo.selectedUnitId = -1;
                    gameInfo.state = GameState2.None;
                }
            }
        }

        bool Occupy()
        {
            bool r = false;
            Unit u = unitManager[gameInfo.selectedUnitId];
            //try
            //{
            int bi = map.buidingData.IndexOf(map.GetVillage((int)u.position.X, (int)u.position.Y));
            int ci = map.castleData.IndexOf(map.GetCastle((int)u.position.X, (int)u.position.Y));
            if (bi != -1&&u.HasProperty(UnitProperty.CanCaptureVillages)&&playerManager.players[map.buidingData[bi].fraction].team!=playerManager.players[gameInfo.nowplayer].team)
            {
                if (map.buidingData[bi].fraction != 0)
                    playerManager.players[map.buidingData[bi].fraction].villagenum--;
                map.buidingData[bi].fraction = gameInfo.nowplayer;
                playerManager.players[gameInfo.nowplayer].villagenum++;
                r = true;
            }
            //}
            //catch { ;}
            //try
            //{
            if (ci != -1 && u.HasProperty(UnitProperty.CanCaptureCastles) && playerManager.players[map.castleData[ci].fraction].team != playerManager.players[gameInfo.nowplayer].team)
            {
                if (map.castleData[ci].fraction != 0)
                    playerManager.players[map.castleData[ci].fraction].castlenum--;
                map.castleData[ci].fraction = gameInfo.nowplayer;
                playerManager.players[gameInfo.nowplayer].castlenum++;
                r = true;
            }
            //}
            //catch { ;}
            if (r)
            {
                unitManager[gameInfo.selectedUnitId].moveEnd = true;
                gameInfo.selectedUnitId = -1;
                gameInfo.state = GameState2.None;
            }
            return r;
        }
        bool AtackUnits(ref int id1, ref int id2)
        {
            bool attackdone;
            int oldlvl = unitManager.units[id1].lvl;
            Unit s = unitManager[id1];
            int damage = unitManager.units[id1].Attack(ref unitManager, id2, ref map, true);
            particalManager.AddNewParticle(new Particle(1, new Vector2(unitManager[id2].position.X * 64 + 32, unitManager[id2].position.Y * 64 + 32), 0.3f, false));
            particalManager.AddNewParticle(new TextParticle(1, damage.ToString(), new Vector2(unitManager[id2].position.X * 64 + 32, unitManager[id2].position.Y * 64 + 54), 1, new Color(254, 255, 205)));

            if (oldlvl != unitManager[id1].lvl)
            {
                particalManager.AddNewParticle(new TextParticle(0, Game.langManager[GameString.LevelUp], new Vector2(unitManager[id1].position.X * 64 + 32 + 5, unitManager[id1].position.Y * 64 + 32), 2, new Color(254, 255, 205)));
                particalManager.AddNewParticle(new Particle(5, new Vector2(unitManager[id1].position.X * 64 + 32 - font.MeasureString(Game.langManager[GameString.LevelUp]).X / 2 - 5, unitManager[id1].position.Y * 64 + 32), 2, true));
            }

            if (unitManager[id2].health <= 0)
            {
                if (!unitManager[id2].HasProperty(UnitProperty.King) && !unitManager[id2].HasProperty(UnitProperty.SkeletonAndCanNotRaised))
                    tombs.Add(new Building((int)unitManager[id2].position.X, (int)unitManager[id2].position.Y, unitManager[id1].team+5));
                particalManager.AddNewParticle(new Particle(2, new Vector2(unitManager[id2].position.X * 64 + 32, unitManager[id2].position.Y * 64 + 32), 0.3f, false));
                s = unitManager[id1];
                unitManager[id1].moveEnd = true;
                playerManager.players[unitManager[id2].team].unitnum--;
                particalManager.AddNewParticle(new Particle(4, unitManager[id2].position * 64 + new Vector2(32, 40), 1, true));
                unitManager.units.RemoveAt(id2);
                //particalManager.AddNewParticle(new Particle(0, unitManager[id2].position*64+new Vector2(32,64),1,4,true));
            }
            else
            {
                if ((unitManager[id2].position - unitManager[id1].position).Length() < 1.1f && unitManager[id2].baseAttackRangeMin == 1)
                {
                    if (id2 != -1 && !unitManager[id2].onceReattaked)
                    {
                        gameInfo.state = GameState2.Reattack;
                        gameInfo.attackUnitId = id2;
                        unitManager[id2].onceReattaked = true;
                    }
                }
                //else
                //{
                s = unitManager[id1];
                unitManager[id1].moveEnd = true;
                //}
            }
            //else
            //{
            //s = unitManager[id1];
            //unitManager[id1].moveEnd = true;
            //}
            id1 = unitManager.units.IndexOf(s);
            attackdone = true;
            if (id1 >= 0)
                if (unitManager.units[id1].HasProperty(UnitProperty.Lighting))
                {
                    for (int k = -2; k < 3; k++)
                        for (int j = -2; j < 3; j++)
                        {
                            if (Math.Abs(k + j) <= 2)
                            {
                                int ui = unitManager.units.IndexOf(unitManager.GetByPosition(k + (int)unitManager[id1].position.X, j + (int)unitManager[id1].position.Y));
                                if (ui != -1 && unitManager[ui].team == unitManager.units[id1].team)
                                {
                                    unitManager.units[ui].bonusDefence += 10;
                                    particalManager.AddNewParticle(new Particle(2, new Vector2(unitManager[ui].position.X * 64 + 32, unitManager[ui].position.Y * 64 + 32), 0.3f, false));
                                    List<UnitEffect> up = new List<UnitEffect>();
                                    foreach (UnitEffect upr in unitManager[ui].effects)
                                        up.Add(upr);
                                    up.Add(UnitEffect.Light);
                                    unitManager.units[ui].effects = up.ToArray();

                                }
                            }
                        }
                }
            return attackdone;
        }
        bool Reattack(ref int id1, ref int id2)
        {
            int oldlvl = unitManager.units[id1].lvl;
            int damage=unitManager.units[id1].Attack(ref unitManager, id2, ref map,false);
            particalManager.AddNewParticle(new Particle(1, new Vector2(unitManager[id2].position.X * 64 + 32, unitManager[id2].position.Y * 64 + 32), 0.3f, false));
            particalManager.AddNewParticle(new TextParticle(1, damage.ToString(), new Vector2(unitManager[id2].position.X * 64 + 32, unitManager[id2].position.Y * 64 + 54), 1, new Color(254, 255, 205)));

            if (oldlvl != unitManager[id1].lvl)
            {
                particalManager.AddNewParticle(new TextParticle(0, Game.langManager[GameString.LevelUp], new Vector2(unitManager[id1].position.X * 64 + 32 + 5, unitManager[id1].position.Y * 64 + 32), 2, new Color(254, 255, 205)));
                particalManager.AddNewParticle(new Particle(5, new Vector2(unitManager[id1].position.X * 64 + 32 - font.MeasureString(Game.langManager[GameString.LevelUp]).X / 2 - 5, unitManager[id1].position.Y * 64 + 32), 2, true));
            }

            if (unitManager[id2].health <= 0)
            {
                tombs.Add(new Building((int)unitManager[id2].position.X, (int)unitManager[id2].position.Y, unitManager[id2].team+5));
                particalManager.AddNewParticle(new Particle(2, new Vector2(unitManager[id2].position.X * 64 + 32, unitManager[id2].position.Y * 64 + 32), 0.3f, false));
                particalManager.AddNewParticle(new Particle(4, unitManager[id2].position * 64 + new Vector2(32, 40), 1, true));
                playerManager.players[unitManager[id2].team - 1].unitnum--;
                unitManager.units.RemoveAt(id2);
            }
            else
                unitManager.units[id2].moveEnd = true;

            gameInfo.state = GameState2.None;
            id1 = -1;
            id2 = -1;
            return true;
        }
        bool Repair()
        {
            bool r = false;
            Unit u = unitManager[gameInfo.selectedUnitId];
            if(u!=null)
            {
                if (map.mapData[(int)u.position.Y, (int)u.position.X] == (62+64))
                {
                    map.mapData[(int)u.position.Y, (int)u.position.X] = 63+64;
                    //map.buidingData[map.buidingData.IndexOf(map.GetVillage((int)u.position.X, (int)u.position.Y))].fraction = gameInfo.nowplayer;
                    map.buidingData[map.buidingData.IndexOf(map.GetVillage((int)u.position.X, (int)u.position.Y))].fraction = 0;
                    r = true;
                }
            }
            if (r)
            {
                unitManager[gameInfo.selectedUnitId].moveEnd = true;
                gameInfo.selectedUnitId = -1;
                gameInfo.state = GameState2.None;
            }
            return r;
        }

        void SetPath(Point start, Point end)
        {
            List<Point> p = new List<Point>();
            p = Unit.GetPathFromPointToPoint(ref map, start, end);
            gameInfo.path = new List<Vector2>();
            int sp = unitManager[gameInfo.selectedUnitId].baseMoveRange;
            map.UpdateFractionData(unitManager);
            for (int i = 0; i < p.Count; i++)
            {
                if (sp > 0)
                    gameInfo.path.Add(new Vector2(p[i].X, p[i].Y));
                else break;
                int k = i + 1;
                if (k >= p.Count) k--;
                sp -= unitManager[gameInfo.selectedUnitId].GetRecSteps(p[k].X, p[k].Y, map);
            }
            if(gameInfo.path.Count>0) { gameInfo.path.RemoveAt(0); }

            for (int i = gameInfo.path.Count - 1; i >= 0; i--)
            {
                if (map.fractionUnitData[(int)gameInfo.path[i].Y, (int)gameInfo.path[i].X] != -1)
                    gameInfo.path.RemoveAt(i);
                else break;
            }
        }
        int NearUnitOn(int x, int y, int fraction)
        {
            int id = -1;
            int lenchto = 1000;
            for (int i = 0; i < unitManager.units.Count; i++)
            {
                int l = Math.Abs((int)unitManager[i].position.X - x) + Math.Abs((int)unitManager[i].position.Y - y);
                if ((id == -1 || l < lenchto) && unitManager[i].team != fraction) { id = i; lenchto = l; }
            }
            return id;
        }
        bool CanBuyUnit(int type, int x, int y,ref PlayerManager pm)
        { 
            Unit u =Unit.GetByType(type,gameInfo.nowplayer,x,y);
            if (unitManager.countUnits(-1, gameInfo.nowplayer) - unitManager.countUnits(10, gameInfo.nowplayer) < 30 && type <= gameInfo.alooweUnit && u.cost <= playerManager.players[gameInfo.nowplayer].gold && u.cost >= 0)
            {
                map.ClearAlphaData();
                u.FillMoveRangeEx(ref map, x, y, u.baseMoveRange, ref unitManager,ref pm);
                int c = map.counterOfMoveCells();
                map.ClearAlphaData();
                return c > 1;
            }
            return false;
        }
        #endregion

        #region Scripts
        public bool UpdateScript(GameTime time,ref ScriptManager me)
        {
            if (me.scriptloaded&&gameState==GameState.Game)
            {
                do
                {
                    if (me.waitTime > 0)
                    {
                        me.waitTime -= (float)time.ElapsedGameTime.TotalSeconds;
                        gameInfo.wait = me.waitTime > 0;
                        return true;
                    }
                    else if (me.nowstring < me.strings.Length&&!gui.dialogShowed)
                    {
                        if (gameInfo.mainFraction >= 0 && map.countCastlesOfFraction(gameInfo.mainFraction) == 0 && !unitManager.IsLiveKingOfFraction(gameInfo.mainFraction))
                        {
                            gui.AddFloatMessage(Game.langManager[GameString.MissionFailed], -1, font);
                            return false;
                        }
                        string command = me.strings[me.nowstring];
                        //me.nextstring = 1;
                        if (command == "")
                        {
                            me.nowstring++;
                        }
                        else
                        {
                            if (command.Contains("@Case"))
                            {
                                command = command.Remove(0, 6);
                                int i = Convert.ToInt32(command);
                                if (i != me.nowcase) me.nowstring = me.GetCaseString(me.nowcase);
                            }
                            else if (command == "NextState")
                            {
                                me.nowcase++;
                                me.nowstring = me.GetCaseString(me.nowcase);
                            }
                            else if (command == "ShowMapName")
                            {
                                int id = map.name.IndexOf('_');
                                gui.AddFloatMessage(map.name.Substring(id + 1, map.name.Length - 1 - id), 1, font);
                            }
                            else if (command.Contains("Wait"))
                            {
                                command = command.Remove(0, 5);
                                float i = Convert.ToSingle(command);
                                me.waitTime = i;
                            }
                            else if (command == "StartPlay") me.gameStarted = true;
                            else if (command == "StopPlay") me.gameStarted = false;
                            else if (command == "CompleteMission")
                            {
                                gui.AddFloatMessage(Game.langManager[GameString.MissionComplate], 1, 3, font);
                                if(music)MediaPlayer.Play(winSound);
                                me.nowstring++;
                                return true;
                            }
                            else if (command == "FailMission")
                            {
                                gui.AddFloatMessage(Game.langManager[GameString.MissionFailed], -1, font);
                                me.nowstring++;
                                return true;
                            }
                            else if (command == "CompleteBattle")
                            {
                                gui.AddFloatMessage(Game.langManager[GameString.Team] + " " + playerManager.players[gameInfo.nowplayer].team.ToString() + " " + Game.langManager[GameString.Win], -1, font);
                                me.nowstring++;
                                if (music) MediaPlayer.Play(winSound);
                                return true;
                            }
                            #region Test
                            else if (command.Contains("Test"))
                            {
                                command = command.Remove(0, 5);
                                float a = 0, b;
                                if (command.Contains("AlphaMap"))
                                {
                                    command = command.Remove(0, 9);
                                    a = map.counterOfMoveCells();
                                }
                                if (command.Contains("SubMenu"))
                                {
                                    command = command.Remove(0, 8);
                                    a = gameInfo.submenu.Count == 0 ? 0 : 1;
                                }
                                if (command.Contains("CurrentPlayer"))
                                {
                                    command = command.Remove(0, 14);
                                    a = gameInfo.nowplayer;
                                }
                                else if (command.Contains("CurrentTurn"))
                                {
                                    command = command.Remove(0, 12);
                                    a = gameInfo.nowturn;
                                }
                                else if (command.Contains("GameState"))
                                {
                                    command = command.Remove(0, 10);
                                    a = (int)gameInfo.state;
                                }
                                else if (command.Contains("MessageOnScreen"))
                                {
                                    command = command.Remove(0, 15);
                                    a = (gui.floatMessageTime != -1 || gui.floatMessageTime > 0) ? 1 : 0;
                                }
                                else if (command.Contains("UnitInMove"))
                                {
                                    command = command.Remove(0, 10);
                                    a = (gameInfo.state == GameState2.Move && gameInfo.path.Count > 0) ? 1 : 0;
                                }
                                else if (command.Contains("CountVillages"))
                                {
                                    command = command.Remove(0, 14);
                                    int id = command.IndexOf(' ');
                                    string substring = command.Substring(0, id);
                                    a = map.countBuildingOfFraction(int.Parse(substring));
                                    command = command.Remove(0, id + 1);
                                }
                                else if (command.Contains("CountCastles"))
                                {
                                    command = command.Remove(0, 13);
                                    int id = command.IndexOf(' ');
                                    string substring = command.Substring(0, id);
                                    a = map.countCastlesOfFraction(int.Parse(substring));
                                    command = command.Remove(0, id + 1);
                                }
                                else if (command.Contains("CountUnitsOfEnemyTeam"))
                                {
                                    command = command.Remove(0, 22);
                                    int id = command.IndexOf(' ');
                                    string substring = command.Substring(0, id);
                                    int k = int.Parse(substring);
                                    command = command.Remove(0, id + 1);
                                    id = command.IndexOf(' ');
                                    substring = command.Substring(0, id);
                                    int k2;
                                    if (substring == "NowPlayerTeam")
                                        k2 = playerManager.players[gameInfo.nowplayer].team;
                                    else k2 = int.Parse(substring);
                                    command = command.Remove(0, id + 1);
                                    a = unitManager.countUnitsOfEnemyTeam(k, k2, playerManager);
                                }
                                else if (command.Contains("CountUnitsOfTeam"))
                                {
                                    command = command.Remove(0, 17);
                                    int id = command.IndexOf(' ');
                                    string substring = command.Substring(0, id);
                                    int k = int.Parse(substring);
                                    command = command.Remove(0, id + 1);
                                    id = command.IndexOf(' ');
                                    substring = command.Substring(0, id);
                                    int k2;
                                    if (substring == "NowPlayerTeam")
                                        k2 = playerManager.players[gameInfo.nowplayer].team;
                                    else k2 = int.Parse(substring);
                                    command = command.Remove(0, id + 1);
                                    a = unitManager.countUnitsOfTeam(k, k2, playerManager);
                                }
                                else if (command.Contains("CountUnits "))
                                {
                                    command = command.Remove(0, 11);
                                    int id = command.IndexOf(' ');
                                    int k = int.Parse(command.Substring(0, id));
                                    command = command.Remove(0, id + 1);
                                    id = command.IndexOf(' ');
                                    int k2 = int.Parse(command.Substring(0, id));
                                    command = command.Remove(0, id + 1);
                                    a = unitManager.countUnits(k, k2);
                                }
                                else if (command.Contains("CountOfEnableUnits"))
                                {
                                    command = command.Remove(0, 19);
                                    int id = command.IndexOf(' ');
                                    int k = int.Parse(command.Substring(0, id));
                                    command = command.Remove(0, id + 1);
                                    id = command.IndexOf(' ');
                                    int k2 = int.Parse(command.Substring(0, id));
                                    command = command.Remove(0, id + 1);
                                    a = unitManager.countEnableUnits(k, k2);
                                }
                                else if (command.Contains("CountUnitsInRange"))
                                {
                                    command = command.Remove(0, 18);
                                    int id = command.IndexOf(' ');
                                    string substring = command.Substring(0, id);
                                    int x1 = int.Parse(substring);
                                    command = command.Remove(0, id + 1);
                                    id = command.IndexOf(' ');
                                    substring = command.Substring(0, id);
                                    int y1 = int.Parse(substring);
                                    command = command.Remove(0, id + 1);
                                    id = command.IndexOf(' ');
                                    substring = command.Substring(0, id);
                                    int x2 = int.Parse(substring);
                                    command = command.Remove(0, id + 1);
                                    id = command.IndexOf(' ');
                                    substring = command.Substring(0, id);
                                    int y2 = int.Parse(substring);
                                    command = command.Remove(0, id + 1);
                                    a = unitManager.countUnitsInRange(x1, y1, x2, y2);
                                    //if (a > 0)
                                      //  y2 = 0;
                                }
                                int spid = command.LastIndexOf(' ');
                                b = float.Parse(command.Substring(spid));
                                command = command.Remove(spid, command.Length - spid);
                                if (TestScript(command, a, b))
                                {
                                    //me.nextstring = me.nowstring + 1;
                                }
                                else
                                {
                                    if (me.nextstring == -1)
                                        me.nowstring = me.GetCaseString(me.nowcase);
                                    else
                                    {
                                        me.nowstring += me.nextstring;
                                        return false;
                                    }
                                }
                                me.nextstring = -1;
                            }
                            #endregion
                            else if (command.Contains("Jump"))
                            {
                                command = command.Remove(0, 5);
                                me.nowstring += int.Parse(command);
                            }
                            else if (command.Contains("Alt"))
                            {
                                command = command.Remove(0, 4);
                                me.nextstring = int.Parse(command);
                            }
                            else if (command.Contains("SetGameActive"))
                            {
                                command = command.Remove(0, 14);
                                gameInfo.onlyUpdateUnits = !bool.Parse(command);
                            }
                            else if (command.Contains("SetCursorVisible"))
                            {
                                command = command.Remove(0, 17);
                                gameInfo.cursorVisible = bool.Parse(command);
                            }
                            else if (command.Contains("SetMainFraction"))
                            {
                                command = command.Remove(0, 16);
                                gameInfo.mainFraction = int.Parse(command);
                            }
                            else if (command.Contains("SetMapCursorVisible"))
                            {
                                command = command.Remove(0, 20);
                                gameInfo.mapcursorVisible = bool.Parse(command);
                            }
                            else if (command.Contains("ShowDialog"))
                            {
                                command = command.Remove(0, 11);
                                int id = command.IndexOf(' ');
                                string substring = command.Substring(0, id);
                                int dialogpers = int.Parse(substring);
                                command = command.Remove(0, id + 1);
                                //int dialogpers = int.Parse(command);
                                me.nowstring++;
                                string str = command;
                                id = -1;
                                if (int.TryParse(command, out id))
                                    str = langManager.dialogs[int.Parse(command)];
                                gui.AddDialog(dialogpers, AlignText(str, width - 400), font);
                                return false;
                                //gameInfo.mapcursorVisible = bool.Parse(command);
                            }
                            else if (command.Contains("ShowHelp"))
                            {
                                command = command.Remove(0, 9);
                                //int id = int.Parse(command);
                                string str = command;
                                int id = -1;
                                if (int.TryParse(command, out id))
                                    str = langManager.dialogs[int.Parse(command)];
                                gui.AddDialog(-1, AlignText(str, width - 250), font);
                                me.nowstring++;
                                return false;
                            }
                            else if (command.Contains("ShowMessage"))
                            {
                                command = command.Remove(0, 12);
                                int id = command.IndexOf(' ');
                                string substring = command.Substring(0, id);
                                float x = float.Parse(substring);
                                command = command.Remove(0, id + 1);
                                string str = command;
                                id = -1;
                                if (int.TryParse(command, out id))
                                    str = langManager.dialogs[int.Parse(command)];
                                gui.AddFloatMessage(str, 1, x, font);
                                me.nowstring++;
                                return false;
                            }
                            else if (command.Contains("MoveMapAndCursor"))
                            {
                                command = command.Remove(0, 17);
                                if (command.Contains("king"))
                                {
                                    command = command.Remove(0, 5);
                                    int id = int.Parse(command);
                                    Unit king = unitManager.GetKingOfFraction(id);
                                    if (king != null)
                                        camera.ToPositionOnMap((int)king.position.X, (int)king.position.Y);
                                }
                                else
                                {
                                    //command = command.Remove(0, 11);
                                    int id = command.IndexOf(' ');
                                    string substring = command.Substring(0, id);
                                    int x = int.Parse(substring);
                                    command = command.Remove(0, id + 1);
                                    int y = int.Parse(command);
                                    camera.ToPositionOnMap((int)x, (int)y);
                                }
                            }
                            else if (command.Contains("GetUnit "))
                            {
                                command = command.Remove(0, 8);
                                int id = command.IndexOf(' ');
                                string substring = command.Substring(0, id);
                                int x = int.Parse(substring);
                                command = command.Remove(0, id + 1);
                                int y = int.Parse(command);
                                gameInfo.selectedUnitId = unitManager.GetIdByPosition(x, y);
                            }
                            else if (command.Contains("GetUnitById"))
                            {
                                command = command.Remove(0, 12);
                                int id = command.IndexOf(' ');
                                string substring = command.Substring(0, id);
                                int i = int.Parse(substring);
                                command = command.Remove(0, id + 1);
                                int fr = int.Parse(command);
                                gameInfo.selectedUnitId = unitManager.GetIds(i, fr)[0];
                            }
                            else if (command.Contains("RemoveUnit"))
                            {
                                if (gameInfo.selectedUnitId >= 0)
                                {
                                    playerManager.players[unitManager[gameInfo.selectedUnitId].team].unitnum--;
                                    unitManager.units.RemoveAt(gameInfo.selectedUnitId);
                                    gameInfo.selectedUnitId = -1;
                                    gameInfo.state = GameState2.None;
                                }
                            }
                            else if (command.Contains("CreateSpriteAtUnit"))
                            {
                                command = command.Remove(0, 19);
                                if (gameInfo.selectedUnitId >= 0)
                                {
                                    //command = command.Remove(0, 8);
                                    int id = command.IndexOf(' ');
                                    string substring = command.Substring(0, id);
                                    int spid = 0;
                                    if (substring == "Smoke") spid = 4;
                                    else if (substring == "Spark") spid = 2;
                                    else if (substring == "RedSpark") spid = 1;
                                    command = command.Remove(0, id + 1);
                                    id = command.IndexOf(' ');
                                    substring = command.Substring(0, id);
                                    int x = int.Parse(substring);
                                    command = command.Remove(0, id + 1);
                                    int y = int.Parse(command);
                                    particalManager.AddNewParticle(new Particle(spid, unitManager[gameInfo.selectedUnitId].position * 64 + new Vector2(32 + x, 32 + y + ((spid == 4) ? 8 : 0)), 0.7f, spid == 4));
                                }
                            }
                            else if (command.Contains("GetUnitPlotRoute"))
                            {
                                command = command.Remove(0, 17);
                                int id = command.IndexOf(' ');
                                string substring = command.Substring(0, id);
                                int x = int.Parse(substring);
                                command = command.Remove(0, id + 1);
                                id = command.IndexOf(' ');
                                substring = command.Substring(0, id);
                                int y = int.Parse(substring);
                                command = command.Remove(0, id + 1);
                                id = command.IndexOf(' ');
                                substring = command.Substring(0, id);
                                int px = int.Parse(substring);
                                command = command.Remove(0, id + 1);
                                id = command.IndexOf(' ');
                                substring = command.Substring(0, id);
                                int py = int.Parse(substring);
                                command = command.Remove(0, id + 1);
                                bool ignoreterrain = bool.Parse(command);
                                gameInfo.selectedUnitId = unitManager.GetIdByPosition(x, y);
                                if (gameInfo.selectedUnitId >= 0)
                                {
                                    unitManager.units[gameInfo.selectedUnitId].FillMoveRange(ref map, ref unitManager, ref playerManager, ignoreterrain);
                                    SetPath(new Point(x, y), new Point(px, py));
                                    gameInfo.wait = true;
                                    gameInfo.state = GameState2.Move;
                                    map.ClearAlphaData();
                                }
                            }
                            else if (command.Contains("CreateUnitPlotRoute"))
                            {
                                command = command.Remove(0, 20);
                                int id = command.IndexOf(' ');
                                string substring = command.Substring(0, id);
                                int type = int.Parse(substring);
                                command = command.Remove(0, id + 1);
                                id = command.IndexOf(' ');
                                substring = command.Substring(0, id);
                                int team = int.Parse(substring);
                                command = command.Remove(0, id + 1);
                                id = command.IndexOf(' ');
                                substring = command.Substring(0, id);
                                int x = int.Parse(substring);
                                command = command.Remove(0, id + 1);
                                id = command.IndexOf(' ');
                                substring = command.Substring(0, id);
                                int y = int.Parse(substring);
                                command = command.Remove(0, id + 1);
                                id = command.IndexOf(' ');
                                substring = command.Substring(0, id);
                                int px = int.Parse(substring);
                                command = command.Remove(0, id + 1);
                                id = command.IndexOf(' ');
                                substring = command.Substring(0, id);
                                int py = int.Parse(substring);
                                command = command.Remove(0, id + 1);
                                bool ignoreterrain = bool.Parse(command);
                                unitManager.AddUnit(new Unit(type, team, x, y));
                                gameInfo.selectedUnitId = unitManager.units.Count - 1;
                                unitManager.units[gameInfo.selectedUnitId].FillMoveRange(ref map, ref unitManager, ref playerManager, ignoreterrain);
                                playerManager.players[unitManager[gameInfo.selectedUnitId].team].unitnum++;
                                SetPath(new Point(x, y), new Point(px, py));
                                gameInfo.wait = true;
                                gameInfo.state = GameState2.Move;
                                map.ClearAlphaData();
                                //gameInfo.selectedUnitId = unitManager.GetIdByPosition(x, y);
                            }
                            else if (command.Contains("LoadMap"))
                            {
                                command = command.Remove(0, 8);
                                LoadGame("Content/Maps/" + command);
                                return false;
                            }
                            else if (command.Contains("ClearUnit"))
                            {
                                gameInfo.selectedUnitId = -1;
                                gameInfo.state = GameState2.None;
                                map.ClearAlphaData();
                            }
                            else if (command.Contains("LoadScript"))
                            {
                                command = command.Remove(0, 11);
                                scriptManager.LoadScript("Content/Scripts/" + command);
                                return false;
                            }
                            else if (command.Contains("SetUnitSpeed"))
                            {
                                command = command.Remove(0, 13);
                                Unit.unitSpeed = float.Parse(command);
                                if (Unit.unitSpeed <= 0) Unit.unitSpeed = 0.05f;
                            }
                            else if (command.Contains("Vibrate"))
                            {
                                if (gameInfo.selectedUnitId >= 0)
                                    unitManager.units[gameInfo.selectedUnitId].vibrate = true;
                            }
                            else if (command.Contains("UnVibrate"))
                            {
                                if (gameInfo.selectedUnitId >= 0)
                                    unitManager.units[gameInfo.selectedUnitId].vibrate = false;
                            }
                            else if (command.Contains("SetColor"))
                            {
                                command = command.Remove(0, 9);
                                int id = command.IndexOf(' ');
                                string substring = command.Substring(0, id);
                                int r = int.Parse(substring);
                                command = command.Remove(0, id + 1);
                                id = command.IndexOf(' ');
                                substring = command.Substring(0, id);
                                int g = int.Parse(substring);
                                command = command.Remove(0, id + 1);
                                int b = int.Parse(command);
                                lightColor = new Color(r, g, b);
                            }
                            else if (command.Contains("SetEnableUnits"))
                            {
                                command = command.Remove(0, 15);
                                gameInfo.alooweUnit = int.Parse(command);
                                if (gameInfo.alooweUnit > 8) gameInfo.alooweUnit = 8;
                                if (gameInfo.alooweUnit < 1) gameInfo.alooweUnit = 1;
                            }
                            else if (command.Contains("SetPlayerAI"))
                            {
                                command = command.Remove(0, 12);
                                int id = command.IndexOf(' ');
                                string substring = command.Substring(0, id);
                                int player = int.Parse(substring);
                                command = command.Remove(0, id + 1);
                                int i = int.Parse(command);
                                playerManager.players[player].state = (PlayerState)i;
                            }
                            else if (command.Contains("SetPlayerGold"))
                            {
                                command = command.Remove(0, 14);
                                int id = command.IndexOf(' ');
                                string substring = command.Substring(0, id);
                                int player = int.Parse(substring);
                                command = command.Remove(0, id + 1);
                                int i = int.Parse(command);
                                playerManager.players[player].gold = i;
                            }
                            else if (command.Contains("AddPlayerGold"))
                            {
                                command = command.Remove(0, 14);
                                int id = command.IndexOf(' ');
                                string substring = command.Substring(0, id);
                                int player = int.Parse(substring);
                                command = command.Remove(0, id + 1);
                                int i = int.Parse(command);
                                playerManager.players[player].gold += i;
                            }
                            else if (command.Contains("Goto"))
                            {
                                command = command.Remove(0, 5);
                                me.nowstring = me.GetCaseString(int.Parse(command));
                            }
                            else if (command.Contains("DestroyBuilding"))
                            {
                                command = command.Remove(0, 16);
                                int id = command.IndexOf(' ');
                                string substring = command.Substring(0, id);
                                int x = int.Parse(substring);
                                command = command.Remove(0, id + 1);
                                int y = int.Parse(command);
                                int i = map.buidingData.IndexOf(map.GetBuilding(x, y));
                                if (i >= 0)
                                {
                                    playerManager.players[map.buidingData[i].fraction].villagenum--;
                                    map.mapData[map.buidingData[i].y, map.buidingData[i].x] = 62 + 64;
                                    map.buidingData[i].fraction = 0;
                                }
                            }
                            else if (command.Contains("CreateParticle"))
                            {
                                command = command.Remove(0, 15);
                                int id = command.IndexOf(' ');
                                string substring = command.Substring(0, id);
                                int spid = 0;
                                if (substring == "Smoke") spid = 4;
                                else if (substring == "Spark") spid = 2;
                                else if (substring == "RedSpark") spid = 1;
                                command = command.Remove(0, id + 1);
                                id = command.IndexOf(' ');
                                substring = command.Substring(0, id);
                                float x = float.Parse(substring);
                                command = command.Remove(0, id + 1);
                                float y = float.Parse(command);
                                particalManager.AddNewParticle(new Particle(spid, new Vector2(x * 64, y * 64), 0.7f, spid == 4));
                            }
                            else if (command.Contains("SetUnitName"))
                            {
                                command = command.Remove(0, 12);
                                int id = command.IndexOf(' ');
                                string substring = command.Substring(0, id);
                                int x = int.Parse(substring);
                                command = command.Remove(0, id + 1);
                                id = command.IndexOf(' ');
                                substring = command.Substring(0, id);
                                int y = int.Parse(substring);
                                command = command.Remove(0, id + 1);
                                int i = unitManager.GetIdByPosition(x, y);
                                if (i >= 0)
                                {
                                    string str = command;
                                    id = -1;
                                    if (int.TryParse(command, out id))
                                        str = langManager.dialogs[int.Parse(command)];
                                    unitManager[unitManager.GetIdByPosition(x, y)].name = str;
                                }
                            }
                            else if (command.Contains("LoadTileset"))
                            {
                                command = command.Remove(0, 12);
                                nowtileset = int.Parse(command);
                                LoadTileset();
                            }
                            else if (command.Contains("DeleteAllUnits"))
                            {
                                unitManager.units.Clear();
                                playerManager.players[0].unitnum = 0;
                                playerManager.players[1].unitnum = 0;
                                playerManager.players[2].unitnum = 0;
                                playerManager.players[3].unitnum = 0;
                                playerManager.players[4].unitnum = 0;
                            }
                            else if (command.Contains("SetMapTile"))
                            {
                                command = command.Remove(0, 11);
                                int id = command.IndexOf(' ');
                                string substring = command.Substring(0, id);
                                int x = int.Parse(substring);
                                command = command.Remove(0, id + 1);
                                id = command.IndexOf(' ');
                                substring = command.Substring(0, id);
                                int y = int.Parse(substring);
                                command = command.Remove(0, id + 1);
                                int px = int.Parse(command);

                                map.mapData[y, x] = px;
                            }


                            me.nowstring++;

                            if (me.gameStarted)
                                return true;
                        }
                        if (me.gameStarted)
                            return true;
                    }
                    else return true;

                } while (true);
            }
            return false;
        }

        bool TestScript(string test, float a, float b)
        {
            if (test == "=")
            {
                return a == b;
            }
            else if (test == "!=")
            {
                return a != b;
            }
            else if (test == ">")
            {
                return a > b;
            }
            else if (test == ">=")
            {
                return a >= b;
            }
            else if (test == "<")
            {
                return a < b;
            }
            else if (test == "<=")
            {
                return a <= b;
            }
            else
            {
                return false;
            }
        }
        #endregion
        string AlignText(string text, int width)
        {
            string resault="";

            int stringsnum = (int)((font.MeasureString(text).X) / width);
            if (stringsnum == 0)
                resault = text;
            else
            {
                int nowwidth=0;
                text.Trim();
                List<string> strings=new List<string>();
                int spacenum = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] == ' ')
                    {
                        spacenum++;
                        strings.Add(text.Substring(i - nowwidth, nowwidth));
                        nowwidth = 0;
                    }
                    else
                    {
                        nowwidth++;
                    }
                }
                strings.Add(text.Substring(text.Length-1 - nowwidth, nowwidth+1));
                nowwidth = 0;
                for (int i = 0; i < strings.Count; i++)
                {
                    int x = (int)font.MeasureString(strings[i]+" ").X;
                    if (nowwidth + x > width)
                    {
                        nowwidth = 0; resault += "\n";
                    }
                    else nowwidth += x;
                    resault += strings[i] + " ";
                }
            }

            return resault;
        }
        #endregion
    }
}