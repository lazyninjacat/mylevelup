using SQLite4Unity3d;

public class Game_Config
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public int play_list_id { get; set; }
    public int infinite_loop { get; set; }
    public int iteration_number { get; set; }
    public int pass_locked { get; set; }

    public override string ToString()
    {
        return string.Format("[Word: id={0}, play_list_id={1}, infinite_loop={2}, iteration_number={3}, pass_locked={4}", id, play_list_id, infinite_loop, iteration_number, pass_locked);
    }
}
