using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Tut2_2
{
    class Camera : GameComponent
    {
        // Attributes
        private Vector3 cameraPosition;
        private Vector3 cameraRotation;
        private float cameraMoveSpeed;
        private float cameraRotateSpeed;
        private Vector3 cameraLookAt;
        private Vector3 mouseRotationBuffer;
        private MouseState currentMouseState;
        private MouseState preMouseState;

        // Properites
        public Vector3 Position
        {
            get { return cameraPosition;  }
            set
            {
                cameraPosition = value;
                UpdateLookAt();
            }
        }

        public Vector3 Rotation
        {
            get { return cameraRotation; }
            set
            {
                cameraRotation = value;
                UpdateLookAt();
            }
        }

        public Matrix Projection
        {
            get;
            protected set;
        }

        public Matrix View
        {
            get
            {
                return Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
            }
        }

        // Constructor
        public Camera(Game game, Vector3 position, Vector3 rotation, float mov_speed, float rot_speed)
            : base(game)
        {
            cameraMoveSpeed = mov_speed;
            cameraRotateSpeed = rot_speed;

            // Setup projection matrix
            Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                Game.GraphicsDevice.Viewport.AspectRatio, 
                0.05f, 
                1000.0f
            );

            // Set camera's position and rotation
            MoveTo(position, rotation);

            preMouseState = Mouse.GetState();
        }

        // Set camera's position and rotation
        private void MoveTo(Vector3 pos, Vector3 rot)
        {
            Position = pos;
            Rotation = rot;
        }

        // Update the look at vector
        private void UpdateLookAt()
        {
            // Build a rotation matrix
            Matrix rotationMatrix = Matrix.CreateRotationX(cameraRotation.X) * Matrix.CreateRotationY(cameraRotation.Y);

            // Build look at offset vector
            Vector3 lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMatrix);

            // Update our camera's look at vector
            cameraLookAt = cameraPosition + lookAtOffset;
        }

        // Preview movement
        private Vector3 PreviewMove(Vector3 amount)
        {
            // Create a rotate matrix
            Matrix rotate = Matrix.CreateRotationY(cameraRotation.Y);
            
            // Create a movement vector
            Vector3 movement = new Vector3(amount.X, amount.Y, amount.Z);
            movement = Vector3.Transform(movement, rotate);
            
            // Return the value of camera position + movement vector
            return cameraPosition + movement;
        }

        // Camera movement
        private void Move(Vector3 scale)
        {
            MoveTo(PreviewMove(scale), Rotation);
        }

        // Update method
        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            currentMouseState = Mouse.GetState();

            KeyboardState ks = Keyboard.GetState();

            // Handle basic key movement
            Vector3 moveVector = Vector3.Zero;
            if (ks.IsKeyDown(Keys.W))
            {
                moveVector.Z = 1;
            }
            if (ks.IsKeyDown(Keys.S))
            {
                moveVector.Z = -1;
            }
            if (ks.IsKeyDown(Keys.A))
            {
                moveVector.X = 1;
            }
            if (ks.IsKeyDown(Keys.D))
            {
                moveVector.X = -1;
            }

            // Normalize the vector so that we dont move faster diagonally
            if (moveVector != Vector3.Zero)
            {
                moveVector.Normalize();
                // smooth and speed
                moveVector *= dt * cameraMoveSpeed;

                // Move the camera
                Move(moveVector);
            }


            // Handle mouse movement
            float deltaX;
            float deltaY;

            if (currentMouseState != preMouseState)
            {
                // Cache mouse location
                deltaX = currentMouseState.X - (Game.GraphicsDevice.Viewport.Width / 2);
                deltaY = currentMouseState.Y - (Game.GraphicsDevice.Viewport.Height / 2);

                mouseRotationBuffer.X -= 0.01f * deltaX * dt * cameraRotateSpeed;
                mouseRotationBuffer.Y -= 0.01f * deltaY * dt * cameraRotateSpeed;

                // no move over the ground and back to the top
                if (mouseRotationBuffer.Y < MathHelper.ToRadians(-75.0f))
                {
                    mouseRotationBuffer.Y = mouseRotationBuffer.Y - (mouseRotationBuffer.Y - MathHelper.ToRadians(-75.0f));
                }
                if (mouseRotationBuffer.Y > MathHelper.ToRadians(75.0f))
                {
                    mouseRotationBuffer.Y = mouseRotationBuffer.Y - (mouseRotationBuffer.Y - MathHelper.ToRadians(75.0f));
                }

                Rotation = new Vector3(-MathHelper.Clamp(mouseRotationBuffer.Y, MathHelper.ToRadians(-75.0f), MathHelper.ToRadians(75.0f)), MathHelper.WrapAngle(mouseRotationBuffer.X), 0);

                deltaX = 0;
                deltaY = 0;
            }

            Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);

            preMouseState = currentMouseState;


            base.Update(gameTime);
        }


    }
}
