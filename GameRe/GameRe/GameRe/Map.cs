using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game
{
    public class Building
    {
        public int x;
        public int y;
        public int fraction;

        public Building() { ;}
        public Building(int _x,int _y,int _f) 
        {
            x = _x;
            y = _y;
            fraction = _f;
        }
    }

    public class Map
    {
        public int width;
        public int height;
        public int[,] mapData;
        public int[,] alphaData;
        public int[,] fractionUnitData;
        public int[,] ai;
        public List<Building> buidingData;
        public List<Building> castleData;
        public string name;

        public static int[] TERRAIN_DEFENCE_BONUS = new int[] { 0, 5, 10, 10, 15, 0, 5, 15, 15, 15 };
        public static int[] TERRAIN_STEPS_REQUIRED = new int[] { 1, 1, 2, 2, 3, 3, 1, 1, 1, 1 };
        public static int[] TILES_IN_TERRAIN = new int[] { 1, 5, 0, 0, 0, 0, 0, 0, 
                                                           5, 5, 5, 5, 5, 5, 5, 5,
                                                           5, 5, 5, 5, 5, 5, 5, 5, 
                                                           5, 5, 5, 5, 5, 5, 5, 5, 
                                                           5, 5, 5, 5, 5, 5, 5, 5, 
                                                           5, 0, 0, 0, 0, 0, 0, 0,
                                                           0, 0, 0, 0, 6, 6, 6, 6,
                                                           6, 6, 6, 6, 6, 6, 6, 6,
                                                           6, 6, 6, 6, 3, 3, 3, 4, 
                                                           2, 2, 2, 2, 1, 1, 1, 1,
                                                           1, 1, 5, 5, 5, 0, 0, 0, 
                                                           0, 0, 0, 0, 0, 0, 0, 0, 
                                                           0, 0, 0, 0, 0, 0, 0, 0, 
                                                           0, 0, 0, 0, 0, 0, 0, 0, 
                                                           0, 3, 1, 0, 0, 0, 0, 0, 
                                                           3, 7, 3, 7, 7, 9, 8, 8};
        public Map()
        {
            width = 5;
            height = 5;

            mapData = new int[height, width];
            alphaData = new int[height, width];
            fractionUnitData = new int[height, width];
            ai = new int[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    mapData[i, j] = 0;
                    alphaData[i, j] = 0;
                }
            }
            mapData[1, 0] = 61;
            mapData[4, 0] = 63;

            buidingData = new List<Building>();
            buidingData.Add(new Building(1, 0, 0));
            buidingData.Add(new Building(4, 0, 0));
            castleData = new List<Building>();
            castleData.Add(new Building(1, 0, 0));
            name = "";
        }

        public Map(int w,int h)
        {
            width = w;
            height = h;

            mapData = new int[height, width];
            alphaData = new int[height, width];
            fractionUnitData = new int[height, width];
            ai = new int[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    mapData[i, j] = 0;
                    alphaData[i, j] = 0;
                }
            }

            buidingData = new List<Building>();
            castleData = new List<Building>();
            name = DateTime.Now.ToString();
            name = name.Replace('.', '_');
            name = name.Replace(':', '_');
            name = name.Replace(' ', '_');
            name = "skirmish_" + name;
            //name = name + "am";
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera, Texture2D mapTileset,Texture2D fractions,Texture2D alpha,ref ParticleSystem particleSystem,Color lightColor,bool mainmenu=false)
        {
            Vector2 offset = camera.offset;
            if (mainmenu) camera.offset = camera.offset + new Vector2(100, 0);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (alphaData[i, j] > 0 && alphaData[i, j] < 127)
                        spriteBatch.Draw(alpha, new Vector2(j * 64, i * 64) - camera.position + camera.offset, new Rectangle(0, 0, 64, 64), lightColor, 0, Vector2.Zero, 1, SpriteEffects.None, camera.GetZ(i * 64 - camera.position.Y + camera.offset.Y, 64));
                    if (alphaData[i, j] > 0 && alphaData[i, j] == 127)
                        spriteBatch.Draw(alpha, new Vector2(j * 64, i * 64) - camera.position + camera.offset, new Rectangle(64, 0, 64, 64), lightColor, 0, Vector2.Zero, 1, SpriteEffects.None, camera.GetZ(i * 64 - camera.position.Y + camera.offset.Y, 64));
                    spriteBatch.Draw(mapTileset, new Vector2(j * 64, i * 64) - camera.position + camera.offset, new Rectangle((mapData[i, j] % 8) * 64, (mapData[i, j] / 8) * 64, 64, 64), lightColor, 0, Vector2.Zero, 1, SpriteEffects.None, camera.GetZ(i * 64 - camera.position.Y + camera.offset.Y));
                    if (TILES_IN_TERRAIN[mapData[i, j]] == 9)
                        spriteBatch.Draw(mapTileset, new Vector2(j * 64, i * 64 - 64) - camera.position + camera.offset, new Rectangle((mapData[i, j] % 8) * 64, (mapData[i, j] / 8 - 1) * 64, 64, 64), lightColor, 0, Vector2.Zero, 1, SpriteEffects.None, camera.GetZ(i * 64 - camera.position.Y + camera.offset.Y, 31));
                }
            }
            foreach (Building b in buidingData)
            {
                if (b.fraction > 0&&mapData[b.y,b.x]==(63+64))
                    spriteBatch.Draw(fractions, new Vector2(b.x * 64, b.y * 64) + camera.offset - camera.position, new Rectangle(64 * (b.fraction - 1), 128, 64, 64), lightColor, 0, Vector2.Zero, 1, SpriteEffects.None, camera.GetZ(b.y * 64 - camera.position.Y + camera.offset.Y, 3));
                if (Timer.GetTimerState(1.5f,(b.x + b.y)%5 / 5.0f) && mapData[b.y, b.x] == (63+64)&&b.fraction!=0) particleSystem.AddNewParticle(new Particle(0, new Vector2(b.x * 64 + 42, b.y * 64+15), 1.5f, true));
            }
            foreach (Building b in castleData)
            {
                if (b.fraction > 0)
                    spriteBatch.Draw(fractions, new Vector2(b.x * 64, b.y * 64 - 64) + camera.offset - camera.position, new Rectangle(64 * (b.fraction - 1), 0, 64, 128), lightColor, 0, Vector2.Zero, 1, SpriteEffects.None, camera.GetZ(b.y * 64 - camera.position.Y + camera.offset.Y, 3));
            }
            camera.offset = offset;
        }
        public void DrawOutLine(SpriteBatch spriteBatch, Camera camera, Texture2D outline,bool mainmenu=false)
        {
            Vector2 offset = camera.offset;
            if (mainmenu) camera.offset = camera.offset + new Vector2(100, 0);
            for (int i = 0; i < height; i++)
            {
                spriteBatch.Draw(outline, new Vector2(-1, i) * 64 - camera.position + camera.offset, new Rectangle(0, 64, 64, 64), Color.White,0,Vector2.Zero,1,SpriteEffects.None,1);
                spriteBatch.Draw(outline, new Vector2(width, i) * 64 - camera.position + camera.offset, new Rectangle(128, 64, 64, 64), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            }

            for (int i = 0; i < width; i++)
            {
                spriteBatch.Draw(outline, new Vector2(i, -1) * 64 - camera.position + camera.offset, new Rectangle(64, 0, 64, 64), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                spriteBatch.Draw(outline, new Vector2(i, height) * 64 - camera.position + camera.offset, new Rectangle(64, 128, 64, 64), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            }
            
            spriteBatch.Draw(outline, new Vector2(-1, -1) * 64 - camera.position + camera.offset, new Rectangle(0, 0, 64, 64), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.Draw(outline, new Vector2(width, -1) * 64 - camera.position + camera.offset, new Rectangle(128, 0, 64, 64), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);           
            spriteBatch.Draw(outline, new Vector2(-1, height) * 64 - camera.position + camera.offset, new Rectangle(0, 128, 64, 64), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.Draw(outline, new Vector2(width, height) * 64 - camera.position + camera.offset, new Rectangle(128, 128, 64, 64), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            camera.offset = offset;
        }

        public void DestroyBuilding(int x, int y)
        {
            Building deletedb = new Building();
            foreach (Building b in buidingData)
            {
                if (b.x == x && b.y == y)
                    deletedb = b;
            }
            buidingData.Remove(deletedb);
        }
        public void DestroyCastle(int x, int y)
        {
            Building deletedc = new Building();
            foreach (Building b in castleData)
            {
                if (b.x == x&& b.y == y)
                    deletedc = b;
            }
            castleData.Remove(deletedc);
        }

        public Building GetBuilding(int x, int y)
        {
            Building r = new Building();
            r = null;
            foreach (Building b in buidingData)
            {
                if (b.x == x && b.y == y)
                    r = b;
            }
            return r;
        }
        public Building GetVillage(int x, int y)
        {
            Building r = new Building();
            r = null;
            foreach (Building b in buidingData)
            {
                if (b.x == x && b.y == y && TILES_IN_TERRAIN[mapData[y, x]]!=9)
                    r = b;
            }
            return r;
        }
        public Building GetCastle(int x, int y)
        {
            Building r = new Building();
            r = null;
            foreach (Building b in castleData)
            {
                if (b.x == x && b.y == y)
                    r = b;
            }
            return r;
        }

        public bool IsBuildingOnPosition(int x, int y)
        {
            for (int i = 0; i < buidingData.Count; i++)
                if (buidingData[i].x == x && buidingData[i].y == y)
                    return true;
            for (int i = 0; i <  castleData.Count; i++)
                if (castleData[i].x == x && castleData[i].y == y)
                    return true;
            return false;
        }
        public bool IsCastleOnPosition(int x, int y)
        {
            for (int i = 0; i < castleData.Count; i++)
                if (castleData[i].x == x && castleData[i].y == y)
                    return true;
            return false;
        }
        public bool IsVillageOnPosition(int x, int y)
        {
            for (int i = 0; i < buidingData.Count; i++)
                if (buidingData[i].x == x && buidingData[i].y == y)
                    return true;
            return false;
        }

        public void ClearAlphaData()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    alphaData[i, j] = 0;
                }
            }
        }
        public bool IsInMoveRange(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
                return alphaData[y, x] > 0 && alphaData[y, x] < 127;
            return false;
        }
        public bool IsInAttackRange(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
                return alphaData[y, x] == 127;
            return false;
        }

        public int GetNearestVilage(int x, int y,int fraction)
        {
            int id = -1;
            int lenchto=1000;

            for (int i = 0; i < buidingData.Count; i++)
            {
                int l=Math.Abs(buidingData[i].x-x)+Math.Abs(buidingData[i].y-y);
                if ((id == -1||l<lenchto)&&buidingData[i].fraction!=fraction) {id = i;lenchto=l;}
            }

            return id;
        }

        public int GetNearestCastle(int x, int y, int fraction)
        {
            int id = -1;
            int lenchto = 1000;

            for (int i = 0; i < castleData.Count; i++)
            {
                int l = Math.Abs(castleData[i].x - x) + Math.Abs(castleData[i].y - y);
                if ((id == -1 || l < lenchto) && castleData[i].fraction != fraction) { id = i; lenchto = l; }
            }

            return id;
        }

        public void UpdateFractionData(UnitManager um)
        { 
            for(int i=0;i<height;i++)
                for(int j=0;j<width;j++)
                    fractionUnitData[i,j]=-1;
            foreach (Unit u in um.units)
            {
                fractionUnitData[(int)u.position.Y, (int)u.position.X] = u.team;
            }
        }

        public int counterOfMoveCells()
        {
            int c = 0;
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    if (alphaData[i, j] > 0) c++;

            return c;
        }
        public List<int> GetVillagesOfNotFractions(int x, int y, int minr, int maxr, int fraction, PlayerManager pm)
        {
            List<int> l = new List<int>();
            for (int i = 0; i < buidingData.Count; i++)
                if (Math.Abs(buidingData[i].x - x) + Math.Abs(buidingData[i].y - y) >= minr && Math.Abs(buidingData[i].x - x) + Math.Abs(buidingData[i].y - y) <= maxr && pm.players[fraction].team != pm.players[buidingData[i].fraction].team && buidingData[i].fraction != 0)
                    l.Add(i);
            return l;
        }
        public List<int> GetVillagesOfNotFractionsWhithZero(int x, int y, int minr, int maxr, int fraction, PlayerManager pm)
        {
            List<int> l = new List<int>();
            for (int i = 0; i < buidingData.Count; i++)
                if (Math.Abs(buidingData[i].x - x) + Math.Abs(buidingData[i].y - y) >= minr && Math.Abs(buidingData[i].x - x) + Math.Abs(buidingData[i].y - y) <= maxr && pm.players[fraction].team != pm.players[buidingData[i].fraction].team)
                    l.Add(i);
            return l;
        }

        public int countBuildingOfFraction(int fraction)
        {
            int i=0;
            foreach (Building b in buidingData)if(b.fraction==fraction)i++;
            return i;
        }
        public int countCastlesOfFraction(int fraction)
        {
            int i = 0;
            foreach (Building b in castleData) if (b.fraction == fraction) i++;
            return i;
        }

    }
}