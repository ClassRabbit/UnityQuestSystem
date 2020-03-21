using SQLite4Unity3d;

namespace QuestSystem
{
    public class SwitchComponentData
    {
        [PrimaryKey, AutoIncrement] public int SwitchComponentId { get; set; }
        [NotNullAttribute] public string SwitchId { get; set; }
        [NotNullAttribute] public int State { get; set; }
        [NotNullAttribute] public int Order { get; set; }
        [NotNullAttribute] public string Operator { get; set; }
        [NotNullAttribute] public string QuestId { get; set; }

        public override string ToString()
        {
            return $"[SwitchComponent: SwitchComponentId={SwitchComponentId}, "
                   + $"SwitchId={SwitchId},"
                   + $"State={State},"
                   + $"Order={Order},"
                   + $"QuestId={QuestId}]";
        }
    }
}