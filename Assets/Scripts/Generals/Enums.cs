public enum TileMode
{
    None, Floor, NormalWall, Mirror, StartFloor, TrueCase, FalseCase,
    MirrorCase, NullCase, Camera, WMannequin, BMannequin, GoalFloor,
    Glass, LightPole, LightGetter
}

public enum ClearType
{
    NFloor,
    NTurret,
    NCase,
    NPlayer,
    AllFloor,
    AllTurret,
    AllCase,
    White,
    Black
}

public enum WallType
{
    Normal,
    Mirror,
    Glass
}

public enum BulletCode
{
    NULL,
    True,
    False,
    Mirror
}

public enum ObjType
{
    Briefcase,
    Camera,
    Mannequin,
    LightPole,
    LightGetter
}

public enum FloorChkMode
{
    Check,
    Add,
    Remove
}
