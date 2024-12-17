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
                newTime = ClockManager.Now.AddMinutes(1);
                break;
            case BO.TimeUnit.Hour:
                newTime = ClockManager.Now.AddHours(1);
                break;
            case BO.TimeUnit.Day:
                newTime = ClockManager.Now.AddDays(1);
                break;
            case BO.TimeUnit.Month:
                newTime = ClockManager.Now.AddMonths(1);
                break;
            case BO.TimeUnit.Year:
                newTime = ClockManager.Now.AddYears(1);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(unit), unit, "Invalid time unit");
        }

        // change the time to the one forwarded
        ClockManager.UpdateClock(newTime);
    }

    DateTime IAdmin.GetClock()
    {
        return DateTime.Now;
    }

    TimeSpan IAdmin.GetMaxRange()
    {
        return _dal.Config.RiskRange; //get risk range from the data layer
    }

    void IAdmin.InitializeDB()
    {
        DalTest.Initialization.Do();
        ClockManager.UpdateClock(ClockManager.Now);
    }

    void IAdmin.ResetDB()
    {
        _dal.ResetDB();
        ClockManager.UpdateClock(ClockManager.Now);
    }

    void IAdmin.SetMaxRange(TimeSpan maxRange)
    {
        _dal.Config.RiskRange = maxRange;   
    }
}
