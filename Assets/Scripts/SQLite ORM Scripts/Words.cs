using SQLite4Unity3d;

public class Words {

    [PrimaryKey, AutoIncrement]
    public int word_id { get; set; }
    public string word_name { get; set; }
    public string stock_custom { get; set; }
    public string word_tags { get; set; }
    public string word_sound { get; set; }
    public string word_image { get; set; }
    public float word_mastery { get; set; }


    public override string ToString() {
		return string.Format("[Word: word_id={0}, word_name={1}, stock_custom={2}, word_tags={3}, word_sound={4}, word_image={5}]", word_id, word_name, stock_custom, word_tags, word_sound, word_image, word_mastery);
    }
}
