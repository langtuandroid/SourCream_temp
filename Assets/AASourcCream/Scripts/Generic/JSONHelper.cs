
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class JSONHelper
{

    public static T Read<T>(string filePath)
    {
        string text = File.ReadAllText(filePath);
        StreamReader file = File.OpenText(filePath);
        JsonSerializer serializer = new JsonSerializer();
        T generic = JsonConvert.DeserializeObject<T>(text);
        return generic;
    }
}
