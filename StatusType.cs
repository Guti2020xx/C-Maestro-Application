using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Database_Control.StatusType;

namespace Database_Control
{
    public class StatusType
    {
        //Private variables to store different parts of class
        private int Stat;
        private int ID;
        private string Name;
        private string Position;
        private string Pass;

        //Constructor to initialize object
        public StatusType(int Value, int ID, string Name, string Position, string Password)
        {
            Stat = Value;
            this.ID = ID;
            this.Name = Name;
            this.Position = Position;
            Pass = Password;
        }

        public void SetPosition(string Pos)
        {
            Position = Pos;
        }

        //Getter method to get password
        public string GetPass()
        {
            return Pass;
        }
        //Getter method to get position
        public string GetPosition()
        {
            return Position;
        }
        //Getter method to get name
        public string GetName()
        {
            return Name;
        }
        //Getter method to get Status number
        public int GetStatNumber()
        {
            return Stat;
        }
        //Getter method to get ID
        public int GetIDNumber()
        {
            return ID;
        }
        //Method to get specific status 
        public bool GetStat(int Index)
        {
            return CheckStatus(Stat, Index);
        }
        //Method to check specific status
        public static bool CheckStatus(int Stat, int Index)
        {
            return (Stat & (1 << Index)) != 0;
        }
        //Method to update bit/value
        public static int UpdateValue(int Current, int NewBit, int pos)
        {
            int clearBit = ~(1 << pos);
            int mask = Current & clearBit;
            return mask | (NewBit << pos);
        }
        //Method to generate a random string with a specified length
        public static string RandomString(int Length)
        {
            string Out = "";
            Random Rng = new Random();
            for (int i = 1; i < Length; i++)
            {
                Out += (char)Rng.Next((int)'!', (int)'~');
            }
            return Out;
        }
        //Dictionary to store permissions for different positions
        private static Dictionary<string, Action[]> Defaults = new Dictionary<string, Action[]>()
        {
            {
                //Admin Permissions
                "Admin",
                new Action[]
                {
                    Action.CanSeeDelivey,
                    Action.CanSeeProduct,
                    Action.CanSeeEmployee,
                    Action.CanSeeCompany,
                    Action.CanUpdateDelivery,
                    Action.CanCreateDelivery,
                    Action.CanDeleteDelivery,
                    Action.CanUpdateProduct,
                    Action.CanCreateProduct,
                    Action.CanDeleteProduct,
                    Action.CanUpdateEmployee,
                    Action.CanCreateEmployee,
                    Action.CanDeleteEmployee,
                    Action.CanUpdateCompany,
                    Action.CanCreateCompany,
                    Action.CanDeleteCompany
                }
            },
            {
                //Grunt permissions
                "Grunt",
                new Action[]
                {
                    Action.CanSeeDelivey,
                    Action.CanSeeProduct,
                    Action.CanSeeCompany
                }
            },
            {
                //Overseer permisssions
                "Overseer",
                new Action[]
                {
                    Action.CanSeeDelivey,
                    Action.CanSeeProduct,
                    Action.CanSeeEmployee,
                    Action.CanCreateDelivery,
                    Action.CanUpdateDelivery
                }
            },
            {
                //Inventory permissions
                "Inventory",
                new Action[]
                {
                    Action.CanSeeProduct,
                    Action.CanSeeCompany,
                    Action.CanUpdateProduct,
                    Action.CanCreateProduct
                }
            }
        };

        //Getter method to get available presets
        public string[] GetPresets()
        {
            return Defaults.Keys.ToArray();
        }

        public Action[] GetPerms(string Position)
        {
            if (Defaults.ContainsKey(Position))
                return Defaults[Position];
            return new Action[0];
        }

        //Method to find match based on parameters
        public string FindMatch(List<Action> ActionItems)
        {
            int Match = 0; //Intialize 

            //Iterate through each item in default collection
            foreach (var item in Defaults)
            {
                //Check to see if Value length is of current item is equal to the action items
                if (item.Value.Length == ActionItems.Count)
                {
                    Match = 0;
                    //Iterates through each perm in value array
                    foreach (var Perm in item.Value)
                    {
                        if (ActionItems.Contains(Perm))
                        {
                            Match++; //If item contains perm increment match count
                        }
                    }
                    //Check to see if match count is equal to value length array, if so return the key of the item
                    if (Match == item.Value.Length)
                    {
                        return item.Key;
                    }
                }
            }
            return CreateStat(ActionItems.ToArray()).ToString();
        }

        public enum Action { CanSeeDelivey, CanSeeProduct, CanSeeEmployee, CanSeeCompany, CanUpdateDelivery, CanCreateDelivery, CanDeleteDelivery, CanUpdateProduct, CanCreateProduct, CanDeleteProduct, CanUpdateEmployee, CanCreateEmployee, CanDeleteEmployee, CanUpdateCompany, CanCreateCompany, CanDeleteCompany }

        //Check to see if object has specific ability
        public bool HasAbility(Action action)
        {
            return CheckStatus(Stat, (int)action);
        }

        //Method to create new form from a preset position or a list of actions
        public static int CreateFrom(string Position)
        {
            if (int.TryParse(Position, out int Result))
            {
                return Result;
            }
            else
            {
                if (Defaults.ContainsKey(Position))
                {
                    return CreateStat(Defaults[Position]);
                }
                return CreateStat(Defaults["Grunt"]);
            }
        }

        //Method to create a new stauts for incoming employees from a list of actions
        private static int CreateStat(params Action[] actions)
        {
            int StatOut = 0;
            foreach (var item in actions)
            {
                StatOut = UpdateValue(StatOut, 1, (int)item);
            }
            return StatOut;
        }

        //Gets all names of permissions the user has access to
        public List<string> PrintStats()
        {
            List<string> stats = new List<string>();

            foreach (Action item in Enum.GetValues(typeof(Action)))
            {
                if (HasAbility(item))
                    stats.Add(item.ToString());
            }

            return stats;
        }

        public static List<string> PrintStats(int Number)
        {
            List<string> stats = new List<string>();

            foreach (Action item in Enum.GetValues(typeof(Action)))
            {
                if (CheckStatus(Number, (int)item))
                    stats.Add(item.ToString());
            }

            return stats;
        }
    }
}
