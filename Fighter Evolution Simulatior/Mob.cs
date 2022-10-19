using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Fighter_Evolution_Simulatior
{
    /// <summary>
    /// This is the parent class for all movable objects
    /// </summary>
    public class Mob
    {
        public int ID; //A unque ID for the Mob
        public string name; //Tha name of the Mob
        public Vector2 location; //The location of the object
        public Texture2D texture; //Obj texture. Need to use TextureSet() for setting it.
        public Color color; //And the color
        private Rectangle rectanle; //Could be used for resizing the texture or for Math
        public int height, weight; //height and weight of the texture.
        public float rotation; //The current rotation angle of the Mob
        public Vector2 origin; //??  рисува спрайта спрямо origin?? и го използва за мащабиране. Need to use TextureSet() for setting it.
        public float spin; //?? За ъгли и т.н бали го
        public float layerDepth; //The layer calculation
        public bool moving = false;//Is the Mob moving or not. Used for the Mobs logic calculation
        public Vector2 placeToGo; //Sometimes the Mob wants to go place... And needs to rememeber their location

        /// <summary>
        /// If there are no specific parameters given the mob randomize itself.
        /// </summary>
        /// <param name="texture"></param>
        public Mob(Texture2D texture)
        {
            TextureSet(texture); //Adding a texture
            RandomizeMob(); //Radomizing position, color, rotation and spin
            ID = Command.GetMobID(); //Getting an unique ID
        }

        /// <summary>
        /// The mob randomizes its position, color, rotation and spin
        /// </summary>
        protected void RandomizeMob()
        {
            //adding mobes everywhere but +-15 from the end of the screen
            location.X = Command.Rnd.Next(15, Command.ResolutionWidth - 15);
            location.Y = Command.Rnd.Next(15, Command.ResolutionHeight - 15);

            //getting random colors
            switch (Command.Rnd.Next(0, 10) + 1)
            {
                case 1: this.color = Color.Black; break;
                case 2: this.color = Color.Red; break;
                case 3: this.color = Color.Orange; break;
                case 4: this.color = Color.White; break;
                case 5: this.color = Color.Blue; break;
                case 6: this.color = Color.Green; break;
                case 7: this.color = Color.Pink; break;
                case 8: this.color = Color.Silver; break;
                case 9: this.color = Color.Gainsboro; break;
                case 10: this.color = Color.BlanchedAlmond; break;
            }

            //Setting a rectangle
            this.rectanle = new Rectangle((int)this.location.X, (int)this.location.Y, height, weight);

            //Setting the rotation
            rotation = 0;

            //Setting the origin
            this.origin = new Vector2(texture.Width / 2, texture.Height / 2);

            //Setting some random sping
            spin = (float)Command.Rnd.Next(-4, 3) / 500;
        }

        /// <summary>
        /// Getter and Setter logic for the Mobs rectangle. Needed for in-game mechanics and math.
        /// </summary>
        public Rectangle Rectanle
        {
            get
            {
                return new Rectangle((int)(location.X + (height / 2f)),
                    (int)(location.Y + (weight / 2f)), height, weight);
            }
            set
            {
                this.rectanle = new Rectangle((int)(location.X + (height / 2f)),
                    (int)(location.Y + (weight / 2f)), height, weight);
            }
        }

        /// <summary>
        /// Setting the texture and the origin
        /// </summary>
        /// <param name="texture"></param>
        protected void TextureSet(Texture2D texture)
        {
            this.texture = texture;
            this.origin = new Vector2(texture.Width / 2, texture.Height / 2);          
            this.height = texture.Height;
            this.weight = texture.Width;
        }

        /// <summary>
        /// Checks if your mob points to a certain coordinates
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        protected bool _AimTerget(Vector2 target)
        {
            float rotationNeeded = (float)Math.Atan2(target.Y - location.Y, target.X - location.X);
            double r1 = _RadianToDegree(rotationNeeded) % 360;
            double r2 = _RadianToDegree(rotation) % 360;

            if ((int)(r1) >= (int)r2 - 12 && (int)(r1) <= (int)r2 + 12) return true;
            return false;
            double _RadianToDegree(float angle) => angle * (180.0 / Math.PI);//converting radians
        }

        /// <summary>
        /// Makes your Mob to point to a certain coordinates by changing its angle
        /// </summary>
        /// <param name="target"></param>
        protected void TurnTo(Vector2 target)
        {
            float rotationNeeded = (float)Math.Atan2(target.Y - location.Y, target.X - location.X);
            if (!_AimTerget(target))
            {
                if (rotationNeeded > 0)
                {
                    //rotation += 0.6f;//How fast is the rotation
                    rotation += 0.10f;//How fast is the rotation
                    if (rotationNeeded > 1f) rotation += 0.30f; //how fast is the toration 0.10f
                }
                else
                {
                    rotation -= 0.10f;
                    if (rotationNeeded < 1f) rotation -= 0.30f;
                }
            }

        }

        /// <summary>
        /// Makes your mobs go to a curtain coordinates in space with a curtain speed and effect
        /// Bigger number is slowing the acceleration (and so the total speed)
        /// </summary>
        /// <param name="target"></param>
        /// <param name="speed"></param>
        public void GotoWithEasing(Vector2 target, int speed)
        {
            TurnTo(target);
            if (_AimTerget(target))
            {

                if (speed < 5) speed = 5;
                float dx = this.location.X - target.X;
                float dy = this.location.Y - target.Y;
                this.location = new Vector2(this.location.X - (dx / speed), this.location.Y - (dy / speed));
            }

        }

        /// <summary>
        /// Makes your mob to go to a target coordinates with constant speed 100f is about 1 pixel
        /// </summary>
        /// <param name="target"></param>
        /// <param name="speed"></param>
        public void GotoWithConstantSpeed(Vector2 target, float speed)
        {
            TurnTo(target);
            if (_AimTerget(target))
            {
                Vector2 start = location;
                Vector2 end = target;
                float elapsed = 0.01f;
                float distance = Vector2.Distance(start, end);
                Vector2 direction = Vector2.Normalize(end - start);
                this.location = start;
                if ((distance * distance) > 25f) this.location += direction * speed * elapsed;
            }

        }

        /// <summary>
        /// The Mob will choose random point on the screen and move there
        /// </summary>
        public void moveRandomly()
        {
            //If the mob haveing nothing else to do, this code will radomly assignt a target
            if (moving == false)
            {
                moving = true; //indicating that the mob is moving now
                //Chouses a new place to go
                placeToGo.X = Command.Rnd.Next(5, Command.ResolutionWidth - 5);
                placeToGo.Y = Command.Rnd.Next(5, Command.ResolutionHeight - 5);
                //placeToGo = GetRandomPointNearMe(150);
            }

            //When that target location is very near we are chousing a new target ot visit
            if (Math.Abs(location.X - placeToGo.X) <= weight + 1 && Math.Abs(location.Y - placeToGo.Y) <= height + 1)
            {
                //Chouses a new place to go
                placeToGo.X = Command.Rnd.Next(5, Command.ResolutionWidth - 5);
                placeToGo.Y = Command.Rnd.Next(5, Command.ResolutionHeight - 5);
                //placeToGo = GetRandomPointNearMe(600);
            }

            GotoWithConstantSpeed2(placeToGo, 300f); //Start moving
        }

        //no rotation for the Mobs, making their movement much faster Not very well documented yet
        public void GotoWithConstantSpeed2(Vector2 target, float speed)
        {
            Vector2 start = location;
            Vector2 end = target;
            float elapsed = 0.01f;
            float distance = Vector2.Distance(start, end);
            Vector2 direction = Vector2.Normalize(end - start);
            this.location = start;
            if ((distance * distance) > 25f) this.location += direction * speed * elapsed;
        }
        public void GotoWithEasing2(Vector2 target, int speed)
        {
            if (speed < 5) speed = 5;
            float dx = this.location.X - target.X;
            float dy = this.location.Y - target.Y;
            this.location = new Vector2(this.location.X - (dx / speed), this.location.Y - (dy / speed));
        }

        /// <summary>
        /// Getting a random point on the screen, depending on its original resolution
        /// </summary>
        /// <returns></returns>
        protected Vector2 GetRandomPointOnMap()
        {
            var r = new Vector2();
            placeToGo.X = Command.Rnd.Next(5, Command.ResolutionWidth - 5);
            placeToGo.Y = Command.Rnd.Next(5, Command.ResolutionHeight - 5);
            return r;
        }

        /// <summary>
        /// Getting a random point near the Mob. The point is always inside the original resolution 
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        protected Vector2 GetRandomPointNearMe(int distance)
        {
            int k = 0;
        againLoop:
            k++;
            float x = location.X + Command.Rnd.Next(-distance, distance + 1);
            float y = location.Y + Command.Rnd.Next(-distance, distance + 1);

            var r = new Vector2(x, y);
            if (Command.InsideResolution(r, height, weight)) return r;
            if (k >= 10000)
            {
                throw new InvalidOperationException(">>GetRandomPointNearMe againLoop is not Working");
            }
            goto againLoop;
        }
    }
}
