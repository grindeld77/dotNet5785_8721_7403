namespace BlImplementation;
using BlApi;
using BO;
using DalApi;
using Helpers;
using System.Linq.Expressions;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void ForwardClock(BO.TimeUnit unit)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        DateTime newTime = AdminManager.Now;
        newTime = unit switch
        {
            BO.TimeUnit.Minute => AdminManager.Now.AddMinutes(1.0),
            BO.TimeUnit.Hour => AdminManager.Now.AddHours(1.0),
            BO.TimeUnit.Day => AdminManager.Now.AddDays(1.0),
            BO.TimeUnit.Month => AdminManager.Now.AddMonths(1),
            BO.TimeUnit.Year => AdminManager.Now.AddYears(1),
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, "Invalid time unit"),
        };
        //Update the clock
        AdminManager.UpdateClock(newTime);
    }

    DateTime IAdmin.GetClock()
    {
        return AdminManager.Now;
    }

    TimeSpan IAdmin.GetMaxRange()
    {
        return AdminManager.MaxRange; //get risk range from the data layer
    }

    void IAdmin.InitializeDB()
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.InitializeDB(); //stage 7
    }

    void IAdmin.ResetDB()
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.ResetDB(); //stage 7
    }

    void IAdmin.SetMaxRange(TimeSpan maxRange)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.MaxRange = maxRange;
    }

    public void StartSimulator(int interval)  //stage 7
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.Start(interval); //stage 7
    }

    public void StopSimulator() => AdminManager.Stop(); //stage 7
    #region Stage 5
    public void AddClockObserver(Action clockObserver) => AdminManager.ClockUpdatedObservers += clockObserver;
    public void RemoveClockObserver(Action clockObserver) => AdminManager.ClockUpdatedObservers -= clockObserver;
    public void AddConfigObserver(Action configObserver) => AdminManager.ConfigUpdatedObservers += configObserver;
    public void RemoveConfigObserver(Action configObserver) => AdminManager.ConfigUpdatedObservers -= configObserver;
    #endregion Stage 5
}
