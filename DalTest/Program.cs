using Dal;
using DalApi;
using System;
namespace DalTest
{

    public static class YourChoice
    {
       
        public static int YourChoiceIs()
        {
            if (!int.TryParse(Console.ReadLine(), out int choice)) throw new FormatException
        ("Your selection is incorrect, please try again and select a number from the menu");
            return choice;
        }
    }
    public static class maneu
    {
        public static int mainMenu()
        {
            Console.WriteLine(@"
       Select an option to proceed
0 Exit main menu.
1 Display submenu for Assignment entity. 
2 Display submenu for Call entity.
3 Display submenu for Volunteer entity. 
4 Initialize data. 
5 Display all data in the database.
6 Display configuration submenu. 
7 Reset database and configuration data.");

            return YourChoice.YourChoiceIs();
        }

       public static int SecondaryMenu(string type)
        {
            Console.WriteLine($@"
            Select an option to proceed
0 Exit sub-menu.
1 Add a new object of the entity {type} to the list.
2 Display an object by its ID.
3 Display a list of all objects of the entity type.
4 Update existing object data.
5 Delete an existing object from the list.
6 Delete all objects in the list.
");

            return YourChoice.YourChoiceIs();
        }

        public class MakingTheChoice
        {

            public static void RestartingTheSystem()
            {
                Initialization.Do(s_dalAssignment, s_dalCall, s_Config, IVolunteer);
            }





            static void assignmentMenu()
            {

                
                int choice = YourChoice.YourChoiceIs();
            }
            static void callMenu()
            {

                int choice = YourChoice.YourChoiceIs();
            }
            static void volunteerMenu()
            {

                int choice = YourChoice.YourChoiceIs();
            }
            static int configMenu()
            {
                Console.WriteLine(@"");
                return YourChoice.YourChoiceIs();
            }
        }

    }
    internal class Program
    {
        private static IAssignment? s_dalAssignment = new AssignmentImplementation(); //stage 1
        private static ICall? s_dalCall = new CallImplementation(); //stage 1
        private static IConfig? s_Config = new ConfigImplementation(); //stage 1
        private static IVolunteer? s_Volunteer = new VolunteerImplementation(); //stage 1

        static void Main(string[] args)
        {
            try
            {
                int choice = maneu.mainMenu();
                while (choice != 0)
                {
                    switch (choice)
                    {
                        case 1:
                            int choice1 = maneu.SecondaryMenu("Assignment");
                            break;
                        case 2:
                            int choice2 = maneu.SecondaryMenu("Call");
                            break;
                        case 3:
                            int choice3 = maneu.SecondaryMenu("Volunteer");
                            break;
                        case 4:
                            s_Config.Init();
                            break;
                        case 5:
                            s_Config.Display();
                            break;
                        case 6:
                            int choice6 = maneu.configMenu();
                            break;
                        case 7:
                            s_Config.Reset();
                            break;
                        default:
                            Console.WriteLine("Invalid selection, please try again.");
                            break;
                    }
                    choice = maneu.mainMenu();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

