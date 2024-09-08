using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CelticKnots;

public enum E_OutlineCap {
    [Description("Do not return a cap, Default for on screen.")]
    NoCap = 0,
    [Description("Cap the Left and Right Start points.")]
    Start = 1,
    [Description("Cap the Left and Right End points.")]
    End = 2
}
public enum E_OutlineType {
    [Description("None or Undefined.")]
    None,
    [Description("The outlone of the tile.")]
    Outline,
    [Description("Middle Point (12) Default for on screen.")]
    Centre,
    [Description("Left path from start (before rotation clockwise from the top centre).")]
    Left,
    [Description("Left path from start (before rotation clockwise from the top centre).")]
    Right ,
    [Description("A FullOutline Left and Right Paths with Caps.")]
    FullOutline ,
    [Description("Just the Caps.")]
    Cap 
}

public enum E_OffsetPointHorizontal {
    [Description("Left of centre")]
    Left,
    [Description("Midpoint")]
    Normal,
    [Description("Right of centre")]
    Right
}
public enum E_OfsetPointVertical {
    [Description("Above Normal")]
    Top,
    [Description("Normal, Centred")]
    Normal,
    [Description("Below Normal")]
    Bottom
}
public enum E_TileLineType {
    [Description("Without data")]
    Empty,
    [Description("A standing cross")]
    Cross,
    [Description("An X shape")]
    DiagonalCross,
    [Description("A horizontal bar")]
    Bar,
    [Description("A diagonal bar")]
    DiagonalBar,
    [Description("Two ninety-degree curve between adjacent mid points")]
    MidPoints, //4 and above consit of curves, they have 2 points each with a control point.
    [Description("Two curves between top and bottom corners")]
    Corners,
    [Description("A ninety-degree curve between adjacent mid points.")]
    HalfMid,
    [Description("A centre mid-point to opposite corner")]
    MidToCorner,
    //TC 24/08/24 Missed this one.
    [Description("A curve between top and bottom corners")]
    HalfCorners
}
public enum E_TileWeave {
    //the corners can be woven. I need to do for this.
    [Description("Without data or irrelevant ")]
    NoWeave = 0,
    [Description("The primary line goes over the second")]
    OverWeave,
    [Description("The primary line goes under the second")]
    UnderWeave 
}
public enum E_TileRotation {
    [Description("Without rotation or irrelevant")]
    NoRitation = 0, //Default
    [Description("90 degree rotation")]
    RotatedNinetyDegrees = 90,
    [Description("180 degree rotation")]
    RotatedOneEightyDegrees = 180,
    [Description("270 degree rotation")]
    RotatedTwoSeventyDegrees = 270
}
public enum E_LinePoint {
    //E_LinePoint needs to be a class, it needs properties and methods.
    //Name
    //Number
    //
    //Description
    //Points X Y 

    //Rotate - andgle
    //Outline

    //
    // Offset points
    [Description("Without rotation or irrelevant")]
    None = 0,
    [Description("The Top left corner - (0,0)")]
    TopLeft,
    [Description("Centre on the top row")]
    TopMiddle,
    [Description("Right on the top row")]
    TopRight,
    //TC new 25/07/24 8 new points, but I may have to do this properly.
    [Description("Between the top left and middle")]
    TopLeftMiddleSplit,
    [Description("Between the top middle and middle")]
    TopMiddleMiddleSplit,
    [Description("Between the top right and middle")]
    TopRightMiddleSplit,

    [Description("Left on the centre row")]
    MiddleLeft,
    //TC new 25/07/24
    [Description("Between MiddleLeft and MiddleMiddle")]
    MiddleLeftsplit,
    [Description("Middle on the centre row")]
    MiddleMiddle,
    //TC new 25/07/24
    [Description("Between MiddleMiddle and MiddleRight")]
    MiddleRightsplit,
    [Description("Right on the centre row")]
    MiddleRight,

    //TC new 25/07/24
    [Description("Between the bottom left and middle")]
    BottomLeftMiddleSplit,
    [Description("Between the bottom middle and middle")]
    BottomMiddleMiddleSplit,
    [Description("Between the bottom right and middle")]
    BottomRightMiddleSplit,

    [Description("Left on the bottom row")]
    BottomLeft,
    [Description("Middle on the bottom row")]
    BottomMiddle,
    [Description("Right on the bottom row")]
    BottomRight,
}



public static class EnumerationHelper {
    ////this looked like a good idea
    ////https://stackoverflow.com/questions/14971631/convert-an-enum-to-liststring
    //public static string GetDescription(this Enum value)
    //{
    //    Type type = value.GetType();
    //    string name = Enum.GetName(type, value);
    //    if (name != null)
    //    {
    //        FieldInfo field = type.GetField(name);
    //        if (field != null)
    //        {
    //            DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
    //            if (attr != null)
    //            {
    //                return attr.Description;
    //            }
    //        }
    //    }
    //    return null;
    //    /* how to use
    //        MyEnum x = MyEnum.NeedMoreCoffee;
    //        string description = x.GetDescription();
    //    */

    //}

    private static string GetEnumDescription(Enum value) {
        //https://stackoverflow.com/questions/2650080/how-to-get-c-sharp-enum-description-from-value
        FieldInfo? fi = value.GetType().GetField(value.ToString());
        //DescriptionAttribute[]? attributes = fi?.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
        if (fi?.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attributes && attributes.Length != 0) {
            return attributes.First().Description;
        }

        return value.ToString();
    }
    public static string GetDescription<TEnum>(this TEnum EnumValue) where TEnum : struct {
        return EnumerationHelper.GetEnumDescription((Enum)(object)((TEnum)EnumValue));
    }
    public static string GetEnumName<TEnum>(this TEnum EnumValue) where TEnum : struct {
        return Enum.GetName(typeof(TEnum), EnumValue) ?? string.Empty;
    }



}
//give this a go sometime
//https://stackoverflow.com/questions/15388072/how-to-add-extension-methods-to-enums
public static class EnumExtensions {
    public static int ToInt<T>(this T soure) where T : IConvertible//enum
    {
        if (!typeof(T).IsEnum)
            throw new ArgumentException("T must be an enumerated type");

        return (int)(IConvertible)soure;
    }

    //ShawnFeatherly funtion (above answer) but as extention method
    public static int Count<T>(this T soure) where T : IConvertible//enum
    {
        if (!typeof(T).IsEnum)
            throw new ArgumentException("T must be an enumerated type");

        return Enum.GetNames(typeof(T)).Length;
    }


}
