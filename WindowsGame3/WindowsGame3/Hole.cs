using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Foldit3D
{
    class Hole
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

        protected VertexPositionTexture[] vertices;
        protected Matrix worldMatrix = Matrix.Identity;
        protected Effect effect;

        public Hole(Texture2D texture, List<List<Vector3>> points, Effect e)
        {
            this.texture = texture;
           // worldRectangle = new Rectangle((int)WorldPosition.X, (int)WorldPosition.Y, texture.Width, texture.Height);
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

        #region Update
        public void Update(GameState state)
        {
           // if (state == GameState.folding && dataWasCalced)
           //     rotate();
            if (state != GameState.folding)
            {
                Trace.WriteLine(state);
                moving = true;
                for (int i = 0; i < vertices.Length; i++)
                    vertices[i].Position = Vector3.Transform(vertices[i].Position, worldMatrix);
            }
        }
        #endregion

        #region Fold

        public void foldData(Vector3 axis, Vector3 point, float a)
        {
            float angle = MathHelper.ToDegrees(a);
            drawInFold = true;

            //  if (angle > -167 && angle < 0 && moving)
            if ((a > -MathHelper.Pi + Game1.closeRate) && (moving))
            {
                if (angle < -90) isDraw = false;
                else isDraw = true;
                worldMatrix = Matrix.Identity;
                worldMatrix *= Matrix.CreateTranslation(-point);
                // if right or up
                worldMatrix *= Matrix.CreateFromAxisAngle(new Vector3(Math.Abs(axis.X), axis.Y, -1 * Math.Abs(axis.Z)), -a);
                // if left or down
                // worldMatrix *= Matrix.CreateFromAxisAngle(new Vector3(-1 * Math.Abs(axis.X), axis.Y, Math.Abs(axis.Z)), -a);
                worldMatrix *= Matrix.CreateTranslation(point);
            }
            
           /* else if (moving)
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
        }
 * */
        #endregion

        #region Public Methods
        public void initializeHole(int random)
        {
            for (int i = 0; i < 6; i++)
            {
                vertices[i].Position.X -= random;
                vertices[i].Position.Z += random;
            }
        }

        public void changeSize()
        {
            vertices[0].Position.X -= 2;
            vertices[0].Position.Z += 2;
            vertices[1].Position.X += 2;
            vertices[1].Position.Z -= 2;
            vertices[2].Position.X -= 2;
            vertices[2].Position.Z -= 2;
            vertices[3].Position.X += 2;
            vertices[3].Position.Z -= 2;
            vertices[4].Position.X -= 2;
            vertices[4].Position.Z += 2;
            vertices[5].Position.X += 2;
            vertices[5].Position.Z += 2;
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
