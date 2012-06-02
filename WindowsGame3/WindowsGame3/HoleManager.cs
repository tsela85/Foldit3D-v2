﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Foldit3D
{
    class HoleManager
    {
        Texture2D texture;
        private static List<Hole> holes;
        private Effect effect;

        public HoleManager(Texture2D texture, Effect e)
        {
            this.texture = texture;
            holes = new List<Hole>();
            effect = e;
        }

        #region Levels

        public void initLevel(List<IDictionary<string, string>> data)
        {
            foreach (IDictionary<string, string> item in data)
            {
                List<List<Vector3>> lst = new List<List<Vector3>>();
                for (int i = 1; i < 7; i++)
                {
                    List<Vector3> pointsData = new List<Vector3>();
                    Vector3 point = new Vector3((float)Convert.ToDouble(item["x" + i]), (float)Convert.ToDouble(item["y" + i]), (float)Convert.ToDouble(item["z" + i]));
                    Vector3 texLoc = new Vector3(Convert.ToInt32(item["tX" + i]), Convert.ToInt32(item["tY" + i]), 0);
                    pointsData.Add(point);
                    pointsData.Add(texLoc);
                    lst.Add(pointsData);
                }
                holes.Add(new Hole(texture, lst, effect));
            }
        }

        public void restartLevel()
        {
            holes.Clear();
        }
        #endregion

        #region Draw
        public void setDrawInFold()
        {
            foreach (Hole h in holes)
                h.setDrawInFold();
        }
        public void DrawInFold()
        {
            foreach (Hole h in holes)
                h.DrawInFold();
        }
        public void Draw()
        {
            foreach (Hole hole in holes)
                hole.Draw();
        }
        #endregion

        #region Update
        public void Update(GameState state)
        {
            foreach (Hole h in holes)
                h.Update(state);
        }
        #endregion

        #region Public Methods

        public void foldData(Vector3 vec, Vector3 point, float angle, Board b)
        {
            foreach (Hole h in holes)
            {
                if (b.PointInBeforeFold(h.getCenter()))
                    h.foldData(vec, point, angle);
            }
        }
        #endregion

        #region Collision
        public static void checkCollision(Player player)
        {
            foreach (Hole h in holes)
            {
                BoundingBox b1 = h.getBox();
                b1.Max.X -= 1.0f;
                b1.Max.Z -= 1.0f;
                b1.Min.X += 1.0f;
                b1.Min.Z += 1.0f; 
                BoundingBox b2 = player.getBox();
                if (b1.Intersects(b2))
                {
                    // WIN!!!
                    Trace.WriteLine("WIN!!!!!!");
                    GameManager.winLevel();
                    break;
                }
            }
        }
        #endregion

        #region ChangeHoles
        public static void changeAllHolesPlace()
        {
            foreach (Hole h in holes)
                h.initializeHole(new Random().Next(-15, 15));
        }
        public static void cangeAllHolesSize()
        {
            foreach (Hole h in holes)
                h.changeSize();
        }
        #endregion
    }
}
