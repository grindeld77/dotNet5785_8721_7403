using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL;
internal class VolunteerCollection : IEnumerable
{
    static readonly IEnumerable<BO.CallType> s_enums = (Enum.GetValues(typeof(BO.CallType)) as IEnumerable<BO.CallType>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class OpenCallCollection : IEnumerable
{
    static readonly IEnumerable<BO.OpenCallInListFields> s_enums = (Enum.GetValues(typeof(BO.OpenCallInListFields)) as IEnumerable<BO.OpenCallInListFields>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
internal class OpenCallCollectionType : IEnumerable
{
    static readonly IEnumerable<BO.CallType> s_enums = (Enum.GetValues(typeof(BO.CallType)) as IEnumerable<BO.CallType>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
