using SQLite4Unity3d;

public class Users {

    [PrimaryKey, AutoIncrement]
    public int user_id {get; set;}
	public string user_name {get; set;}
	public string user_avatar {get; set;}

    

    public override string ToString() {
        return string.Format("[Users: user_id={0}, user_name={1}, user_avatar={2}]", user_id, user_name, user_avatar);
    }
}