using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Handles saving each video watched to the Reward_Stats table */
public class RewardHistoryController : MonoBehaviour {

	private DataService ds;
	private List<Reward_Stats> videosWatched;
	// Use this for initialization
	void Start () {
		ds = StartupScript.ds;
		videosWatched = new List<Reward_Stats>();
	}
	
	///<summary>Adds a video to the videosWatched list</summary>
	///<param name="rs">an object containing the values to enter into the DB</param>
	///<returns> a boolean signalling the result of adding the video</returns>

	public bool AddWatchedVideo(Reward_Stats rs){
		Debug.Log("Adding video to list to add to DB");
		videosWatched.Add(rs);
		if (!videosWatched.Contains(rs)){
			return false;
		}
		return true;
	}

	///<summary>Called when the Reward is complete. Saves all the videos in the videosWatched list to the database</summary>
	///<returns>the number of rows inserted</returns>

	public int SaveVideoList(){
		int retVal = 0;
		foreach (Reward_Stats video in videosWatched){
			retVal += ds.InsertRewardStats(video);
		}
		return retVal;
	}
}
