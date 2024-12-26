using BO;
using DalApi;
using DalTest;
using DO;
using System.Collections.Specialized;
using Helpers;
using System.Runtime.CompilerServices;
using System.Diagnostics.Metrics;
using System.Transactions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlTest
{
    internal class Program
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();


        enum Menu
        {
            Exit = 0,
            Volunteer,
            Call,
            Admin
        };
        enum volunteerMenu
        {
            Exit = 0,
            Add,
            Display,
            DisplayAllByFilter,
            Update,
            Delete,
            DeleteAll,
            Login
        };
        enum callMenu
        {
            Exit = 0,
            Add,
            Display,
            DisplayAll,
            Update,
            Delete,
            DeleteAll,
            DisplayOpenCalls,
            DisplayClosedCalls,
            CancelCall,
            AssignCall,
            DisplayAmountOfCallsByStatus,
            CompleteCall

        };
        enum adminMenu
        {
            Exit = 0,
            AdvanceMinute,
            AdvanceHour,
            AdvanceDay,
            AdvanceMonth,
            AdvanceYear,
            DisplayClock,
            SetClock,
            DisplayRiskTime,
            SetRiskTime,
            Reset,
            Initialize
        };
        public static int ConvertStringToNumber()
        {
            string input = Console.ReadLine();
            int number;
            while (!int.TryParse(input, out number))
            {
                Console.WriteLine("Invalid input, please try again.");
                input = Console.ReadLine();
            }
            return number;
        }
        public static int mainMenu()
        {
            Console.WriteLine(
    @"    Select an option to proceed
0) Exit main menu.
1) Display submenu for Volunteer entity. 
2) Display submenu for Call entity.
3) Display submenu for Admin entity. ");
            return ConvertStringToNumber();
       }

        public static void VolunteerMenu()
        {
            int choice=0;
            do
            {
                try { 
                Console.WriteLine(
        @"    Select an option to proceed:
0. Exit
1. Add Volunteer
2. Display Volunteer
3. View volunteers by filter
4. Update Volunteer
5. Delete Volunteer
6. Delete All Volunteers
7. log in
");
                choice = ConvertStringToNumber();
                switch ((volunteerMenu)choice)
                {
                    case volunteerMenu.Exit:
                        mainMenu(); // Return to main menu
                        break;
                    case volunteerMenu.Add:
                        {
                            Console.WriteLine("Enter Id:");
                            int id = ConvertStringToNumber();

                            Console.WriteLine("Enter full name:");
                            string fullName = Console.ReadLine();

                            Console.WriteLine("Enter phone number:");
                            string phoneNumber = Console.ReadLine();

                            Console.WriteLine("Enter email address:");
                            string email = Console.ReadLine();

                            Console.WriteLine("Enter password (will be hashed):");
                            string password = Console.ReadLine(); // You would hash the password before saving it

                            Console.WriteLine("Enter address (optional):");
                            string address = Console.ReadLine();

                            // Creating a BO.Volunteer object
                            var newVolunteer = new BO.Volunteer
                            {
                                Id = id,
                                FullName = fullName,
                                PhoneNumber = phoneNumber,
                                Email = email,
                                // TODO: PasswordHash = HashPassword(password), // Use your preferred hashing method 
                                FullAddress = address,
                                Role = BO.Role.Volunteer, // Set the role to Volunteer by default
                                IsActive = true, // Assuming the volunteer is active by default
                                MaxDistanceForCall = 50, // default distance
                                DistanceType = BO.DistanceType.Aerial // Default distance type

                            };
                            // Call the AddVolunteer method to add the volunteer
                            s_bl.Volunteer.AddVolunteer(newVolunteer);
                        }
                        break;

                    case volunteerMenu.Display:
                        {
                            Console.WriteLine("Enter Volunteer ID: ");
                            int id = ConvertStringToNumber();
                            Console.WriteLine(s_bl.Volunteer.GetVolunteerDetails(id)); // Display Volunteer logic here
                        }
                        break;
                    case volunteerMenu.DisplayAllByFilter:
                        {
                                Console.WriteLine("Enter 1 to see all active volunteers or 0 to see inactive volunteers. Or nothing to see the entire list: ");
                                int input = ConvertStringToNumber();
                                Console.WriteLine(
@"    Choose a number to sort by:
0  ID.
1  Full Name.
2  Total Completed Calls.
3  Total Cancelled Calls.
4  Total Expired Calls.
5  Current Call ID.
6  Current Call Type.
");
                                int sort = 0;
                                string tamp = Console.ReadLine();
                                int.TryParse(tamp, out sort);
                                IEnumerable<VolunteerInList> volunteers = s_bl.Volunteer.GetVolunteers(input == 1 ? true : input == 0 ? false : null, (VolunteerFieldVolunteerInList)sort);
                                foreach (var item in volunteers) // Display All Volunteers logic here
                                {
                                    Console.WriteLine(item);
                                }
                            }
                        break;
                    case volunteerMenu.Update:
                        {
                                string idAsks;
                                int idAsks1;
                                BO.Volunteer updatedVolunteer = new BO.Volunteer();
                                Console.WriteLine("Enter yaur ID:");
                                while(true)
                                {
                                       idAsks = Console.ReadLine();
                                        if (int.TryParse( idAsks, out idAsks1))
                                        { 
                                            break; // יוצאים מהלולאה אם ההמרה הצליחה
                                        }
                                        else
                                        {
                                            Console.WriteLine("Invalid ID format. Please enter a valid integer ID.");
                                        }
                                }
                                // Request for the ID  of the volunteer to be updated
                                Console.WriteLine("Enter the ID of the volunteer to update:");
                                while (true)
                                {
                                    string inputId = Console.ReadLine();
                                    if (int.TryParse(inputId, out int id))
                                    {
                                        updatedVolunteer.Id = id;
                                        break; // יוצאים מהלולאה אם ההמרה הצליחה
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid ID format. Please enter a valid integer ID.");
                                    }
                                }
                                // Request for the new volunteer object with all the updated values


                                Console.WriteLine("Enter full name (leave empty to keep unchanged):");
                                string fullName = Console.ReadLine();
                                updatedVolunteer.FullName = string.IsNullOrEmpty(fullName) ? null : fullName;

                                Console.WriteLine("Enter phone number (leave empty to keep unchanged):");
                                string phoneNumber = Console.ReadLine();
                                updatedVolunteer.PhoneNumber = string.IsNullOrEmpty(phoneNumber) ? null : phoneNumber;

                                Console.WriteLine("Enter email (leave empty to keep unchanged):");
                                string email = Console.ReadLine();
                                updatedVolunteer.Email = string.IsNullOrEmpty(email) ? null : email;

                                Console.WriteLine("Enter full address (leave empty to keep unchanged):");
                                string fullAddress = Console.ReadLine();
                                updatedVolunteer.FullAddress = string.IsNullOrEmpty(fullAddress) ? null : fullAddress;

                                Console.WriteLine("Enter role (Admin = 0, Volunteer = 1) if updating role, leave empty to keep unchanged:");
                                string roleInput = Console.ReadLine();
                                if (int.TryParse(roleInput, out int roleNumber))
                                {
                                    // אם ההמרה הצליחה, נבדוק אם המספר הוא 0 או 1
                                    if (roleNumber == 0 || roleNumber == 1)
                                    {
                                        updatedVolunteer.Role = (BO.Role)int.Parse(roleInput);
                                    }
                                }
                                else
                                {
                                    updatedVolunteer.Role = BO.Role.Volunteer;
                                }
                                //if (!string.IsNullOrEmpty(roleInput))
                                //{
                                //    if (roleInput == "0") // Function to check if the user is an admin
                                //    {

                                //        updatedVolunteer.Role = 0;
                                //    }
                                //    else if (roleInput == "1")
                                //    {
                                //        Console.WriteLine("Only an admin can update the role.");
                                //        return;
                                //    }
                                //    else
                                //    {
                                //        throw new ArgumentException("Invalid role. Please enter a valid role.");
                                //    }
                                //}
                                // Check if the volunteer exists in the data layer
                                //var existingVolunteer = s_bl.Volunteer.GetVolunteerDetails(updatedVolunteer.Id); // Assuming there's a function like this
                                //if (existingVolunteer == null)
                                //{
                                //    throw new ArgumentException("Volunteer with the provided ID does not exist.");
                                //}

                                // Perform the update in the data layer
                                s_bl.Volunteer.UpdateVolunteer(idAsks1, updatedVolunteer);
                        }
                        break;
                    case volunteerMenu.Delete:
                        {
                            Console.WriteLine("Enter Volunteer ID: ");
                            int id = ConvertStringToNumber();
                            s_bl.Volunteer.DeleteVolunteer(id); // Delete Volunteer logic here
                        }
                        break;
                    case volunteerMenu.DeleteAll:
                        {
                            // Retrieve all volunteers from the data layer
                            IEnumerable<VolunteerInList> volunteers = s_bl.Volunteer.GetVolunteers(null, null);

                            // If no volunteers to delete, throw an exception
                            if (!volunteers.Any())
                            {
                                throw new ArgumentException("There are no volunteers to delete.");
                            }

                            // Delete each volunteer
                            foreach (var item in volunteers)
                            {
                                s_bl.Volunteer.DeleteVolunteer((int)item.Id);
                            }
                        }
                        break;
                    case volunteerMenu.Login:
                        {
                            Console.WriteLine("Enter username:");
                            int id = ConvertStringToNumber();

                            Console.WriteLine("Enter password:");
                            string password = Console.ReadLine();

                            Console.WriteLine(s_bl.Volunteer.Login(id, password));
                        }
                        break;
                    default:
                        throw new ArgumentException("Invalid selection, please try again.");
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
            int choice=0;
            do
            {
                try {
                Console.WriteLine(
                    @"    Select an option to proceed:
0. Exit
1. Add Call
2. Display Call
3. Display All Calls
4. Update Call
5. Delete Call
6. Delete All Calls
7. Display open calls
8. Display closed calls
9. Cancel call
10. Assign call
11. Display amount of calls by type
12. Complete call
");
                choice = ConvertStringToNumber();
                switch ((callMenu)choice)
                {
                    case callMenu.Exit:
                        mainMenu(); // Return to main menu
                        break;
                    case callMenu.Add:
                        {
                            BO.Call newCall = new BO.Call();

                            Console.Write("Enter Full Address: ");
                            newCall.FullAddress = Console.ReadLine();

                            Console.Write("Enter Description: ");
                            newCall.Description = Console.ReadLine();


                            s_bl.Call.AddCall(newCall);
                        }
                        break;
                    case callMenu.Display:
                        {
                            Console.WriteLine("Enter Call ID: ");
                            int id = ConvertStringToNumber();
                            Console.WriteLine(s_bl.Call.GetCallDetails(id)); // Display Call logic here
                        }
                        break;
                    case callMenu.DisplayAll:
                        {
                            foreach (var item in s_bl.Call.GetCalls(null, null, CallInListFields.OpenTime)) // Display All Calls logic here
                            {
                                Console.WriteLine(item);
                            }
                        }
                        break;
                    case callMenu.Update:
                        {
                            Console.Write("Enter the Call ID to update: ");
                            if (!int.TryParse(Console.ReadLine(), out int callId))
                            {
                                throw new ArgumentException("Invalid input for Call ID. Please enter a valid integer.");
                            }

                            BO.Call existingCall = s_bl.Call.GetCallDetails(callId);
                            if (existingCall == null)
                            {
                                throw new ArgumentException($"Call with ID {callId} not found.");
                            }

                            Console.Write("Enter the call description: ");
                            string callDescription = Console.ReadLine();

                            Console.Write("Enter the call address: ");
                            string callAddress = Console.ReadLine();

                            Console.Write("Enter the Latitude: ");
                            if (!double.TryParse(Console.ReadLine(), out double latitude))
                            {
                                throw new ArgumentException("Invalid input for Latitude. Please enter a valid number.");
                            }

                            Console.Write("Enter the Longitude: ");
                            if (!double.TryParse(Console.ReadLine(), out double longitude))
                            {
                                throw new ArgumentException("Invalid input for Longitude. Please enter a valid number.");
                                break;
                            }

                            Console.Write("Enter the call status (0 = Open, 1 = In Progress, 2 = Completed, 3 = Canceled, 4 = OpenAtRisk, 5 = InProgressAtRisk): ");
                            if (!int.TryParse(Console.ReadLine(), out int status) || !Enum.IsDefined(typeof(BO.CallStatus), status))
                            {
                                Console.WriteLine("Invalid input for Status. Please select a valid status.");
                                break;
                            }

                            BO.Call updatedCall = new BO.Call
                            {
                                Id = existingCall.Id,
                                FullAddress = callAddress,
                                Latitude = latitude,
                                Longitude = longitude,
                                Description = callDescription,
                                OpenTime = existingCall.OpenTime,
                                MaxEndTime = existingCall.MaxEndTime,
                                Status = (BO.CallStatus)status
                            };
                            s_bl.Call.UpdateCall(updatedCall);
                        }
                        break;
                    case callMenu.Delete:
                        {
                            Console.Write("Enter the Call ID to delete: ");
                            if (!int.TryParse(Console.ReadLine(), out int callId))
                            {
                                Console.WriteLine("Invalid input for Call ID. Please enter a valid integer.");
                                break;
                            }
                            s_bl.Call.DeleteCall(callId);
                        }
                        break;
                    case callMenu.DeleteAll:
                        {
                                // Retrieve all calls from the data layer
                                IEnumerable<CallInList> calls = s_bl.Call.GetCalls(CallInListFields.AssignmentId, null, null);

                            // If no calls to delete, throw an exception
                            if (!calls.Any())
                            {
                                throw new ArgumentException("There are no open and unassigned calls to delete.");
                            }

                            // Delete each call
                            foreach (var item in calls)
                            {
                                s_bl.Call.DeleteCall((int)item.AssignmentId);
                            }
                        }
                        break;
                    case callMenu.DisplayOpenCalls:
                        {
                            foreach (var item in s_bl.Call.GetCalls(null, null, null)) // Display Open Calls logic here
                            {
                                Console.WriteLine(item);
                            }
                        }
                        break;
                    case callMenu.DisplayClosedCalls:
                        {
                            foreach (var item in s_bl.Call.GetCalls(null, null, null)) // Display Closed Calls logic here
                            {
                                Console.WriteLine(item);
                            }
                        }
                        break;
                    case callMenu.CancelCall:
                        {
                            Console.WriteLine("Enter your ID: ");
                            int id = ConvertStringToNumber();
                            Console.Write("Enter the Call ID to cancel: ");
                            if (!int.TryParse(Console.ReadLine(), out int callId))
                            {
                                Console.WriteLine("Invalid input for Call ID. Please enter a valid integer.");
                                break;
                            }
                            s_bl.Call.CancelCallAssignment(id, callId);
                        }
                        break;
                    case callMenu.AssignCall:
                        {
                            Console.Write("Enter the Call ID to assign: ");
                            if (!int.TryParse(Console.ReadLine(), out int callId))
                            {
                                Console.WriteLine("Invalid input for Call ID. Please enter a valid integer.");
                                break;
                            }
                            Console.Write("Enter the Volunteer ID to assign: ");
                            if (!int.TryParse(Console.ReadLine(), out int volunteerId))
                            {
                                Console.WriteLine("Invalid input for Volunteer ID. Please enter a valid integer.");
                                break;
                            }
                            s_bl.Call.AssignVolunteerToCall(volunteerId, callId);
                        }
                        break;
                    case callMenu.DisplayAmountOfCallsByStatus:
                        {
                            Console.Write("Enter the status to display the amount of calls for (0 = Open, 1 = In Progress, 2 = Completed, 3 = Canceled, 4 = OpenAtRisk, 5 = InProgressAtRisk): ");
                            if (!int.TryParse(Console.ReadLine(), out int status) || !Enum.IsDefined(typeof(BO.CallStatus), status))
                            {
                                Console.WriteLine("Invalid input for Status. Please select a valid status.");
                                break;
                            }
                            int count = 0;
                            foreach (var item in s_bl.Call.GetCallCountsByStatus()) // Display Amount of Calls by Status logic here
                            {
                                if (item == status)
                                {
                                    count++;
                                }
                            }
                            Console.WriteLine("Amount of calls with status {0}: {1}", (BO.CallStatus)status, count);
                        }
                        break;
                    case callMenu.CompleteCall:
                        {
                            Console.Write("Enter the Volunteer ID: ");
                            if (!int.TryParse(Console.ReadLine(), out int volunteerId))
                            {
                                Console.WriteLine("Invalid input for Volunteer ID. Please enter a valid integer.");
                                break;
                            }
                            Console.Write("Enter the Assignment ID: ");
                            if (!int.TryParse(Console.ReadLine(), out int assignmentId))
                            {
                                Console.WriteLine("Invalid input for Assignment ID. Please enter a valid integer.");
                                break;
                            }
                            s_bl.Call.CompleteCallAssignment(volunteerId, assignmentId);
                        }
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
        public static void AdminMenu()
        {
            int choice=0;
            do
            {
                try
                {
                    Console.WriteLine(
                    @"    Select an option to proceed:
0. Exit
1. Advance Clock by 1 minute
2. Advance Clock by 1 hour
3. Advance Clock by 1 day
4. Advance Clock by 1 month
5. Advance Clock by 1 year
6. Display Clock
7. Set Clock
8. Display Risk Time
9. Set Risk Time
10. Reset Database
11. Initialize Database
");

                    choice = ConvertStringToNumber();
                    switch ((adminMenu)choice)
                    {
                        case adminMenu.Exit:
                            mainMenu(); // Return to main menu
                            break;
                        case adminMenu.AdvanceMinute:
                            s_bl.Admin.ForwardClock(TimeUnit.Minute); // Advance Clock by 1 minute logic here
                            break;
                        case adminMenu.AdvanceHour:
                            s_bl.Admin.ForwardClock(TimeUnit.Hour); // Advance Clock by 1 hour logic here
                            break;
                        case adminMenu.AdvanceDay:
                            s_bl.Admin.ForwardClock(TimeUnit.Day); // Advance Clock by 1 day logic here
                            break;
                        case adminMenu.AdvanceMonth:
                            s_bl.Admin.ForwardClock(TimeUnit.Month); // Advance Clock by 1 month logic here
                            break;
                        case adminMenu.AdvanceYear:
                            s_bl.Admin.ForwardClock(TimeUnit.Month); // Advance Clock by 1 month logic here
                            break;
                        case adminMenu.DisplayClock:
                            DateTime time = s_bl.Admin.GetClock(); // Display Clock logic here
                            Console.WriteLine(time);
                            break;
                        case adminMenu.SetClock:
                            break;
                        case adminMenu.DisplayRiskTime:
                            TimeSpan range = s_bl.Admin.GetMaxRange(); // Display Risk Time logic here
                            Console.WriteLine(range);
                            break;
                        case adminMenu.SetRiskTime:
                            int hours = 0, minutes = 0, seconds = 0;
                            Console.WriteLine("enter hours, minutes and second: ");
                            hours = ConvertStringToNumber();
                            minutes = ConvertStringToNumber();
                            seconds = ConvertStringToNumber();
                            s_bl.Admin.SetMaxRange(new TimeSpan(hours, minutes, seconds)); // Set Risk Time logic here
                            break;
                        case adminMenu.Reset:
                            s_bl.Admin.ResetDB(); // Reset logic here
                            break;

                        case adminMenu.Initialize:
                            s_bl.Admin.InitializeDB(); // Initialize logic here
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
                        case Menu.Admin:
                            AdminMenu();
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
}

/*
 sing BO;
using System.ComponentModel.Design;
using System.Globalization;
using System.Net.Mail;
namespace BlTest { public class Program
{ static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
/// <summary> ///
/// 
/// 
/// /// <summary> ///
/// The main entry point for the application. /// This method displays the main menu and handles user input to navigate to different submenus.
/// /// It catches and displays exceptions that occur during execution.
/// /// </summary> /// <param name="args">Command-line arguments.</param> /// <exception cref="BO.BlInvalidOptionException">Thrown when an invalid option is selected from the main menu.</exception> /// </summary> /// <param name="args"></param> /// <exception cref="BO.BlInvalidOptionException"></exception> static void Main(string[] args) { do { try { Console.Write(@" ---------------------------------------------------------------- Select Your Option: Press 0 To Exit Press 1 To Use ICall Interface Press 2 To Use IVolunteer Interface Press 3 To Use IAdmin Interface ---------------------------------------------------------------- >>> "); string input; MainMenuOption option; input = Console.ReadLine() ?? ""; if (!Enum.TryParse(input, out option)) throw new BO.BlInvalidOptionException($"Bl: Enum value for the main window is not a valid option"); switch (option) { case MainMenuOption.Exit: return; case MainMenuOption.Call: ICallSubMenu(); break; case MainMenuOption.Volunteer: IVolunteerSubMenu(); break; case MainMenuOption.Admin: IAdminSubMenu(); break; } } catch (Exception ex) { ExceptionDisplay(ex); } } while (true); } /// <summary> /// Displays the submenu for ICall interface operations. /// </summary> /// <exception cref="BO.BlInvalidOptionException">Thrown when an invalid option is selected.</exception> /// <exception cref="BO.BlInputValueUnConvertableException">Thrown when an input value cannot be converted.</exception> private static void ICallSubMenu() { do { Console.WriteLine( @" ---------------------------------------------------------------- Select Your Option: Press 1 - To Exit Press 2 - To GetCallCountsByStatus Press 3 - To GetCallList Press 4 - To GetCallDetails Press 5 - To UpdateCallDetails Press 6 - To DeleteCall Press 7 - To AddCall Press 8 - To GetClosedCallsByVolunteer Press 9 - To GetOpenCallsForVolunteer Press 10 - To CompleteTreatment Press 11 - To CancelTreatment Press 12 - To AssignCallToVolunteer ---------------------------------------------------------------- "); Console.Write(">>> "); string input; ICallSubMenuOption CallOperation; input = Console.ReadLine() ?? ""; if (!Enum.TryParse(input, out CallOperation)) throw new BO.BlInvalidOptionException($"Bl: Enum value for the main window is not a valid operation"); switch (CallOperation) { case ICallSubMenuOption.Exit: return; case ICallSubMenuOption.AddCall: BO.CallType callType; string CallAddress; string Description; DateTime DeadLine; Console.Write(@" ---------------------------------------------------------------- Pls enter the type of call: Press 0 For CarCrash Press 1 For Emergency Press 2 For MegaEmergency Press 3 For TerrorAttack press 4 For Logistics ---------------------------------------------------------------- >>> "); input = Console.ReadLine() ?? ""; if (!CallType.TryParse(input, out callType)) throw new BO.BlInputValueUnConvertableException($"Bl: Enum value for the main window is not a valid operation"); Console.WriteLine("Pls describe your call [optional]:"); Description = Console.ReadLine() ?? ""; Console.WriteLine("Pls enter the address of the call:"); CallAddress = Console.ReadLine() ?? ""; Console.WriteLine("What is the deadline for the call:"); if (!DateTime.TryParse(Console.ReadLine() ?? "", out DeadLine)) throw new BO.BlInputValueUnConvertableException($"Bl: The value is not a valid DateTime value"); BO.Call call = new BO.Call() { CallType = callType, ReadingDescription = Description, Address = CallAddress, OpenTime = s_bl.Admin.GetSystemClock(), MaxEndTime = DeadLine }; s_bl.Call.AddCall(call); break; case ICallSubMenuOption.UpdateCallDetails: BO.CallType callType1; string CallAddress1; string Description1; DateTime DeadLine1; string answer; Console.WriteLine("Please give me the call ID you want to update:"); int callId = int.Parse(Console.ReadLine() ?? ""); BO.Call call1 = s_bl.Call.GetCallDetails(callId); Console.WriteLine("do you want to change the type of the call: Y/N"); answer = Console.ReadLine() ?? ""; if (answer == "Y") { //Issue #20 Console.WriteLine("Pls enter the type of the call:"); input = Console.ReadLine() ?? ""; if (!Enum.TryParse(input, out callType1)) { throw new BO.BlInputValueUnConvertableException($"Bl: Enum value for the main window is not a valid operation"); } call1.CallType = callType1; } Console.WriteLine("do you want to change the description of the call: Y/N"); answer = Console.ReadLine() ?? ""; if (answer == "Y") { Console.WriteLine("Pls describe your call [optional]:"); Description1 = Console.ReadLine() ?? ""; call1.ReadingDescription = Description1; } Console.WriteLine("Do you want to change the call adress: Y/N"); answer = Console.ReadLine() ?? ""; if (answer == "Y") { Console.WriteLine("Pls enter the address of the call:"); CallAddress1 = Console.ReadLine() ?? ""; call1.Address = CallAddress1; } Console.WriteLine("Do you want to change the deadline of the call: Y/N"); answer = Console.ReadLine() ?? ""; if (answer == "Y") { Console.WriteLine("What is the deadline for the call:"); DeadLine1 = DateTime.Parse(Console.ReadLine() ?? ""); if (DeadLine1 < s_bl.Admin.GetSystemClock()) { throw new BO.BlInputValueUnConvertableException($"Bl: The deadline for the call is invalid"); } call1.MaxEndTime = DeadLine1; } s_bl.Call.UpdateCallDetails(call1); break; case ICallSubMenuOption.AssignCallToVolunteer: int callId1; int VolunteerId; Console.WriteLine("Please give me the Assignment ID you want to select:"); callId1 = int.Parse(Console.ReadLine() ?? ""); Console.WriteLine("Please give me the volunteer ID you want to select:"); VolunteerId = int.Parse(Console.ReadLine() ?? ""); s_bl.Call.AssignCallToVolunteer(VolunteerId, callId1); break; case ICallSubMenuOption.CancelTreatment: int callId2; int VolunteerId1; Console.WriteLine("Please give me the call ID you want to update:"); callId2 = int.Parse(Console.ReadLine() ?? ""); Console.WriteLine("Please give me the volunteer ID you want to update:"); VolunteerId1 = int.Parse(Console.ReadLine() ?? ""); try { s_bl.Call.CancelTreatment(VolunteerId1, callId2); Console.WriteLine("Assignement has been Updated"); } catch (Exception ex) { ExceptionDisplay(ex); } break; case ICallSubMenuOption.CompleteTreatment: { int callId3; int VolunteerId2; Console.WriteLine("Please give me the Assignment ID you want to update:"); callId3 = int.Parse(Console.ReadLine() ?? ""); Console.WriteLine("Please give me the volunteer ID you want to update:"); VolunteerId2 = int.Parse(Console.ReadLine() ?? ""); try { s_bl.Call.CompleteTreatment(VolunteerId2, callId3); Console.WriteLine("Assignement has been Updated"); } catch (Exception ex) { ExceptionDisplay(ex); } break; } case ICallSubMenuOption.DeleteCall: { DeleteCallReqeust(); break; } case ICallSubMenuOption.GetCallList: { GetListOfCalls(); break; } case ICallSubMenuOption.GetCallDetails: { GetDetielsOfCall(); break; } case ICallSubMenuOption.GetClosedCallsByVolunteer: { GetClosedCallsByVolunteer(); break; } case ICallSubMenuOption.GetOpenCallsForVolunteer: { GetOpenCallsForVolunteer(); break; } case ICallSubMenuOption.GetCallCountsByStatus: { GetTotalCallsByStatus(); break; } } } while (true); } /// <summary> /// This method displays the submenu for volunteers and handles the user's input. /// It provides options to create, delete, update, get, and list volunteers, as well as sign in. /// If an invalid option is selected, a BO.BlInvalidOptionException is thrown. /// </summary> /// <exception cref="BO.BlInvalidOptionException"></exception> private static void IVolunteerSubMenu() { do { try { //Get operation from user Console.Write( @" ---------------------------------------------------------------- Volunteer SubMenu: Please Select One Of The Presented Options press 0: To Exit to The Main Hub Press 1: To Login as a Volunteer Press 2: To Read All Volunteers Press 3: To Read a Volunteer Press 4: To Update a Volunteer Press 5: To Remove an Volunteer Press 6: To Add a New Volunteer Press 0: To Exit ---------------------------------------------------------------- >>> "); IVolunteerSubMenuOption operation; string input = Console.ReadLine() ?? ""; if (!Enum.TryParse(input, out operation)) throw new BO.BlInvalidOptionException($"Bl: Operation {input}, is not available"); switch (operation) { case IVolunteerSubMenuOption.Exit: return; case IVolunteerSubMenuOption.createVolunteer: createVol(); break; case IVolunteerSubMenuOption.DeleteVolunteer: deleteVol(); break; case IVolunteerSubMenuOption.UpdateVolunteer: updateVol(); break; case IVolunteerSubMenuOption.GetVolunteer: GetVol(); break; case IVolunteerSubMenuOption.getVolunteersList: getVolList(); break; case IVolunteerSubMenuOption.signIn: signInVol(); break; } } catch (Exception ex) { ExceptionDisplay(ex); } } while (true); } /// <summary> /// This method displays the submenu for administrators and handles the user's input. /// It provides options to get and update the system clock, get and update the risk range, reset and initialize the database. /// If an invalid option is selected, a BO.BlInvalidOptionException is thrown. /// </summary> /// <exception cref="BO.BlInvalidOptionException"></exception> private static void IAdminSubMenu() { do { try { //Get operation from user Console.Write( @" ---------------------------------------------------------------- Admin's SubMenu: Please Select One Of The Presented Options Press 0: To Exit to The Main Hub Press 1: To Print The Current System Clock Press 2: To Update System's Clock Press 3: To Print The Current System Risk Range Value Press 4: To Update System's System Risk Range Press 5: To Reset The Database Press 6: To Initialize The Database ---------------------------------------------------------------- >>> "); IAdminOperations operation; string input = Console.ReadLine() ?? ""; if (!Enum.TryParse(input, out operation)) throw new BO.BlInvalidOptionException($"Bl: Operation {input}, is not available"); switch (operation) { case IAdminOperations.Exit: return; case IAdminOperations.GetSystemClock: getSysClock(); break; case IAdminOperations.ClockUpdate: clockUpdate(); break; case IAdminOperations.GetRiskRange: getRiskRangeFunc(); break; case IAdminOperations.SetRiskRange: UpdateSysRiskRange(); break; case IAdminOperations.ResetAllData: ResetSysDB(); break; case IAdminOperations.Initialization: InitializeSysDB(); break; } } catch (Exception ex) { ExceptionDisplay(ex); } } while (true); } #region Help Methods /// <summary> /// An help method for waiting for the user to prsss enter to continue /// </summary> /// <param name="ex"></param> static public void ExceptionDisplay(Exception ex) { Console.WriteLine(ex.Message); Console.WriteLine("Press enter to continue"); Console.Write(">>> "); var tmp = Console.ReadLine(); } /// <summary> /// An help method which handles boolean input from the user /// </summary> /// <param name="msg">The display presented to the user</param> /// <returns>The user's answer (yes = true, no = false)</returns> static private bool RequestBooleanAnswer(string msg) { string input; do { Console.WriteLine(msg); input = Console.ReadLine() ?? ""; if (input != "yes" && input != "no") Console.WriteLine($"Please Choose Either 'yes' or 'no'"); else break; } while (true); return input == "yes"; } #endregion #region vol func //build a new volunteer private static BO.Volunteer buildVol() { Console.WriteLine("Please enter the following details to create a Volunteer:"); int volunteerID = RequestIntegerInputFromUser("Volunteer ID: "); Console.Write("Name: "); string name = Console.ReadLine(); Console.Write("Phone Number: "); string phoneNumber = Console.ReadLine(); Console.Write("Email: "); string email = Console.ReadLine(); Console.Write("Password: "); string? password = Console.ReadLine(); Console.Write("Current Address: "); string? currentAddress = Console.ReadLine(); double? latitude = null; double? longitude = null; double? maxDistance = RequestDoubleInputFromUser("Max Distance: "); Console.Write("Is Active (true/false): "); bool isActive = RequestBooleanAnswer("Do You Want to Get Active (yes / no)? "); int rChoice = RequestIntegerInputFromUser("Role (0 for Volunteer, any else number for manager): "); Role role; if (rChoice == 0) role = Role.Volunteer; else role = Role.Management; int dType = RequestIntegerInputFromUser("Distance Type (0 for walking, 1 for Road, 2 for air (air-deafult)): "); DistanceType distanceType; if (dType == 0) distanceType = DistanceType.Walk; else if (dType == 1) distanceType = DistanceType.Travel; else distanceType = DistanceType.Airial; BO.Volunteer newVolunteer = new BO.Volunteer { Id = volunteerID, Name = name, Phone= phoneNumber, Email = email, Password = password, Address = currentAddress, Latitude = latitude, Longitude = longitude, MaxDistance = maxDistance, Active = isActive, Role = role, DistanceType = distanceType, TotalCallsHandled = 0, TotalCallsCancelled = 0, TotalCallsExpired = 0, WhileCall = null }; return newVolunteer; } /// Updates the details of an existing volunteer. /// </summary> /// <param name="ID">The ID of the volunteer to update.</param> /// <returns>The updated volunteer object.</returns> /// <exception cref="ArgumentException">Thrown when an invalid input is provided.</exception> private static BO.Volunteer UpdateHelper(int ID) { BO.Volunteer updateVol = s_bl.Volunteer.GetVolunteer(ID); int? volunteerID = ID; string? name = null; if (RequestBooleanAnswer("Do you want to update Name? (yes / no): ")) { Console.Write("Name: "); name = Console.ReadLine(); } else { name = updateVol.Name; } string? phoneNumber = null; if (RequestBooleanAnswer("Do you want to update Phone Number? (yes / no): ")) { Console.Write("Phone Number: "); phoneNumber = Console.ReadLine(); } else { phoneNumber = updateVol.Phone; } string? email = null; if (RequestBooleanAnswer("Do you want to update Email? (yes / no): ")) { Console.Write("Email: "); email = Console.ReadLine(); } else { email = updateVol.Email; } string? password = null; if (RequestBooleanAnswer("Do you want to update Password? (yes / no): ")) { Console.Write("Password: "); password = Console.ReadLine(); } else { password = updateVol.Password; } string? currentAddress = null; if (RequestBooleanAnswer("Do you want to update Current Address? (yes / no): ")) { Console.Write("Current Address: "); currentAddress = Console.ReadLine(); } else { currentAddress = updateVol.Address; } double? latitude = null; double? longitude = null; double? maxDistance = null; if (RequestBooleanAnswer("Do you want to update Max Distance? (yes / no): ")) { Console.Write("Max Distance: "); maxDistance = double.Parse(Console.ReadLine()); } else { maxDistance = updateVol.MaxDistance; } bool? isActive = null; if (RequestBooleanAnswer("Do you want to update Is Active? (yes / no): ")) { isActive = RequestBooleanAnswer("Is Active (yes / no): "); } else { isActive = updateVol.Active; } Role? role = null; if (updateVol.Role == Role.Management) { if (RequestBooleanAnswer("Do you want to update Role? (yes / no): ")) { Console.Write("Role (0 for Volunteer, 1 for Manager): "); int rChoice = int.Parse(Console.ReadLine()); role = rChoice == 0 ? Role.Volunteer : Role.Management; } else { role = updateVol.Role; } } else { role = Role.Volunteer; } DistanceType? distanceType = null; if (RequestBooleanAnswer("Do you want to update Distance Type? (yes / no): ")) { Console.Write("Distance Type (0 for Air, 1 for Road, 2 for Walking): "); int dType = int.Parse(Console.ReadLine()); distanceType = dType switch { 0 => DistanceType.Airial, 1 => DistanceType.Walk, 2 => DistanceType.Travel, _ => throw new ArgumentException("Invalid Distance Type") }; } else { distanceType = updateVol.DistanceType; } BO.Volunteer newVolunteer = new BO.Volunteer { Id = volunteerID ?? 0, Name = name, Phone = phoneNumber, Email = email, Password = password, Address = currentAddress, Latitude = latitude, Longitude = longitude, MaxDistance = maxDistance, Active = isActive ?? false, Role = role ?? Role.Volunteer, DistanceType = distanceType ?? DistanceType.Airial, TotalCallsHandled = 0, TotalCallsCancelled = 0, TotalCallsExpired = 0, WhileCall = null }; return newVolunteer; } /// <summary> /// Requests a double input from the user with a given message. /// </summary> /// <param name="msg">The message to display to the user.</param> /// <returns>The double value input by the user.</returns> static private double RequestDoubleInputFromUser(string msg) { double res; string input; do { Console.Write(msg); input = Console.ReadLine() ?? ""; if (double.TryParse(input, out res) || input == null) break; else Console.WriteLine($"Error: The value '{input}' is not a valid double. Please try again."); } while (true); return res; } //build a new volunteer and add him to the system private static void createVol() { BO.Volunteer newVolunteer = buildVol(); s_bl.Volunteer.createVolunteer(newVolunteer); } //remove a volunteer from the system private static void deleteVol() { Console.Write("Enter the ID of the volunteer to delete: "); int ID = int.Parse(Console.ReadLine()); s_bl.Volunteer.DeleteVolunteer(ID); } //update a volunteer in the system(without checking if he is allowed to do so) private static void updateVol() { Console.Write("Enter the your ID: "); int changerID = int.Parse(Console.ReadLine()); Console.Write("Enter the new details of the volunteer that you want to update: "); BO.Volunteer updateVol =UpdateHelper(changerID); s_bl.Volunteer.UpdateVolunteer(changerID, updateVol); } //print the details of a volunteer private static void GetVol() { Console.Write("Enter the Id of the volunteer to read: "); int ID = int.Parse(Console.ReadLine()); Console.WriteLine(s_bl.Volunteer.GetVolunteer(ID)); } //print the list of volunteers in the system private static void getVolList() { bool? isActive = null; VolunteerSortBy? volInListProp = null; int sortBy; if (RequestBooleanAnswer("Do You want To Filter By IsActive Value? (yes / no): ")) isActive = RequestBooleanAnswer("Do You Want to Get Active (yes / no)? "); Console.WriteLine( @" ---------------------------------------------------------------- what is the order that you want to sort the list by? press 0: To VolunteerID press 1: To NumOCTreated press 2: To NumOCCanceled press 3: To NumOCExpired press 4: To CallIDInProgress press 5: To Name press 6: To IsActive press 7: To CallType default: VolunteerID ---------------------------------------------------------------- "); sortBy = int.Parse(Console.ReadLine()); if (sortBy < 0 || sortBy > 7) throw new BO.BlInvalidOptionException("Bl: The value is not a valid option"); var Volunteer = s_bl.Volunteer.getVolunteersList(isActive, (BO.VolunteerSortBy)sortBy); foreach (BO.VolunteerInList volunteer in Volunteer) { Console.WriteLine("-------------------------------------------"); Console.WriteLine(volunteer); Console.WriteLine("-------------------------------------------"); } } //sign in as a volunteer private static void signInVol() { Console.Write("Enter Your Username (Email Address): "); string emailAddress = Console.ReadLine() ?? ""; Console.Write("Enter Your Password: "); string password = Console.ReadLine() ?? ""; BO.Role userType = s_bl.Volunteer.signIn(emailAddress, password); Console.WriteLine($"The account under the email address of: {emailAddress} is a {userType.ToString()}"); } #endregion #region admin func //update the system clock private static void clockUpdate() { Console.Write( @$" ------------------------------------------------------------------------------ Clock Update Menu: Please Select the Time Unit that You are willing to forward to time with: Press 1: To Forward By One SECOND Press 2: To Forward By One MINUTE Press 3: To Forward By One HOUR Press 4: To Forward By One DAY Press 5: To Forward By One MONTH Press 6: To Forward By One YEAR ------------------------------------------------------------------------------ >>> "); int choice = int.Parse(Console.ReadLine()); TimeUnit timeUnit = choice switch { 1 => TimeUnit.SECOND, 2 => TimeUnit.MINUTE, 3 => TimeUnit.HOUR, 4 => TimeUnit.DAY, 5 => TimeUnit.MONTH, 6 => TimeUnit.YEAR, _ => throw new BO.BlInvalidOptionException("Bl: The value is not a valid option") }; //string input = Console.ReadLine() ?? ""; //if (!Enum.TryParse(input, out TimeUnit option)) // throw new BO.BLInvalidInput($"Bl: The value {input}, is not a vaid IAdminOperations value"); s_bl.Admin.AdvanceSystemClock(timeUnit); } //return the system clock private static void getSysClock() => Console.WriteLine($"Current System Clock: {s_bl.Admin.GetSystemClock()}"); //return the system risk range private static void getRiskRangeFunc() => Console.WriteLine($"Current System RiskRange: {s_bl.Admin.GetRiskTimeRange()}"); //initialize the system database private static void InitializeSysDB() => s_bl.Admin.InitializeDatabase(); //reset the system database private static void ResetSysDB() => s_bl.Admin.ResetDatabase(); //update the system risk range private static void UpdateSysRiskRange() { Console.Write("Enter the new Risk Range (in this format: DD:HH:MM:SS): "); string input = Console.ReadLine() ?? ""; if (!TimeSpan.TryParse(input, out TimeSpan newRiskRange)) throw new BO.BLInvalidInput($"Bl: The value {input}, is not a valid TimeSpan value"); s_bl.Admin.SetRiskTimeRange(newRiskRange); } #endregion #region ICall Methods /// <summary> /// /// Get the total number of calls by their status /// </summary> private static void GetTotalCallsByStatus() { int typeOfCall = 1; foreach (int value in s_bl.Call.GetCallCountsByStatus()) Console.WriteLine($"Number of {(BO.CallStatus)typeOfCall++}: {value}"); } /// <summary> /// Get the details of a specific call /// </summary> /// <exception cref="BO.BlInputValueUnConvertableException"></exception> private static void GetOpenCallsForVolunteer() { BO.CallDetailsFields sortField = BO.CallDetailsFields.Id; BO.CallType? callType = null; int id = RequestIntegerInputFromUser("Enter the Volunteer's Id: "); if (RequestBooleanAnswer("Do You Want to Filter the Values? (yes/ no): ")) { //Issue #20: Not enough CallTypes Console.Write($"Enter the Value Which You Want To Filter the Results By ({BO.CallType.CarCrash},{BO.CallType.Emergency},{BO.CallType.MegaEmergency},{BO.CallType.TerrorAttack},{BO.CallType.Logistics}): "); string input = Console.ReadLine() ?? ""; if (!Enum.TryParse(input, out BO.CallType tmp)) throw new BO.BlInputValueUnConvertableException($"Bl: The value {input}, is not a valid CallType value"); else callType = tmp; } if (RequestBooleanAnswer("Do You Want to Sort the Values? (yes / no): ")) { Console.Write( $@" -------------------------------------------------------- Choose One Of the Presented Options: {BO.CallDetailsFields.Id} {BO.CallDetailsFields.CallType} {BO.CallDetailsFields.Description} {BO.CallDetailsFields.Address} {BO.CallDetailsFields.OpenTime} {BO.CallDetailsFields.MaxEndTime} {BO.CallDetailsFields.DistanceFromVolunteer} -------------------------------------------------------- >>> "); string input = Console.ReadLine() ?? ""; if (!Enum.TryParse(input, out sortField)) throw new BO.BlInputValueUnConvertableException($"Bl: The value {input}, is not a valid CallType value"); } foreach (var call in s_bl.Call.GetOpenCallsForVolunteer(id, callType , sortField)) Console.WriteLine(call); } /// <summary> /// Get the details of a specific call /// </summary> /// <exception cref="BO.BlInputValueUnConvertableException"></exception> private static void GetClosedCallsByVolunteer() { BO.ClosedCallInListFields? sortField = null; BO.CallType? callType = null; int id = RequestIntegerInputFromUser("Enter the Volunteer's Id: "); if (RequestBooleanAnswer("Do You Want to Filter the Values? (yes / no): ")) { //Issue #20: Not enough CallTypes Console.Write($"Enter the Value Which You Want To Filter the Results By ({BO.CallType.CarCrash},{BO.CallType.Emergency},{BO.CallType.MegaEmergency},{BO.CallType.TerrorAttack},{BO.CallType.Logistics}): "); string input = Console.ReadLine() ?? ""; if (Enum.TryParse(input, out BO.CallType tmp)) throw new BO.BlInputValueUnConvertableException($"Bl: The value {input}, is not a valid CallType value"); else callType = tmp; } if (RequestBooleanAnswer("Do You Want to Sort the Values? (yes / no): ")) { Console.Write( $@" -------------------------------------------------------- Choose One Of the Presented Options: {BO.ClosedCallInListFields.Id} {BO.ClosedCallInListFields.CallType} {BO.ClosedCallInListFields.Address} {BO.ClosedCallInListFields.Beginning} {BO.ClosedCallInListFields.AssignTime} {BO.ClosedCallInListFields.End} {BO.ClosedCallInListFields.EndType} -------------------------------------------------------- >>> "); string input = Console.ReadLine() ?? ""; if (Enum.TryParse(input, out BO.ClosedCallInListFields tmp)) throw new BO.BlInputValueUnConvertableException($"Bl: The value {input}, is not a valid CallType value"); else sortField = tmp; } foreach (var call in s_bl.Call.GetClosedCallsByVolunteer(id, (DO.CallType?)callType, sortField)) Console.WriteLine(call); } /// <summary> /// Get details of a specific call by its ID. /// </summary> private static void GetDetielsOfCall() { int id = RequestIntegerInputFromUser("Enter the Cal Id that You Would Like Inforamtion About: "); Console.WriteLine(s_bl.Call.GetCallDetails(id)); } /// <summary> /// This method retrieves a list of calls based on optional filtering and sorting criteria. /// It prompts the user to decide if they want to filter or sort the list of calls. /// If filtering is chosen, the user is prompted to select a field and provide a value to filter by. /// If sorting is chosen, the user is prompted to select a field to sort by. /// The method then retrieves the list of calls from the business logic layer and displays each call. /// </summary> /// <exception cref="BlInputValueUnConvertableException">Thrown when the user input cannot be converted to the expected type.</exception> private static void GetListOfCalls() { BO.CallInListFields? filterField = null; object? filterValue = null; BO.CallInListFields? sortingField = null; if (RequestBooleanAnswer("Do you want to filter the calls? (yes / no): ")) { Console.Write( $@" ------------------------------------------------------------ Please select one of the following fields to filter by: {CallInListFields.AssignmentId} {CallInListFields.CallId} {CallInListFields.CallType} {CallInListFields.OpenTime} {CallInListFields.RemainingTime} {CallInListFields.LastVolunteerName} {CallInListFields.CompletionTime} {CallInListFields.Status} {CallInListFields.TotalAssignments} ------------------------------------------------------------ >>> "); if (!Enum.TryParse(Console.ReadLine(), out CallInListFields tmp)) throw new BlInputValueUnConvertableException($"Bl: The value is not a valid CallInListField value"); else { filterField = tmp; } Console.Write($"Enter the value that you are willing the fields of {filterField} to contained: "); filterValue = Console.ReadLine() ?? ""; } if (RequestBooleanAnswer("Do you want to sort the calls? (yes / no): ")) { Console.Write($@" ------------------------------------------------------------ Please select one of the following fields to sort by: {CallInListFields.AssignmentId} {CallInListFields.CallId} {CallInListFields.CallType} {CallInListFields.OpenTime} {CallInListFields.RemainingTime} {CallInListFields.LastVolunteerName} {CallInListFields.CompletionTime} {CallInListFields.Status} {CallInListFields.TotalAssignments} ------------------------------------------------------------ >>> "); if (!Enum.TryParse(Console.ReadLine(), out CallInListFields tmp)) throw new BlInputValueUnConvertableException($"Bl: The value is not a valid CallInListField value"); else { sortingField = tmp; } } foreach (CallInList callInList in s_bl.Call.GetCallList(filterField, filterValue, sortingField)) Console.WriteLine(callInList); } /// <summary> /// This method requests a Volunteer's id and un-assigns him from his current task /// </summary> private static void DeleteCallReqeust() { int id = RequestIntegerInputFromUser("Enter the Call Id that you want to delete: "); s_bl.Call.DeleteCall(id); } /// <summary> /// This method requests a Volunteer's id and un-assigns him from his current task /// </summary> /// <param name="msg"></param> /// <returns></returns> /// <exception cref="BlInputValueUnConvertableException"></exception> static private int RequestIntegerInputFromUser(string msg) { int res; string input; do { Console.Write(msg); input = Console.ReadLine() ?? ""; if (Int32.TryParse(input, out res)) break; else throw new BlInputValueUnConvertableException($"Bl: The value {input}, is not a valid integer"); } while (true); return res; } #endregion } }
*/

