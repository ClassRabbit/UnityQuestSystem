using SQLite4Unity3d;

public class SwitchStateResultData  
{
    [PrimaryKey, AutoIncrement] 
    public int SwitchStateResultId { get; set; }
    [NotNullAttribute]
    public string SwitchId { get; set; }
    [NotNullAttribute]
    public int State { get; set; }
    [NotNullAttribute]
    public bool Result { get; set; }

    public override string ToString ()
    {
        return $"[SwitchStateResult: SwitchId={SwitchId}, State={State}, Result={Result}]";
    }
}