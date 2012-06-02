﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Foldit3D
{
    class PlayerManager
    {
        private Texture2D texture;
        private List<Player> players;
        private Effect effect;
        public PlayerManager(Texture2D texture, Effect effect)
        {
            this.texture = texture;
            players = new List<Player>();
            this.effect = effect;
        }

        #region Levels

        public void initLevel(List<IDictionary<string, string>> data)
        {
            foreach (IDictionary<string, string> item in data)
            {
                List<List<Vector3>> lst = new List<List<Vector3>>();
                for(int i =1; i<7; i++){
                    List<Vector3> pointsData = new List<Vector3>();
                    Vector3 point = new Vector3((float)Convert.ToDouble(item["x" + i]), (float)Convert.ToDouble(item["y" + i]), (float)Convert.ToDouble(item["z" + i]));
                    Vector3 texLoc = new Vector3(Convert.ToInt32(item["tX" + i]),Convert.ToInt32(item["tY" + i]),0);
                    pointsData.Add(point);
                    pointsData.Add(texLoc);
                    lst.Add(pointsData);
                }
                players.Add(makeNewPlayer(item["type"], lst));
            }
        }

        public void restartLevel()
        {
            players.Clear();
        }
        #endregion

        #region Update and Draw

        public void Draw() {
            foreach (Player p in players)
            {
                p.Draw();
            }
        }
        public void Update(GameTime gameTime, GameState state) {
            foreach (Player p in players)
            {
                p.Update( gameTime, state);
            }
        }

        #endregion

        #region Public Methods

        public Player makeNewPlayer(String type, List<List<Vector3>> lst)
        {
            Player newP = null;
            if (type.CompareTo("normal") == 0)
            {
                newP = new NormalPlayer(texture, lst,this, effect);
            }
            else if (type.CompareTo("static") == 0)
            {
                newP = new StaticPlayer(texture, lst, this, effect);
            }
            else if (type.CompareTo("duplicate") == 0)
            {
                newP = new DuplicatePlayer(texture, lst,this, effect);
            }
            return newP;
        }

       /* public void foldOver()
        {
            foreach (Player p in players)
            {
                p.foldOver();
            }
        }*/

        public void changePlayerType(Player p,String type, int x, int y)
        {
            if (players.Contains(p))
            {
                //players.Add(makeNewPlayer(type, x, y));
                players.Remove(p);
            }
            else Trace.WriteLine("changePlayerType Error!");
        }

        public void foldData(Vector3 vec, Vector3 point, float angle, Board b)
        {
            foreach (Player p in players)
            {
                    p.foldData(vec, point, angle, b.PointInBeforeFold(p.getCenter()), b.PointInAfterFold(p.getCenter()));
            }
        }
        #endregion

    }
}
