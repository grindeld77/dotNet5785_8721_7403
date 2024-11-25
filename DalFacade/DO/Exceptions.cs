namespace DO;

[Serializable]
public class DalException : Exception//General error
{
    public DalException(string? message) : base(message) { }
}

[Serializable]
public class DalDoesNotExistException : DalException//Entity not found
{
    public DalDoesNotExistException(string? message) : base(message) { }
}

[Serializable]
public class DalAlreadyExistsException : DalException//Entity already exists
{
    public DalAlreadyExistsException(string? message) : base(message) { }
}

