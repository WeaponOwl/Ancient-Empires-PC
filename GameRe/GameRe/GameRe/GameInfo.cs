using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Game
{
    public enum GameState
    {
        Any,
        MainMenu,
        LoadSavesList,
        StartMenu,
        NewLoadList,
        LoadList,
        Editor,
        Game
    }
    public enum GameState2
    { 
        None=0,
        Move=1,
        Attack=2,
        Reattack=3,
        Raise=4,
        Buy=5,
        AttackPre=6
    }

    public class EditorInfo
    {
        public int nw, nh;
        public int id;
        public int startIndexOfTiles;
        public bool unitMode;
        public int nowFraction;

        public EditorInfo() 
        { 
            nw = 10; 
            nh = 10; 
            id = 0; 
            startIndexOfTiles = 0;
            unitMode = false;
            nowFraction = 0;
        }
    }

    public class GameInfo
    {
        public int selectedUnitId;
        public int attackUnitId;
        public int alooweUnit;
        public List<Vector2> path;
        public bool wait;
        public GameState2 state;
        public List<string> submenu;
        public int nowplayer;
        public Vector2 oldposition;
        public int buystartid;
        public int buyid;
        Unit buyselectedUnit;
        public Vector2 castlePosition;
        public int[] playersTeams;
        public PlayerState[] playersStates;
        public int startmoney;
        public string halloWorld;
        public int nowturn;
        public bool onlyUpdateUnits;
        public bool cursorVisible;
        public bool mapcursorVisible;
        public int firstMapName;
        public int mainFraction;

        public GameInfo()
        {
            selectedUnitId = -1;
            attackUnitId = -1;
            path = new List<Vector2>();
            wait = false;
            state = GameState2.None;
            submenu = new List<string>();
            nowplayer = 1;
            mainFraction = -1;
            buystartid = 0;
            buyid = 0;
            buyselectedUnit = Unit.GetByType(0,0,0,0);
            castlePosition = new Vector2();
            alooweUnit = 8;
            playersTeams=new int[]{-1,1,2,3,4};
            playersStates = new PlayerState[] { PlayerState.None, PlayerState.Player, PlayerState.Ai, PlayerState.Ai2, PlayerState.Ai };
            startmoney = 500;
            nowturn = -1;
            //halloWorld = "Ancient empires forever";
            SetRandomText();
            onlyUpdateUnits = false;
            cursorVisible = true;
            mapcursorVisible = true;
            firstMapName = 0;
        }

        public Unit BuySelectionUnit
        {
            set { buyselectedUnit=value; }
            get { return buyselectedUnit; }
        }

        public void SetRandomText()
        {
            switch (new Random().Next(11))
            {
                case 0: halloWorld = Game.langManager[GameString.RandomText1]; break;
                case 1: halloWorld = Game.langManager[GameString.RandomText2]; break;
                case 2: halloWorld = Game.langManager[GameString.RandomText3]; break;
                case 3: halloWorld = Game.langManager[GameString.RandomText4]; break;
                case 4: halloWorld = Game.langManager[GameString.RandomText5]; break;
                case 5: halloWorld = Game.langManager[GameString.RandomText6]; break;
                case 6: halloWorld = Game.langManager[GameString.RandomText7]; break;
                case 7: halloWorld = Game.langManager[GameString.RandomText8]; break;
                case 8: halloWorld = Game.langManager[GameString.RandomText9]; break;
                case 9: halloWorld = Game.langManager[GameString.RandomText10]; break;
                case 10: halloWorld = Game.langManager[GameString.RandomText11]; break;
            }
        }
    }
}
