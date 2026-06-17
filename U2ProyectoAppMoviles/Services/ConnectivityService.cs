namespace U2ProyectoAppMoviles.Services;

public class ConnectivityService
{
    public async Task<bool> HayConexion()
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            return false;

        return await TieneInternetRealAsync();
    }

    private async Task<bool> TieneInternetRealAsync(int timeoutMs = 5000)
    {
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromMilliseconds(timeoutMs) };
            var response = await client.GetAsync("https://www.google.com/generate_204");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
