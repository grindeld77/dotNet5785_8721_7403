using BO;
using DalApi;
using DalTest;
using DO;
using System.Collections.Specialized;
using Helpers;
using System.Runtime.CompilerServices;
using System.Diagnostics.Metrics;
using System.Transactions;

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
            Admin,
            Initialize,
            DisplayAll,
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
0. Exit main menu.
1. Display submenu for Volunteer entity. 
2. Display submenu for Call entity.
3. Display submenu for Admin entity. 
4. Initialize data. 
5. Display all data in the database.
6. Reset database and configuration data.");
            return ConvertStringToNumber();
        }
        /*ToDo:*/
        public static void VolunteerMenu()
        {
            int choice;
            do
            {
                Console.WriteLine(
        @"    Select an option to proceed:
0. Exit
1. Add Volunteer
2. Display Volunteer
3. Display All Volunteers
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
                    case volunteerMenu.DisplayAll:
                        {
                            foreach (var item in s_bl.Volunteer.GetVolunteers(null, null)) // Display All Volunteers logic here
                            {
                                Console.WriteLine(item);
                            }
                        }
                        break;
                    case volunteerMenu.Update:
                        {
                            // Request for the ID  of the volunteer to be updated
                            Console.WriteLine("Enter the ID of the volunteer to update:");
                            int id = ConvertStringToNumber(); // Volunteer ID

                            // Request for the new volunteer object with all the updated values
                            BO.Volunteer updatedVolunteer = new BO.Volunteer();

                            Console.WriteLine("Enter full name (leave empty to keep unchanged):");
                            updatedVolunteer.FullName = Console.ReadLine();

                            Console.WriteLine("Enter phone number (leave empty to keep unchanged):");
                            updatedVolunteer.PhoneNumber = Console.ReadLine();

                            Console.WriteLine("Enter email (leave empty to keep unchanged):");
                            updatedVolunteer.Email = Console.ReadLine();

                            Console.WriteLine("Enter full address (leave empty to keep unchanged):");
                            updatedVolunteer.FullAddress = Console.ReadLine();

                            //// If the volunteer updated the address, check the latitude and longitude
                            //if (!string.IsNullOrEmpty(updatedVolunteer.FullAddress))
                            //{
                            //    updatedVolunteer.Latitude = latitude;
                            //    updatedVolunteer.Longitude = longitude;
                            //}

                            // Request for role (if there is a role field - only an admin can update it)
                            Console.WriteLine("Enter role (Admin = 0, Volunteer = 1) if updating role, leave empty to keep unchanged:");
                            string roleInput = Console.ReadLine();
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
                            // Check if the volunteer exists in the data layer
                            var existingVolunteer = s_bl.Volunteer.GetVolunteerDetails(id); // Assuming there's a function like this
                            if (existingVolunteer == null)
                            {
                                throw new ArgumentException("Volunteer with the provided ID does not exist.");
                            }

                            // Perform the update in the data layer
                            s_bl.Volunteer.UpdateVolunteer(id, updatedVolunteer);
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
                            string username = Console.ReadLine();

                            Console.WriteLine("Enter password:");
                            string password = Console.ReadLine();

                            Console.WriteLine(s_bl.Volunteer.Login(username, password));
                        }
                        break;
                    default:
                        throw new ArgumentException("Invalid selection, please try again.");
                }
            } while (choice != 0);
        }
        public static void CallMenu()
        {
            int choice;
            do
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
                            foreach (var item in s_bl.Call.GetCalls(null, null, null)) // Display All Calls logic here
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
                            IEnumerable<CallInList> calls = s_bl.Call.GetCalls(BO.CallStatus.Open, null, null);

                            // If no calls to delete, throw an exception
                            if (!calls.Any())
                            {
                                throw new ArgumentException("There are no open and unassigned calls to delete.");
                            }

                            // Delete each call
                            foreach (var item in calls)
                            {
                                s_bl.Call.DeleteCall((int)item.Id);
                            }
                        }
                        break;
                    case callMenu.DisplayOpenCalls:
                        {
                            foreach (var item in s_bl.Call.GetCalls(BO.CallStatus.Open, null, null)) // Display Open Calls logic here
                            {
                                Console.WriteLine(item);
                            }
                        }
                        break;
                    case callMenu.DisplayClosedCalls:
                        {
                            foreach (var item in s_bl.Call.GetCalls(BO.CallStatus.Closed, null, null)) // Display Closed Calls logic here
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
                            s_bl.Call.AssignVolunteerToCall(volunteerId,callId);
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

            } while (choice != 0);
        }
        public static void AdminMenu()
        {
            int choice;
            do
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
                        case Menu.Initialize:
                            Initialization.Do(); //stage 4
                            break;
                        case Menu.DisplayAll:
                            Console.WriteLine("All data in the database:\n");
                            Console.WriteLine("Volunteer List:\n");
                            var volunteers = s_bl.Volunteer.GetVolunteers(null, null);

                            if (volunteers != null && volunteers.Any())
                            {
                                foreach (var volunteer in volunteers)
                                {
                                    Console.WriteLine(volunteer?.ToString() ?? "Volunteer data is null");
                                }
                            }
                            else
                            {
                                Console.WriteLine("No volunteers found.");
                            }
                            Console.WriteLine("call List:\n");
                            var calls = s_bl.Call.GetCalls(null,null, null);

                            if (calls != null && calls.Any())
                            {
                                foreach (var call in calls)
                                {
                                    Console.WriteLine(call?.ToString() ?? "Call data is null");
                                }
                            }
                            else
                            {
                                Console.WriteLine("No volunteers found.");
                            }
                            break;
                        case Menu.Reset:
                            s_bl.Admin.ResetDB();
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



