using Mess.Util;

namespace Mess;

public class Config
{
    public static Config? Instance;

    public string DbConnectionString
    {
        get => "Data Source=" + DbFilePath + ";Cache=Shared";
    }

    public string DbFilePath { get; set; } =
        "Database.db";
    public string FrontendUrl { get; set; } = "http://192.168.178.24/";
    public string Secret { get; set; } = "";


    public bool UseOAuth { get; set; } = true;
    public string OAuthAuthority { get; set; } = "";
    public string OAuthClientId { get; set; } = "";
    public string OAuthClientSecret { get; set; } = "";
    public string JwtIssuer { get; set; } = "mess";


    public static void GetFromEnvironment()
    {
        Instance = new Config();
        EnvBinder.PopulateFromEnvironment(Instance);
    }
}