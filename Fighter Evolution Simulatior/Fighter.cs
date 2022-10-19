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

    public class Fighter : Mob
    {
        /// <summary>
        /// The Fighter will be born with a random value of attributes.
        /// Each time it touches some other fighter, they will fight for some number of rounds.
        /// If the Fighter Mob wins he will have 70% chance to reproduce 
        /// by spawning one additional fighter identical to his parent.
        /// and also gets an upgrade by 1 randomly selected point
        /// 
        /// If the Fighter dies it has 30% chance to re-spawn
        /// if the Fighter Draws in his battle each of them have 15% chance to spawn a copy of them
        /// 
        /// </summary>
        public int minDmg; //For a radom hit calcuation in the range (minDmg, MaxDmg+1)
        public int maxDmg; //
        public int armor; //1 point of armor reduces one point of total damage taken
        public int HP; //TODO indroduce a MAX HP property instead of calculating it runtime.. saves calcualtions
        public int vitality; //1 point of vitality gives ... 10 Hp
        public int critChance; //1 point = 1%
        public int critDamage; //1 point = 10%
        public int defense; //1 point of defense reduces 1% of damge taken
        public int attack; //1 point of attack adds 1% to the total damage
        public int healing; //the amount afadgf of health the Figher regenrates every game move. Cannot be more then vitality*10

        public int kills = 0; //The nubmer of killed fighter
        public int children = 0; //The number of children spowned

        int ubgradePoints; //Ubgrades the Fighter

        public Fighter(Texture2D texture) : base (texture)
        {
            TextureSet(texture); //Setting the texture
            InitializeFighter(); //Initializing the fighter
            RandomizeMob(); //Radomizing position, color, rotation and spin
            RadomizeFighter();  //Randomizing the fighting properties
            this.ID = Command.GetMobID(); //Setting Fighter ID
        }

        /// <summary>
        /// Initializes the Fighter stats with their default values
        /// </summary>
        void InitializeFighter()
        {
            minDmg = 5;
            maxDmg = 10;
            armor = 1;
            vitality = 10;
            HP = vitality * 10;
            critChance = 5;
            critDamage = 100;
            defense = 5;
            attack = 5;
            healing = 1;
            ubgradePoints = 30;
        }

        /// <summary>
        /// Randomizes the fighter, using ubgradePoints.
        /// </summary>
        void RadomizeFighter()
        {
            ubgradePoints += Command.Rnd.Next(-2, 3); //adding (-2 to +2) ubgradePoints
            while (ubgradePoints > 0)
            {
                minDmg += Chanse();
                if (minDmg == maxDmg) maxDmg++; if (ubgradePoints == 0) break;
                maxDmg += Chanse(); if (ubgradePoints == 0) break;
                armor += Chanse(); if (ubgradePoints == 0) break;
                vitality += Chanse(); if (ubgradePoints == 0) break;
                HP = vitality * 10;
                critChance += Chanse(); if (ubgradePoints == 0) break;
                critDamage += (Chanse() * 10); if (ubgradePoints == 0) break;
                defense += Chanse() * 5; if (ubgradePoints == 0) break;
                attack += Chanse() * 5; if (ubgradePoints == 0) break;
            }
            //There is 5% chanse this function will return +1. If that happes ubgradePoints-- 
            int Chanse()
            {
                int ubgrade = 0;
                if ((Command.Rnd.Next(0, 100) + 1) <= 5) //5% chanse;
                {
                    ubgrade++; ubgradePoints--;
                }
                return ubgrade;
            }
        }

        /// <summary>
        /// The total damage the Fighter does with a single hit 
        /// </summary>
        /// <returns></returns>
        public double Hit()
        {
            double dmg = Command.Rnd.Next(minDmg, maxDmg) + 1;
            //Console.WriteLine(minDmg + " " + maxDmg);
            //Console.WriteLine("dmg: " + dmg);
            dmg = dmg * ((100 + attack) / 100f);
            //Console.WriteLine("dmg + " + attack + " attack = "+dmg );

            if ((Command.Rnd.Next(0, 100) + 1) < critChance)
            {
                dmg = dmg * ((100 + critDamage) / 100f);
                //Console.WriteLine("Crit successful Crit Damage = " + critDamage + " = "+ dmg);
            }
            return dmg;
        }

        /// <summary>
        /// The fighter is using all its defenses. Currently no less then 1 true damage per hit.
        /// </summary>
        /// <param name="dmg"></param>
        public void TakeDmg(double dmg)
        {
            //Console.WriteLine("Damage " + dmg + " Defense " +defense);
            dmg = dmg * ((100 - defense) / 100f);
            //Console.WriteLine("New dmg "+dmg);
            dmg = dmg - armor;
            if (dmg <= 0) dmg = 1;
            HP = HP - (int)dmg;
        }

        /// <summary>
        /// Healing the Fighter
        /// </summary>
        public void Heal()
        {
            HP += healing;
            if (vitality * 10 < HP) HP = vitality * 10;
        }

        /// <summary>
        /// Returns a new fighter with the same fighting attributes, 
        /// but different location, color, spin, rotation.
        /// </summary>
        /// <returns></returns>
        public Fighter GetClone()
        {
            var f = new Fighter(this.texture);
            TextureSet(texture);
            f.ID = Command.GetMobID();
            f.HP = this.vitality * 10;
            f.minDmg = this.minDmg;
            f.maxDmg = this.maxDmg;
            f.armor = this.armor;
            f.vitality = this.vitality;
            f.critChance = this.critChance;
            f.critDamage = this.critDamage;
            f.defense = this.defense;
            f.attack = this.attack;
            f.healing = this.healing; 
            return f; 
        }

        /// <summary>
        /// Gives new upgrade points to the Fighter and forces it to use them.
        /// </summary>
        public void Win()
        {
            this.ubgradePoints = 3;
            RadomizeFighter();
            kills++;

        }

        public string PrintItself()
        {
            string r = "";
           // if (name != "") r += "Name: " + name+Environment.NewLine;
           // r += "ID: " + ID + Environment.NewLine;
            r += "minDmg: " + minDmg + Environment.NewLine;
            r += "maxDmg: " + maxDmg + Environment.NewLine;
            r += "armor: " + armor + Environment.NewLine;
            r += "HP: " + HP + Environment.NewLine;
            r += "vitality: " + vitality + Environment.NewLine;
            r += "critChance: " + critChance + Environment.NewLine;
            r += "critDamage: " + critDamage + Environment.NewLine;
            r += "defense: " + defense + Environment.NewLine;
            r += "attack: " + attack + Environment.NewLine;
            r += "kills: " + kills + Environment.NewLine;

            return r;
        }






    }
}
