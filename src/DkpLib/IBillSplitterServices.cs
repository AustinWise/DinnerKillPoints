namespace Austin.DkpLib;

public interface IBillSplitterServices
{
    Task<SplitPerson[]> GetAllPeopleAsync();

    Task SaveBillSplitResult(BillSplitResult result);
}
