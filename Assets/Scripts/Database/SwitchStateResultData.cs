using SQLite4Unity3d;

public class SwitchStateResultData  
{
    [PrimaryKey, AutoIncrement] 
    public int SwitchComponentId { get; set; }
    public string SwitchId { get; set; }
    public int State { get; set; }
    public bool Result { get; set; }

    public override string ToString ()
    {
        return $"[SwitchStateResult: SwitchId={SwitchId}, State={State}, Result={Result}]";
    }
}