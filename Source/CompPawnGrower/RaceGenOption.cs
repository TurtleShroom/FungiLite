using System.Xml;
using Verse;

namespace CompPawnGrower;

public class RaceGenOption
{
    public float selectionWeight;
    public ThingDef thingdef;

    public float Cost => thingdef.BaseMarketValue;

    public override string ToString()
    {
        return
            $"({(thingdef != null ? thingdef.ToString() : "null")} w={selectionWeight:F2} c={(thingdef != null ? Cost.ToString("F2") : "null")})";
    }

    public void LoadDataFromXmlCustom(XmlNode xmlRoot)
    {
        DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "thingdef", xmlRoot.Name);
        selectionWeight = ParseHelper.FromString<float>(xmlRoot.FirstChild.Value);
    }
}