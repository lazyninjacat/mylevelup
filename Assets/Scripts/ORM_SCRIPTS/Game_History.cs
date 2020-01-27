using SQLite4Unity3d;

public class Game_History {

    [PrimaryKey, AutoIncrement]
    public int game_history_id {get; set;}
	public int user_id {get; set;}
	public int rounds_completed {get; set;}
	public string words_solved {get; set;}
	public int solve_time {get; set;}
	public int num_tile_moves {get; set;}
	


    public override string ToString() {
        return string.Format("[Rewards: game_history_id={0}, user_id={1}, rounds_completed={2}, words_solved={3}, solve_time={4}, num_tile_moves={5}]", game_history_id, user_id, rounds_completed, words_solved, solve_time, num_tile_moves);
    }
}
