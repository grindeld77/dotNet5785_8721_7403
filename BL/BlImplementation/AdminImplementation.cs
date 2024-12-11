namespace BlImplementation;
using BlApi;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void ForwardClock(BO.TimeUnit unit)
    {

    }

    DateTime IAdmin.GetClock()
    {
        throw new NotImplementedException();
    }

    int IAdmin.GetMaxRange()
    {
        throw new NotImplementedException();
    }

    void IAdmin.InitializeDB()
    {
        throw new NotImplementedException();
    }

    void IAdmin.ResetDB()
    {
        throw new NotImplementedException();
    }

    void IAdmin.SetMaxRange(int maxRange)
    {
        throw new NotImplementedException();
    }




}
