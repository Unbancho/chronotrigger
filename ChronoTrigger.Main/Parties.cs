/*
namespace ChronoTrigger.Parties
{

    public class PartyMember : Character, IBattler
    {
        public int Experience {get; set;}
        public int Level {get; private set;} = 1;
    }

    public class Ayla : PartyMember, IRecruitable
    {
        // Because Ayla is her own fucking badass gender.
        public override int Attack {
            get{
                return (int) ((POW * 1.75) + (Math.Pow(Level, 2) / 45.5));
            }
        }

        public override int MaxRandomDamageBonus{
            get{
                return Level/3;
            }
        }

        SortedDictionary<int, int> HPGains = new SortedDictionary<int, int>(){
            {10, 13},
            {21, 15},
            {48, 21},
            {99, 10}
        };

        public override int LevelUp(int levels=1)
        {
            int newLevel = base.LevelUp(levels);
            foreach (var range in HPGains.Keys)
            {
                if(newLevel < range)
                {
                    HP += HPGains[range];
                    break;
                }
            }
            if(newLevel < 35)
            {
                MP += 2;
            }
            else
            {
                MP += 1;
            }
            if(newLevel % 2 == 0)
            {
                POW += 1;
            }
            else
            {
                POW += 2;
            }
            return newLevel;
        }

        public Ayla():base("Ayla")
        {
            Gender = Gender.Female;
            HP = 80;
            MP = 4;
            POW = 10;
            MAG = 3;
            MDEF = 1;
            SPD = 13;
        }
        
    }

    public class Crono : PartyMember, IRecruitable
    {
        SortedDictionary<int, int> HPGains = new SortedDictionary<int, int>(){
            {10, 13},
            {21, 15},
            {48, 21},
            {99, 10}
        };

        SortedDictionary<int, int> MPGains = new SortedDictionary<int, int>(){
            {34, 2},
            {99, 1}
        };

        public Crono():base("Crono")
        {
            Gender = Gender.Male;
            HP = 70;
            MP = 8;
            POW = 5;
            //Hit
            MAG = 5;
            //Evade
            //Stamina
            MDEF = 2;
            SPD = 12;
        }
    }

    public class Frog : PartyMember, IRecruitable
    {
        public Frog():base("Frog")
        {
            Gender = Gender.Male;
            HP = 80;
            MP = 9;
            POW = 4;
            //Hit
            MAG = 6;
            //Evade
            //Stamina
            MDEF = 3;
            SPD = 11;
        }
    }

    public class Robo : PartyMember
    {
        public Robo():base("Robo")
        {
            Gender = Gender.Male;
            HP = 130;
            MP = 6;
            POW = 7;
            //Hit
            MAG = 3;
            //Evade
            //Stamina
            MDEF = 1;
            SPD = 6;
        }
    }

    public class Lucca : PartyMember
    {
        public Lucca():base("Lucca")
        {
            Gender = Gender.Female;
            HP = 62;
            MP = 12;
            POW = 2;
            //Hit
            MAG = 8;
            //Evade
            //Stamina
            MDEF = 7;
            SPD = 6;
        }
    }

    public class Magus : PartyMember
    {
        public Magus():base("Magus")
        {
            Gender = Gender.Male;
            HP = 110;
            MP = 14;
            POW = 8;
            //Hit
            MAG = 10;
            //Evade
            //Stamina
            MDEF = 9;
            SPD = 12;
        }
    }

    public class Marle : PartyMember
    {
        public Marle():base("Marle")
        {
            Gender = Gender.Female;
            HP = 65;
            MP = 12;
            POW = 2;
            //Hit
            MAG = 8;
            //Evade
            //Stamina
            MDEF = 8;
            SPD = 8;
        }
    }

}
*/

