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