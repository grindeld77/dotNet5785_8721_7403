namespace DalTest;
using DalApi;
using DO;
using Dal;
using System.Data.SqlTypes;
internal class Program
{
    //static readonly IDal s_dal = new DalList(); //stage 2
    //static readonly IDal s_dal = new DalXml(); //stage 3
    static readonly IDal s_dal = Factory.Get; //stage 4

    enum Menu
    {
        Exit = 0,
        Volunteer,
        Call,
        Assignment,
        Initialize,
        DisplayAll,
        Configuration,
        Reset
    };
    enum volunteerMenu
    {
        Exit = 0,
        Add,
        Display,
        DisplayAll,
        Update,
        Delete,
        DeleteAll
    };
    enum callMenu
    {
        Exit = 0,
        Add,
        Display,
        DisplayAll,
        Update,
        Delete,
        DeleteAll
    };
    enum assignmentMenu
    {
        Exit = 0,
        Add,
        Display,
        DisplayAll,
        Update,
        Delete,
        DeleteAll
    };
    enum configMenu
    {
        Exit = 0,
        AdvanceMinute,
        AdvanceHour,
        AdvanceDay,
        AdvanceMonth,
        DisplayClock,
        SetClock,
        DisplayRiskTime,
        SetRiskTime,
        Reset
    };

    public static int SelectingFromTheMenu()
    {
        Console.WriteLine("Please make a selection:");
        if (!int.TryParse(Console.ReadLine(), out int choice))
        {
            Console.WriteLine("Invalid selection. Please enter a number corresponding to the menu options.");
            return SelectingFromTheMenu();
        }
        return choice;
    }
    public static int ConvertStringToNumber()
    {
        Console.WriteLine("Please enter a number:");
        if (!int.TryParse(Console.ReadLine(), out int choice))
        {
            Console.WriteLine("Invalid input. Please enter a valid number.");
            return ConvertStringToNumber();
        }
        return choice;
    }
    public static int mainMenu()
    {
        Console.WriteLine(
@"    Select an option to proceed
            0 Exit main menu.
            1 Display submenu for Volunteer entity. 
            2 Display submenu for Call entity.
            3 Display submenu for Assignment entity. 
            4 Initialize data. 
            5 Display all data in the database.
            6 Display configuration submenu. 
            7 Reset database and configuration data.");
        return ConvertStringToNumber();
    }
    public static int SecondaryMenu(string type)
    {
        Console.WriteLine(
$@"        Select an option to proceed
            0 Exit sub-menu.
            1 Add a new object of the entity {type} to the list.
            2 Display an object by its ID.
            3 Display a list of all objects of the entity type.
            4 Update existing object data.
            5 Delete an existing object from the list.
            6 Delete all objects in the list.");
        return ConvertStringToNumber();
    }
    public static void VolunteerMenu()
    {
        int choice = 0;
        volunteerMenu vm;
        do
        {
            choice = SecondaryMenu("Volunteer");
            vm = (volunteerMenu)choice;
            try
            {
                switch (vm)
                {
                    case volunteerMenu.Exit:
                        break;
                    case volunteerMenu.Add:
                        {
                            Console.WriteLine("Enter the volunteer ID");
                            int volunteerI = ConvertStringToNumber();
                            if (s_dal.Volunteer.Read(volunteerI) != null)
                                throw new Exception($"An object of type Volunteer with such ID={volunteerI} already exists");
                            Console.WriteLine("Enter the volunteer name");
                            string volunteerName = Console.ReadLine();
                            Console.WriteLine("Enter the volunteer phone number");
                            string volunteerPhone = Console.ReadLine();
                            Console.WriteLine("Enter the volunteer address");
                            string volunteerAddress = Console.ReadLine();
                            s_dal.Volunteer.Create(new Volunteer() { Id = volunteerI, FullName = volunteerName, MobilePhone = volunteerPhone, CurrentAddress = volunteerAddress });
                            break;
                        }
                    case volunteerMenu.Display:
                        {
                            Console.WriteLine("Enter the volunteer ID");
                            int volunteerI = ConvertStringToNumber();
                            Volunteer v = s_dal.Volunteer.Read(volunteerI);
                            if (v != null)
                            {
                                Console.WriteLine(v);
                            }
                            else
                            {
                                throw new Exception($"Volunteer with ID {volunteerI} not found");
                            }
                            break;
                        }
                    case volunteerMenu.DisplayAll:
                        {
                            IEnumerable<Volunteer> volunteers = s_dal.Volunteer.ReadAll();
                            foreach (Volunteer v in volunteers)
                            {
                                Console.WriteLine(v);
                            }
                            break;
                        }
                    case volunteerMenu.Update:
                        {
                            Console.WriteLine("Enter the volunteer ID");
                            int volunteerI = ConvertStringToNumber();
                            Volunteer volunteer = s_dal.Volunteer.Read(volunteerI);
                            if (volunteer != null)
                            {
                                Console.WriteLine("Enter the volunteer name");
                                string volunteerName = Console.ReadLine();
                                Console.WriteLine("Enter the volunteer phone number");
                                string volunteerPhone = Console.ReadLine();
                                Console.WriteLine("Enter the volunteer address");
                                string volunteerAddress = Console.ReadLine();
                                s_dal.Volunteer.Update(new Volunteer() { Id = volunteerI, FullName = volunteerName, MobilePhone = volunteerPhone, CurrentAddress = volunteerAddress });
                            }
                            else
                            {
                                throw new Exception($"Volunteer with ID {volunteerI} not found");
                            }
                            break;
                        }
                    case volunteerMenu.Delete:
                        {
                            Console.WriteLine("Enter the volunteer ID");
                            int volunteerI = ConvertStringToNumber();
                            s_dal.Volunteer.Delete(volunteerI);
                            break;
                        }
                    case volunteerMenu.DeleteAll:
                        s_dal.Volunteer.DeleteAll();
                        break;
                    default:
                        Console.WriteLine("Invalid selection, please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        } while (choice != 0);
    }
    public static void CallMenu()
    {
        int choice = 0;
        callMenu cm;
        do
        {
            choice = SecondaryMenu("Call");
            cm = (callMenu)choice;
            try
            {
                switch (cm)
                {
                    case callMenu.Exit:
                        break;
                    case callMenu.Add:
                        {
                            Console.WriteLine("Enter the call description");
                            string callDescription = Console.ReadLine();
                            Console.WriteLine("Enter the call address");
                            string callAddress = Console.ReadLine();
                            s_dal.Call.Create(new Call() { Description = callDescription, Address = callAddress });
                            break;
                        }
                    case callMenu.Display:
                        {
                            Console.WriteLine("Enter the Call ID");
                            int callI = ConvertStringToNumber();
                            Call c = (Call)s_dal.Call.Read(callI);
                            if (c != null)
                            {
                                Console.WriteLine(c);
                            }
                            else
                            {
                                throw new Exception($"Volunteer with ID {callI} not found");
                            }
                            break;
                        }
                    case callMenu.DisplayAll:
                        {
                            IEnumerable<Call>? calls = s_dal.Call.ReadAll();
                            foreach (Call c in calls)
                            {
                                Console.WriteLine(c);
                            }
                            break;
                        }
                    case callMenu.Update:
                        {
                            Console.WriteLine("Enter the Call ID");
                            int callI = ConvertStringToNumber();
                            Call call = (Call)s_dal.Call.Read(callI);
                            if (call != null)
                            {
                                Console.WriteLine("Enter the call description");
                                string callDescription = Console.ReadLine();
                                Console.WriteLine("Enter the call address");
                                string callAddress = Console.ReadLine();
                                s_dal.Call.Create(new Call() { Id = callI, Description = callDescription, Address = callAddress });
                            }
                            else
                            {
                                throw new Exception($"Volunteer with ID {call} not found");
                            }
                            break;
                        }
                    case callMenu.Delete:
                        {
                            Console.WriteLine("Enter the Call ID");
                            int callI = ConvertStringToNumber();
                            s_dal.Call.Delete(callI);
                            break;
                        }
                    case callMenu.DeleteAll:
                        s_dal.Call.DeleteAll();
                        break;
                    default:
                        Console.WriteLine("Invalid selection, please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        } while (choice != 0);
    }
    public static void AssignmentMenu()
    {
        int choice = 0;
        assignmentMenu am;
        do
        {
            choice = SecondaryMenu("assignment");
            am = (assignmentMenu)choice;
            try
            {
                switch (am)
                {
                    case assignmentMenu.Exit:
                        break;
                    case assignmentMenu.Add:
                        {
                            Console.WriteLine("Enter the call id");
                            int callId;
                            if (!int.TryParse(Console.ReadLine(), out callId))
                            {
                                Console.WriteLine("Invalid input. Please enter a valid number.");
                                break;
                            }
                            Console.WriteLine("Enter the volunteer id");
                            int volunteerId;
                            if (!int.TryParse(Console.ReadLine(), out volunteerId))
                            {
                                Console.WriteLine("Invalid input. Please enter a valid number.");
                                break;
                            }
                            s_dal.Assignment.Create(new Assignment() { CallId = callId, VolunteerId = volunteerId });
                            break;
                        }
                    case assignmentMenu.Display:
                        {
                            Console.WriteLine("Enter the assignment ID");
                            int assignmentId;
                            if (!int.TryParse(Console.ReadLine(), out assignmentId))
                            {
                                Console.WriteLine("Invalid input. Please enter a valid number.");
                                break;
                            }
                            Assignment a = s_dal.Assignment.Read(assignmentId);
                            if (a != null)
                            {
                                Console.WriteLine(a);
                            }
                            else
                            {
                                throw new Exception($"Assignment with ID {assignmentId} not found");
                            }
                            break;
                        }
                    case assignmentMenu.DisplayAll:
                        {
                            IEnumerable<Assignment> assignments = s_dal.Assignment.ReadAll();
                            foreach (Assignment a in assignments)
                            {
                                Console.WriteLine(a);
                            }
                            break;
                        }
                    case assignmentMenu.Update:
                        {
                            Console.WriteLine("Enter the assignment ID");
                            int assignmentId;
                            if (!int.TryParse(Console.ReadLine(), out assignmentId))
                            {
                                Console.WriteLine("Invalid input. Please enter a valid number.");
                                break;
                            }
                            Assignment assignment = s_dal.Assignment.Read(assignmentId);
                            if (assignment != null)
                            {
                                Console.WriteLine("Enter the call id");
                                int callId;
                                if (!int.TryParse(Console.ReadLine(), out callId))
                                {
                                    Console.WriteLine("Invalid input. Please enter a valid number.");
                                    break;
                                }
                                Console.WriteLine("Enter the volunteer id");
                                int volunteerId;
                                if (!int.TryParse(Console.ReadLine(), out volunteerId))
                                {
                                    Console.WriteLine("Invalid input. Please enter a valid number.");
                                    break;
                                }
                                s_dal.Assignment.Update(new Assignment() { Id = assignmentId, CallId = callId, VolunteerId = volunteerId });
                            }
                            else
                            {
                                throw new Exception($"Assignment with ID {assignmentId} not found");
                            }
                            break;
                        }
                    case assignmentMenu.Delete:
                        {
                            Console.WriteLine("Enter the assignment ID");
                            int assignmentId;
                            if (!int.TryParse(Console.ReadLine(), out assignmentId))
                            {
                                Console.WriteLine("Invalid input. Please enter a valid number.");
                                break;
                            }
                            s_dal.Assignment.Delete(assignmentId);
                            break;
                        }
                    case assignmentMenu.DeleteAll:
                        s_dal.Assignment.DeleteAll();
                        break;
                    default:
                        Console.WriteLine("Invalid selection, please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        } while (choice != 0);
    }
    public static void ConfigMenu()
    {
        int choice = 0;
        configMenu cm;
        do
        {
            choice = SecondaryMenu("config");
            cm = (configMenu)choice;
            try
            {
                switch (cm)
                {
                    case configMenu.Exit:
                        break;
                    case configMenu.AdvanceMinute:
                        {
                            Console.WriteLine("Enter the number of minutes to advance");
                            int minutes;
                            if (!int.TryParse(Console.ReadLine(), out minutes))
                            {
                                Console.WriteLine("Invalid input. Please enter a valid number.");
                                break;
                            }
                            s_dal.Config.Clock = s_dal.Config.Clock.AddMinutes(minutes);
                            break;
                        }
                    case configMenu.AdvanceHour:
                        {
                            Console.WriteLine("Enter the number of hours to advance");
                            int hours;
                            if (!int.TryParse(Console.ReadLine(), out hours))
                            {
                                Console.WriteLine("Invalid input. Please enter a valid number.");
                                break;
                            }
                            s_dal.Config.Clock = s_dal.Config.Clock.AddHours(hours);
                            break;
                        }
                    case configMenu.AdvanceDay:
                        {
                            Console.WriteLine("Enter the number of days to advance");
                            int days;
                            if (!int.TryParse(Console.ReadLine(), out days))
                            {
                                Console.WriteLine("Invalid input. Please enter a valid number.");
                                break;
                            }
                            s_dal.Config.Clock = s_dal.Config.Clock.AddDays(days);
                            break;
                        }
                    case configMenu.AdvanceMonth:
                        {
                            Console.WriteLine("Enter the number of months to advance");
                            int months;
                            if (!int.TryParse(Console.ReadLine(), out months))
                            {
                                Console.WriteLine("Invalid input. Please enter a valid number.");
                                break;
                            }
                            s_dal.Config.Clock = s_dal.Config.Clock.AddMonths(months);
                            break;
                        }
                    case configMenu.DisplayClock:
                        {
                            Console.WriteLine($"Current clock: {s_dal.Config.Clock}");
                            break;
                        }
                    case configMenu.SetClock:
                        {
                            Console.WriteLine("Enter the new clock time (yyyy-MM-dd HH:mm:ss)");
                            if (!DateTime.TryParse(Console.ReadLine(), out DateTime newClock))
                            {
                                Console.WriteLine("Invalid input. Please enter a valid date and time.");
                                break;
                            }
                            s_dal.Config.Clock = newClock;
                            break;
                        }
                    case configMenu.DisplayRiskTime:
                        {
                            Console.WriteLine($"Current risk time: {s_dal.Config.RiskRange}");
                            break;
                        }
                    case configMenu.SetRiskTime:
                        {
                            Console.WriteLine("Enter the new risk time (hh:mm:ss)");
                            if (!TimeSpan.TryParse(Console.ReadLine(), out TimeSpan newRiskTime))
                            {
                                Console.WriteLine("Invalid input. Please enter a valid time span.");
                                break;
                            }
                            s_dal.Config.RiskRange = newRiskTime;
                            break;
                        }
                    case configMenu.Reset:
                        s_dal.Config.Reset();
                        break;
                    default:
                        Console.WriteLine("Invalid selection, please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        } while (choice != 0);
    }
    static void Main(string[] args)
    {
        int choice;
        do
        {
            choice = mainMenu();
            Menu menu = (Menu)choice;
            switch (menu)
            {
                case Menu.Exit:
                    break;
                case Menu.Volunteer:
                    VolunteerMenu();
                    break;
                case Menu.Call:
                    CallMenu();
                    break;
                case Menu.Assignment:
                    AssignmentMenu();
                    break;
                case Menu.Initialize:
                    // Initialize data
                    break;
                case Menu.DisplayAll:
                    // Display all data
                    break;
                case Menu.Configuration:
                    ConfigMenu();
                    break;
                case Menu.Reset:
                    s_dal.ResetDB();
                    break;
                default:
                    Console.WriteLine("Invalid selection, please try again.");
                    break;
            }
        } while (choice != 0);
    }
}


