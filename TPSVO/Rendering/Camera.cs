using System;
using OpenTK;

namespace TPSVO.Rendering
{
    //This is the camera class as it could be set up after the tutorials on the website
    //It is important to note there are a few ways you could have set up this camera, for example
    //you could have also managed the player input inside the camera class, and a lot of the properties could have
    //been made into functions.
    
    //TL;DR: This is just one of many ways in which we could have set up the camera
    //Check out the web version if you don't know why we are doing a specific thing or want to know more about the code
    public class Camera
    {
        //We need quite the amount of vectors to define the camera
        //The position is simply the position of the camera
        //the other vectors are directions pointing outwards from the camera to define how it is rotated
        public Vector3 Position;
        private Vector3 front = -Vector3.UnitZ;
        public Vector3 Front => front;
        private Vector3 up = Vector3.UnitY;
        public Vector3 Up => up;
        private Vector3 right = Vector3.UnitX;
        public Vector3 Right => right;

        //Pitch is the rotation around the x axis, and it is explained more specifically in the tutorial how we can use this
        private float _pitch;
        public float Pitch
        {
            get => _pitch;
            set
            {
                //We clamp the pitch value between -89 and 89 to prevent the camera from going upside down, and a bunch
                //of weird "bugs" when you are using euler angles for rotation. If you want to read more about this you can try researching a topic called gimbal lock
                if (value > 89.0f)
                {
                    _pitch = 89.0f;
                }
                else if (value <= -89.0f)
                {
                    _pitch = -89.0f;
                }
                else
                {
                    _pitch = value;
                }
                UpdateVertices();
            }
        }
        //Yaw is the rotation around the y axis, and it is explained more specifically in the tutorial how we can use this
        private float yaw;
        public float Yaw
        {
            get => yaw;
            set
            {
                yaw = value;
                UpdateVertices();
            }
        }

        //The speed and the sensitivity are the speeds of respectively,
        //the movement of the camera and the rotation of the camera (mouse sensitivity)
        public float Speed = 50f;
        public float Sensitivity = 0.1f;

        //In the instructor we take in a position
        //We also set the yaw to -90, the code would work without this, but you would be started rotated 90 degrees away from the rectangle
        public Camera(Vector3 position)
        {
            Position = position;
        }

        private Matrix3 RotateY(float rad)
        {
            float c = (float)Math.Cos(rad);
            float s = (float)Math.Sin(rad);
            return new Matrix3(
                c, 0.0f, s,
                0.0f, 1.0f, 0.0f,
                -s, 0.0f, c
            );
        }

        private Matrix3 RotateZ(float rad)
        {
            float c = (float)Math.Cos(rad);
            float s = (float)Math.Sin(rad);
            return new Matrix3(
                c, -s, 0.0f,
                s, c, 0.0f,
                0.0f, 0.0f, 1.0f
            );
        }


        //This function is going to update the direction vertices using some of the math learned in the web tutorials
        private void UpdateVertices()
        {
            //First the front matrix is calculated using some basic trigonometry
            front = new Vector3(1, 0, 0);

            front = RotateY(MathHelper.DegreesToRadians(-this.Pitch)) * front;
            front = RotateZ(MathHelper.DegreesToRadians(-this.Yaw)) * front;

            //We need to make sure the vectors are all normalized, as otherwise we would get some funky results
            front = Vector3.Normalize(front);

            right = new Vector3(0, 1, 0);
            right = RotateY(MathHelper.DegreesToRadians(-this.Pitch)) * right;
            right = RotateZ(MathHelper.DegreesToRadians(-this.Yaw)) * right;
            right.Normalize();

            up = new Vector3(0, 0, 1);
            up = RotateY(MathHelper.DegreesToRadians(-this.Pitch)) * up;
            up = RotateZ(MathHelper.DegreesToRadians(-this.Yaw)) * up;
            up.Normalize();

        }
    }
}