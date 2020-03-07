using SQLite4Unity3d;

public class SwitchComponentData  
{
    [PrimaryKey, AutoIncrement] 
    public int SwitchComponentId { get; set; }
    
    public string SwitchId { get; set; }
    
    public int State { get; set; }
    
    public int Order { get; set; }
    
    public string Operator { get; set; }
    
    public string QuestId { get; set; }

    public override string ToString ()
    {
        return $"[SwitchComponent: SwitchComponentId={SwitchComponentId}, "
               + $"SwitchId={SwitchId},"
               + $"State={State},"
               + $"Order={Order},"
               + $"QuestId={QuestId}]";
    }
}