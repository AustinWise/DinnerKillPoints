using Austin.DkpLib;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Json;

namespace DkpWeb.Blazor.Services;

public sealed class HttpBillSplitterServices : IBillSplitterServices
{
    private readonly HttpClient mClient;
    private readonly AntiforgeryStateProvider mAntiforgery;

    public HttpBillSplitterServices(HttpClient httpClient, AntiforgeryStateProvider antiforgery)
    {
        this.mClient = httpClient;
        this.mAntiforgery = antiforgery;
    }

    public async Task<SplitPerson[]> GetAllPeopleAsync()
    {
        return (await mClient.GetFromJsonAsync<SplitPerson[]>("api/Person/")) ?? [];
    }

    public async Task SaveBillSplitResult(BillSplitResult result)
    {
        var antiforgery = mAntiforgery.GetAntiforgeryToken();
        var request = new HttpRequestMessage(HttpMethod.Post, "api/BillSplit");
        if (antiforgery is not null)
        {
            request.Headers.Add("RequestVerificationToken", antiforgery.Value);
        }
        JsonContent content = JsonContent.Create(result);
        request.Content = content;
        await mClient.SendAsync(request);
    }
}
