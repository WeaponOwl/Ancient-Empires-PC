using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace Game
{
    public class ScriptManager
    {
        public string[] strings;
        public int nowstring;
        public int nextstring;
        public int nowcase;
        public int nowcasestartstring;
        public float waitTime;
        public bool scriptloaded;
        public bool gameStarted;

        public ScriptManager() 
        { 
            strings=new string[0];
            nowstring = 0;
            nextstring = 0;
            nowcase = 0;
            nowcasestartstring = 0;
            scriptloaded = false;
            gameStarted = false;
        }
        public bool LoadScript(string name)
        {
            try
            {
                strings = new string[0];
                strings = File.ReadAllLines("Content/Scripts/" + name);
                nowstring = 0;
                nextstring = -1;
                nowcase = 0;
                nowcasestartstring = 0;
                scriptloaded = true;
                gameStarted = false;
                for (int i = 0; i < strings.Length; i++)
                {
                    int commentid = strings[i].IndexOf(';');
                    if (commentid >= 0)
                        strings[i] = strings[i].Remove(commentid, strings[i].Length - commentid);
                    strings[i]=strings[i].Trim();
                }
                return true;
            }
            catch { ;};
            return false;
        }

        public delegate bool ScriptUpdateDelegate(GameTime time,ref ScriptManager me);
        public ScriptUpdateDelegate UpdateFunction;
        public bool Update(GameTime time,ref ScriptManager me)
        {
           return UpdateFunction(time, ref me);
        }
        public int GetCaseString(int casenum)
        {
            int k = -1;
            for (int i = 0; i < strings.Length; i++)
            {
                string command = strings[i];
                if (command.Contains("@Case"))
                {
                    command = command.Remove(0, 6);
                    int q = Convert.ToInt32(command);
                    if (q == casenum)
                    {
                        k = i;
                        break;
                    }
                }
            }
            return k;
        }
    }
}
