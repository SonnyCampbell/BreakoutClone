using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Pong
{
    public static class Collisions
    {
        public static Color[,] TextureTo2DArray(Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                    colors2D[x, y] = colors1D[x + y * texture.Width];

            return colors2D;
        }

        public static Vector2 TexturesCollide(Color[,] tex1, Matrix mat1, Color[,] tex2, Matrix mat2)
        {
            Matrix mat1to2 = mat1 * Matrix.Invert(mat2);
            int width1 = tex1.GetLength(0);
            int height1 = tex1.GetLength(1);
            int width2 = tex2.GetLength(0);
            int height2 = tex2.GetLength(1);

            for (int x1 = 0; x1 < width1; x1++)
            {
                for (int y1 = 0; y1 < height1; y1++)
                {
                    Vector2 pos1 = new Vector2(x1, y1);
                    Vector2 pos2 = Vector2.Transform(pos1, mat1to2);


                    int x2 = (int)pos2.X;
                    int y2 = (int)pos2.Y;
                    if ((x2 >= 0) && (x2 < width2))
                    {
                        if ((y2 >= 0) && (y2 < height2))
                        {
                            if (tex1[x1, y1].A > 0)
                            {
                                if (tex2[x2, y2].A > 0)
                                {
                                    Vector2 screenPos = Vector2.Transform(pos1, mat1);
                                    return screenPos;
                                }
                            }
                        }
                    }


                }
            }

            return new Vector2(-1, -1);
        }

        public static Vector2 CheckBallPaddleCollision( Ball ball, Paddle paddle)
        {
            Matrix ballMat = Matrix.CreateScale(ball.Scale) * Matrix.CreateTranslation(ball.X, ball.Y, 0);

            Matrix paddleMat = Matrix.CreateScale(paddle.Scale) * Matrix.CreateTranslation(paddle.X, paddle.Y, 0);

            Vector2 BPCollisionPoint = TexturesCollide(paddle.ColorArray, paddleMat, ball.ColorArray, ballMat);

            if (BPCollisionPoint.X > -1)
            {
                return BPCollisionPoint;
            }

            return new Vector2(-1, -1);



        }

        public static void BallPaddleCollision(Ball ball, Paddle paddle)
        {
            // If hitting the side of the paddle the ball is coming toward, 
            // switch the ball's horz direction
            float ballMiddle = (ball.X + (ball.Width / 2));
            float paddleMiddle = (paddle.X + paddle.Width) / 2;
            if ((ballMiddle < paddle.X && ball.SpeedX > 0) ||
                (ballMiddle > (paddle.X + paddle.Width) && ball.SpeedX < 0))
            {
                ball.ChangeHorzDirection();
            }

            // Go back up the screen and speed up
            ball.ChangeVertDirection();
            ball.SpeedUp();
        }

        public static bool CheckBallScreenCollision(Ball ball,ref float delayTimer)
        {
            int maxX = Game1.graphics.GraphicsDevice.Viewport.Width - ball.Width;
            int maxY = Game1.graphics.GraphicsDevice.Viewport.Height - ball.Height;

            // Check for bounce. Make sure to place ball back inside the screen
            // or it could remain outside the screen on the next iteration and cause
            // a back-and-forth bouncing logic error.
            if (ball.X > maxX)
            {
                ball.ChangeHorzDirection();
                ball.X = maxX;
                return false;
            }
            else if (ball.X < 0)
            {
                ball.ChangeHorzDirection();
                ball.X = 0;
                return false;
            }

            if (ball.Y < 0)
            {
                ball.ChangeVertDirection();
                ball.Y = 0;
                return false;
            }
            else if (ball.Y > maxY)
            {
                // Game over - reset ball
                Game1.crashSound.Play();
                ball.Reset();

                // Reset timer and stop ball's Update() from executing
                delayTimer = 0;
                ball.Enabled = false;
                return true;
            }

            return false;
        }

        public static void CheckBallBrickCollision(Ball ball, List<List<Brick>> brickRows, out Vector2 colPoint, out Brick colBrick)
        {
            Matrix ballMat = Matrix.CreateScale(ball.Scale) * Matrix.CreateTranslation(ball.X, ball.Y, 0);

            for (int j = (brickRows.Count - 1); j >= 0; j--)
            {

                for (int i = (brickRows[j].Count - 1); i >= 0; i--)
                {

                    Brick brick = brickRows[j][i];

                    if (!brick.IsBroken && ball.Boundary.Intersects(brick.Boundary))
                    {
                        Matrix brickMat = Matrix.CreateScale(brick.ScaleVector.X, brick.ScaleVector.Y, 0) * Matrix.CreateTranslation(brick.X, brick.Y, 0);

                        Vector2 ballBrickCollisionPoint = Collisions.TexturesCollide(brick.ColorArray, brickMat, ball.ColorArray, ballMat);

                        if (ballBrickCollisionPoint.X > -1)
                        {
                            brick.IsBroken = true;
                            brickRows[j].RemoveAt(i);
                            colPoint = ballBrickCollisionPoint;
                            colBrick = brick;
                            return;
                        }
                    }
                }
            }

            colPoint = new Vector2(-1, -1);
            colBrick = null;
        }

        public static void BallBrickCollision(Ball ball, Vector2 colPoint, Brick brick)
        {
            float ballMiddle = (ball.X + (ball.Width / 2));

            if (colPoint.Y >= (brick.Y + brick.Height - 1)
                || (colPoint.Y <= (brick.Y) && (ballMiddle > brick.X ||  ballMiddle < (brick.X + brick.Width))))
            {
                ball.ChangeVertDirection();
            }
            else if (ball.MidY > (brick.Y + brick.Height) && ball.SpeedY < 0)
            {
                ball.ChangeVertDirection();
                ball.ChangeHorzDirection();
            }
            else 
            {
                ball.ChangeHorzDirection();
            }
                
        }
    }
}
