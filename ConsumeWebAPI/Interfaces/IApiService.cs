namespace ConsumeWebAPI.Interfaces
{
    public  interface IApiService
    {
       Task<string> GetTokenAsync(string email, string password);
    }
}
