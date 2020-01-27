using SQLite4Unity3d;

public class Game_Settings {

    [PrimaryKey]
    public int game_settings_id {get; set;}
	public int game_difficulty {get; set;}
	public string game_type {get; set;}
	public string word_config {get; set;}
	public int reward_config {get; set;}
    //[ForeignKey(typeof(Rewards))]
	public int reward_id{get; set;}
    //[ForeignKey(typeof(WordsList))]
	public int? word_list_id{get; set;}
    public int reward_time_limit{get; set;}
    public int letter_snap{get; set;}

    public override string ToString() {
        return string.Format("[GameSettings: game_settings_id={0}, game_difficulty={1}, game_type={2}, word_config={3}, reward_config={4}, reward_id={5}, word_list_id={6}, reward_time_limit={7}, letter_snap={8}]", game_settings_id, game_difficulty, game_type, word_config, reward_config, reward_id, word_list_id, reward_time_limit, letter_snap);
    }
}