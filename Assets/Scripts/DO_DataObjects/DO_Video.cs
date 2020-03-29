using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

///<summary>
/// This class represents a data object that contains all the data belonging to a video
///</summary>
[Serializable]
public class DO_Video{

	public int watchCount;
	public string title;
	public string description;
	public string thumbUrl;
	public List<int> totalTimeWatched;
	public List<int> seekTimes;
	public List<int> playTimes;
	public List<int> pauseTimes;

	public DO_Video() { }

	public DO_Video(int wc, string tit, string desc, string url, List<int> ttw, List<int> seek, List<int> play, List<int> pause){
		watchCount = wc;
		title = tit;
		description = desc;
		thumbUrl = url;
		totalTimeWatched = ttw;
		seekTimes = seek;
		playTimes = play;
		pauseTimes = pause;
	}

    public override string ToString() {
        return string.Format("[Video: watchCount={0}, title={1}, description={2}, thumbUrl={3}, totalTimeWatched={4}, seekTimes={5}, playTimes={6}, pauseTimes={7}]", watchCount, title, description, thumbUrl, totalTimeWatched, seekTimes, playTimes, pauseTimes);
    }
}
