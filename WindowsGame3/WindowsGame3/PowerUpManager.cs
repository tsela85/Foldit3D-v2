﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Foldit3D
{
    class PowerUpManager
    {
        Texture2D texture;
        private static List<PowerUp> powerups;
        private Effect effect;

        public PowerUpManager(Texture2D texture, Effect e)
        {
            this.texture = texture;
            powerups = new List<PowerUp>();
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
                powerups.Add(new PowerUp(texture, ConvertType(Convert.ToInt32(item["type"])), lst, effect));
            }
        }

        public void restartLevel()
        {
            powerups.Clear();
        }
        #endregion

        #region Draw
        public void setDrawInFold()
        {
            foreach (PowerUp p in powerups)
                p.setDrawInFold();
        }
        public void DrawInFold()
        {
            foreach (PowerUp p in powerups)
                p.DrawInFold();
        }
        public void Draw()
        {
            foreach (PowerUp p in powerups)
                p.Draw();
        }
        #endregion

        #region Update
        public void Update(GameState state)
        {
            foreach (PowerUp p in powerups)
                p.Update(state);
        }
        #endregion

        #region Public Methods

        public void foldData(Vector3 vec, Vector3 point, float angle, Board b)
        {
            foreach (PowerUp p in powerups)
            {
                if (b.PointInBeforeFold(p.getCenter()))
                    p.foldData(vec, point, angle);
            }
        }
        #endregion

        #region Collision
        public static void checkCollision(Player player)
        {
            PowerUp pToRemove = null;
            foreach (PowerUp p in powerups)
            {
                BoundingBox b1 = p.getBox();
                b1.Max.X -= 1.0f;
                b1.Max.Z -= 1.0f;
                b1.Min.X += 1.0f;
                b1.Min.Z += 1.0f; 
                BoundingBox b2 = player.getBox();
                if (b1.Intersects(b2))
                {
                    p.doYourThing(player);
                    pToRemove = p;
                    break;
                }
            }
            if (pToRemove!=null)
                powerups.Remove(pToRemove);
        }
        #endregion

        #region Private Methods
        private PowerUpType ConvertType(int type)
        {
            switch (type)
            {
                case 0:
                    return PowerUpType.HoleSize;
                case 1:
                    return PowerUpType.HolePos;
                case 2:
                    return PowerUpType.PlayerSize;
                case 3:
                    return PowerUpType.PlayerPos;
                case 4:
                    return PowerUpType.SplitPlayer;
                case 5:
                    return PowerUpType.DryPlayer;
                case 6:
                    return PowerUpType.NormalPlayer;
                default:
                    return PowerUpType.NormalPlayer;
            }
        }
        #endregion

    }
}
