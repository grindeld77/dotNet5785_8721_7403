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
using Microsoft.VisualBasic;
using System.Runtime.Intrinsics.Arm;
using System.Data;

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

        public static bool TryParsePhoneNumber(string phoneNumber, out string result)
        {
            result = phoneNumber;
            if (string.IsNullOrWhiteSpace(phoneNumber) || phoneNumber.Length != 10)
            {
                return false;
            }
            return true;
        }

        public static bool TryParseEmail(string email, out string result)
        {
            result = email;
            var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(email))
            {
                return false;
            }
            return true;
        }

        public static bool TryParseRole(string roleInput, out BO.Role role)
        {
            role = BO.Role.Volunteer; // ברירת מחדל
            if (string.IsNullOrWhiteSpace(roleInput)) return true; // לא הזין כלום אז נשאר ברירת מחדל
            if (int.TryParse(roleInput, out int roleNumber))
            {
                if (roleNumber == 0)
                {
                    role = BO.Role.Admin;
                    return true;
                }
                else if (roleNumber == 1)
                {
                    role = BO.Role.Volunteer;
                    return true;
                }
            }
            return false;
        }

        public static void VolunteerMenu()
        {
            int choice = 0;
            do
            {
                try
                {
                    Console.WriteLine(
                @"    Select an option to proceed:
0. Exit
1. Add Volunteer
2. Display Volunteer
3. View volunteers by filter
4. Update Volunteer
5. Delete Volunteer
6. Delete All Volunteers
7. Log in
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

                                string phoneNumber;
                                do
                                {
                                    Console.WriteLine("Enter phone number (10 digits):");
                                    phoneNumber = Console.ReadLine();
                                } while (!TryParsePhoneNumber(phoneNumber, out phoneNumber));

                                string email;
                                do
                                {
                                    Console.WriteLine("Enter email address:");
                                    email = Console.ReadLine();
                                } while (!TryParseEmail(email, out email));

                                Console.WriteLine("Enter password (will be hashed):");
                                string password = Console.ReadLine(); // You would hash the password before saving it

                                Console.WriteLine("Enter address (optional):");
                                string address = Console.ReadLine();

                                BO.Role role = BO.Role.Volunteer; // Default role
                                Console.WriteLine("Enter role (0 for Admin, 1 for Volunteer):");
                                string roleInput = Console.ReadLine();
                                while (!TryParseRole(roleInput, out role))
                                {
                                    Console.WriteLine("Invalid role. Please enter 0 for Admin or 1 for Volunteer.");
                                    roleInput = Console.ReadLine();
                                }

                                // Creating a BO.Volunteer object
                                var newVolunteer = new BO.Volunteer
                                {
                                    Id = id,
                                    FullName = fullName,
                                    PhoneNumber = phoneNumber,
                                    Email = email,
                                    FullAddress = address,
                                    Role = role,
                                    IsActive = true, // Assuming the volunteer is active by default
                                    MaxDistanceForCall = 50, // Default distance
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

                        case volunteerMenu.Update:
                            {
                                // Request for the ID of the volunteer to be updated
                                Console.WriteLine("Enter the ID of the volunteer to update:");
                                int volunteerId = ConvertStringToNumber();

                                // Check if the volunteer exists in the data layer
                                var existingVolunteer = s_bl.Volunteer.GetVolunteerDetails(volunteerId); // Assuming there's a function like this
                                if (existingVolunteer == null)
                                {
                                    throw new ArgumentException("Volunteer with the provided ID does not exist.");
                                }

                                BO.Volunteer updatedVolunteer = new BO.Volunteer();
                                updatedVolunteer.Id = volunteerId;

                                Console.WriteLine("Enter full name (leave empty to keep unchanged):");
                                string fullName = Console.ReadLine();
                                updatedVolunteer.FullName = string.IsNullOrEmpty(fullName) ? existingVolunteer.FullName : fullName;

                                string phoneNumber;
                                do
                                {
                                    Console.WriteLine("Enter phone number (leave empty to keep unchanged):");
                                    phoneNumber = Console.ReadLine();
                                } while (!string.IsNullOrEmpty(phoneNumber) && !TryParsePhoneNumber(phoneNumber, out phoneNumber));

                                updatedVolunteer.PhoneNumber = string.IsNullOrEmpty(phoneNumber) ? existingVolunteer.PhoneNumber : phoneNumber;

                                string email;
                                do
                                {
                                    Console.WriteLine("Enter email (leave empty to keep unchanged):");
                                    email = Console.ReadLine();
                                } while (!string.IsNullOrEmpty(email) && !TryParseEmail(email, out email));

                                updatedVolunteer.Email = string.IsNullOrEmpty(email) ? existingVolunteer.Email : email;

                                Console.WriteLine("Enter full address (leave empty to keep unchanged):");
                                string fullAddress = Console.ReadLine();
                                updatedVolunteer.FullAddress = string.IsNullOrEmpty(fullAddress) ? existingVolunteer.FullAddress : fullAddress;

                                Console.WriteLine("Enter role (0 for Admin, 1 for Volunteer) if updating role, leave empty to keep unchanged:");
                                string roleInput = Console.ReadLine();
                                if (!string.IsNullOrEmpty(roleInput))
                                {
                                    while (!TryParseRole(roleInput, out BO.Role role))
                                    {
                                        Console.WriteLine("Invalid role. Please enter 0 for Admin or 1 for Volunteer.");
                                        roleInput = Console.ReadLine();
                                        updatedVolunteer.Role = role;
                                    }
                                }

                                // Perform the update in the data layer
                                s_bl.Volunteer.UpdateVolunteer(volunteerId, updatedVolunteer);
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
                                IEnumerable<VolunteerInList> volunteers = s_bl.Volunteer.GetVolunteers(null, null, null);

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
            int choice = 0;
            do
            {
                try
                {
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
                                BO.CallType callType;
                                string callAddress;
                                string description;
                                DateTime endTime;

                                Console.Write(@"
Enter the type of call:
1. MedicalEmergency  
2. PatientTranspor    
3. TrafficAccident
4. FirstAid
5. Rescue
6. FireEmergency
7. CardiacEmergency
8. Poisoning          
9. AllergicReaction   
10. MassCausalities     
11. TerrorAttack
");
                                string input = Console.ReadLine();
                                if (!Enum.TryParse(input, out callType))
                                    throw new BO.BlException($"The value '{input}' is not a valid CallType value");

                                Console.WriteLine("Enter description for your call:");
                                description = Console.ReadLine() ?? "";

                                Console.WriteLine("Enter the address of the call:");
                                callAddress = Console.ReadLine() ?? "";

                                Console.WriteLine("What is the deadline for the call:");
                                if (!DateTime.TryParse(Console.ReadLine(), out endTime))
                                    throw new BO.BlException("Invalid input for the EndTime. Please enter a valid DateTime.");

                                BO.Call call = new BO.Call()
                                {
                                    Type = callType,
                                    Description = description,
                                    FullAddress = callAddress,
                                    OpenTime = s_bl.Admin.GetClock(),
                                    MaxEndTime = endTime
                                };
                                s_bl.Call.AddCall(call);
                            }
                            break;
                        case callMenu.Display:
                            {
                                Console.WriteLine("Enter Call ID: ");
                                if (!int.TryParse(Console.ReadLine(), out int id))
                                    throw new ArgumentException("Invalid input for Call ID. Please enter a valid integer.");
                                Console.WriteLine(s_bl.Call.GetCallDetails(id)); // Display Call logic here
                            }
                            break;
                        case callMenu.DisplayAll:
                            {
                                BO.CallStatus? filterField = null;
                                object? filterValue = null;
                                BO.CallInListFields? sortingField = null;
                                Console.WriteLine("Do you want to filter the calls? yes / no:");
                                string answer = Console.ReadLine()?.ToLower();
                                if (answer == "yes")
                                {
                                    Console.WriteLine(@"
Please select one of the following fields to filter by:
0. Open
1. InProgress
2. Closed
3. Expired
4. OpenAtRisk
5. InProgressAtRisk
");
                                    if (!Enum.TryParse(Console.ReadLine(), out BO.CallStatus filter))
                                        throw new BlInvalidOperationException("Invalid input for filter field. Please enter a valid field.");
                                    else
                                        filterField = filter;

                                    Console.Write($"Enter the value for the {filterField} field: ");
                                    filterValue = Console.ReadLine() ?? "";
                                }

                                Console.WriteLine("Do you want to sort the calls? yes / no:");
                                answer = Console.ReadLine()?.ToLower();
                                if (answer == "yes")
                                {
                                    Console.WriteLine(@"
Please select one of the following fields to sort by:
0. Assignment ID
1. Call ID
2. CallType
3. Open time
4. remaining time
5. Last Volunteer
6. TotalHandlingTime
7. Status
8. Total Assignments
");
                                    if (!Enum.TryParse(Console.ReadLine(), out CallInListFields sort))
                                        throw new BlInvalidOperationException("Invalid input for sort field. Please enter a valid field.");
                                    else
                                        sortingField = sort;
                                }
                                foreach (var item in s_bl.Call.GetCalls(filterField, filterValue, sortingField)) // Display All Calls logic here
                                    Console.WriteLine(item);
                            }
                            break;
                        case callMenu.Update:
                            {
                                Console.Write("Enter the Call ID to update: ");
                                if (!int.TryParse(Console.ReadLine(), out int callId))
                                {
                                    Console.WriteLine("Invalid input for Call ID. Please enter a valid integer.");
                                    break;
                                }

                                BO.Call existingCall = s_bl.Call.GetCallDetails(callId);
                                if (existingCall == null)
                                {
                                    Console.WriteLine($"Call with ID {callId} not found.");
                                    break;
                                }

                                Console.WriteLine("Do you want to change the type of the call? Yes / No");
                                string answer = Console.ReadLine()?.ToLower();
                                if (answer == "yes")
                                {
                                    Console.WriteLine("Enter the type of call: ");
                                    if (!Enum.TryParse(Console.ReadLine(), out BO.CallType callType))
                                    {
                                        Console.WriteLine("Invalid input for Call Type.");
                                        break;
                                    }
                                    existingCall.Type = callType;
                                }

                                Console.WriteLine("Do you want to change the description of the call? Yes / No");
                                answer = Console.ReadLine()?.ToLower();
                                if (answer == "yes")
                                {
                                    Console.WriteLine("Enter the description of the call: ");
                                    existingCall.Description = Console.ReadLine() ?? "";
                                }

                                Console.WriteLine("Do you want to change the address of the call? Yes / No");
                                answer = Console.ReadLine()?.ToLower();
                                if (answer == "yes")
                                {
                                    Console.WriteLine("Enter the address of the call: ");
                                    existingCall.FullAddress = Console.ReadLine() ?? "";
                                }

                                Console.WriteLine("Do you want to change the EndTime of the call? Yes / No");
                                answer = Console.ReadLine()?.ToLower();
                                if (answer == "yes")
                                {
                                    Console.WriteLine("Enter the EndTime of the call: ");
                                    if (!DateTime.TryParse(Console.ReadLine(), out DateTime endTime))
                                    {
                                        Console.WriteLine("Invalid input for EndTime.");
                                        break;
                                    }
                                    existingCall.MaxEndTime = endTime;
                                }

                                Console.WriteLine("Do you want to change the status of the call? Yes / No");
                                answer = Console.ReadLine()?.ToLower();
                                if (answer == "yes")
                                {
                                    Console.WriteLine("Enter the status of the call (0 = Open, 1 = In Progress, 2 = Completed, 3 = Canceled, 4 = OpenAtRisk, 5 = InProgressAtRisk): ");
                                    if (!int.TryParse(Console.ReadLine(), out int status) || !Enum.IsDefined(typeof(BO.CallStatus), status))
                                    {
                                        Console.WriteLine("Invalid input for Status.");
                                        break;
                                    }
                                    existingCall.Status = (BO.CallStatus)status;
                                }

                                s_bl.Call.UpdateCall(existingCall);
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
                                IEnumerable<CallInList> calls = s_bl.Call.GetCalls(null, null, BO.CallInListFields.CallId);

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
                                BO.OpenCallInListFields sortField = BO.OpenCallInListFields.Id;
                                BO.CallType? callType = null;
                                Console.WriteLine("Enter the Volunteer ID: ");
                                int id = ConvertStringToNumber();

                                Console.WriteLine("Do you want to filter the calls? yes / no: ");
                                string answer = Console.ReadLine()?.ToLower();
                                if (answer == "yes")
                                {
                                    Console.WriteLine(@"
Please select one of the following fields to filter by:
0. NotAllocated
1. MedicalEmergency
2. PatientTransport
3. TrafficAccident
4. FirstAid
5. Rescue
6. FireEmergency
7. CardiacEmergency
8. Poisoning
9. AllergicReaction
10. MassCausalities
11. TerrorAttack
");
                                    if (!Enum.TryParse(Console.ReadLine(), out BO.CallType filter))
                                        throw new BlInvalidOperationException("Invalid input for filter field.");
                                    else
                                        callType = filter;
                                }

                                Console.WriteLine("Do you want to sort the calls? yes / no: ");
                                answer = Console.ReadLine()?.ToLower();
                                if (answer == "yes")
                                {
                                    Console.WriteLine(@"
Please select one of the following fields to sort by:
0. Id
1. Type
2. Description
3. FullAddress
4. OpenTime
5. MaxEndTime
6. DistanceFromVolunteer
");
                                    if (!Enum.TryParse(Console.ReadLine(), out OpenCallInListFields sort))
                                        throw new BlInvalidOperationException("Invalid input for sort field.");
                                    else
                                        sortField = sort;
                                }

                                foreach (OpenCallInList callInList in s_bl.Call.GetOpenCallsForVolunteer(id, callType, sortField))
                                    Console.WriteLine(callInList);
                            }
                            break;
                        case callMenu.DisplayClosedCalls:
                            {
                                Console.WriteLine("Enter the Volunteer ID: ");
                                int id = ConvertStringToNumber();
                                BO.CallType? filterField = null;
                                BO.ClosedCallInListFields? sortField = null;

                                Console.WriteLine("Do you want to filter the calls? yes / no:");
                                string answer = Console.ReadLine()?.ToLower();
                                if (answer == "yes")
                                {
                                    Console.WriteLine(@"
Please select one of the following fields to filter by:
0. NotAllocated
1. MedicalEmergency
2. PatientTransport
3. TrafficAccident
4. FirstAid
5. Rescue
6. FireEmergency
7. CardiacEmergency
8. Poisoning
9. AllergicReaction
10. MassCausalities
11. TerrorAttack
");
                                    if (!Enum.TryParse(Console.ReadLine(), out BO.CallType filter))
                                        throw new BlInvalidOperationException("Invalid input for filter field.");
                                    else
                                        filterField = filter;
                                }

                                Console.WriteLine("Do you want to sort the calls? yes / no:");
                                answer = Console.ReadLine()?.ToLower();
                                if (answer == "yes")
                                {
                                    Console.WriteLine(@"
0. Id
1. Type
2. CallType
3. OpenTime
4. AssignedTime
5. ClosedTime
6. FullAddress
7. Status
");
                                    if (!Enum.TryParse(Console.ReadLine(), out ClosedCallInListFields sort))
                                        throw new BlInvalidOperationException("Invalid input for sort field.");
                                    else
                                        sortField = sort;
                                }

                                foreach (ClosedCallInList callInList in s_bl.Call.GetClosedCallsByVolunteer(id, filterField, sortField))
                                    Console.WriteLine(callInList);
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
                                int typeOfCall = 0;
                                foreach (int value in s_bl.Call.GetCallCountsByStatus())
                                    Console.WriteLine($"Number of {(BO.CallStatus)typeOfCall++}: {value}");
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
            int choice = 0;
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

                    if (!int.TryParse(Console.ReadLine(), out choice))
                    {
                        Console.WriteLine("Invalid selection, please enter a valid number.");
                        continue;
                    }

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
                            s_bl.Admin.ForwardClock(TimeUnit.Year); // Advance Clock by 1 year logic here
                            break;
                        case adminMenu.DisplayClock:
                            DateTime time = s_bl.Admin.GetClock(); // Display Clock logic here
                            Console.WriteLine(time);
                            break;
                        case adminMenu.SetClock:
                            // You can implement the set clock logic here with TryParse if needed
                            break;
                        case adminMenu.DisplayRiskTime:
                            TimeSpan range = s_bl.Admin.GetMaxRange(); // Display Risk Time logic here
                            Console.WriteLine(range);
                            break;
                        case adminMenu.SetRiskTime:
                            int hours = 0, minutes = 0, seconds = 0;
                            Console.WriteLine("Enter hours, minutes, and seconds: ");

                            if (!int.TryParse(Console.ReadLine(), out hours))
                            {
                                Console.WriteLine("Invalid input for hours.");
                                continue;
                            }
                            if (!int.TryParse(Console.ReadLine(), out minutes))
                            {
                                Console.WriteLine("Invalid input for minutes.");
                                continue;
                            }
                            if (!int.TryParse(Console.ReadLine(), out seconds))
                            {
                                Console.WriteLine("Invalid input for seconds.");
                                continue;
                            }

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
                    if (!Enum.IsDefined(typeof(Menu), choice))
                    {
                        Console.WriteLine("Invalid selection, please try again.");
                        continue;
                    }

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

