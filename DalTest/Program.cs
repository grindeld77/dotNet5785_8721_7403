using Dal;
using DalApi;
using DO;
using System;
namespace DalTest
{

    internal class Program
    {
        private static IAssignment? s_dalAssignment = new AssignmentImplementation(); //stage 1
        private static ICall? s_dalCall = new CallImplementation(); //stage 1
        private static IConfig? s_Config = new ConfigImplementation(); //stage 1
        private static IVolunteer? s_Volunteer = new VolunteerImplementation(); //stage 1
        public static class YourChoice
        {

            public static int YourChoiceIs()
            {
                if (!int.TryParse(Console.ReadLine(), out int choice)) throw new FormatException
            ("Your selection is incorrect, please try again and select a number from the menu");
                return choice;
            }
        }
        public static class menu
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
            public static void AssignmentMenu()
            {
                int choice = menu.SecondaryMenu("Assignment");
                switch (choice)
                {
                    case 1:
                        Console.WriteLine("Enter the call ID");
                        int callId = YourChoice.YourChoiceIs();
                        Console.WriteLine("Enter the volunteer ID");
                        int volunteerId = YourChoice.YourChoiceIs();
                        s_dalAssignment.Create(new Assignment() { CallId = callId, VolunteerId = volunteerId });
                        break;
                    case 2:
                        Console.WriteLine("Enter the assignment ID");
                        int assignmentId = YourChoice.YourChoiceIs();
                        Assignment assignment = s_dalAssignment.Read(assignmentId);
                        if (assignment != null)
                        {
                            Console.WriteLine(assignment);
                        }
                        else
                        {
                            Console.WriteLine("Assignment not found");
                        }
                        break;
                    case 3:
                        List<Assignment> assignments = s_dalAssignment.ReadAll();
                        foreach (Assignment assignment in assignments)
                        {
                            Console.WriteLine(assignment);
                        }
                        break;
                    case 4:
                        Console.WriteLine("Enter the assignment ID");
                        int assignmentId = YourChoice.YourChoiceIs();
                        Assignment assignment = s_dalAssignment.Read(assignmentId);
                        if (assignment != null)
                        {
                            Console.WriteLine("Enter the call ID");
                            int callId = YourChoice.YourChoiceIs();
                            Console.WriteLine("Enter the volunteer ID");
                            int volunteerId = YourChoice.YourChoiceIs();
                            s_dalAssignment.Update(new Assignment() { Id = assignmentId, CallId = callId, VolunteerId = volunteerId });
                        }
                        else
                        {
                            Console.WriteLine("Assignment not found");
                        }
                        break;
                    case 5:
                        Console.WriteLine("Enter the assignment ID");
                        int assignmentId = YourChoice.YourChoiceIs();
                        s_dalAssignment.Delete(assignmentId);
                        break;
                    case 6:
                        s_dalAssignment.DeleteAll();
                        break;
                    default:
                        Console.WriteLine("Invalid selection, please try again.");
                        break;
                }
            }
            public static void CallMenu()
            {
                int choice = menu.SecondaryMenu("Call");
                switch (choice)
                {
                    case 1:
                        Console.WriteLine("Enter the call ID");
                        int callId = YourChoice.YourChoiceIs();
                        Console.WriteLine("Enter the call time");
                        DateTime callTime = DateTime.Parse(Console.ReadLine());
                        Console.WriteLine("Enter the call duration");
                        int callDuration = YourChoice.YourChoiceIs();
                        Console.WriteLine("Enter the call number");
                        string callNumber = Console.ReadLine();
                        s_dalCall.Create(new Call() { Id = callId, Time = callTime, Duration = callDuration, Number = callNumber });
                        break;
                    case 2:
                        Console.WriteLine("Enter the call ID");
                        int callId = YourChoice.YourChoiceIs();
                        Call call = s_dalCall.Read(callId);
                        if (call != null)
                        {
                            Console.WriteLine(call);
                        }
                        else
                        {
                            Console.WriteLine("Call not found");
                        }
                        break;
                    case 3:
                        List<Call> calls = s_dalCall.ReadAll();
                        foreach (Call call in calls)
                        {
                            Console.WriteLine(call);
                        }
                        break;
                    case 4:
                        Console.WriteLine("Enter the call ID");
                        int callId = YourChoice.YourChoiceIs();
                        Call call = s_dalCall.Read(callId);
                        if (call != null)
                        {
                            Console.WriteLine("Enter the call time");
                            DateTime callTime = DateTime.Parse(Console.ReadLine());
                            Console.WriteLine("Enter the call duration");
                            int callDuration = YourChoice.YourChoiceIs();
                            Console.WriteLine("Enter the call number");
                            string callNumber = Console.ReadLine();
                            s_dalCall.Update(new Call() { Id = callId, Time = callTime, Duration = callDuration, Number = callNumber });
                        }
                        else
                        {
                            Console.WriteLine("Call not found");
                        }
                        break;
                    case 5:
                        Console.WriteLine("Enter the call ID");
                        int callId = YourChoice.YourChoiceIs();
                        s_dalCall.Delete(callId);
                        break;
                    case 6:
                        s_dalCall.DeleteAll();
                        break;
                    default:
                        Console.WriteLine("Invalid selection, please try again.");
                        break;
                }

            }
            public static void VolunteerMenu()
            {
                int choice = menu.SecondaryMenu("Volunteer");
                switch (choice)
                {
                    case 1:
                        Console.WriteLine("Enter the call ID");
                        int callId = YourChoice.YourChoiceIs();
                        Console.WriteLine("Enter the call time");
                        DateTime callTime = DateTime.Parse(Console.ReadLine());
                        Console.WriteLine("Enter the call duration");
                        int callDuration = YourChoice.YourChoiceIs();
                        Console.WriteLine("Enter the call number");
                        string callNumber = Console.ReadLine();
                        s_dalCall.Create(new Call() { Id = callId, Time = callTime, Duration = callDuration, Number = callNumber });
                        break;
                    case 2:
                        Console.WriteLine("Enter the call ID");
                        int callId = YourChoice.YourChoiceIs();
                        Call call = s_dalCall.Read(callId);
                        if (call != null)
                        {
                            Console.WriteLine(call);
                        }
                        else
                        {
                            Console.WriteLine("Call not found");
                        }
                        break;
                    case 3:
                        List<Call> calls = s_dalCall.ReadAll();
                        foreach (Call call in calls)
                        {
                            Console.WriteLine(call);
                        }
                        break;
                    case 4:
                        Console.WriteLine("Enter the call ID");
                        int callId = YourChoice.YourChoiceIs();
                        Call call = s_dalCall.Read(callId);
                        if (call != null)
                        {
                            Console.WriteLine("Enter the call time");
                            DateTime callTime = DateTime.Parse(Console.ReadLine());
                            Console.WriteLine("Enter the call duration");
                            int callDuration = YourChoice.YourChoiceIs();
                            Console.WriteLine("Enter the call number");
                            string callNumber = Console.ReadLine();
                            s_dalCall.Update(new Call() { Id = callId, Time = callTime, Duration = callDuration, Number = callNumber });
                        }
                        else
                        {
                            Console.WriteLine("Call not found");
                        }
                        break;
                    case 5:
                        Console.WriteLine("Enter the call ID");
                        int callId = YourChoice.YourChoiceIs();
                        s_dalCall.Delete(callId);
                        break;
                    case 6:
                        s_dalCall.DeleteAll();
                        break;
                    default:
                        Console.WriteLine("Invalid selection, please try again.");
                        break;
                }
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

        static void Main(string[] args)
        {
            try
            {
                int choice = menu.mainMenu();
                while (choice != 0)
                {
                    switch (choice)
                    {
                        case 1:
                            menu.AssignmentMenu();
                            break;
                        case 2:
                            menu.CallMenu();
                            break;
                        case 3:
                            menu.VolunteerMenu();
                            break;
                        case 4:
                            s_Config.Init();
                            break;
                        case 5:
                            s_Config.Display();
                            break;
                        case 6:
                            int choice6 = menu.configMenu();
                            break;
                        case 7:
                            s_Config.Reset();
                            break;
                        default:
                            Console.WriteLine("Invalid selection, please try again.");
                            break;
                    }
                    choice = menu.mainMenu();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

