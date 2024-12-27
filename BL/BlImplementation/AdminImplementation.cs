namespace BlImplementation;
using BlApi;
using DalApi;
using Helpers;
using System.Linq.Expressions;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void ForwardClock(BO.TimeUnit unit)
    {
        DateTime newTime;

        switch (unit) // using switch to know how much to advance the clock 
        {
            case BO.TimeUnit.Minute:
                newTime = AdminManager.Now.AddMinutes(1);
                break;
            case BO.TimeUnit.Hour:
                newTime = AdminManager.Now.AddHours(1);
                break;
            case BO.TimeUnit.Day:
                newTime = AdminManager.Now.AddDays(1);
                break;
            case BO.TimeUnit.Month:
                newTime = AdminManager.Now.AddMonths(1);
                break;
            case BO.TimeUnit.Year:
                newTime = AdminManager.Now.AddYears(1);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(unit), unit, "Invalid time unit");
        }

        // change the time to the one forwarded
        AdminManager.UpdateClock(newTime);
    }

    DateTime IAdmin.GetClock()
    {
        return DateTime.Now;
    }

    TimeSpan IAdmin.GetMaxRange()
    {
        return AdminManager.MaxRange; //get risk range from the data layer
    }

    void IAdmin.InitializeDB()
    {
        DalTest.Initialization.Do();
        AdminManager.UpdateClock(AdminManager.Now);
        AdminManager.MaxRange = AdminManager.MaxRange;
    }

    void IAdmin.ResetDB()
    {
        _dal.ResetDB();
        AdminManager.UpdateClock(AdminManager.Now);
        AdminManager.MaxRange = AdminManager.MaxRange;
    }

    void IAdmin.SetMaxRange(TimeSpan maxRange)
    {
        AdminManager.MaxRange = maxRange;
    }

    #region Stage 5
    public void AddClockObserver(Action clockObserver) => AdminManager.ClockUpdatedObservers += clockObserver;
    public void RemoveClockObserver(Action clockObserver) => AdminManager.ClockUpdatedObservers -= clockObserver;
    public void AddConfigObserver(Action configObserver) => AdminManager.ConfigUpdatedObservers += configObserver;
    public void RemoveConfigObserver(Action configObserver) => AdminManager.ConfigUpdatedObservers -= configObserver;
    #endregion Stage 5
}
