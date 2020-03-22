using SQLite4Unity3d;

namespace QuestSystem
{
    public class QuestData  {

        [PrimaryKey]
        public string QuestId { get; set; }
        public string Description { get; set; }

        public QuestData()
        {
            
        }

        public QuestData(string questId, string description)
        {
            QuestId = questId;
            Description = description;
        }
        
        public override string ToString ()
        {
            return $"[Quest: QuestId={QuestId}, Desc={Description}]";
        }
    }
}
