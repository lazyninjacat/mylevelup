using SQLite4Unity3d;

public class User_Stats {

    [PrimaryKey, AutoIncrement]
    public int user_stats_id {get; set;}
	public int user_id {get; set;}
	public int game_history_id {get; set;}
    public int reward_stats_id {get; set;}

    

    public override string ToString() {
        return string.Format("[User_Stats: user_stats_id={0}, user_id={1}, game_history_id={2}, reward_stats_id={3}]", user_stats_id, user_id, game_history_id, reward_stats_id);
    }
}