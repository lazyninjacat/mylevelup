using SQLite4Unity3d;

public class Reward_Stats {

    [PrimaryKey, AutoIncrement]
    public int reward_stats_id {get; set;}
	public string reward_type {get; set;}
	public int user_id {get; set;}
	public string video_id {get; set;}
    public string video_title {get; set;}
	public string keywords {get; set;}


    public override string ToString() {
        return string.Format("[Reward_Stats: reward_stats_id={0}, reward_type={1}, user_id={2}, video_id={3}, video_title={4}, keywords={5}]", reward_stats_id, reward_type, user_id, video_id, video_title, keywords);
    }
}
