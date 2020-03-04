using SQLite4Unity3d;

public class QuestData  {

    [PrimaryKey]
    public string QuestId { get; set; }
    public string Description { get; set; }

    public override string ToString ()
    {
        return $"[Quest: QuestId={QuestId}, Desc={Description}]";
    }
}