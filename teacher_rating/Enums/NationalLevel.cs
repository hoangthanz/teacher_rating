using System.ComponentModel;

namespace teacher_rating.Enums;

public enum NationalLevel
{
    [Description("Quốc gia")]
    Nation = 0,
    [Description("Quốc tế")]
    International = 1,
    [Description("Tỉnh")]
    City = 2
}