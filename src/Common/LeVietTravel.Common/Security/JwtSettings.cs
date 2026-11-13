namespace LeVietTravel.Common.Security;

public class JwtSettings
{
    public string SecretKey { get; set; } = "LeVietTravel_Super_Secret_Key_For_Jwt_Auth_2026_DotNet8_Microservice!";
    public string Issuer { get; set; } = "LeVietTravel";
    public string Audience { get; set; } = "LeVietTravelClients";
    public int AccessTokenExpirationMinutes { get; set; } = 15;
    public int RefreshTokenExpirationDays { get; set; } = 7;
}
