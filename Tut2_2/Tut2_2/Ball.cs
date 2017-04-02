using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tut2_2
{
    class Ball
    {
        Model ballModel;
        float aspectRatio;
        float modelRotation;

        Vector3 ballPos;

        public Ball()
        {
            ballPos = new Vector3(0, 0, 0);
            aspectRatio = 0.0f;
            modelRotation = 0.0f;
        }

        public Vector3 getPos()
        {
            return ballPos;
        }

        public void Initialize(Model model, Vector3 position, GraphicsDeviceManager graphics)
        {
            ballModel = model;
            ballPos = position;
            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
        }

        public void Update(Vector3 position)
        {
            ballPos = position;
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            // Copy any parent transforms. 
            Matrix[] transforms = new Matrix[ballModel.Bones.Count];
            ballModel.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop
            foreach (ModelMesh mesh in ballModel.Meshes)
            {
                // This is where the mesh orientation is set, as well
                // as our camera and projection. 
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] *
                        Matrix.CreateRotationY(modelRotation) *
                        Matrix.CreateTranslation(ballPos);
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;

                }
                mesh.Draw();
            }
        }
    }
}
