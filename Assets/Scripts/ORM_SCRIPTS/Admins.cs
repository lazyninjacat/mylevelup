using SQLite4Unity3d;

public class Admins {

    [PrimaryKey, AutoIncrement]
    public int admin_id {get; set;}
	public string admin_name {get; set;}
	public string admin_pass {get; set;}
	public string admin_email{get; set;}


    public override string ToString() {
        return string.Format("[Admins: admin_id={0}, admin_name={1}, admin_pass={2}, admin_email={3}]", admin_id, admin_name, admin_pass, admin_email);
    }
}
