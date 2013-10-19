using System;

[Serializable]
public class LifeSystem
{
    public int CurrentLifes;

    public int Lifes
    {
        get { return CurrentLifes; }
        private set
        {
            CurrentLifes = value;
            InvokeLifesChanged();
        }
    }

    public bool HasLife
    {
        get { return Lifes > 0; }
    }

    #region Events

    public event Action<int> LifesChanged;

    #endregion

    #region Event Invokators

    protected virtual void InvokeLifesChanged()
    {
        var handler = LifesChanged;
        if (handler != null) handler(Lifes);
    }

    #endregion

    public bool SpentLife(int amount = 1)
    {
        if (amount <= Lifes)
        {
            Lifes -= amount;
            return true;
        }
        return false;
    }

    public void AddLife(int amount = 1)
    {
        Lifes += amount;
    }

    public JSONObject Serialize()
    {
        var jsonObject = new JSONObject(JSONObject.Type.OBJECT);
        jsonObject.AddField("Lifes", Lifes);
        return jsonObject;
    }

    public static LifeSystem Unserialize(JSONObject jsonObject)
    {
        return new LifeSystem {Lifes = jsonObject.GetField("Lifes").integer};
    }
}