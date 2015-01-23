using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Game
{
    public enum Lang
    {
        Eng = 0,
        Rus = 1,
        Ukr = 2
    }

    public enum GameString
    {
        SimpleGame,
        Editor,
        Quit,
        ToMenu,
        Create,
        Save,
        Load,
        ContinueLast,
        Units,
        Tiles,
        LeftArrow,
        RightArrow,
        EndTurn,
        Buy,
        Cansel,
        LoadGame,
        SaveGame,
        Position,
        GameSaved,
        Lvl,
        Exp,
        Hp,
        Att,
        Def,
        Steps,
        Price,
        Gold,
        GoldNum,
        CastlesNum,
        VillagesNum,
        UnitsNum,
        Attack,
        Deffence,
        Unknown,
        YouWin,
        YouLose,
        Move,
        Occupy,
        Repair,
        Raise,
        Player1,
        Player2,
        Player3,
        Player4,
        Player1Team,
        Player2Team,
        Player3Team,
        Player4Team,
        StartMoney,
        GreenTileset,
        WhileTileset,
        Error,
        BluePlayer,
        RedPlayer,
        GreenPlayer,
        BlackPlayer,
        LevelUp,
        MissionFailed,
        MissionComplate,
        Team,
        Win,
        RandomText1,
        RandomText2,
        RandomText3,
        RandomText4,
        RandomText5,
        RandomText6,
        RandomText7,
        RandomText8,
        RandomText9,
        RandomText10,
        RandomText11,
        Soldier,
        Archer,
        Lizard,
        Soreccer,
        Wisp,
        Wolf,
        Golem,
        Catapult,
        Wyvern,
        Comander,
        Skeleton,
        Crystal,
        SecondCommander,
        SoldierDescription,
        ArcherDescription,
        LizardDescription,
        SoreccerDescription,
        WispDescription,
        WolfDescription,
        GolemDescription,
        CatapultDescription,
        WyvernDescription,
        ComanderDescription,
        SkeletonDescription,
        CrystalDescription,
        SecondCommanderDescription,
        Story1,
        Story2,
        Green2Tileset,
        LavaTileset,
        About,
        Lizard2,
        Spider,
        Lizard2Description,
        SpiderDescription
    }

    public class LangManager
    {
        public string[] strings;
        public string[] dialogs;

        public LangManager(string name)
        {
            strings = System.IO.File.ReadAllLines(name);
        }

        public void LoadDialogs(string name)
        {
            dialogs = System.IO.File.ReadAllLines(name);
        }

        public string GetString(GameString str)
        {
            return strings[(int)str];
        }

        public string this[GameString s]
        {
            get
            {
                return strings[(int)s];
            }
        }
    }
}
