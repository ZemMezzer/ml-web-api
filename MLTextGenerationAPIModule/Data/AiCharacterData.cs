namespace MLTextGenerationAPIModule.Data;

[Serializable]
public struct AiCharacterData
{
    public string Name;
    public string InnerMessage;
    public string Context;

    public AiCharacterData(string name, string innerMessage, string context)
    {
        Name = name;
        InnerMessage = innerMessage;
        Context = context;
    }
}