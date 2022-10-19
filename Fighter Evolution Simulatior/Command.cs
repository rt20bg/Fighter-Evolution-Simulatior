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
    public static class Command
    {
        public static Random Rnd; //The game only Random instance
        public static int ResolutionWidth; //Maybe we shoud migrate to resizable resolution ..
        public static int ResolutionHeight;
        public static int MobID; //Used for assigning new mobIDs usualy with ID = ++Command.MobID

        public static int GetMobID() => ++MobID;

        /// <summary>
        /// Check if a coordinates and an item Height and Width are inside the original resolution.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="h"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public static bool InsideResolution(Vector2 pos, int h, int w)
        {
            if (pos.X <= w || pos.X >= ResolutionWidth - w) return false;
            if (pos.Y <= h || pos.Y >= ResolutionHeight - h) return false;
            return true;
        }

        /// <summary>
        /// Given two set of Coordinates if one of them touches or is inside the other
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool CubesTouching(Rectangle a, Rectangle b)
        {
            if (a.Height<=0 || a.Width<=0 || b.Height<=0 || b.Height <= 0)
            { 
                throw new Exception("Cube with zero area");
            }
            if (PointInsdieRectangle(a, new Vector2(b.X,b.Y))) return true;
            if (PointInsdieRectangle(a, new Vector2(b.X + b.Width, b.Y))) return true;
            if (PointInsdieRectangle(a, new Vector2(b.X, b.Y + b.Height))) return true;
            if (PointInsdieRectangle(a, new Vector2(b.X + b.Width, b.Y + b.Height) )) return true;
            return false;
        }
        
        /// <summary>
        /// Checking if a given coordinates are inside a Rectangle
        /// </summary>
        /// <param name="a"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool PointInsdieRectangle(Rectangle a, Vector2 point)
        {
            if (!(point.X <= a.X + a.Width && point.X > a.X))  return false;
            if (!(point.Y >= a.Y && point.Y <= a.Y + a.Height)) return false;
            return true;
        }

        /// <summary>
        /// The Cube eats mobs and gives back the number of mobs eaten
        /// </summary>
        /// <param name="Tochkovci"></param>
        /// <param name="Cubes"></param>
        /// <param name="counter"></param>
        /// <returns></returns>
        public static int CubeEat(List<Fighter> Tochkovci, List<Mob> Cubes, int counter)
        {
            //Let me remind here that all Lists are send by reference, meaning 
            //that any change on the List here will affect the original list
            //Aloso that .ToArray() is so I can use the List Remove() method. 
            foreach (var item in Tochkovci.ToArray())
            {
                foreach (var cube in Cubes)
                {
                    if (Command.CubesTouching(cube.Rectanle, item.Rectanle))
                    {
                        Tochkovci.Remove(item);
                        counter++;
                        cube.moving = false;
                        cube.moveRandomly();
                    }
                }

            }
            return counter;
        }

        /// <summary>
        /// Fighters try to fight each-other if their are close enough. Also they heal a bit before that.
        /// </summary>
        /// <param name="fighters"></param>
        /// <returns></returns>
        public static List<Fighter> FightersFight(List<Fighter> fighters)
        {            
            List <Fighter> clones= new List<Fighter>(); //Holds the newly born clones
            foreach (var item in fighters) item.Heal(); //Healing all of the fighters a bit.

            //Using two nested for loops becsue they can be used
            //not only for looping thorugh the list of fighters, 
            //but for removing items form the list            
            for (int i = fighters.Count - 1; i >= 0; i--)
            {
                for (int j = fighters.Count - 1; j >= 0; j--)
                {
                    if (i >= fighters.Count - 1) i = fighters.Count - 1;
                    //If the fighter isn't fighting iteslf
                    if (fighters[i].ID != fighters[j].ID)
                    {
                        //We check if the two fighters are close enough to fight 
                        if (CubesTouching(fighters[i].Rectanle, fighters[j].Rectanle))
                        {                            
                            Battle(fighters[i], fighters[j]); //Then start a Fight
                            if (fighters[i].HP > 0 && fighters[j].HP < 0)
                            {
                                fighters[i].Win();
                                if (Rnd.Next(0, 101) < 50)//50% chance for the winner to clone himself
                                {
                                    clones.Add(fighters[i].GetClone());
                                    fighters[i].children++;
                                }
                                if (Rnd.Next(0, 101) < 15)//15% chance for the loser to revive as a clone 
                                {
                                    clones.Add(fighters[j].GetClone());
                                }
                                fighters.Remove(fighters[j]);
                                if (i > 1 && j > 1)
                                {
                                    i--; j--;
                                }
                            }
                            else if (fighters[j].HP > 0 && fighters[i].HP < 0)
                            {
                                fighters[j].Win();
                                if (Rnd.Next(0, 101) < 50)
                                {
                                    clones.Add(fighters[j].GetClone());
                                    fighters[j].children++;
                                }
                                if (Rnd.Next(0, 101) < 15)
                                {
                                       clones.Add(fighters[i].GetClone());
                                }
                                fighters.Remove(fighters[i]);
                                if (i > 1 && j > 1)
                                {
                                    i--; j--;
                                }
                            }                            
                        }
                    }
                }
            }                         
            return clones;
        }

        /// <summary>
        /// Fight between two Fighter class Mobs It could return a for the battle.
        /// The battle is well documented and returned in the form of a string.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string BattleWithLog(Fighter a, Fighter b)
        {
            string log = "";
            int k = 0;

            while (k <= 500) //The fighter will fight for up to 500 rounds
            {
                var aHit = a.Hit(); var bHit = b.Hit(); //Lets save both attacks

                if (aHit >= bHit) //The one with the better HIT() will go first
                {
                    //f1 hits first
                    {
                        var _HP = b.HP; //Saving the current hp for some calculations
                        log += "Fighter 1 hits for " + aHit + Environment.NewLine;
                        b.TakeDmg(aHit); //Calculating the various damage reductions
                        log += "Fighter 2 takes " + (_HP - b.HP) + " damage" + Environment.NewLine;
                        if (b.HP <= 0)
                        {
                            log += "Fighter 1 Wins!!" + Environment.NewLine;
                            log += "The battle lasted for " + k + " rounds" + Environment.NewLine;
                            return log;
                        }
                    }

                    //f2 hits second
                    {
                        var _HP = a.HP; //Saving the current hp for some calculations
                        log += "Fighter 2 hits for " + bHit + Environment.NewLine;
                        a.TakeDmg(bHit); // Calculating the various damga reductions
                        log += "Fighter 1 takes " + (_HP - a.HP) + " damage" + Environment.NewLine;
                        if (a.HP <= 0)
                        {
                            log += "Fighter 2 Wins!!" + Environment.NewLine;
                            log += "The battle lasted for " + k + " rounds" + Environment.NewLine;
                            return log;
                        }
                    }
                }
                else
                {
                    //f2 hits first
                    {
                        var _HP = a.HP; //Saving the current hp for some calculations
                        log += "Fighter 2 hits for " + bHit + Environment.NewLine;
                        a.TakeDmg(bHit); // Calculating the various damga reductions
                        log += "Fighter 1 takes " + (_HP - a.HP) + " damage" + Environment.NewLine;
                        if (a.HP <= 0)
                        {
                            log += "Fighter 2 Wins!!" + Environment.NewLine;
                            log += "The battle lasted for " + k + " rounds" + Environment.NewLine;
                            return log;
                        }
                    }

                    //f1 hits second
                    {
                        var _HP = b.HP; //Saving the current hp for some calculations
                        log += "Fighter 1 hits for " + aHit + Environment.NewLine;
                        b.TakeDmg(aHit); //Calculating the various damga reductions
                        log += "Fighter 2 takes " + (_HP - b.HP) + " damage" + Environment.NewLine;
                        if (b.HP <= 0)
                        {
                            log += "Fighter 1 Wins!!" + Environment.NewLine;
                            log += "The battle lasted for " + k + " rounds" + Environment.NewLine;
                            return log;
                        }
                    }
                }
                k++;
            }
            log += "After 500 rounds, the battle ended in a Draw!";
            return log;
        }

        /// <summary>
        /// Fight between two Fighter class Mobs It could return a for the battle.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static void Battle( Fighter a,  Fighter b)
        {
            int k = 0;

            while (k <= 500) //The fighter will fight for up to 500 rounds
            {
                var aHit = a.Hit(); var bHit = b.Hit(); //Lets save both attacks

                if (aHit >= bHit) //The one with the better HIT() will go first
                {
                    //f1 hits first
                    {
                        var _HP = b.HP; //Saving the current hp for some calculations                                     
                        b.TakeDmg(aHit); //Calculating the various damage reductions                                       
                        if (b.HP <= 0) return; //If B dies, the battle is over
                    }
                    //f2 hits second
                    {
                        var _HP = a.HP; //Saving the current hp for some calculations                                        
                        a.TakeDmg(bHit); // Calculating the various damga reductions                                         
                        if (a.HP <= 0) return;//If A dies, the battle is over
                    }
                }
                else
                {
                    //f2 hits first
                    {
                        var _HP = a.HP; //Saving the current hp for some calculations                        
                        a.TakeDmg(bHit); // Calculating the various damga reductions                        
                        if (a.HP <= 0) return; //If A dies, the battle is over
                    }
                    //f1 hits second
                    {
                        var _HP = b.HP; //Saving the current hp for some calculations                        
                        b.TakeDmg(aHit); //Calculating the various damga reductions                        
                        if (b.HP <= 0) return; //If B dies, the battle is over
                    }
                }
                k++;
            }
            return ;
        }

    }
}
