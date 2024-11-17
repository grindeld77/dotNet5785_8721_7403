namespace DalTest;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Security.AccessControl;
using System;

public static class Initialization
{
    private static ICall? s_dalCall;
    private static IAssignment? s_dalAssignment;
    private static IVolunteer? s_dalVolunteer;
    private static IConfig? s_dalConfig;

    private static readonly Random s_rand = new();

    private static void createVolunteers()
    {
        string[] volunteerNames = { "Daniel Cohen", "Michal Levy", "Yossi Mizrahi", "Sarit Israeli", "Tomer Friedman", "Naama Avidan", "Ron Goldman", "Elinor Bachar", "Moshe Barak", "Tali Katz", "Dina Rozenberg", "Liat Sharon", "Avi Hasson", "Yael Zimmerman", "Omar Rafael", "israel biton" };
        string[] phones = { "052-3456789", "054-8765432", "053-2345678", "050-9876543", "055-8765432", "058-2233445", "052-6677889", "053-9988776", "054-7788990", "050-3344556", "058-4455667", "052-8899001", "055-9900112", "053-0011223", "054-1222333" };
        string[] emails = { "daniel.cohen@gmail.com", "michal.levy@gmail.com", "yossi.mizrahi@gmail.com", "sarit.israeli@gmail.com", "tomer.friedman@gmail.com", "naama.avidan@gmail.com", "ron.goldman@gmail.com", "elinor.bachar@gmail.com", "moshe.barak@gmail.com", "tali.katz@gmail.com", "dina.rozenberg@gmail.com", "liat.sharon@gmail.com", "avi.hasson@gmail.com", "yael.zimmerman@gmail.com", "omar.rafael@gmail.com" };

        for (int i = 0; i < volunteerNames.Length; i++)
        {
            int id = s_rand.Next(200000000, 400000000);
            double distance = s_rand.Next(1, 20);
            while (s_dalVolunteer!.Read(id) != null) ;

            s_dalVolunteer!.Create(new Volunteer
            {
                Id = id,
                FullName = volunteerNames[i],
                MobilePhone = phones[i],
                Email = emails[i],
                MaxCallDistance = distance
            });
        }
    }
}

