﻿public enum TileMode
{
    None, Floor, NormalWall, Mirror, StartFloor, TrueCase, FalseCase,
    MirrorCase, NullCase, Camera, WMannequin, BMannequin, GoalFloor
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
    NULL,
    Normal,
    Mirror
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
    NULL,
    Briefcase,
    Camera,
    Mannequin
}