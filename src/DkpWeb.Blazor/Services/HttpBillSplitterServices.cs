using Austin.DkpLib;
using System.Net.Http.Json;

namespace DkpWeb.Blazor.Services;

public sealed class HttpBillSplitterServices : IBillSplitterServices
{
    private readonly HttpClient mClient;

    public HttpBillSplitterServices(HttpClient httpClient)
    {
        this.mClient = httpClient;
    }

    public async Task<SplitPerson[]> GetAllPeopleAsync()
    {
        return (await mClient.GetFromJsonAsync<SplitPerson[]>("api/Person/")) ?? new SplitPerson[0];
    }

    public async Task SaveBillSplitResult(BillSplitResult result)
    {
        await mClient.PostAsJsonAsync("api/BillSplit", result);
    }
}
