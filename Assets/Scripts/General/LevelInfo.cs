using System;

[Serializable]
public class LevelInfo
{
    public bool IsCompleted;

    public int LevelNumber;
    public int BestScore;
    public int NumberOfAttempts;

    public JSONObject Serialize()
    {
        var jsonObject = new JSONObject(JSONObject.Type.OBJECT);

        jsonObject.AddField("LevelNumber", LevelNumber);
        jsonObject.AddField("BestScore", BestScore);
        jsonObject.AddField("NumberOfAttempts", NumberOfAttempts);
        jsonObject.AddField("IsCompleted", IsCompleted);

        return jsonObject;
    }

    public static LevelInfo Unserialize(JSONObject jsonObject)
    {
        return new LevelInfo
        {
            BestScore = (int) jsonObject.GetField("BestScore").n,
            NumberOfAttempts = (int) jsonObject.GetField("NumberOfAttempts").n,
            LevelNumber = (int)jsonObject.GetField("LevelNumber").n,
            IsCompleted = jsonObject.GetField("IsCompleted").b,
        };
    }
}