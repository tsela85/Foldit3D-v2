using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Foldit3D
{
    class NormalPlayer : Player
    {
        private float lastAngle = 0;
        
        private bool needToStop = false;
        private bool dataSet = false;
        private bool before = false;
        private bool after = false;
        public NormalPlayer(Texture2D texture, List<List<Vector3>> points, PlayerManager pm, Effect effect) : base(texture, points, pm, effect) { }

        #region fold

        public override void foldData(Vector3 axis, Vector3 point, float a,bool beforeFold,bool afterFold)
        {
            float angle = MathHelper.ToDegrees(a);

            if (!dataSet)
            {
                dataSet = true;
                before = beforeFold;
                after = afterFold;
            }

            if (before && moving)
            {
                if (a > -MathHelper.Pi + Game1.closeRate)
                {
                    worldMatrix = Matrix.Identity;
                    worldMatrix *= Matrix.CreateTranslation(-point);
                   // worldMatrix *= Matrix.CreateFromAxisAngle(axis, -a);
                    // if right or up
                    worldMatrix *= Matrix.CreateFromAxisAngle(new Vector3(Math.Abs(axis.X), axis.Y, -1 * Math.Abs(axis.Z)), -a);
                    // if left or down
                    // worldMatrix *= Matrix.CreateFromAxisAngle(new Vector3(-1 * Math.Abs(axis.X), axis.Y, Math.Abs(axis.Z)), -a);
                    worldMatrix *= Matrix.CreateTranslation(point);
                }
                else if(a<-MathHelper.Pi+Game1.closeRate){
                    moving = false;
                    worldMatrix = Matrix.Identity;
                    worldMatrix *= Matrix.CreateTranslation(-point);
                    worldMatrix *= Matrix.CreateFromAxisAngle(axis, MathHelper.Pi);
                    worldMatrix *= Matrix.CreateTranslation(point);
                    for (int i = 0; i < vertices.Length; i++)
                    {
                        vertices[i].Position = Vector3.Transform(vertices[i].Position, worldMatrix);
                    }
                    switchPoints();
                    worldMatrix = Matrix.Identity;
                    needToStop = true;
                }
            }
            else if (after)
            {
                if(!foldBack &&lastAngle!=0 &&  (lastAngle<angle)){
                    foldBack = true;
                    switchPoints();
                }
                else if (foldBack)
                {

                    if (a > -Game1.openRate)
                    {
                         worldMatrix = Matrix.Identity;
                         worldMatrix *= Matrix.CreateTranslation(-point);
                         worldMatrix *= Matrix.CreateFromAxisAngle(axis, a + MathHelper.Pi);
                         worldMatrix *= Matrix.CreateTranslation(point);
                         //switchPoints();
                        for (int i = 0; i < vertices.Length; i++)
                        {
                           // vertices[i].Position = Vector3.Transform(vertices[i].Position, worldMatrix);
                        }
                        //switchPoints();
                    }
                    else
                    {
                        worldMatrix = Matrix.Identity;
                        worldMatrix *= Matrix.CreateTranslation(-point);
                        worldMatrix *= Matrix.CreateFromAxisAngle(-axis, a + MathHelper.Pi);
                        worldMatrix *= Matrix.CreateTranslation(point);
                    }
                }

                lastAngle = angle;
      
            }
        }


        private void switchPoints()
        {
            for (int j = 0; j < Math.Floor((double)vertices.Length / 2); j++)
            {
                VertexPositionTexture temp = new VertexPositionTexture();
                temp = vertices[j];
                vertices[j] = vertices[vertices.Length - j - 1];
                vertices[vertices.Length - j - 1] = temp;
            }
        }

        #endregion
    }
}
