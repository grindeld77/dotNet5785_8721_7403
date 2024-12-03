namespace DalTest;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Security.AccessControl;
using System;
using System.Collections;
using Dal;

public static class Initialization
{
    private static readonly Random s_rand = new();
    private static IDal? s_dal;
    private static void createVolunteers()
    {
        // for those information i used in AI to get logical data for the volunteers (the address, the phone number, the email, the name)
        string[] volunteerNames =
            {
            "Daniel Cohen",
            "Michal Levy",
            "Yossi Mizrahi",
            "Sarit Israeli",
            "Tomer Friedman",
            "Naama Avidan",
            "Ron Goldman",
            "Elinor Bachar",
            "Moshe Barak",
            "Tali Katz",
            "Dina Rozenberg",
            "Liat Sharon",
            "Avi Hasson",
            "Yael Zimmerman",
            "Omar Rafael" };
        string[] volunteerAddresses =
{
    "Dizengoff St 50, Tel Aviv",
    "Jaffa St 1, Jerusalem",
    "Rothschild Blvd 16, Tel Aviv",
    "Haatzmaut St 85, Haifa",
    "Herzl St 4, Beersheba",
    "Hanasi St 120, Ashdod",
    "Chen Blvd 3, Ra'anana",
    "Hacarmel St 5, Afula",
    "Degania Blvd 50, Holon",
    "Ben Gurion St 35, Bat Yam",
    "Herzl St 40, Kfar Saba",
    "Gordon St 8, Netanya",
    "Yehudit Blvd 10, Tel Aviv",
    "Hayam St 2, Eilat",
    "Horev St 17, Haifa"
};
        double[] volunteerLatitudes =
        {
    32.0764, // Dizengoff St 50, Tel Aviv
    31.7823, // Jaffa St 1, Jerusalem
    32.0657, // Rothschild Blvd 16, Tel Aviv
    32.8191, // Haatzmaut St 85, Haifa
    31.2526, // Herzl St 4, Beersheba
    31.7926, // Hanasi St 120, Ashdod
    32.1847, // Chen Blvd 3, Ra'anana
    32.6072, // Hacarmel St 5, Afula
    32.0094, // Degania Blvd 50, Holon
    32.0233, // Ben Gurion St 35, Bat Yam
    32.1753, // Herzl St 40, Kfar Saba
    32.3328, // Gordon St 8, Netanya
    32.0625, // Yehudit Blvd 10, Tel Aviv
    29.5535, // Hayam St 2, Eilat
    32.7811  // Horev St 17, Haifa
};
        double[] volunteerLongitudes =
        {
    34.7745, // Dizengoff St 50, Tel Aviv
    35.2191, // Jaffa St 1, Jerusalem
    34.7741, // Rothschild Blvd 16, Tel Aviv
    34.9972, // Haatzmaut St 85, Haifa
    34.7925, // Herzl St 4, Beersheba
    34.6496, // Hanasi St 120, Ashdod
    34.8709, // Chen Blvd 3, Ra'anana
    35.2903, // Hacarmel St 5, Afula
    34.7766, // Degania Blvd 50, Holon
    34.7503, // Ben Gurion St 35, Bat Yam
    34.9067, // Herzl St 40, Kfar Saba
    34.8556, // Gordon St 8, Netanya
    34.7942, // Yehudit Blvd 10, Tel Aviv
    34.9519, // Hayam St 2, Eilat
    35.0105  // Horev St 17, Haifa
};
        for (int i = 0; i < 15; i++)
        {
            int id;
            double distance = s_rand.Next(1, 20);
            do
            {
                id = s_rand.Next(200000000, 400000000);
            }
            while (s_dal!.Volunteer.Read(id) != null);

            string phone = "05" + s_rand.Next(10000000, 99999999).ToString();
            string email = volunteerNames[i].Replace(" ", ".").ToLower() + "@gmail.com";
            string password = s_rand.Next(1000000, 999999999).ToString();

            Volunteer volunteerToAdd = new Volunteer(id, volunteerNames[i], phone, email, Role.Volunteer, true, password, volunteerAddresses[i], volunteerLatitudes[i], volunteerLongitudes[i], distance);

            s_dal.Volunteer.Create(volunteerToAdd);
        }
    }
    private static void createCalls()
    {
        string[] callAddresses =
        {
    "Weizmann St 2, Rehovot",
    "Eli Cohen St 18, Beersheba",
    "Allenby St 35, Tel Aviv",
    "Hashalom St 12, Herzliya",
    "Emek Refaim St 34, Jerusalem",
    "Balfour St 45, Bat Yam",
    "HaGalil St 90, Tiberias",
    "Sokolov St 50, Ramat Gan",
    "Hagalim Blvd 25, Caesarea",
    "Palmach St 10, Nahariya",
    "Ben Yehuda St 28, Tel Aviv",
    "Ayalon St 6, Modi'in",
    "David Ben Gurion Blvd 3, Ashkelon",
    "Hechalutz St 8, Kiryat Shmona",
    "Haifa St 120, Karmiel",
    "Shenkin St 4, Tel Aviv",
    "Ben Ami St 14, Acre",
    "Herzog St 22, Holon",
    "Almog St 7, Eilat",
    "Kibbutz Galuyot St 32, Haifa",
    "Hertzl St 15, Rishon LeZion",
    "Mishol Hagefen 9, Givatayim",
    "Mevo HaShaked 12, Kfar Saba",
    "Neve Zohar 1, Dead Sea",
    "Yitzhak Rabin Blvd 50, Petah Tikva",
    "Hanasi St 100, Zichron Yaakov",
    "Meir Dizengoff St 88, Tel Aviv",
    "Eilat St 10, Hadera",
    "Gordon St 3, Rishon LeZion",
    "King David St 20, Jerusalem",
    "Tchernichovsky St 4, Netanya",
    "Arlozorov St 56, Tel Aviv",
    "HaNassi St 14, Nesher",
    "Hatsedef St 12, Herzliya",
    "Shlomo Hamelech St 5, Ramat HaSharon",
    "Rabin Blvd 23, Be'er Yaakov",
    "Ha'Aliya St 70, Lod",
    "Menachem Begin Blvd 35, Ashdod",
    "The German Colony, Haifa",
    "HaYarkon St 105, Tel Aviv",
    "Hapoel St 13, Afula",
    "Lev Ha'ir 9, Sderot",
    "Neve Tzedek, Tel Aviv",
    "Hertzl St 98, Hadera",
    "Yigal Alon Blvd 76, Tel Aviv",
    "Einstein St 40, Haifa",
    "Mivtza Sinai St 5, Eilat",
    "HaZait St 30, Kiryat Malakhi",
    "Begin Blvd 12, Ashkelon",
    "HaRav Kook St 25, Safed",
    "Gush Etzion Blvd, Ma'ale Adumim"
};
        double[] callLatitudes =
        {
    31.8938,  // Weizmann St 2, Rehovot
    31.2417,  // Eli Cohen St 18, Beersheba
    32.0673,  // Allenby St 35, Tel Aviv
    32.1676,  // Hashalom St 12, Herzliya
    31.7614,  // Emek Refaim St 34, Jerusalem
    32.0212,  // Balfour St 45, Bat Yam
    32.7889,  // HaGalil St 90, Tiberias
    32.0836,  // Sokolov St 50, Ramat Gan
    32.5115,  // Hagalim Blvd 25, Caesarea
    33.0081,  // Palmach St 10, Nahariya
    32.0795,  // Ben Yehuda St 28, Tel Aviv
    31.9015,  // Ayalon St 6, Modi'in
    31.6648,  // David Ben Gurion Blvd 3, Ashkelon
    33.2085,  // Hechalutz St 8, Kiryat Shmona
    32.9187,  // Haifa St 120, Karmiel
    32.0683,  // Shenkin St 4, Tel Aviv
    32.9249,  // Ben Ami St 14, Acre
    32.0135,  // Herzog St 22, Holon
    29.5581,  // Almog St 7, Eilat
    32.8197,  // Kibbutz Galuyot St 32, Haifa
    31.9645,  // Hertzl St 15, Rishon LeZion
    32.0705,  // Mishol Hagefen 9, Givatayim
    32.1748,  // Mevo HaShaked 12, Kfar Saba
    31.1357,  // Neve Zohar 1, Dead Sea
    32.0973,  // Yitzhak Rabin Blvd 50, Petah Tikva
    32.5734,  // Hanasi St 100, Zichron Yaakov
    32.0753,  // Meir Dizengoff St 88, Tel Aviv
    32.4396,  // Eilat St 10, Hadera
    31.9743,  // Gordon St 3, Rishon LeZion
    31.7767,  // King David St 20, Jerusalem
    32.3274,  // Tchernichovsky St 4, Netanya
    32.0847,  // Arlozorov St 56, Tel Aviv
    32.7441,  // HaNassi St 14, Nesher
    32.1591,  // Hatsedef St 12, Herzliya
    32.1457,  // Shlomo Hamelech St 5, Ramat HaSharon
    31.9353,  // Rabin Blvd 23, Be'er Yaakov
    31.9517,  // Ha'Aliya St 70, Lod
    31.8017,  // Menachem Begin Blvd 35, Ashdod
    32.8184,  // The German Colony, Haifa
    32.0892,  // HaYarkon St 105, Tel Aviv
    32.6123,  // Hapoel St 13, Afula
    31.5183,  // Lev Ha'ir 9, Sderot
    32.0615,  // Neve Tzedek, Tel Aviv
    32.4383,  // Hertzl St 98, Hadera
    32.0622,  // Yigal Alon Blvd 76, Tel Aviv
    32.7755,  // Einstein St 40, Haifa
    29.5534,  // Mivtza Sinai St 5, Eilat
    31.7313,  // HaZait St 30, Kiryat Malakhi
    31.6852,  // Begin Blvd 12, Ashkelon
    32.9653,  // HaRav Kook St 25, Safed
    31.7706   // Gush Etzion Blvd, Ma'ale Adumim
};
        double[] callLongitudes =
        {
    34.8113,  // Weizmann St 2, Rehovot
    34.7976,  // Eli Cohen St 18, Beersheba
    34.7702,  // Allenby St 35, Tel Aviv
    34.8346,  // Hashalom St 12, Herzliya
    35.2219,  // Emek Refaim St 34, Jerusalem
    34.7515,  // Balfour St 45, Bat Yam
    35.5373,  // HaGalil St 90, Tiberias
    34.8248,  // Sokolov St 50, Ramat Gan
    34.9039,  // Hagalim Blvd 25, Caesarea
    35.0952,  // Palmach St 10, Nahariya
    34.7689,  // Ben Yehuda St 28, Tel Aviv
    35.0051,  // Ayalon St 6, Modi'in
    34.5784,  // David Ben Gurion Blvd 3, Ashkelon
    35.5721,  // Hechalutz St 8, Kiryat Shmona
    35.2938,  // Haifa St 120, Karmiel
    34.7735,  // Shenkin St 4, Tel Aviv
    35.0723,  // Ben Ami St 14, Acre
    34.7713,  // Herzog St 22, Holon
    34.9529,  // Almog St 7, Eilat
    34.9983,  // Kibbutz Galuyot St 32, Haifa
    34.8006,  // Hertzl St 15, Rishon LeZion
    34.8071,  // Mishol Hagefen 9, Givatayim
    34.9031,  // Mevo HaShaked 12, Kfar Saba
    35.3671,  // Neve Zohar 1, Dead Sea
    34.8774,  // Yitzhak Rabin Blvd 50, Petah Tikva
    35.0067,  // Hanasi St 100, Zichron Yaakov
    34.7737,  // Meir Dizengoff St 88, Tel Aviv
    34.9203,  // Eilat St 10, Hadera
    34.7979,  // Gordon St 3, Rishon LeZion
    35.2273,  // King David St 20, Jerusalem
    34.8498,  // Tchernichovsky St 4, Netanya
    34.7829,  // Arlozorov St 56, Tel Aviv
    35.0453,  // HaNassi St 14, Nesher
    34.8417,  // Hatsedef St 12, Herzliya
    34.8357,  // Shlomo Hamelech St 5, Ramat HaSharon
    34.8345,  // Rabin Blvd 23, Be'er Yaakov
    34.8943,  // Ha'Aliya St 70, Lod
    34.6672,  // Menachem Begin Blvd 35, Ashdod
    35.0047,  // The German Colony, Haifa
    34.7712,  // HaYarkon St 105, Tel Aviv
    35.2902,  // Hapoel St 13, Afula
    34.6004,  // Lev Ha'ir 9, Sderot
    34.7711,  // Neve Tzedek, Tel Aviv
    34.9202,  // Hertzl St 98, Hadera
    34.7889,  // Yigal Alon Blvd 76, Tel Aviv
    35.0223,  // Einstein St 40, Haifa
    34.9523,  // Mivtza Sinai St 5, Eilat
    34.7467,  // HaZait St 30, Kiryat Malakhi
    34.5789,  // Begin Blvd 12, Ashkelon
    35.4972,  // HaRav Kook St 25, Safed
    35.0142   // Gush Etzion Blvd, Ma'ale Adumim
};
        CallType[] callTypes =
{
    CallType.MedicalEmergency,
    CallType.PatientTransport,
    CallType.TrafficAccident,
    CallType.FirstAid,
    CallType.Rescue,
    CallType.FireEmergency,
    CallType.CardiacEmergency,
    CallType.Poisoning,
    CallType.AllergicReaction,
    CallType.MassCausalities,
    CallType.TerrorAttack,
    CallType.MedicalEmergency,
    CallType.PatientTransport,
    CallType.TrafficAccident,
    CallType.FirstAid,
    CallType.Rescue,
    CallType.FireEmergency,
    CallType.CardiacEmergency,
    CallType.Poisoning,
    CallType.AllergicReaction,
    CallType.MassCausalities,
    CallType.TerrorAttack,
    CallType.MedicalEmergency,
    CallType.PatientTransport,
    CallType.TrafficAccident,
    CallType.FirstAid,
    CallType.Rescue,
    CallType.FireEmergency,
    CallType.CardiacEmergency,
    CallType.Poisoning,
    CallType.AllergicReaction,
    CallType.MassCausalities,
    CallType.TerrorAttack,
    CallType.MedicalEmergency,
    CallType.PatientTransport,
    CallType.TrafficAccident,
    CallType.FirstAid,
    CallType.Rescue,
    CallType.FireEmergency,
    CallType.CardiacEmergency,
    CallType.Poisoning,
    CallType.AllergicReaction,
    CallType.MassCausalities,
    CallType.TerrorAttack,
    CallType.MedicalEmergency,
    CallType.PatientTransport,
    CallType.TrafficAccident,
    CallType.FirstAid,
    CallType.Rescue,
    CallType.TerrorAttack,
    CallType.TrafficAccident
};
        string[] callDescriptions =
        {
    "Heart attack reported.",
    "Elderly transport to hospital.",
    "Car accident with multiple injuries.",
    "First aid for an injured person.",
    "Rescue operation for a trapped individual.",
    "Fire in a residential building.",
    "Cardiac arrest requiring immediate attention.",
    "Chemical poisoning incident.",
    "Severe allergic reaction to food or medication.",
    "Explosion causing multiple casualties.",
    "Terror attack in a crowded area.",
    "Severe dehydration reported.",
    "Routine patient transport.",
    "Multi-vehicle collision on a highway.",
    "Burn treatment after an accident.",
    "Rescue from a collapsed structure.",
    "Fire at an industrial site.",
    "CPR required for a collapsed individual.",
    "Worker exposed to hazardous materials.",
    "Anaphylactic shock due to insect sting.",
    "Mass casualty event on a busy road.",
    "Armed assault causing injuries.",
    "Medical distress requiring urgent response.",
    "Rescue operation in a dangerous environment.",
    "Transport to a specialized medical facility.",
    "Structural collapse requiring emergency response.",
    "Medical emergency during a public event.",
    "Traffic accident with critical injuries.",
    "First aid for minor injuries at a sports event.",
    "Emergency response for a cardiac issue.",
    "Chemical exposure causing severe symptoms.",
    "Injuries from a large-scale accident.",
    "Fire emergency requiring evacuation.",
    "Burn injuries from a domestic incident.",
    "Severe trauma in a public area.",
    "Flood rescue operation.",
    "Evacuation due to a hazardous situation.",
    "Massive fire requiring immediate response.",
    "Heart attack in a crowded place.",
    "Poisoning from accidental ingestion.",
    "Severe swelling and difficulty breathing.",
    "Casualties reported after a public incident.",
    "Explosion-related injuries.",
    "Medical distress requiring urgent transport.",
    "Train collision causing significant injuries.",
    "Rescue operation in a natural disaster.",
    "Fire-related injuries in an industrial zone.",
    "Toxic chemical exposure in a workplace.",
    "Injury during a rescue mission.",
    "shooting terror attack",
    "car crash on highway"
};

        for (int i = 0; i < 45; i++)
        {
            int id = 0; //will be generated by the create method in CallImplementation
            DateTime TempTime = s_dal!.Config.Clock.AddYears(-s_rand.Next(1, 10)); //time initialized to system time minus random days
            DateTime Open = TempTime.AddMinutes(-s_rand.Next(1, 60)); //open time is random minutes before current time
            DateTime Max = TempTime.AddMinutes(s_rand.Next(10, 50)); //max time is random minutes after current time (only a prototype)
            Call callToAdd = new Call(id, callTypes[i], callDescriptions[i], callAddresses[i], callLatitudes[i], callLongitudes[i], Open, Max);
            if (callToAdd != null)
            {
                s_dal!.Call.Create(callToAdd);
            }
        }
        for (int i = 45; i < 50; i++) //5 that expird 
        {
            DateTime tempTime = s_dal!.Config.Clock.AddDays(-s_rand.Next(1, 10)); //time initialized to system time minus random days
            int id = 0; //will be generated by the create method in CallImplementation
            DateTime Open = tempTime.AddMinutes(-s_rand.Next(60, 120)); //open time is random minutes significantly before current time
            DateTime Max = tempTime.AddMinutes(s_rand.Next(10, 20)); //max time is random minutes significantly after current time so it will be expired
            Call callToAdd = new Call(id, callTypes[i], callDescriptions[i], callAddresses[i], callLatitudes[i], callLongitudes[i], Open, Max);

            if (callToAdd != null)
            {
                s_dal!.Call.Create(callToAdd);
            }
        }
        for (int i = 0; i < 15; i++) //15 not allcoated call
        {
            int id = 0; //will be generated by the create method in CallImplementation
            DateTime TempTime = s_dal!.Config.Clock.AddDays(-s_rand.Next(1, 10)); //time initialized to system time minus random days
            Call callToAdd = new Call(id, 0, null, "", 0, 0, TempTime, null);
            if (callToAdd != null)
            {
                s_dal!.Call.Create(callToAdd);
            }
        }
    }
    private static void createAssignments()
    {
        IEnumerable<Volunteer> volunteersList = s_dal!.Volunteer.ReadAll();
        IEnumerable<Call> callsList = s_dal!.Call.ReadAll();

        DateTime currentTime = DateTime.Now; // Get the current system time

        foreach (var call in callsList) // Iterate over all calls
        {
            int id = call.Id;


            int index = s_rand.Next(0, volunteersList.Count()); // Select a random volunteer from the list
            Volunteer selectedVolunteer = volunteersList.Skip(index).Take(1).FirstOrDefault();

            DateTime StartTime = call.OpenedAt.AddMinutes(s_rand.Next(1, 36));
            DateTime? EndTime = s_dal!.Config.Clock;
            CompletionStatus status;

            switch (s_rand.Next(0, 3)) // Assign different statuses in a randomized order
            {
                case 0: // Call in treatment
                    status = CompletionStatus.Handled;
                    break;
                case 1: // Self-cancellation
                    status = CompletionStatus.SelfCancel;
                    EndTime = call.MaxCompletionTime;
                    break;
                case 2: // Completed by manager
                    status = CompletionStatus.AdminCancel;
                    EndTime = StartTime.AddHours(s_rand.Next(1, 24));
                    if (call.MaxCompletionTime.HasValue)
                        if (call.MaxCompletionTime.Value < EndTime)
                            EndTime = call.MaxCompletionTime.Value.AddHours(-1); // Ensure within valid range
                    break;
                default: // Expired call, no treatment
                    status = CompletionStatus.Expired;

                    if (call.MaxCompletionTime.HasValue)
                        EndTime = call.MaxCompletionTime.Value.AddHours(-s_rand.Next(1, 36)); // Ensure end time is before MaxCompletionTime
                    else
                        EndTime = currentTime.AddHours(-s_rand.Next(1, 36)); // Ensure end time is in the past

                    if (EndTime >= currentTime)
                        EndTime = currentTime.AddHours(-s_rand.Next(1, 36)); // Force it into the past
                    break;
            }

            Assignment assignmentToAdd = new Assignment(id, call.Id, selectedVolunteer.Id, StartTime, EndTime, status);
            s_dal!.Assignment.Create(assignmentToAdd);
        }
    }
    public static void Do(IDal dal) //stage 1
    {
        s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!");
        s_dal.ResetDB();
        Console.WriteLine("Reset Configuration values and List values...");
        createVolunteers();
        createCalls();
       createAssignments();
    }
}