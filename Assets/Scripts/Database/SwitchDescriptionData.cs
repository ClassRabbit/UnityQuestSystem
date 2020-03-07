using SQLite4Unity3d;

public class SwitchDescriptionData  
{
    [PrimaryKey]
    public string SwitchId { get; set; }
    public string Description { get; set; }

    public override string ToString ()
    {
        return $"[SwitchDescription: SwitchId={SwitchId}, Desc={Description}]";
    }
}