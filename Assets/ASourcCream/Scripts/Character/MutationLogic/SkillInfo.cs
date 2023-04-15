using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;

[MessagePackObject]
[Serializable]
public class SkillInfo
{
    // Skill
    [Key(0)]
    public string skillName = "skill name";
    [Key(1)]
    public string skillDescription = "skill description";
}