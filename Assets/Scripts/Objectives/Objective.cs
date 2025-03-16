using System;

[Serializable]
public class Objective
{
    public string Title;
    public string Description;
    public bool IsCompleted;

    public Objective(string title, string description)
    {
        Title = title;
        Description = description;
        IsCompleted = false;
    }

    public void CompleteObjective()
    {
        IsCompleted = true;
    }
}

