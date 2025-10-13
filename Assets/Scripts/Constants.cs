public static class Constants
{
    public const string HorizontalAxis = "Horizontal";
    public const string VerticalAxis = "Vertical";

    //aim
    public const float AimCircleOuterRadiusPercent = 0.06f; //TODO: scriptable objects to be able to edit in inspector?
    public const float AimCircleInnerRadiusPercent = 0.018f;
    public const float AimCircleOffsetPercentX = 0.10f;
    public const float AimCircleOffsetPercentY = 0.85f;
    public const float UpwardAimThresholdY = 0.4f;
    public const float DownwardAimThresholdY = -0.4f;
    //projectile
    public const float ProjectileOffset = 1f;

    //healthbar
    public const int TeamHealthbarOffsetPixelsX = 20;
    public const int TeamHealthbarOffsetPixelsY = 10;

    //layers
    public const int CharacterLayer = 3;
    public const int GroundLayer = 8;

    //tags 
    public const string CharacterTag = "Character";
    public const string ProjectileTag = "Projectile";
    public const string PackageTag = "Package";
    public const string GroundTag = "Ground";
    public const string DeadZoneTag = "DeadZone";

    //input action maps
    public const string GameplayActionMap = "Gameplay";
    public const string InventoryActionMap = "Inventory";

}
