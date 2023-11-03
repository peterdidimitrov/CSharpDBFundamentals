namespace P02_FootballBetting.Data.Common;

public static class ValidationConstants
{
    //Team
    public const int TeamNameMaxLength = 50;
    public const int TeamLogoUrlMaxLength = 2048;
    public const int TeamLogoInitialMaxLength = 4;

    //color
    public const int ColorNameMaxLength = 10;

    //Town
    public const int TownNameMaxLength = 58;

    //Country
    public const int CountryNameMaxLength = 56;

    //Player
    public const int PlayerNameMaxLength = 100;

    //Position
    public const int PositionNameMaxLength = 50;

    //Game
    public const int GameResultMaxLength = 7;

    //User
    public const int UserUserNameMaxLength = 36;
    public const int UserPasswordMaxLength = 255;
    public const int UserEmailMaxLength = 255;
    public const int UserNameMaxLength = 100;
}
