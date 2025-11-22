public static class Constants
{
    public const string HorizontalAxis = "Horizontal";
    public const string VerticalAxis = "Vertical";

    //aim
    public const float UpwardAimThresholdY = 0.4f;
    public const float DownwardAimThresholdY = -0.4f;

    //projectile
    public const float ProjectileOffset = 1.1f;

    // raycast
    public const int RaycastHitColliderNumMax = 5;
    public const int ProjectileRaycastDistance = 30;

    //healthbar
    public const int TeamHealthbarOffsetPixelsX = 20;
    public const int TeamHealthbarOffsetPixelsY = 10;
    public const int CharacterHealthbarValuePerUnit = 10;

    //layers
    public const int CharacterLayer = 3;
    public const int GroundLayer = 8;
    public const int DeadZoneLayer = 9;
    public const int DeadCharacterLayer = 11;

    public static readonly int[] ProjectileCollisionLayers = new int[]{ CharacterLayer, GroundLayer, DeadZoneLayer };

    //tags 
    public const string CharacterTag = "Character";
    public const string ProjectileTag = "Projectile";
    public const string PackageTag = "Package";
    public const string GroundTag = "Ground";
    public const string DeadZoneTag = "DeadZone";

    //input action maps
    public const string GameplayActionMap = "Gameplay";
    public const string InventoryActionMap = "Inventory";

    // destructible terrain
    public const float AlphaThreshold = 0.1f;

    // multiplayer
    public const int MultiplayerMinPlayers = 2;
    public const int MultiplayerMaxPlayers = 4;

    //scenes
    public const string MenuSceneName = "MenuScene";

    //minimap
    public const string MinimapFolderPath = "Assets/Resources/Sprites/Minimap";

    //simulation & bots
    public const float MaxParabolicPathSimulationTime = 10f; 
    public const float ParabolicPathSimulationDeltaForMovement = .05f;
    public const float ParabolicPathSimulationDeltaForProjectiles = .02f;
}
