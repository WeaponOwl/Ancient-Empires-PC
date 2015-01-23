using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public enum PlayerState
    { 
        None=0,
        Player=1,
        Ai=2,
        Ai2=3,
        Net=4
    }
    public class Player
    {
        public int gold;
        public int fraction;
        public int team;
        public int castlenum;
        public int villagenum;
        public int unitnum;
        public PlayerState state;

        public Player(int g,int f,int t,int cn,int vn,int un,PlayerState st)
        {
            gold = g;
            team = t;
            fraction = f;
            castlenum = cn;
            villagenum = vn;
            unitnum = un;
            state = st;
        }

        public void Update(Map m,UnitManager um)
        {
            villagenum = 0;
            castlenum = 0;
            unitnum = 0;
            foreach (Building b in m.buidingData)
                if (b.fraction == fraction && fraction != 0&&m.mapData[b.y,b.x]==(63+64)) villagenum++;
            foreach (Building b in m.castleData)
                if (b.fraction == fraction && fraction != 0) castlenum++;
            foreach (Unit u in um.units)
                if (u.team == fraction && fraction != 0) unitnum++;
            gold += villagenum * 30 + castlenum * 50;
        }
    }

    public class PlayerManager
    {
        public List<Player> players;
        public int playersNumber;
        public int[,] playersRelations;
        public int numberofnetworkplayers;
        public int localnumberofplayers;

        public PlayerManager() { players = new List<Player>(); playersNumber = 0; }
        public void SetPlayers(Map map, UnitManager um, int startingmoney)
        {
            players = new List<Player>();
            playersNumber = 0;
            for (int i = 0; i < 5; i++)
            {
                //if (i == 1) players.Add(new Player(startingmoney, plaeyrsid[i], plaeyrsid[i], 0, 0, 0, false));
                //else players.Add(new Player(startingmoney, plaeyrsid[i], plaeyrsid[i], 0, 0, 0, true));
                players.Add(new Player(0, 0, 0, 0, 0, 0, PlayerState.None));
                    if (i != 0)
                        if (i == 1) players[i] = new Player(startingmoney, i, i, 0, 0, 0, PlayerState.Player);
                        else players[i] = new Player(startingmoney, i, i, 0, 0, 0, PlayerState.Ai);
                    else players[i] = new Player(startingmoney, i, -1, 0, 0, 0, PlayerState.None);
            }
            foreach (Building b in map.buidingData)
                //if (b.fraction == players[i].fraction) players[i].villagenum++;
                players[b.fraction].villagenum++;
            foreach (Building b in map.castleData)
                //if (b.fraction == players[i].fraction) players[i].castlenum++;
                players[b.fraction].villagenum++;
            foreach (Unit u in um.units)
                //if (u.team == players[i].fraction) players[i].unitnum++;
                players[u.team].unitnum++;
        }

        public void SetPlayers(Map map, UnitManager um, int startingmoney, int[] teams,PlayerState[] states)
        {
            players = new List<Player>();
            playersNumber = 0;
            numberofnetworkplayers = 0;
            localnumberofplayers = 0;
            for (int i = 0; i < 5; i++)
            {
                //if (i == 1) players.Add(new Player(startingmoney, plaeyrsid[i], plaeyrsid[i], 0, 0, 0, false));
                //else players.Add(new Player(startingmoney, plaeyrsid[i], plaeyrsid[i], 0, 0, 0, true));
                players.Add(new Player(0, 0, 0, 0, 0, 0, PlayerState.None));
                   if (i != 0)
                        players[i] = new Player(startingmoney, i, teams[i], 0, 0, 0, states[i]);
                   else players[i] = new Player(startingmoney, i, -1, 0, 0, 0, states[i]);

                   if (states[i] == PlayerState.Net) numberofnetworkplayers++;
                   if (states[i] == PlayerState.Player) localnumberofplayers++;
            }
            foreach (Building b in map.buidingData)
                //if (b.fraction == players[i].fraction) players[i].villagenum++;
                players[b.fraction].villagenum++;
            foreach (Building b in map.castleData)
                //if (b.fraction == players[i].fraction) players[i].castlenum++;
                players[b.fraction].villagenum++;
            foreach (Unit u in um.units)
                //if (u.team == players[i].fraction) players[i].unitnum++;
                players[u.team].unitnum++;
        }
        
        public void Update()
        {
            ;
        }
    }
}
