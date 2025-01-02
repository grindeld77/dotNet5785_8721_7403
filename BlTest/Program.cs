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
                                Console.WriteLine("Do you want to get the list filtered by the Call Type? Yes / No");
                                string answer = Console.ReadLine() ?? "";
                                if (answer == "Yes" || answer == "yes")
                                {
                                    Console.Write($@" 
Choose a call type to sort by:
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
10. MassCausal
11. TerrorAttack
12. None
");
                                    if (!Enum.TryParse(Console.ReadLine(), out BO.CallType type))
                                        throw new BlInvalidOperationException("Invalid input for call type. Please enter a valid call type.");
                                    IEnumerable<VolunteerInList> volunteers = s_bl.Volunteer.GetVolunteers(input == 1 ? true : input == 0 ? false : null, (VolunteerFieldVolunteerInList)sort, type);
                                    foreach (var item in volunteers) // Display All Volunteers logic here
                                    {
                                        Console.WriteLine(item);
                                    }
                                }
                                else
                                {
                                    IEnumerable<VolunteerInList> v = s_bl.Volunteer.GetVolunteers(input == 1 ? true : input == 0 ? false : null, (VolunteerFieldVolunteerInList)sort, null);
                                    foreach (var item in v) // Display All Volunteers logic here
                                    {
                                        Console.WriteLine(item);
                                    }
                                }
                                break;
                            }
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
                                if (!string.IsNullOrEmpty(roleInput))
                                {
                                    if (roleInput == "0") // Function to check if the user is an admin
                                    {

                                        updatedVolunteer.Role = 0;
                                    }
                                    else if (roleInput == "1")
                                    {
                                        Console.WriteLine("Only an admin can update the role.");
                                        return;
                                    }
                                    else
                                    {
                                        throw new ArgumentException("Invalid role. Please enter a valid role.");
                                    }
                                }
                                //Check if the volunteer exists in the data layer
                                var existingVolunteer = s_bl.Volunteer.GetVolunteerDetails(updatedVolunteer.Id); // Assuming there's a function like this
                                if (existingVolunteer == null)
                                {
                                    throw new ArgumentException("Volunteer with the provided ID does not exist.");
                                }

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
                                BO.CallType callType;
                                string CallAddress;
                                string Description;
                                DateTime EndTime;

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
                                var C = Console.ReadLine();
                                if (!BO.CallType.TryParse(C, out callType))
                                    throw new BO.BlException($"The value is not a valid CallType value");

                                Console.WriteLine("Enter description for your call:");
                                Description = Console.ReadLine() ?? "";

                                Console.WriteLine("Enter the address of the call:");
                                CallAddress = Console.ReadLine() ?? "";

                                Console.WriteLine("What is the deadline for the call:");
                                if (!DateTime.TryParse(Console.ReadLine() ?? "", out EndTime))
                                    throw new BO.BlException($"The value is not a valid CallType value");

                                BO.Call call = new BO.Call()
                                {
                                    Type = callType,
                                    Description = Description,
                                    FullAddress = CallAddress,
                                    OpenTime = s_bl.Admin.GetClock(),
                                    MaxEndTime = EndTime
                                };
                                s_bl.Call.AddCall(call);
                                break;
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
                                BO.CallInListFields? filterField = null;
                                object? filterValue = null;
                                BO.CallInListFields? sortingField = null;
                                Console.WriteLine("Do you want to filter the calls? yes / no:");
                                string answer = Console.ReadLine() ?? "";
                                if (answer == "Yes" || answer == "yes")
                                {
                                    Console.Write(
                        $@"
Please select one of the following fields to filter by:
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
                                    if (!Enum.TryParse(Console.ReadLine(), out CallInListFields filter))
                                        throw new BlInvalidOperationException("Invalid input for filter field. Please enter a valid field.");
                                    else
                                        filterField = filter;
                                    Console.Write($"Enter the value that you are willing the fields of {filterField} to contained: ");
                                    filterValue = Console.ReadLine() ?? "";
                                }
                                Console.WriteLine("Do you want to sort the calls? yes / no:");
                                answer = Console.ReadLine() ?? "";
                                if (answer == "Yes" || answer == "yes")
                                {
                                    Console.Write(
                                        $@"
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
                                throw new ArgumentException("Invalid input for Call ID. Please enter a valid integer.");
                            }

                            BO.Call existingCall = s_bl.Call.GetCallDetails(callId);
                            if (existingCall == null)
                            {
                                throw new ArgumentException($"Call with ID {callId} not found.");
                            }
                                Console.WriteLine("Do you want to change the type of the call: Yes / No");
                                string answer = Console.ReadLine() ?? "";
                                if (answer == "Yes" || answer == "yes")
                                {
                                    Console.WriteLine("Enter the type of call: ");
                                    if (!Enum.TryParse(Console.ReadLine(), out BO.CallType callType))
                                    {
                                        Console.WriteLine("Invalid input for Call Type. Please enter a valid Call Type.");
                                        break;
                                    }
                                    existingCall.Type = callType;
                                }
                                Console.WriteLine("Do you want to change the Description of the call: Yes / No");
                                answer = Console.ReadLine() ?? "";
                                if (answer == "Yes" || answer == "yes")
                                {
                                    Console.WriteLine("Enter the Description of the call: ");
                                    existingCall.Description = Console.ReadLine() ?? "";
                                }
                                Console.WriteLine("Do you want to change the Address of the call: Yes / No");
                                answer = Console.ReadLine() ?? "";
                                if (answer == "Yes" || answer == "yes")
                                {
                                    Console.WriteLine("Enter the Address of the call: ");
                                    existingCall.FullAddress = Console.ReadLine() ?? "";
                                }
                                Console.WriteLine("Do you want to change the Endtime of the call: Yes / No");
                                if (answer == "Yes" || answer == "yes")
                                {
                                    Console.WriteLine("Enter the Endtime of the call: ");
                                    if (!DateTime.TryParse(Console.ReadLine(), out DateTime endTime))
                                    {
                                        Console.WriteLine("Invalid input for Endtime. Please enter a valid Endtime.");
                                        break;
                                    }
                                    existingCall.MaxEndTime = endTime;
                                }
                                Console.Write("Do you want to change the status of the call: Yes / No");
                                answer = Console.ReadLine() ?? "";
                                if (answer == "Yes" || answer == "yes")
                                {
                                    Console.Write("Enter the status of the call (0 = Open, 1 = In Progress, 2 = Completed, 3 = Canceled, 4 = OpenAtRisk, 5 = InProgressAtRisk): ");
                                    if (!int.TryParse(Console.ReadLine(), out int status) || !Enum.IsDefined(typeof(BO.CallStatus), status))
                                    {
                                        Console.WriteLine("Invalid input for Status. Please select a valid status.");
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
                                BO.OpenCallInListFields sortField = BO.OpenCallInListFields.Id;
                                BO.CallType? callType = null;
                                Console.WriteLine("Enter the Volunteer's Id: ");
                                int id = ConvertStringToNumber();
                                Console.WriteLine("Do you want to filter the calls? yes / no: ");
                                string answer = Console.ReadLine() ?? "";
                                if (answer == "Yes" || answer =="yes")
                                {
                                    Console.Write($@"
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
                                        throw new BlInvalidOperationException($"Bl: The value is not a valid CallInListField value");
                                    else
                                        callType = filter;
                                }
                                Console.WriteLine("Do you want to sort the calls? yes / no: ");
                                answer = Console.ReadLine() ?? "";
                                if (answer == "Yes" || answer == "yes")
                                {
                                    Console.Write($@"
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
                                        throw new BlInvalidOperationException($"Bl: The value is not a valid CallInListField value");
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
                                string answer = Console.ReadLine() ?? "";
                                if (answer == "Yes" || answer == "yes")
                                {
                                    Console.Write($@"
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
                                        throw new BlInvalidOperationException($"Bl: The value is not a valid CallInListField value");
                                    else
                                        filterField = filter;
                                }
                                Console.WriteLine("Do you want to sort the calls? yes / no:");
                                answer = Console.ReadLine() ?? "";
                                if (answer == "Yes" || answer == "yes")
                                {
                                    Console.Write($@"
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
                                        throw new BlInvalidOperationException($"Bl: The value is not a valid CallInListField value");
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

