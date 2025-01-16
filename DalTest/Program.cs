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
        if (!int.TryParse(Console.ReadLine(), out int choice)) throw new FormatException
    ("Your selection is incorrect, please try again and select a number from the menu");
        return choice;
    }
    public static int ConvertStringToNumber()
    {
        if (!int.TryParse(Console.ReadLine(), out int choice)) throw new FormatException
    ("Please enter a valid value.");
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
                             IEnumerable <Call> ? calls = s_dal.Call.ReadAll();
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
                            int callId = ConvertStringToNumber();
                            if (s_dal.Call.Read(callId) == null)
                                throw new Exception($"An object of type Call with such ID={callId} does not exist");
                            Console.WriteLine("Enter the VolunteerId id");
                            int volunteerId = ConvertStringToNumber();
                            if (s_dal.Volunteer.Read(volunteerId) == null)
                                throw new Exception($"An object of type Volunteer with such ID={volunteerId} does not exist");
                            s_dal.Assignment.Create(new Assignment() { CallId = callId, VolunteerId = volunteerId });
                            break;
                        }
                    case assignmentMenu.Display:
                        {
                            Console.WriteLine("Enter the assignment ID");
                            int assignmentI = ConvertStringToNumber();
                            Assignment a = s_dal.Assignment.Read(assignmentI);
                            if (a != null)
                            {
                                Console.WriteLine(a);
                            }
                            else
                            {
                                throw new Exception($"Volunteer with ID {assignmentI} not found");
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
                            int assignmentI = ConvertStringToNumber();
                            Assignment assignment = s_dal.Assignment.Read(assignmentI);
                            if (assignment != null)
                            {
                                Console.WriteLine("Enter the call id");
                                int callId = ConvertStringToNumber();
                                Console.WriteLine("Enter the VolunteerId id");
                                int volunteerId = ConvertStringToNumber();
                            }
                            else
                            {
                                throw new Exception($"Volunteer with ID {assignmentI} not found");
                            }
                            break;
                        }
                    case assignmentMenu.Delete:
                        {
                            Console.WriteLine("Enter the assignment ID");
                            int assignmentI = ConvertStringToNumber();
                            s_dal.Assignment.Delete(assignmentI);
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
            Console.WriteLine(@"
0 Exit sub-menu
1 Advance system clock by minute
2 Advance system clock by hour
3 Advance system clock by day
4 Advance system clock by month
5 Display current value of system clock
6 Set new value for system clock
7 Display current value of risk time
8 Set new value for risk time
9 Reset values for all configuration variables
");
            try
            {
                choice = SelectingFromTheMenu();
                cm = (configMenu)choice;
                switch (cm)
                {
                    case configMenu.Exit:
                        break;
                    case configMenu.AdvanceMinute:
                        s_dal.Config.Clock.Add(TimeSpan.FromMinutes(1));
                        break;
                    case configMenu.AdvanceHour:
                        s_dal.Config.Clock.Add(TimeSpan.FromHours(1));
                        break;
                    case configMenu.AdvanceDay:
                        s_dal.Config.Clock.Add(TimeSpan.FromDays(1));
                        break;
                    case configMenu.AdvanceMonth:
                        s_dal.Config.Clock.Add(TimeSpan.FromDays(30));
                        break;
                    case configMenu.DisplayClock:
                        Console.WriteLine($"Current system clock value: {s_dal.Config.Clock}");
                        break;
                    case configMenu.SetClock:
                        Console.WriteLine("Enter the new system clock value");
                        s_dal.Config.Clock = DateTime.Parse(Console.ReadLine());
                        break;
                    case configMenu.DisplayRiskTime:
                        Console.WriteLine($"Current risk time value: {s_dal.Config.RiskRange}");
                        break;
                    case configMenu.SetRiskTime:
                        Console.WriteLine("Enter the new risk time value");
                        s_dal.Config.RiskRange = TimeSpan.Parse(Console.ReadLine());
                        break;
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

        int choice = 0;
        Menu m;
        do
        {
            try
            {
                choice = mainMenu();
                m = (Menu)choice;
                switch (m)
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
                        //Initialization.Do(s_dal); //stage 2
                        Initialization.Do(); //stage 4
                        break;
                    case Menu.DisplayAll:
                        Console.WriteLine("All data in the database:\n volunteer\n");
                        foreach (Assignment assignment in s_dal.Assignment.ReadAll())
                        {
                            Console.WriteLine(assignment);
                            Volunteer v = s_dal.Volunteer.Read(assignment.VolunteerId);
                            Console.Write($"Volunteer {'\n'} MaxCallDistance={v.MaxCallDistance}        ");
                            Console.Write($"Address={v.CurrentAddress}\n");
                            Call c = s_dal.Call.Read(assignment.CallId);
                            Console.Write($"Call \n Address={c.Address}     Status={c.Status}       ");
                            Console.Write($"OpenedAt={c.OpenedAt}       ");
                            Console.WriteLine($"MaxCompletionTime={c.MaxCompletionTime}");
                            Console.WriteLine("--------------------------------------------------------------------");
                        }
                        Console.WriteLine("Calls without assignment:\n");
                        foreach (Call call in s_dal.Call.ReadAll())
                        {
                            if (s_dal.Assignment.ReadAll().Where(a => a.CallId == call.Id).Count() == 0)
                            {
                                Console.WriteLine(call);
                            }
                        }
                        Console.WriteLine("Volunteers without assignment:\n");
                        foreach (Volunteer volunteer in s_dal.Volunteer.ReadAll())
                        {
                            if (s_dal.Assignment.ReadAll().Where(a => a.VolunteerId == volunteer.Id).Count() == 0)
                            {
                                Console.WriteLine(volunteer);
                            }
                        }
                        break;
                    case Menu.Configuration:
                        ConfigMenu();
                        break;
                    case Menu.Reset:
                        s_dal.Call.DeleteAll();
                        s_dal.Assignment.DeleteAll();
                        s_dal.Volunteer.DeleteAll();
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
}


