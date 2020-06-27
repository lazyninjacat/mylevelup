using SQLite4Unity3d;

public class Rewards {

    [PrimaryKey]
    public int reward_id {get; set;}
	public string reward_name {get; set;}
	public string reward_type {get; set;}
    public string reward_url { get; set; }


    public override string ToString() {
        return string.Format("[Rewards: reward_id={0}, reward_name={1}, reward_type={3}, reward_url{4}]", reward_id, reward_name, reward_type, reward_url);
    }
}
