using SQLite4Unity3d;

namespace QuestSystem
{
    public class SwitchDescriptionData
    {
        [PrimaryKey] public string SwitchId { get; set; }
        public bool DefaultResult { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"[SwitchDescription: SwitchId={SwitchId}, DefaultResult={DefaultResult}, Desc={Description}]";
        }
    }
}