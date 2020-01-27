using SQLite4Unity3d;

public class Words_List {

    [PrimaryKey, AutoIncrement]
    public int word_group_id { get; set; }
    public int word_id { get; set; }
    public int word_list_id { get; set; }

    public override string ToString() {
        return string.Format("[Word_List: word_group_id={0}, word_id={1}, word_list_id={2}", word_group_id, word_id, word_list_id);
    }
}
