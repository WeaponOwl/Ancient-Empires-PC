using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game
{
    public enum UnitProperty
    {
        SkeletonAndCanNotRaised=1,
        WaterElemental=2,
        CanCaptureVillages=3,
        CanCaptureCastles=4,
        CanRaise=5,
        GetBonusWhithSceleton=6,
        CanPoisone=7,
        Lighting=8,
        CanNotAttackAndMoveAtOnce=9,
        Fly=10,
        CanGoOnWater=11,
        CanDestroyVillages=12,
        King=13
    }
    public enum UnitEffect
    {
        Poison=1,
        Light=2
    }

    struct AiPoint
    {
        public int x, y, m, m2, l, r;
        public AiPoint(int x, int y, int m, int m2, int l, int r)
        {
            this.x = x;
            this.y = y;
            this.m = m;
            this.m2 = m2;
            this.l = l;
            this.r = r;
        }
    }
    struct PathPoint
    {
        public int X, Y, itteration;

        public PathPoint(int x, int y,int itt)
        { this.X = x; this.Y = y; itteration = itt; }
    }

    public class Unit
    {
        public int id;

        public string name;
        public bool male;
        public int lvl;
        public int experience;

        public Vector2 position;
        public int team;
        public int baseAttackMin;
        public int baseAttackMax;
        public int baseDefence;
        public int health;
        public int baseMoveRange;
        public int baseAttackRangeMin;
        public int baseAttackRangeMax;
        public int bonusAttack;
        public int bonusDefence;
        public int cost;
        public bool animationActive;
        public bool moveEnd;
        public UnitEffect[] effects;
        public UnitProperty[] properties;
        public bool vibrate;
        public bool onceReattaked;

        static public Vector2 vibrateOffcet=Vector2.Zero;
        static public float unitSpeed = 0.05f;

        public Unit() { ;}

        public Unit(int id, int _fraction, int _x, int _y)
        {
            Unit u = GetByType(id, _fraction, _x, _y);
            this.id = u.id;
            this.name = u.name;
            this.male = u.male;
            this.lvl = u.lvl;
            this.experience = u.experience;
            this.position = u.position;
            this.team = u.team;
            this.baseAttackMin = u.baseAttackMin;
            this.baseAttackMax = u.baseAttackMax;
            this.baseDefence = u.baseDefence;
            this.health = u.health;
            this.baseMoveRange = u.baseMoveRange;
            this.baseAttackRangeMin = u.baseAttackRangeMin;
            this.baseAttackRangeMax = u.baseAttackRangeMax;
            this.bonusAttack = u.bonusAttack;
            this.bonusDefence = u.bonusDefence;
            this.cost = u.cost;
            this.animationActive = u.animationActive;
            this.moveEnd = u.moveEnd;
            this.effects = u.effects;
            this.properties = u.properties;
        }

        public void Draw(SpriteBatch spriteBatch,SpriteFont font, Camera camera, Texture2D tileset,Texture2D fractions,bool inwater,Color lightColor)
        {
            Color gray = Color.Multiply(lightColor, 0.5f);
            gray.A = 255;
            if (team > 0)
                spriteBatch.Draw(fractions, position * 64 - camera.position + camera.offset+new Vector2(0,4), new Rectangle((team - 1) * 64, 128+64, 64, 64), lightColor, 0, Vector2.Zero, 1, SpriteEffects.None, camera.GetZ(position.Y * 64 - camera.position.Y + camera.offset.Y -1, 62));
            spriteBatch.Draw(tileset, position * 64 - camera.position + camera.offset + (vibrate ? vibrateOffcet : Vector2.Zero), new Rectangle(id * 64, (animationActive ? 64 : 0)+(team*128), 64, 64), !moveEnd ? lightColor : gray, 0, Vector2.Zero, 1, SpriteEffects.None, camera.GetZ(position.Y * 64 - camera.position.Y + camera.offset.Y, 62));
            if (health < 100)
            {
                spriteBatch.DrawString(font, health.ToString(), position * 64 - camera.position + camera.offset + new Vector2(1, 43 + 1), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, camera.GetZ(position.Y * 64 - camera.position.Y + camera.offset.Y, 65));
                spriteBatch.DrawString(font, health.ToString(), position * 64 - camera.position + camera.offset + new Vector2(1, 43 - 1), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, camera.GetZ(position.Y * 64 - camera.position.Y + camera.offset.Y, 65));
                spriteBatch.DrawString(font, health.ToString(), position * 64 - camera.position + camera.offset + new Vector2(1 + 1, 43), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, camera.GetZ(position.Y * 64 - camera.position.Y + camera.offset.Y, 65));
                spriteBatch.DrawString(font, health.ToString(), position * 64 - camera.position + camera.offset + new Vector2(1 - 1, 43), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, camera.GetZ(position.Y * 64 - camera.position.Y + camera.offset.Y, 65));
                spriteBatch.DrawString(font, health.ToString(), position * 64 - camera.position + camera.offset + new Vector2(1, 43), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, camera.GetZ(position.Y * 64 - camera.position.Y + camera.offset.Y, 66));
            }
            //if(moveEnd)
        }

        public void PreMove(Vector2 target) { ;}
        public void Move(Vector2 target)
        {
            Vector2 dir = target - position;
            dir.Normalize();
            position += dir * unitSpeed;
            if (Timer.GetTimerState(0.1f)) animationActive = !animationActive;
        }
        public void PostMove(Vector2 target) { ;}

        public void PreAttack(ref Unit victim, ref Map map) { ;}
        public int Attack(ref UnitManager um,int victim,ref Map map,bool attacked) 
        {
            Random random = new Random();
            if (HasProperty(UnitProperty.CanPoisone)&&attacked) 
            { 
                um.units[victim].bonusDefence -= 10; um.units[victim].bonusAttack -= 10;
                List<UnitEffect> up=new List<UnitEffect>();
                foreach (UnitEffect upr in um[victim].effects)
                    up.Add(upr);
                up.Add(UnitEffect.Poison);
                um.units[victim].effects = up.ToArray();
            }
            int damage = random.Next(baseAttackMin, baseAttackMax) * health / 100 + bonusAttack - Map.TERRAIN_DEFENCE_BONUS[Map.TILES_IN_TERRAIN[map.mapData[(int)um.units[victim].position.Y, (int)um.units[victim].position.X]]]-um[victim].bonusDefence-um[victim].baseDefence;
            if (HasProperty(UnitProperty.WaterElemental) && Map.TILES_IN_TERRAIN[map.mapData[(int)position.Y, (int)position.X]] == 5) damage += 10;
            if (um[victim].HasProperty(UnitProperty.SkeletonAndCanNotRaised) && HasProperty(UnitProperty.GetBonusWhithSceleton)) damage = (int)(damage*1.5);
            if (um.units[victim].HasProperty(UnitProperty.WaterElemental) && Map.TILES_IN_TERRAIN[map.mapData[(int)um.units[victim].position.Y, (int)um.units[victim].position.X]]==5) damage -= 10;
            if (damage < 0) damage = 0;
            if (damage > um.units[victim].health) { damage = um.units[victim].health; }
            um.units[victim].health -= damage;
            AddExp(um.units[victim].GetExp()*damage);
            um.units[victim].vibrate=true;
            return -damage;
        }
        public void PostAttack(ref Unit victim, ref Map map) { ;}

        public void PreReattack(ref Unit victim, ref Map map) { ;}
        public void Reattack(ref Unit victim, ref Map map) { ;}
        public void PostReattack(ref Unit victim, ref Map map) { ;}

        public int GetDeffenceBonus(Map map,Unit victim)
        {
            return (victim.HasProperty(UnitProperty.WaterElemental) && Map.TILES_IN_TERRAIN[map.mapData[(int)victim.position.Y, (int)victim.position.X]] == 5)?10:0;
        }
        public int GetAttackBonus(Map map, Unit victim)
        {
            int r = (HasProperty(UnitProperty.WaterElemental) && Map.TILES_IN_TERRAIN[map.mapData[(int)position.Y, (int)position.X]] == 5)?10:0;
            r+=(victim.HasProperty(UnitProperty.SkeletonAndCanNotRaised) && HasProperty(UnitProperty.GetBonusWhithSceleton))?10:0;
            return r;
        }

        public void AddExp(int exp)
        {
            experience += exp;
            if (experience >= GetNextLvlExp())
            {
                experience -= GetNextLvlExp();
                LvlUp();
            }
        }
        public void LvlUp()
        {
            baseAttackMin += 2;
            baseAttackMax += 2;
            baseDefence += 2;
            lvl++;
            if (lvl > 10) lvl = 10;
        }
        public int GetExp()
        {
            return GetQuality();
        }
        public int GetNextLvlExp()
        {
            return GetQuality() * 100 * 2 / 3;
        }
        public int GetQuality()
        {
            return baseAttackMin + baseAttackMax + baseDefence;
        }

        public void AddEffect(UnitEffect newEffect)
        { 
            List<UnitEffect>e=effects.ToList();
            e.Add(newEffect);
            effects=e.ToArray();
        }
        public void UdpateEffect()
        {
            foreach (int e in effects)
            {
                switch (e)
                {
                    case 1: bonusAttack += 10; break;
                    case 2: bonusAttack -= 10; bonusDefence -= 10; break;
                }
            }
        }

        public int GetRecSteps(int x, int y, Map map,bool ignoreterrain=false)
        {
            if (x >= 0 && x < map.width && y >= 0 && y < map.height)
            {
                int rec = Map.TERRAIN_STEPS_REQUIRED[Map.TILES_IN_TERRAIN[map.mapData[y, x]]];
                if (HasProperty(UnitProperty.Fly) || ignoreterrain) rec = 1;
                if (HasProperty(UnitProperty.CanGoOnWater) && Map.TILES_IN_TERRAIN[map.mapData[y, x]] == 5) rec = 1;
                if (id == 11 && Map.TERRAIN_STEPS_REQUIRED[Map.TILES_IN_TERRAIN[map.mapData[y, x]]] == 3) rec++;
                return rec;
            }
            else return 9999;
        }
        public void FillMoveRange(ref Map map,ref UnitManager um,ref PlayerManager pm,bool ignoreterrain=false)
        {
            FillMoveRangeEx(ref map, (int)position.X, (int)position.Y, baseMoveRange,ref um,ref pm,ignoreterrain);
        }
        public void FillMoveRangeEx(ref Map map, int x, int y, int steps, ref UnitManager um, ref PlayerManager pm, bool ignoreterrain = false)
        {
            Unit u = um.GetByPosition(x, y);
            bool iu = false;
            if (u != null && pm.players[u.team].team != pm.players[team].team) iu = true;
            if (steps > 0 && x >= 0 && x < map.width && y >= 0 && y < map.height&&!iu)
            {
                if (map.alphaData[y, x] >= 0)
                {
                    if (map.alphaData[y, x]<steps)
                        map.alphaData[y, x] = steps;
                    int nx, ny;

                    nx = x - 1; ny = y;
                    if (nx >= 0 && ny >= 0 && nx < map.width && ny < map.height)
                        FillMoveRangeEx(ref map, nx, ny, steps - GetRecSteps(nx, ny, map, ignoreterrain), ref um, ref pm);

                    nx = x + 1; ny = y;
                    if (nx >= 0 && ny >= 0 && nx < map.width && ny < map.height)
                        FillMoveRangeEx(ref map, nx, ny, steps - GetRecSteps(nx, ny, map, ignoreterrain), ref um, ref pm);

                    nx = x; ny = y - 1;
                    if (nx >= 0 && ny >= 0 && nx < map.width && ny < map.height)
                        FillMoveRangeEx(ref map, nx, ny, steps - GetRecSteps(nx, ny, map, ignoreterrain), ref um, ref pm);

                    nx = x; ny = y + 1;
                    if (nx >= 0 && ny >= 0 && nx < map.width && ny < map.height)
                        FillMoveRangeEx(ref map, nx, ny, steps - GetRecSteps(nx, ny, map, ignoreterrain), ref um, ref pm);
                }
                else map.alphaData[y, x] = 0;
            }
        }
        public void FillAttackRangeEx(ref Map map, int x, int y,ref UnitManager um)
        {
            int minRange = baseAttackRangeMin;
            int maxRange = baseAttackRangeMax;

            int mStartX = x - maxRange;
            if (mStartX < 0)
            {
                mStartX = 0;
            }

            int mStartY = y - maxRange;
            if (mStartY < 0)
            {
                mStartY = 0;
            }

            int mEndX = x + maxRange;
            if (mEndX >= map.width)
            {
                mEndX = map.width - 1;
            }

            int mEndY = y + maxRange;
            if (mEndY >= map.height)
            {
                mEndY = map.height - 1;
            }

            for (int mx = mStartX; mx <= mEndX; ++mx)
            {
                for (int my = mStartY; my <= mEndY; ++my)
                {
                    int range = Math.Abs(mx - x) + Math.Abs(my - y);

                    if (range >= minRange && range <= maxRange && map.alphaData[my,mx] <= 127)
                    {
                        map.alphaData[my,mx] = 127;
                    }
                }
            }
        }
        public void FillAttackRange(ref Map map,ref UnitManager um,ref PlayerManager pm)
        {
            if (HasProperty(UnitProperty.CanNotAttackAndMoveAtOnce))
            {
                FillAttackRangeEx(ref map, (int)position.X, (int)position.Y,ref um);
            }
            else
            {
                FillMoveRange(ref map, ref um, ref pm);
                int[,] alpha = new int[map.height, map.width];
                for (int i = 0; i < map.height; i++)
                {
                    for (int j = 0; j < map.width; j++)
                    {
                        alpha[i, j] = map.alphaData[i, j];
                    }
                }
                map.ClearAlphaData();
                for (int i = 0; i < map.height; i++)
                {
                    for (int j = 0; j < map.width; j++)
                    {
                        if (alpha[i, j] > 0 && alpha[i, j] < 127)
                        {
                            FillAttackRangeEx(ref map, j, i,ref um);
                        }
                    }
                }

            }
        }

        public bool HasProperty(UnitProperty ip)
        {
            foreach (UnitProperty p in properties)
                if (p == ip) return true;
            return false;
        }
        public bool HasEffect(UnitEffect e)
        {
            foreach (UnitEffect ef in effects)
                if (ef == e) return true;
            return false;
        }

        public void FillMoveAllMap(ref Map map,ref UnitManager um,int endx,int endy,PlayerManager pm)
        {
            //FillMoveRangeEx(ref map,(int)position.X, (int)position.Y,map.width,ref um);
            map.ClearAlphaData();
            //FillMoveangeBySpiralEx(ref map, (int)position.X, (int)position.Y, ex, ey, 0,1, 126, ref um);

            List<PathPoint> openset = new List<PathPoint>();
            List<PathPoint> closedset = new List<PathPoint>();
            closedset.Add(new PathPoint((int)position.X, (int)position.Y,0));
            int steps = 126;
            map.alphaData[(int)position.Y, (int)position.X] = steps;
            openset.Add(new PathPoint((int)position.X + 1, (int)position.Y,0));
            openset.Add(new PathPoint((int)position.X - 1, (int)position.Y,0));
            openset.Add(new PathPoint((int)position.X, (int)position.Y + 1,0));
            openset.Add(new PathPoint((int)position.X, (int)position.Y - 1,0));
            PathPoint p = openset[0];
            int itteration = 0;

            do
            {
                int l = 999;// Math.Abs(openset[0].X - endx) + Math.Abs(openset[0].Y - endy);
                int ind = 0;
                List<int> lind = new List<int>();
                for (int i = 0; i < openset.Count - 1; i++)
                {
                    int l2 = Math.Abs(openset[i].X - endx) + Math.Abs(openset[i].Y - endy) + (itteration - openset[i].itteration) * 10 + GetRecSteps(openset[i].X, openset[i].Y, map) * 10;
                    if (l2 <= l) { p = openset[i]; ind = i; l = l2; }
                }
                //for (int i = 0; i < openset.Count - 1; i++)
                //{
                //    int l2 = Math.Abs(openset[i].X - endx) + Math.Abs(openset[i].Y - endy) + itteration - openset[i].itteration + GetRecSteps(openset[i].X, openset[i].Y, map);
                //    if (l2 == l) { lind.Add(i); }
                //}
                //ind = new Random().Next(lind.Count);
                //p = openset[ind];
                //p = openset[0];
                //ind = 0;
                openset.RemoveAt(ind);
                closedset.Add(p);

                if (p.X >= 0 && p.Y >= 0 && p.X < map.width && p.Y < map.height)
                {
                    int top = 0, down = 0, right = 0, left = 0;
                    try { top = map.alphaData[p.Y - 1, p.X]; }
                    catch { ;}
                    try { down = map.alphaData[p.Y + 1, p.X]; }
                    catch { ;}
                    try { left = map.alphaData[p.Y, p.X - 1]; }
                    catch { ;}
                    try { right = map.alphaData[p.Y, p.X + 1]; }
                    catch { ;}
                    int max = Math.Max(Math.Max(top, down), Math.Max(right, left));
                    if (map.alphaData[p.Y, p.X] == 0)
                        map.alphaData[p.Y, p.X] = max - Map.TERRAIN_STEPS_REQUIRED[Map.TILES_IN_TERRAIN[map.mapData[p.Y, p.X]]];
                    if (HasProperty(UnitProperty.Fly)) map.alphaData[p.Y, p.X] = max - 1;
                    if (HasProperty(UnitProperty.CanGoOnWater) && Map.TILES_IN_TERRAIN[map.mapData[p.Y, p.X]] == 5) map.alphaData[p.Y, p.X] = max - 1;
                    if (map.fractionUnitData[p.Y, p.X] != -1 && pm.players[map.fractionUnitData[p.Y, p.X]].team != pm.players[team].team) map.alphaData[p.Y, p.X] = -1;

                    PathPoint np = new PathPoint(p.X + 1, p.Y, itteration);
                    if (!closedset.Contains(np)) openset.Add(np);
                    np = new PathPoint(p.X - 1, p.Y, itteration);
                    if (!closedset.Contains(np)) openset.Add(np);
                    np = new PathPoint(p.X, p.Y + 1, itteration);
                    if (!closedset.Contains(np)) openset.Add(np);
                    np = new PathPoint(p.X, p.Y - 1, itteration);
                    if (!closedset.Contains(np)) openset.Add(np);
                }

            } while ((p.X != endx || p.Y != endy) && closedset.Count > 0 && closedset.Count < 550);
        }
        
        static public List<Point> GetPathFromPointToPoint(ref Map map,Point p1, Point p2)
        {
            List<Point> lp = new List<Point>();
                if (p1 == p2) lp.Add(p2);
                else
                {
                    int top = 0;
                    int bottom = 0;
                    int left = 0;
                    int right = 0;

                    if (p2.Y > 0)
                        top = map.alphaData[p2.Y - 1, p2.X];
                    if (p2.Y < map.height - 1)
                        bottom = map.alphaData[p2.Y + 1, p2.X];
                    if (p2.X > 0)
                        left = map.alphaData[p2.Y, p2.X - 1];
                    if (p2.X < map.width - 1)
                        right = map.alphaData[p2.Y, p2.X + 1];

                    int max = Math.Max(Math.Max(top, bottom), Math.Max(left, right));

                    if (max != 0)
                    {
                        if (max == top)
                            lp.AddRange(GetPathFromPointToPoint(ref map, p1, new Point(p2.X, p2.Y - 1)));
                        else if (max == bottom)
                            lp.AddRange(GetPathFromPointToPoint(ref map, p1, new Point(p2.X, p2.Y + 1)));
                        else if (max == left)
                            lp.AddRange(GetPathFromPointToPoint(ref map, p1, new Point(p2.X - 1, p2.Y)));
                        else if (max == right)
                            lp.AddRange(GetPathFromPointToPoint(ref map, p1, new Point(p2.X + 1, p2.Y)));
                    }
                    else
                    {
                        //if (map.fractionUnitData[p2.Y + 1, p2.X] == -1)
                        //    lp.AddRange(GetPathFromPointToPoint(ref map, p1, new Point(p2.X, p2.Y + 1)));
                        //else if (map.fractionUnitData[p2.Y - 1, p2.X] == -1)
                        //    lp.AddRange(GetPathFromPointToPoint(ref map, p1, new Point(p2.X, p2.Y - 1)));
                        //else if (map.fractionUnitData[p2.Y , p2.X+1] == -1)
                        //    lp.AddRange(GetPathFromPointToPoint(ref map, p1, new Point(p2.X+1, p2.Y)));
                        //else if (map.fractionUnitData[p2.Y , p2.X-1] == -1)
                        //    lp.AddRange(GetPathFromPointToPoint(ref map, p1, new Point(p2.X-1, p2.Y )));
                    }
                    lp.Add(p2);
                }
            return lp;
        }
        static public Unit GetByType(int id, int _fraction, int _x, int _y)
        {
            Unit u = new Unit();

            u.id = id;
            u.team = _fraction;
            u.position = new Vector2(_x, _y);
            u.male = false;
            u.lvl = 1;
            u.experience = 0;
            u.health = 100;
            u.bonusAttack = 0;
            u.bonusDefence = 0;
            u.animationActive = false;
            u.moveEnd = false;
            u.effects = new UnitEffect[0];
            switch (id)
            {
                case 0: u.baseAttackMax = 55;
                    u.baseAttackMin = 50;
                    u.baseAttackRangeMax = 1;
                    u.baseAttackRangeMin = 1;
                    u.baseDefence = 5;
                    u.baseMoveRange = 5;
                    u.cost = 150;
                    u.name = Game.langManager[GameString.Soldier];
                    u.properties = new UnitProperty[1] { UnitProperty.CanCaptureVillages }; break;
                case 1: u.baseAttackMax = 55;
                    u.baseAttackMin = 50;
                    u.baseAttackRangeMax = 2;
                    u.baseAttackRangeMin = 1;
                    u.baseDefence = 5;
                    u.baseMoveRange = 5;
                    u.cost = 250;
                    u.name = Game.langManager[GameString.Archer];
                    u.properties = new UnitProperty[1] { UnitProperty.GetBonusWhithSceleton }; break;
                case 2: u.baseAttackMax = 55;
                    u.baseAttackMin = 50;
                    u.baseAttackRangeMax = 1;
                    u.baseAttackRangeMin = 1;
                    u.baseDefence = 10;
                    u.baseMoveRange = 5;
                    u.cost = 300;
                    u.name = Game.langManager[GameString.Lizard];
                    u.properties = new UnitProperty[2] { UnitProperty.CanGoOnWater, UnitProperty.WaterElemental }; break;
                case 3: u.baseAttackMax = 45;
                    u.baseAttackMin = 40;
                    u.baseAttackRangeMax = 1;
                    u.baseAttackRangeMin = 1;
                    u.baseDefence = 5;
                    u.baseMoveRange = 5;
                    u.cost = 400;
                    u.name = Game.langManager[GameString.Soreccer];
                    u.properties = new UnitProperty[1] { UnitProperty.CanRaise }; break;
                case 4: u.baseAttackMax = 40;
                    u.baseAttackMin = 35;
                    u.baseAttackRangeMax = 1;
                    u.baseAttackRangeMin = 1;
                    u.baseDefence = 10;
                    u.baseMoveRange = 5;
                    u.cost = 500;
                    u.name = Game.langManager[GameString.Wisp];
                    u.properties = new UnitProperty[2] { UnitProperty.Lighting, UnitProperty.GetBonusWhithSceleton }; break;
                case 5: u.baseAttackMax = 65;
                    u.baseAttackMin = 60;
                    u.baseAttackRangeMax = 1;
                    u.baseAttackRangeMin = 1;
                    u.baseDefence = 15;
                    u.baseMoveRange = 6;
                    u.cost = 600;
                    u.name = Game.langManager[GameString.Wolf];
                    u.properties = new UnitProperty[1] { UnitProperty.CanPoisone }; break;
                case 6: u.baseAttackMax = 70;
                    u.baseAttackMin = 60;
                    u.baseAttackRangeMax = 1;
                    u.baseAttackRangeMin = 1;
                    u.baseDefence = 30;
                    u.baseMoveRange = 5;
                    u.cost = 600;
                    u.name = Game.langManager[GameString.Golem];
                    u.properties = new UnitProperty[0]; break;
                case 7: u.baseAttackMax = 70;
                    u.baseAttackMin = 50;
                    u.baseAttackRangeMax = 4;
                    u.baseAttackRangeMin = 2;
                    u.baseDefence = 10;
                    u.baseMoveRange = 4;
                    u.cost = 700;
                    u.name = Game.langManager[GameString.Catapult];
                    u.properties = new UnitProperty[2] { UnitProperty.CanDestroyVillages, UnitProperty.CanNotAttackAndMoveAtOnce }; break;
                case 8: u.baseAttackMax = 80;
                    u.baseAttackMin = 70;
                    u.baseAttackRangeMax = 1;
                    u.baseAttackRangeMin = 1;
                    u.baseDefence = 25;
                    u.baseMoveRange = 7;
                    u.cost = 1000;
                    u.name = Game.langManager[GameString.Wyvern];
                    u.properties = new UnitProperty[2] { UnitProperty.Fly, UnitProperty.SkeletonAndCanNotRaised }; break;
                case 9: u.baseAttackMax = 65;
                    u.baseAttackMin = 55;
                    u.baseAttackRangeMax = 1;
                    u.baseAttackRangeMin = 1;
                    u.baseDefence = 20;
                    u.baseMoveRange = 5;
                    u.cost = 200;
                    u.name = Game.langManager[GameString.Comander];
                    u.properties = new UnitProperty[3] { UnitProperty.King, UnitProperty.CanCaptureCastles, UnitProperty.CanCaptureVillages }; break;
                case 10: u.baseAttackMax = 50;
                    u.baseAttackMin = 40;
                    u.baseAttackRangeMax = 1;
                    u.baseAttackRangeMin = 1;
                    u.baseDefence = 2;
                    u.baseMoveRange = 5;
                    u.cost = -1;
                    u.name = Game.langManager[GameString.Skeleton];
                    u.properties = new UnitProperty[1] { UnitProperty.SkeletonAndCanNotRaised }; break;
                case 11: u.baseAttackMax = 0;
                    u.baseAttackMin = 0;
                    u.baseAttackRangeMax = 0;
                    u.baseAttackRangeMin = 0;
                    u.baseDefence = 15;
                    u.baseMoveRange = 4;
                    u.cost = -1;
                    u.name = Game.langManager[GameString.Crystal];
                    u.properties = new UnitProperty[1] { UnitProperty.CanNotAttackAndMoveAtOnce }; break;
                case 12: u.baseAttackMax = 65;
                    u.baseAttackMin = 55;
                    u.baseAttackRangeMax = 1;
                    u.baseAttackRangeMin = 1;
                    u.baseDefence = 20;
                    u.baseMoveRange = 5;
                    u.cost = 200;
                    u.name = Game.langManager[GameString.SecondCommander];
                    u.properties = new UnitProperty[3] { UnitProperty.King, UnitProperty.CanCaptureCastles, UnitProperty.CanCaptureVillages }; break;
                case 13: u.baseAttackMax = 55;
                    u.baseAttackMin = 50;
                    u.baseAttackRangeMax = 1;
                    u.baseAttackRangeMin = 1;
                    u.baseDefence = 10;
                    u.baseMoveRange = 5;
                    u.cost = 300;
                    u.name = Game.langManager[GameString.Lizard2];
                    u.properties = new UnitProperty[2] { UnitProperty.CanGoOnWater, UnitProperty.WaterElemental }; break;
                case 14: u.baseAttackMax = 45;
                    u.baseAttackMin = 40;
                    u.baseAttackRangeMax = 1;
                    u.baseAttackRangeMin = 1;
                    u.baseDefence = 5;
                    u.baseMoveRange = 5;
                    u.cost = 400;
                    u.name = Game.langManager[GameString.Soreccer];
                    u.properties = new UnitProperty[1] { UnitProperty.CanRaise }; break;
                case 15: u.baseAttackMax = 65;
                    u.baseAttackMin = 60;
                    u.baseAttackRangeMax = 1;
                    u.baseAttackRangeMin = 1;
                    u.baseDefence = 15;
                    u.baseMoveRange = 6;
                    u.cost = 600;
                    u.name = Game.langManager[GameString.Spider];
                    u.properties = new UnitProperty[1] { UnitProperty.CanPoisone }; break;
            }

            return u;
        }

        public Point GetAiTarget(ref Map map, ref UnitManager um, List<Building> tombs,ref PlayerManager pm)
        {
            //Point t = new Point((int)position.X, (int)position.Y);
            Point t = new Point(0, 0);
            bool targetseted = false;
            int now = 0;
            int x = (int)position.X, y = (int)position.Y;
            for (int i = 0; i < map.height; i++)
                for (int j = 0; j < map.width; j++)
                {
                    map.ai[i, j] = (int)(Map.TERRAIN_DEFENCE_BONUS[Map.TILES_IN_TERRAIN[map.mapData[i, j]]]);
                    if (HasProperty(UnitProperty.WaterElemental) && Map.TILES_IN_TERRAIN[map.mapData[y, x]] == 5) map.ai[i, j] += 25;
                    if (health < 100 && Map.TILES_IN_TERRAIN[map.mapData[i, j]] == 7) map.ai[i, j] += 100 - health;
                }
            if (HasProperty(UnitProperty.CanCaptureVillages))
            {
                int id = um.units.IndexOf(this);
                int idinunits = um.GetIdByProperty(UnitProperty.CanCaptureVillages).IndexOf(id);
                if (idinunits >= 0)
                {
                    List<int> bids = map.GetVillagesOfNotFractionsWhithZero((int)position.X, (int)position.Y, 3, 100, team, pm);
                    if (bids.Count > 0)
                    {
                        int bid = bids[idinunits % bids.Count];

                        int xofnearunit = (int)map.buidingData[bid].x, yofnearunit = (int)map.buidingData[bid].y;
                        int nowlenchtoid = Math.Abs(x - xofnearunit) + Math.Abs(y - yofnearunit);
                        int l = 9999;
                        float k = 7;
                        for (int i = 0; i < map.height; i++)
                            for (int j = 0; j < map.width; j++)
                            {
                                l = Math.Abs(j - xofnearunit) + Math.Abs(i - yofnearunit);
                                if (l < nowlenchtoid)
                                {
                                    map.ai[i, j] += (int)((nowlenchtoid - l) * k);
                                }
                            }
                    }
                }
            }
            if (HasProperty(UnitProperty.CanCaptureVillages))
                foreach (Building b in map.buidingData)
                    if (pm.players[b.fraction].team != pm.players[team].team)
                        map.ai[(int)b.y, (int)b.x] += 200;
            if (HasProperty(UnitProperty.CanCaptureCastles))
                foreach (Building b in map.castleData)
                    if (pm.players[b.fraction].team != pm.players[team].team)
                        map.ai[(int)b.y, (int)b.x] += 300;
            if (HasProperty(UnitProperty.CanDestroyVillages))
                foreach (Building b in map.buidingData)
                    if (pm.players[b.fraction].team != pm.players[team].team && b.fraction != 0)
                        for (int i = -baseAttackRangeMax; i <= baseAttackRangeMax; i++)
                            for (int j = -baseAttackRangeMax; j <= baseAttackRangeMax; j++)
                                if (Math.Abs(i) + Math.Abs(j) >= baseAttackRangeMin)
                                    try { map.ai[(int)b.y + i, (int)b.x + j] += 20; }
                                    catch { ;}
            if (HasProperty(UnitProperty.CanRaise))
                foreach (Building tmb in tombs)
                    if (map.fractionUnitData[tmb.y, tmb.x] == -1)
                        for (int i = -1; i <= 1; i++)
                            for (int j = -1; j <= 1; j++)
                                if (Math.Abs(i) + Math.Abs(j) == 1)
                                    try{map.ai[(int)tmb.y + i, (int)tmb.x + j] += 100;}catch { ;}
            if (HasProperty(UnitProperty.Lighting))
                foreach (Unit u in um.units)
                    if (u.team == team)
                        for (int i = -2; i <= 2; i++)
                            for (int j = -2; j <= 2; j++)
                                try { map.ai[(int)u.position.Y + i, (int)u.position.X + j] += 25; }
                                catch { ;}
            foreach (Unit u in um.units)
            {
                if (pm.players[u.team].team != pm.players[team].team && !HasProperty(UnitProperty.CanNotAttackAndMoveAtOnce))
                    for (int i = -baseAttackRangeMax; i <= baseAttackRangeMax; i++)
                        for (int j = -baseAttackRangeMax; j <= baseAttackRangeMax; j++)
                            if (Math.Abs(i) + Math.Abs(j) >= baseAttackRangeMin)
                            {
                                try
                                {
                                    if (id == 1 && u.HasProperty(UnitProperty.Fly))
                                        map.ai[(int)u.position.Y + i, (int)u.position.X + j] += 30;
                                    else map.ai[(int)u.position.Y + i, (int)u.position.X + j] += 20;
                                    //if (HasProperty(UnitProperty.Lighting)) map.ai[(int)u.position.Y + i, (int)u.position.X + j] -= 10;
                                }
                                catch { ;}
                            }
                if (pm.players[u.team].team != pm.players[team].team && HasProperty(UnitProperty.CanNotAttackAndMoveAtOnce) && Math.Abs(x - u.position.X) + Math.Abs(y - u.position.Y) >= baseAttackRangeMin && Math.Abs(x - u.position.X) + Math.Abs(y - u.position.Y) <= baseAttackRangeMax)
                {
                    targetseted = true;
                    t = new Point(x, y);
                }
            }
            if (health < 100)
            {
                foreach (Building b in map.buidingData)
                    if (pm.players[b.fraction].team != pm.players[team].team)
                    {
                        if (health < 100) map.ai[(int)b.y, (int)b.x] += 100 - health;
                    }
                foreach (Building b in map.castleData)
                    if (pm.players[b.fraction].team != pm.players[team].team)
                    {
                        if (health < 100) map.ai[(int)b.y, (int)b.x] += 100 - health;
                    }
            }

            map.ClearAlphaData();
            int nid = um.NearUnitOn(x, y, team,pm);
            if (nid >= 0)
            {
                int xofnearunit = (int)um[nid].position.X, yofnearunit = (int)um[nid].position.Y;
                int nowlenchtoid = Math.Abs(x - xofnearunit) + Math.Abs(y - yofnearunit);
                int l = 9999;
                float k = 5;
                for (int i = 0; i < map.height; i++)
                    for (int j = 0; j < map.width; j++)
                    {
                        l = Math.Abs(j - xofnearunit) + Math.Abs(i - yofnearunit);
                        if (l < nowlenchtoid)
                        {
                            map.ai[i, j] += (int)((nowlenchtoid - l) * k);
                        }
                    }
            }

                FillMoveRange(ref map, ref um,ref pm);
                //now = map.ai[(int)position.Y, (int)position.X];
                now = -1;//map.ai[0, 0];
                if (!targetseted)
                    for (int i = 0; i < map.height; i++)
                        for (int j = 0; j < map.width; j++)
                        {
                            //map.ai[i, j] -= map.ai[i, j] * (int)((Math.Abs(x - j) + Math.Abs(y - i)) / 5 * 0.5);
                            if (now < map.ai[i, j] && map.fractionUnitData[i, j] == -1 && map.alphaData[i, j] > 0)
                            {
                                now = map.ai[i, j];
                                //t = new Point((int)position.X, (int)position.Y);
                                t = new Point(j,i);
                            }
                        }

            return t;
        }

        public Point GetAiTargetTest(ref Map map, ref UnitManager um, List<Building> tombs, ref PlayerManager pm)
        {
            Point target = new Point((int)position.X, (int)position.Y);

            List<AiPoint> points = new List<AiPoint>();

            foreach (Unit u in um.units)
            {
                if (pm.players[u.team].team != pm.players[team].team)
                {
                    int damage = GetDamage(map, u);
                    if (damage < u.health)
                        points.Add(new AiPoint((int)u.position.X, (int)u.position.Y, damage , 0, 0,baseAttackRangeMax));
                    else
                        points.Add(new AiPoint((int)u.position.X, (int)u.position.Y, damage * 2, 0, 0,baseAttackRangeMax));
                }
                if (HasProperty(UnitProperty.Lighting))
                    if (u.team == team) points.Add(new AiPoint((int)u.position.X, (int)u.position.Y, 200, 0, 0,2));
            }

            List<Building> bl = new List<Building>();
            foreach (Building b in map.castleData)
            {
                if (HasProperty(UnitProperty.CanCaptureCastles))
                    if (pm.players[b.fraction].team != pm.players[team].team)
                        if (um.GetIdByPosition(b.x, b.y) < 0)
                        {
                            bl.Add(b);
                        }
            }

            foreach (Building b in map.buidingData)
            {
                if (HasProperty(UnitProperty.CanCaptureVillages))
                    if (pm.players[b.fraction].team != pm.players[team].team)
                        if (um.GetIdByPosition(b.x, b.y) < 0 || ((int)position.X==b.x&&(int)position.Y== b.y))
                        {
                            bl.Add(b);
                            //map.ClearAlphaData();
                                //FillMoveAllMap(ref map, ref um, target.X, target.Y);
                        }
                            //points.Add(new AiPoint(b.x, b.y, 300, 0, 0,0));
                if (HasProperty(UnitProperty.CanDestroyVillages))
                    if (pm.players[b.fraction].team != pm.players[team].team)
                        points.Add(new AiPoint(b.x, b.y, 300, 0, 0,baseAttackRangeMax));
                if (pm.players[team].team == pm.players[b.fraction].team)
                    if (health != 100)
                        points.Add(new AiPoint(b.x, b.y, 100 - health, 0, 0,0));
            }
            if (bl.Count > 0)
            {
                int minl = 9999;
                foreach (Building b in bl)
                {
                    int l = Math.Abs(b.x - (int)position.X) + Math.Abs(b.y - (int)position.Y);
                    if (minl > l) { minl = l; target = new Point(b.x, b.y); }
                }
                if (!(target == new Point((int)position.X, (int)position.Y)&& pm.players[map.GetBuilding((int)position.X, (int)position.Y).fraction].team == pm.players[team].team))
                {
                    map.ClearAlphaData();
                    FillMoveAllMap(ref map, ref um, target.X, target.Y,pm);
                    return target;
                }
            }
            foreach (Building b in tombs)
            {
                points.Add(new AiPoint(b.x, b.y, 400, 0, 0, 1));
            }

            if (HasProperty(UnitProperty.CanCaptureVillages) || HasProperty(UnitProperty.CanCaptureCastles))
            {
                map.ClearAlphaData();
            }

            if (points.Count > 0)
            {
                AiPoint[] parr = points.ToArray();
                int minimuml = 9999;
                int maxm = -6666;
                for (int i = 0; i < points.Count; i++)
                {
                    parr[i].l = Math.Abs(parr[i].x - (int)position.X) + Math.Abs(parr[i].y - (int)position.Y);
                    if (minimuml > parr[i].l) minimuml = parr[i].l;
                }
                for (int i = 0; i < points.Count; i++)
                {
                    if(parr[i].l!=0)
                    parr[i].m = parr[i].m * minimuml / parr[i].l;
                    if (maxm < parr[i].m)
                    {
                        maxm = parr[i].m;
                        //target = new Point(parr[i].x, parr[i].y);
                    }
                }
                points.Clear();
                for (int i = 0; i < parr.Length; i++)
                {
                    if (parr[i].r > 0)
                        for (int k = -parr[i].r; k < parr[i].r; k++)
                            for (int l = -parr[i].r; l < parr[i].r; l++)
                                if (Math.Abs(k + l) <= parr[i].r && Math.Abs(k + l)!=0)
                                {
                                    int x = parr[i].x + k;
                                    int y = parr[i].y + l;
                                    if (x >= 0 && x < map.width && y >= 0 && y < map.height&&um.GetIdByPosition(x,y)<0)
                                    {
                                        int m = parr[i].m + Map.TERRAIN_DEFENCE_BONUS[Map.TILES_IN_TERRAIN[map.mapData[y, x]]];
                                        if (HasProperty(UnitProperty.CanGoOnWater)&&Map.TILES_IN_TERRAIN[map.mapData[y,x]]==5) m += 25;
                                        points.Add(new AiPoint(x, y, m, 0, 0, 0));
                                    }
                                }
                    if (parr[i].r == 0) points.Add(new AiPoint(parr[i].x, parr[i].y, parr[i].m, 0, 0, 0));
                }
                maxm = -9999;
                foreach (AiPoint p in points)
                {
                    if (maxm < p.m)
                    {
                        maxm = p.m;
                        target = new Point(p.x, p.y);
                    }
                }
            }

            map.ClearAlphaData();
            //if (Math.Abs(target.X - position.X) + Math.Abs(target.Y - position.Y) <= baseMoveRange)
            //    FillMoveRange(ref map, ref um, ref pm);
            //else
                FillMoveAllMap(ref map, ref um, target.X, target.Y,pm);

            return target;
        }

        public int GetDamage(Map map, Unit target)
        {
            return baseAttackMax + baseAttackMin + baseDefence + GetAttackBonus(map, target) + GetDeffenceBonus(map, target) * health / 100;
        }

        public string GetDescription()
        {
            string r="";

            switch (id)
            {
                case 0: r = Game.langManager[GameString.SoldierDescription]; break;
                case 1: r = Game.langManager[GameString.ArcherDescription]; break;
                case 2: r = Game.langManager[GameString.LizardDescription]; break;
                case 3: r = Game.langManager[GameString.SoreccerDescription]; break;
                case 4: r = Game.langManager[GameString.WispDescription]; break;
                case 5: r = Game.langManager[GameString.WolfDescription]; break;
                case 6: r = Game.langManager[GameString.GolemDescription]; break;
                case 7: r = Game.langManager[GameString.CatapultDescription]; break;
                case 8: r = Game.langManager[GameString.WyvernDescription]; break;
                case 9: r = Game.langManager[GameString.ComanderDescription]; break;
                case 10: r = Game.langManager[GameString.SkeletonDescription]; break;
                case 11: r = Game.langManager[GameString.CrystalDescription]; break;
                case 12: r = Game.langManager[GameString.SecondCommanderDescription]; break;
                case 13: r = Game.langManager[GameString.Lizard2Description]; break;
                case 14: r = Game.langManager[GameString.SoreccerDescription]; break;
                case 15: r = Game.langManager[GameString.SpiderDescription]; break;
            }

            return r;
        }
    }
}