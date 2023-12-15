using MLDbModule.Data;

namespace MLDbModule;

public static class DbExtensions
{
    public static bool TryValidateAuth(this Dictionary<string, string?> input, DataBaseController dataBaseController, out DataBaseRecord? userRecord)
    {
        userRecord = default;
        
        if (!input.TryGetValue("username", out string? username) || string.IsNullOrEmpty(username))
            return false;

        if (!input.TryGetValue("password", out string? password) || string.IsNullOrEmpty(password))
            return false;

        if (!dataBaseController.TryGetRecordByName(DbKeywords.Users, username, out DataBaseRecord? record) || record == null)
            return false;

        if (!record.TryGet("password", out string? userPassword) || string.IsNullOrEmpty(userPassword) || userPassword != password)
            return false;

        userRecord = record;
        return true;
    } 
}