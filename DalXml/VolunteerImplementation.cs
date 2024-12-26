namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

internal class VolunteerImplementation : IVolunteer
{
    static Volunteer getVolunteer(XElement vol)
    {
        return new DO.Volunteer()
        {
            Id = vol.ToIntNullable("Id") ?? 0,
            FullName = (string?)vol.Element("Name") ?? "",
            MobilePhone = (string?)vol.Element("MobilePhone") ?? "",
            Email = (string?)vol.Element("Email") ?? "",
            Role = vol.ToEnumNullable<Role>("Role") ?? Role.Volunteer,
            IsActive = (bool?)vol.Element("IsActive") ?? false,
            Password = (string?)vol.Element("Password") ?? null,
            CurrentAddress = (string?)vol.Element("CurrentAddress") ?? null,
            Latitude = vol.ToDoubleNullable("Latitude"),
            Longitude = vol.ToDoubleNullable("Longitude"),
            MaxCallDistance = vol.ToDoubleNullable("MaxCallDistance"),
            DistancePreference = vol.ToEnumNullable<DistanceType>("DistancePreference") ?? DistanceType.Aerial
        };
    }
    public XElement createVolunteerElement(Volunteer item)
    {
            return
            new XElement("Volunteer",
            new XElement("Id", item.Id),
            new XElement("Name", item.FullName),
            new XElement("MobilePhone", item.MobilePhone),
            new XElement("Email", item.Email),
            new XElement("Role", item.Role),
            new XElement("IsActive", item.IsActive),
            new XElement("Password", item.Password),
            new XElement("CurrentAddress", item.CurrentAddress),
            new XElement("Latitude", item.Latitude),
            new XElement("Longitude", item.Longitude),
            new XElement("MaxCallDistance", item.MaxCallDistance),
            new XElement("DistancePreference", item.DistancePreference)
        );
    }

    public void Create(Volunteer item)
    {
        XElement volunteerRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        if (volunteerRootElem.Elements().Any(vol => (int?)vol.Element("Id") == item.Id))
            throw new DO.DalAlreadyExistsException($"Volunteer with ID={item.Id} already exists");

        volunteerRootElem.Add(createVolunteerElement(item));

        XMLTools.SaveListToXMLElement(volunteerRootElem, Config.s_volunteers_xml);
    }

    public void Delete(int id)
    {
        XElement volunteerRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        XElement? volunteerToDelete = volunteerRootElem.Elements().FirstOrDefault(vol => (int?)vol.Element("Id") == id);

        if (volunteerToDelete is null)
        {
            throw new DO.DalDoesNotExistException($"Volunteer with ID={id} does Not exist");
        }

        volunteerToDelete.Remove();
        XMLTools.SaveListToXMLElement(volunteerRootElem, Config.s_volunteers_xml);
    }

    public void DeleteAll()
    {
        XElement volunteersRootElem = new XElement("Volunteers");  // Create an empty root element
        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteers_xml);
    }

    public Volunteer? Read(int id)
    {
        XElement? volunteerElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().FirstOrDefault(vol => (int?)vol.Element("Id") == id);
        return volunteerElem is null ? null : getVolunteer(volunteerElem);
    }

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().Select(vol => getVolunteer(vol)).FirstOrDefault(filter);
    }

    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        XElement volunteerRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        IEnumerable<Volunteer> volunteers = volunteerRootElem.Elements()
            .Select(vol => getVolunteer(vol));

        if (filter != null)
        {
            volunteers = volunteers.Where(filter);
        }

        return volunteers;
    }

    public void Update(Volunteer item)
    {
        XElement volunteerRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        (volunteerRootElem.Elements().FirstOrDefault(vol => (int?)vol.Element("Id") == item.Id) ??
            throw new DO.DalDoesNotExistException($"Volunteer with ID={item.Id} does Not exist")).Remove();

        volunteerRootElem.Add(createVolunteerElement(item));
        XMLTools.SaveListToXMLElement(volunteerRootElem, Config.s_volunteers_xml);
    }
}
