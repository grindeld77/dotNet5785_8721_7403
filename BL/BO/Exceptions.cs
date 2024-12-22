using System;

namespace BO;

[Serializable]
public class BlException : Exception//General error
{
    public BlException(string? message) : base(message) { }
}

[Serializable]
public class BloesNotExistException : BlException//Entity not found
{
    public BloesNotExistException(string? message) : base(message) { }
}

[Serializable]
public class BlAlreadyExistsException : BlException//Entity already exists
{
    public BlAlreadyExistsException(string? message) : base(message) { }
}

[Serializable]
public class BlXMLFileLoadCreateException : BlException//Entity already exists
{
    public BlXMLFileLoadCreateException(string? message) : base(message) { }
}

[Serializable]
public class BlInvalidIdentificationException : BlException //Invalid identification
{
    public BlInvalidIdentificationException(string? message) : base(message) { }
}
[Serializable]
public class BlInvalidAddressException : BlException//Invalid address
{
    public BlInvalidAddressException(string? message) : base(message) { }
}
[Serializable]
public class BlPoneNomber : BlException//Invalid address
{
    public BlPoneNomber(string? message) : base(message) { }
}
[Serializable]
public class BlInvalidTimeException : BlException//Invalid time
{
    public BlInvalidTimeException(string? message) : base(message) { }
}
[Serializable]
public class BlInvalidIdentityNumberException : BlException//Invalid identity number
{
    public BlInvalidIdentityNumberException(string message, string v) : base(message) { }
}
[Serializable]
public class BlInvalidCallIdException : BlException//Invalid call ID
{
    public BlInvalidCallIdException(string message, string v) : base(message) { }
}



    [Serializable]
public class BlInvalidOperationException : BlException//Invalid operation
{
    public BlInvalidOperationException(string? message) : base(message) { }
}

public class BlInvalidAssignmentIdException : BlException//Invalid assignment ID
{
    public BlInvalidAssignmentIdException(string message, string v) : base(message) { }
}
[Serializable]
public class BlInvalidRequestException : BlException//Invalid request
{
    public BlInvalidRequestException(string message) : base(message) { }
}
[Serializable]
public class BlRequestFailedException : BlException//Request failed
{
    public BlRequestFailedException(string message) : base(message) { }
}

[Serializable]
public class BlGeneralException : BlException//General exception
{
    public object Data { get; set; }

    public BlGeneralException(string message, object data) : base(message)
    {
        Data = data;
    }
}

[Serializable]
public class InvalidEmailException : BlException
{
    public InvalidEmailException(string message) : base(message) { }
}

///BlException: שגיאה כללית
//BloesNotExistException: ישות לא נמצאה
//BlAlreadyExistsException: ישות כבר קיימת
//BlXMLFileLoadCreateException: שגיאה בעת טעינה/יצירה של קובץ XML
//BlInvalidIdentificationException: זיהוי לא תקין
//BlInvalidAddressException: כתובת לא תקינה
//BlInvalidTimeException: זמן לא תקין
//BlInvalidIdentityNumberException: תעודת זהות לא תקינה
//BlInvalidCallIdException: מזהה שיחה לא תקין
//BlInvalidOperationException: פעולה לא חוקית
//BlInvalidAssignmentIdException: מזהה הקצאה לא תקין
//BlInvalidRequestException: בקשה לא חוקית
//BlRequestFailedException: הבקשה נכשלה
