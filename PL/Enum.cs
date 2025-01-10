using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL;

public class VolunteerCollection : IEnumerable
{
    static readonly IEnumerable<BO.CallType> s_enums = (Enum.GetValues(typeof(BO.CallType)) as IEnumerable<BO.CallType>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

public class OpenCallCollection : IEnumerable
{
    static readonly IEnumerable<BO.OpenCallInListFields> s_enums = (Enum.GetValues(typeof(BO.OpenCallInListFields)) as IEnumerable<BO.OpenCallInListFields>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

public class OpenCallCollectionType : IEnumerable
{
    static readonly IEnumerable<BO.CallType> s_enums = (Enum.GetValues(typeof(BO.CallType)) as IEnumerable<BO.CallType>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

public class CallCollectionFilter : IEnumerable
{
    static readonly IEnumerable<BO.CallStatus> s_enums = (Enum.GetValues(typeof(BO.CallStatus)) as IEnumerable<BO.CallStatus>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

public class CallCollectionSort : IEnumerable
{
    static readonly IEnumerable<BO.CallInListFields> s_enums = (Enum.GetValues(typeof(BO.CallInListFields)) as IEnumerable<BO.CallInListFields>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
