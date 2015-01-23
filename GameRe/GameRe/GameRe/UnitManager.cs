using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game
{
    public class UnitManager
    {
        public List<Unit> units;

        public UnitManager() { units = new List<Unit>(); }
        public Unit GetByPosition(int x, int y)
        {
            foreach (Unit u in units)
            {
                if ((int)u.position.X == x && (int)u.position.Y == y) return u;
            }
            return null;
        }
        public void AddUnit(Unit u)
        {
            units.Add(u);
        }
        public void Update()
        {
            for (int i = units.Count - 1; i >= 0; i--)
            {
                if (units[i].health <= 0) units.RemoveAt(i);
            }
        }
        public void Draw(SpriteBatch spriteBatch,SpriteFont font, Camera camera, Texture2D tileset,Texture2D fractions,Map map,Color lightColor)
        {
            foreach (Unit u in units)
            {
                if (Timer.GetTimerState(0.5f, (u.position.X + u.position.Y) / 4)) u.animationActive = !u.animationActive;
                    u.Draw(spriteBatch, font, camera, tileset, fractions, Map.TILES_IN_TERRAIN[map.mapData[(int)u.position.Y, (int)u.position.X]] == 5, lightColor);
            }
        }

        public Unit this[int i] { get { try { return units[i]; } catch { ;} return null; } }

        public bool IsLiveKingOfFraction(int f)
        {
            foreach (Unit u in units)
            {
                if (u.HasProperty(UnitProperty.King) && u.team == f) return true;
            }
            return false;
        }

        public int GetNumberOfUnitsOnPoition(Vector2 pos)
        {
            int n = 0;
            foreach (Unit u in units)
                if (u.position == pos) n++;
            return n;
        }
        public Unit GetKingOfFraction(int fr)
        {
            foreach (Unit u in units) if (u.HasProperty(UnitProperty.King) && u.team == fr) return u;
            return null;
        }

        public int NearUnitOn(int x, int y, int fraction,PlayerManager pm)
        {
            int id = -1;
            int lenchto = 1000;
            for (int i = 0; i < units.Count; i++)
            {
                if (pm.players[fraction].team != pm.players[units[i].team].team)
                {
                    int l = Math.Abs((int)units[i].position.X - x) + Math.Abs((int)units[i].position.Y - y);
                    if ((id == -1 || l < lenchto) && units[i].team != fraction) { id = i; lenchto = l; }
                }
            }
            return id;
        }
        public int GetIdByPosition(int x,int y)
        {
            for (int i = 0; i < units.Count; i++)
                if ((int)units[i].position.X == x && (int)units[i].position.Y == y) return i;
            return -1;
        }

        public int countUnits(int type, int fraction)
        {
            int c = 0;
            foreach (Unit u in units)
                if ((u.id == type || type == -1) && u.team == fraction) c++;
            return c;
        }
        public int countEnableUnits(int type, int fraction)
        {
            int c = 0;
            foreach (Unit u in units)
                if ((u.id == type || type == -1) && u.team == fraction&& !u.moveEnd) c++;
            return c;
        }
        public int countUnitsOfTeam(int type, int team, PlayerManager pm)
        {
            int c = 0;
            foreach (Unit u in units)
                if ((u.id == type || type == -1) && pm.players[u.team] == pm.players[team]) c++;
            return c;
        }
        public int countUnitsOfEnemyTeam(int type, int team, PlayerManager pm)
        {
            int c = 0;
            foreach (Unit u in units)
                if ((u.id == type || type == -1) && pm.players[u.team] != pm.players[team]) c++;
            return c;
        }
        public int countUnitsInRange(int x, int y, int minr, int maxr,int fraction)
        {
            int c=0;
            foreach (Unit u in units)
            {
                if (u.team == fraction)
                {
                    int r = Math.Abs(x - (int)u.position.X) + Math.Abs(y - (int)u.position.Y);
                    if (r >= minr && r <= maxr) c++;
                }
            }
            return c;
        }
        public List<int> GetIdByProperty(UnitProperty upr,int fraction=-1)
        {
            List<int> id = new List<int>();
            for (int i = 0; i < units.Count; i++)
                if (units[i].HasProperty(upr)&&(fraction==-1||units[i].team==fraction))
                    id.Add(i);
            return id;
        }
        public List<int> GetIds(int k,int fraction)
        {
            List<int> id = new List<int>();
            for (int i = 0; i < units.Count; i++)
                if (units[i].id==k&&(units[i].team==fraction||fraction==-1))
                    id.Add(i);
            return id;
        }
        public int countUnitsInRange(int x1,int y1,int x2,int y2)
        {
            int c = 0;
            foreach (Unit u in units)
                if (u.position.X >= x1 && u.position.X <= x2 && u.position.Y >= y1 && u.position.Y <= y2) c++;
            return c;
        }
    }
}
