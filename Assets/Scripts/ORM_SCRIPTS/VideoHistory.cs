using SQLite4Unity3d;

public class VideoHistory{

	[PrimaryKey, AutoIncrement]
	public int id {get; set;}
	public string videoId {get; set;}
	public string video_data {get; set;}

    public override string ToString() {
        return string.Format("[VideoHistory: id={0}, videoId={1}, videoData={2}]", id, videoId, video_data);
    }
}
