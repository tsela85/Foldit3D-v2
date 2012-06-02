﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Foldit3D
{
    enum PowerUpType { HoleSize, HolePos, PlayerSize, PlayerPos, SplitPlayer, DryPlayer, NormalPlayer };

    class PowerUp
    {
        float ROTATION_DEGREE = 0.01f;
        float rotAngle;
        float angle;
        bool reverse = false;
        Vector2 center = Vector2.Zero;
        double radius;
        bool dataWasCalced = false;
        bool moving = true;
        bool isDraw = true;
        bool drawInFold = false;


        Texture2D texture;
        Vector2 worldPosition;
        Rectangle worldRectangle;
        PowerUpType type;

        protected VertexPositionTexture[] vertices;
        protected Matrix worldMatrix = Matrix.Identity;
        protected Effect effect;

        public PowerUp(Texture2D t, PowerUpType ty, List<List<Vector3>> points, Effect e) 
        {
            texture = t;
            type = ty;
            effect = e;
            setUpVertices(points);
        }

        #region Properties
        public Vector2 WorldPosition
        {
            get { return worldPosition; }
            set { worldPosition = value; }
        }

        public Rectangle WorldRectangle
        {
            get
            {
               return worldRectangle;
            }
        }
        #endregion

        #region Action
        public void doYourThing(Player player)
        {
            switch (type)
            {
                // V
                case PowerUpType.HoleSize:
                    HoleManager.cangeAllHolesSize();
                    break;
                // V
                case PowerUpType.HolePos:
                    HoleManager.changeAllHolesPlace();
                    break;
                // V
                case PowerUpType.PlayerSize:
                    player.changeSize();
                    break;
                // V
                case PowerUpType.PlayerPos:
                    player.changePos(new Random().Next(-25, 25));
                    break;
                case PowerUpType.SplitPlayer:
                    player.changePlayerType("duplicate", (int)worldPosition.X, (int)worldPosition.Y);
                    break;
                case PowerUpType.DryPlayer:
                    player.changePlayerType("static", (int)worldPosition.X, (int)worldPosition.Y);
                    break;
                case PowerUpType.NormalPlayer:
                    player.changePlayerType("normal", (int)worldPosition.X, (int)worldPosition.Y);
                    break;
            }
        }
        #endregion

        #region Update
        public void Update(GameState state)
        {
            //if (state == GameState.folding && dataWasCalced)
            //    rotate();
            if (state != GameState.folding)
            {
                Trace.WriteLine(state);
                moving = true;
                for (int i = 0; i < vertices.Length; i++)
                    vertices[i].Position = Vector3.Transform(vertices[i].Position, worldMatrix);
            }
        }
        #endregion

        #region fold


        public void foldData(Vector3 axis, Vector3 point, float a)
        {
            float angle = MathHelper.ToDegrees(a);
            drawInFold = true;
            Vector3 center = getCenter();
            float test = MathHelper.ToDegrees((float)Math.Atan2(axis.X - getCenter().X, axis.Z - getCenter().Z));

            //  if (angle > -167 && angle < 0 && moving)
            if ((a > -MathHelper.Pi + Game1.closeRate) && (moving))
            {
                if (angle < -90) isDraw = false;
                else isDraw = true;
                worldMatrix = Matrix.Identity;
                worldMatrix *= Matrix.CreateTranslation(-point);
                // worldMatrix *= Matrix.CreateFromAxisAngle(axis, -a);
                // if right or up
                worldMatrix *= Matrix.CreateFromAxisAngle(new Vector3(Math.Abs(axis.X), axis.Y, -1 * Math.Abs(axis.Z)), -a);
                // if left or down
                //worldMatrix *= Matrix.CreateFromAxisAngle(new Vector3(-1 * Math.Abs(axis.X), axis.Y, Math.Abs(axis.Z)), -a);
                worldMatrix *= Matrix.CreateTranslation(point);
            }
          /*  else if (moving)
            {

                for (int i = 0; i < vertices.Length; i++)
                {
                    worldMatrix = Matrix.Identity;
                    worldMatrix *= Matrix.CreateTranslation(-point);
                    worldMatrix *= Matrix.CreateFromAxisAngle(axis, MathHelper.Pi);
                    worldMatrix *= Matrix.CreateTranslation(point);
                    vertices[i].Position = Vector3.Transform(vertices[i].Position, worldMatrix);
                }
                worldMatrix = Matrix.Identity;
                moving = false;
            }*/
        }
/*
        public void reverseRotation()
        {
            if (rotAngle > 0)
            {
                rotAngle -= ROTATION_DEGREE;
                worldPosition.X = (int)(center.X - radius * Math.Cos(rotAngle + angle));
                worldPosition.Y = (int)(center.Y - radius * Math.Sin(rotAngle + angle));
            }
        }
        public void rotate()
        {
            if (reverse)
            {
                reverseRotation();
                return;
            }
            if (rotAngle < MathHelper.Pi)
            {
                rotAngle += ROTATION_DEGREE;
                worldPosition.X = (int)(center.X - radius * Math.Cos(rotAngle + angle));
                worldPosition.Y = (int)(center.Y - radius * Math.Sin(rotAngle + angle));
            }
            else
            {
                reverse = true;
                reverseRotation();
            }
        }*/
        #endregion

        #region Draw
        public void setDrawInFold()
        {
            drawInFold = false;
        }
        public void DrawInFold()
        {
            if (drawInFold)
                Draw();
        }

        public void Draw()
        {
            if (isDraw)
            {
                effect.CurrentTechnique = effect.Techniques["TexturedNoShading"];
                effect.Parameters["xWorld"].SetValue(worldMatrix);
                effect.Parameters["xView"].SetValue(Game1.camera.View);
                effect.Parameters["xProjection"].SetValue(Game1.camera.Projection);
                effect.Parameters["xTexture"].SetValue(texture);

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    Game1.device.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 2, VertexPositionTexture.VertexDeclaration);
                }
            }
        }
        #endregion

        #region 3D
        private void setUpVertices(List<List<Vector3>> points)
        {
            vertices = new VertexPositionTexture[6];

            for (int i = 0; i < 6; i++)
            {
                vertices[i].Position = points.ElementAt(i).ElementAt(0);
                vertices[i].TextureCoordinate.X = points.ElementAt(i).ElementAt(1).X;
                vertices[i].TextureCoordinate.Y = points.ElementAt(i).ElementAt(1).Y;
            }

        }
        public BoundingBox getBox()
        {
            Vector3[] p = new Vector3[2];
            p[0] = vertices[2].Position;
            p[1] = vertices[5].Position;
            return BoundingBox.CreateFromPoints(p);
        }

        public Vector3 getCenter()
        {
            float x = (vertices[2].Position.X + vertices[5].Position.X) / 2;
            float z = (vertices[2].Position.Z + vertices[5].Position.Z) / 2;
            return new Vector3(x, 0, z);
        }
        #endregion
    }
}
