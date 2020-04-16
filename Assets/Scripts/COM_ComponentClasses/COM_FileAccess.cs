using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using Unity.Collections;
using UnityEngine.UI;
using System.Text;

/// <summary>
/// A static component class that allows access to the file reading and writing.
/// </summary>
public static class FileAccessUtil
{
    private const int IMAGE_CAP = 8;
    static string PICTURE_DIRECTORY = Application.persistentDataPath + "/WordPictures";
    static string AUDIO_DIRECTORY = Application.persistentDataPath + "/WordAudio";
    static string REWARD_PIC_DIRECTORY = Application.persistentDataPath + "/RewardPictures";
    static string yt = "YouTube";
    static string cbckg = "CBC Kids Games";
    static string cbcktv = "CBC Kids TV";
    static string lr = "Line Rider";
    static string ttv = "Twitch TV";


    static WavEncoder encoder = new WavEncoder();
    static DateTime currentTime = DateTime.Now;


    public static bool DoesAudioExist(string word) { return File.Exists(AUDIO_DIRECTORY + "/" + word + ".wav"); }
    public static bool DoesWordPicExist(string word) { return File.Exists(PICTURE_DIRECTORY + "/" + word + ".png"); }
    public static bool DoesRewardPicExist(string word) { return (File.Exists(REWARD_PIC_DIRECTORY + "/" + word + ".png") || word == yt || word == cbckg || word == cbcktv || word == lr || word == ttv); }


    // All public facing multi file repair methods
    //public static bool WordImageDirRepair(string word) { return RepairImageDirectory(word, PICTURE_DIRECTORY); }

    // All public facing multi pic delete methods
    public static void DeleteWordPictures(List<string> filesToDelete, string word) { DeletePicturesFromDir(filesToDelete, PICTURE_DIRECTORY, word); }

    // All the public facing save methods
    public static bool SaveRewardPic(Texture2D photo, string fileName) { return SavePhotoToDir(photo, fileName, REWARD_PIC_DIRECTORY); }
    public static bool SaveWordPic(Texture2D photo, string fileName) { return SavePhotoToDir(photo, fileName, PICTURE_DIRECTORY); }
    public static bool SaveWordAudio(AudioClip clip, string fileName) { return SaveAudioToDir(clip, fileName, AUDIO_DIRECTORY); }
    public static bool SaveDLCWordAudio(byte[] audioFile, string fileName) { return SaveDLCAudioToDir(audioFile, fileName, AUDIO_DIRECTORY);  }

    // All the public facing Load methods
    public static byte[] LoadRewardPic(string fileName) { return LoadPhotoByDir(fileName, REWARD_PIC_DIRECTORY); }
    public static byte[] LoadWordPic(string fileName) { return LoadPhotoByDir(fileName, PICTURE_DIRECTORY); }
    public static AudioClip LoadWordAudio(string fileName) { return LoadAudioByDir(fileName, AUDIO_DIRECTORY); }

    // All the public facing Delete methods
    public static void DeleteRewardPic(string fileName) { DeleteImageFromDir(fileName, REWARD_PIC_DIRECTORY); }
    public static void DeleteWordPic(string fileName) { DeleteImageFromDir(fileName, PICTURE_DIRECTORY); }
    public static void DeleteWordAudio(string fileName) { DeleteAudioFromDir(fileName, AUDIO_DIRECTORY); }

    // All the public facing picture recovery methods
    public static List<byte[]> RetrieveWordPics(string fileName) { return RetrievePics(fileName, PICTURE_DIRECTORY); }
    public static List<byte[]> RetrieveRewardPics(string fileName) { return RetrievePics(fileName, REWARD_PIC_DIRECTORY); }


    // All the public facing picture collection saving methods
    public static List<string> SaveWordPics(List<Texture2D> texList, string fileName, List<string> namesList = null) { return SavePics(texList, fileName, PICTURE_DIRECTORY, namesList); }

    // All the public facing random picture selction methods
    public static byte[] LoadRandomWordPic(string fileName) { return LoadRandomPic(fileName, PICTURE_DIRECTORY); }

    public static string[] GetWordImagePaths(string word) { return GetImageFilePathArray(PICTURE_DIRECTORY + "/" + word.ToLower() + "/"); }
    public static bool DeleteWordImageDir(string word) { return DeleteDirectory(word, PICTURE_DIRECTORY); }

    /*
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="directory"></param>
    /// <returns>bool</returns>
    private static bool RepairImageDirectory(string fileName, string directory)
    {
        if (!Directory.Exists(directory))
        {
            //TODO: User feedback of failed attempt
            return false;
        }

        Debug.Log("*********************************************\nREPAIR: IN REPAIR FOR WORD"+fileName+"\n*******************************************");

        // TODO: replace the single digit search with a more robust search
        string pattern = "*.png";
        //////
        Debug.Log("*********************************************\nREPAIR: Pattern string is " + pattern + "\n*******************************************");
        int lastGoodInt = -1;
        bool fileFound = false;

        for (int x = 1; x < IMAGE_CAP; x++)
        {
            if ()
            {

            }
        }

        //string[] imageFiles = Directory.GetFiles(directory, pattern, 0);

        //int fileCount = Directory.GetFiles(directory, pattern, 0).Length;

        //Debug.Log("*********************************************\nREPAIR: FILES ARRAY IS LEN OF " +imageFiles.Length.ToString()+ "\n*******************************************");
    
        /*
        foreach (string name in imageFiles)
        {
            Debug.Log("*********************************************\nFOREACH: last good int is" + lastGoodInt.ToString() +  "\n*******************************************");
            Debug.Log("*********************************************\n FOREACH the name is " + name + "\n*******************************************");
            lastGoodInt += 1;
        }
        
        return true;
    }
    */
    /// <summary>
    /// Takes in a hash set of unique file names and deletes the corresponding files
    /// </summary>
    /// <param name="filesToDelete"></param>
    private static void DeletePicturesFromDir(List<string> filesToDelete, string directory, string word)
    {
        foreach (string fileName in filesToDelete)
        {
            Debug.Log("******************************\n DELETEING FILE " +fileName+ "\n****************************");
            File.Delete(directory + "/" + word + "/" + fileName);
        }
    }

    /// <summary>
    /// Deletes the target content sub directory.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="directory"></param>
    private static bool DeleteDirectory(string fileName, string directory)
    {
        directory = directory + "/" + fileName.ToLower() + "/";

        if (!Directory.Exists(directory))
        {
            //TODO: User feedback of failed attempt
            Debug.Log("Attempting to delete non existent directory for " + directory);
            return true;
        }
        else
        {
            Debug.Log("Deleting directory for " + directory);
            Directory.Delete(directory);
            return !Directory.Exists(directory);
        }
    }

    /// <summary>
    /// Takes a word and a directory and returns a random byte array from a choice of 
    /// the different word image files. Returns null if directory or file does not exist.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="directory"></param>
    /// <returns>byte array</returns>
    private static byte[] LoadRandomPic(string fileName, string directory)
    {
        // Create a directory path of directory/<word>/ to access that word's image dir
        directory = directory + "/" + fileName.ToLower() + "/";
        //int x = 1;
        //bool moreEntries = true;

        Debug.Log("COM: created image path for word:" + fileName + ". Path of directory = ");


        if (!Directory.Exists(directory))
        {
            Debug.Log("COM: Trying to load images, but directory doesn't exist.");
            return null;
        }

        /*
        filePath = dir + fileName.ToLower() ".png";

        do
        {
            Debug.Log("*******************************************\nCOM: DO WHILE PATH IS: " + filePath + "\n********************************************");

            if (File.Exists(filePath))
            {
                x += 1;
            }
            else
            {
                // If we are on the first check and no files are present return null
                if (x == 1) return null;

                moreEntries = false;
            }

            filePath = dir + x + ".png";
        } while (moreEntries);

        if ((x - 1) > 1)
        {
            System.Random rand = new System.Random();
            filePath = dir + (rand.Next(1, (x - 1)).ToString()) + ".png";
            Debug.Log("*******************************************\nCOM: RANDO FILE PATH IS: " + filePath + "\n********************************************");
        }
        else
        {
            filePath = dir + 1 + ".png";
        }
        */
    
        // Grab all the paths of the .png files in the current word image directory
        string[] tempList = Directory.GetFiles(directory, "*.png");
        foreach (string filename in tempList)
        {
            Debug.Log("COM: Found " + filename + " and added to templist");
        }

        // Create a new system random object
        System.Random rand = new System.Random();

        // Choose a random .png file to read bytes from
        byte[] bytes = File.ReadAllBytes(tempList[rand.Next(0, tempList.Length - 1)]);


        if (bytes.Length == 0)
        {
            Debug.Log("COM: BYTES LENGTH WAS 0!");
            bytes = null;
        }

        return bytes;
    }

    /// <summary>
    /// This function takes a file name, directory string, and list of Texture2D and 
    /// saves each texture to the given directory with incremented file names. 
    /// Returns an int representing the number of failed write attempts.
    /// </summary>
    /// <param name="list"></param>
    /// <param name="fileName"></param>
    /// <param name="directory"></param>
    /// <returns>int</returns>
    private static List<string> SavePics(List<Texture2D> list, string fileName, string directory, List<string> namesList = null)
    {
        directory = directory + "/" + fileName.ToLower() + "/";

        Debug.Log("COMFILE: NEW DIR IS " + directory);

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        //string dirStr = directory + fileName.ToLower();
        string filePath;
        int x = 2;
        List<string> createdFileNames = new List<string>();

        for (int y = 0; y < list.Count; y++)
        {
            if (namesList == null || namesList[y] == null)
            {
                filePath = directory + fileName.ToLower() + CreateUniqueSuffix(x) + ".png";

                Debug.Log("*******************\nCOMFILE: FILE PATH IS NOW " + filePath + "\n&&&&&&&&&&&&&&&&&&&&&&&&&&");

                // Encode to a PNG
                //byte[] bytes = photo.EncodeToPNG();
                byte[] bytes = list[y].EncodeToPNG();

                // Write out the PNG to the file path.
                File.WriteAllBytes(filePath, bytes);

                if (File.Exists(filePath))
                {
                    createdFileNames.Add(filePath);
                }

                x += 1;
            }
        }

        return createdFileNames;
    }

    /// <summary>
    /// Takes a file path as a string and returns an arry of the file paths located within that dir.
    /// </summary>
    /// <param name="dir"></param>
    /// <returns>string[]</returns>
    private static string[] GetImageFilePathArray(string dir) { return Directory.GetFiles(dir, "*.png"); }

    /// <summary>
    /// This function takes a filename of a png image and a directory string to 
    /// search for each image bearing that name in the given directory. It returns
    /// a List of byte arrays.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="directory"></param>
    /// <returns>Byte[] list</returns>
    private static List<byte[]> RetrievePics(string fileName, string directory)
    {
        Debug.Log("++++++++++++++++++++++++++++++++\n COM: RETRIEVING PICS\n ****************************");

        List<byte[]> tempList = new List<byte[]>();

        directory = directory + "/" + fileName.ToLower() + "/";
        //string filePath;

        Debug.Log("++++++++++++++++++++++++++++++++\n COMFILE: NEW DIR PATH IS " +directory+ "\n ****************************");

        if (!Directory.Exists(directory))
        {
            Debug.Log("++++++++++++++++++++++++++++++++\n COM: DIR DOES NOT EXIST\n ****************************");
            return null;
        }

        string[] nameArray = GetImageFilePathArray(directory);

        foreach (string name in nameArray)
        {
            Debug.Log("++++++++++++++++++++++++++++++++\n COM: READING BYTES FOR NAME "+name+" \n ****************************");

            byte[] bytes = File.ReadAllBytes(name);

            if (bytes.Length > 0)
            {
                Debug.Log("++++++++++++++++++++++++++++++++\n FILECOM: BYTE LENGTH IS GOOD ADDING TO TEMPLIST! \n ****************************");

                tempList.Add(bytes);
            }
            else
            {
                Debug.Log("++++++++++++++++++++++++++++++++\n FILECOM: byte LENGTH FOR " +name+ " IS 0 \n ****************************");
            }
        }

        return tempList;

        /*
        for (int x = 1; ; x++)
        {
            filePath = dirStr + x + ".png";
            Debug.Log("++++++++++++++++++++++++++++++++\n COM: NEW FILEPATH IS "+filePath+" \n ****************************");

            if (!Directory.Exists(directory))
            {
                //TODO: User feedback of failed attempt
                return null;
            }
            else if (!File.Exists(filePath))
            {
                // Check if templist has any byte arrays in it and if so return the list
                if (tempList.Count > 0)
                {
                    Debug.Log("++++++++++++++++++++++++++++++++\n COM: HIT THE END RET LIS x IS "+x.ToString()+"\n ****************************");
                    return tempList;
                }
                else
                {
                    //TODO: User feedback of failed attempt
                    return null;
                }
            }

            Debug.Log("++++++++++++++++++++++++++++++++\n COM: READING NEW BYTES! \n ****************************");

            byte[] bytes = File.ReadAllBytes(filePath);

            if (bytes.Length > 0)
            {
                Debug.Log("++++++++++++++++++++++++++++++++\n FILECOM: BYTE LENGTH IS GOOD ADDING TO TEMPLIST! \n ****************************");

                tempList.Add(bytes);
            }
            else 
            {
                Debug.Log("++++++++++++++++++++++++++++++++\n FILECOM: byte LENGTH IS 0 \n ****************************");
            }

            Debug.Log("++++++++++++++++++++++++++++++++\n FILECOM: BYTE LENGTH IS " + bytes.Length.ToString() + "\n ****************************");

        }
        */
    }

    //TODO: Fix false positive when overwriting existing files
    /// <summary>
    /// This method takes in a texture2D, file name, and the file directory to build a byte array
    /// and write the array to the proper path. Returns true if the file exists afterwards.
    /// </summary>
    /// <param name="photo"></param>
    /// <param name="fileName"></param>
    /// <param name="fileDir"></param>
    /// <returns>bool true if successful</returns>
    private static bool SavePhotoToDir(Texture2D photo, string fileName, string fileDir)
    {
        //To be used once on first play
        if (!Directory.Exists(Application.persistentDataPath + "/WordPictures"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/WordPictures");
        }


        if (!Directory.Exists(fileDir))
        {
            Directory.CreateDirectory(fileDir);
        }

        string filePath = fileDir + "/" + fileName.ToLower() + ".png";

        // Encode to a PNG
        //byte[] bytes = photo.EncodeToPNG();
        byte[] bytes = photo.EncodeToPNG();

        // Write out the PNG to the file path.
        File.WriteAllBytes(filePath, bytes);

        return File.Exists(filePath);
    }

    // TODO: Find refs to this method and change them over to the new setup
    public static byte[] LoadPhoto(string fileName)
    {
        string filePath = PICTURE_DIRECTORY + "/" + fileName.ToLower();

        if (!Directory.Exists(PICTURE_DIRECTORY))
        {
            //TODO: User feedback of failed attempt
            return null;
        }
        else if (!File.Exists(filePath))
        {
            //TODO: User feedback of failed attempt
            return null;
        }

        byte[] bytes = File.ReadAllBytes(filePath);

        return bytes;
    }

    /// <summary>
    /// Reads and returns the byte array from the given file directory. Returns null if 
    /// file could not be recovered.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="fileDir"></param>
    /// <returns>byte[]</returns>
    private static byte[] LoadPhotoByDir(string fileName, string fileDir)
    {
        string filePath = fileDir + "/" + fileName.ToLower() + ".png";

        if (!Directory.Exists(fileDir))
        {
            //TODO: User feedback of failed attempt
            Debug.Log("Error - Photo Directory doesn't exist!");
            return null;
        }
        else if (!File.Exists(filePath))
        {
            //TODO: User feedback of failed attempt
            Debug.Log("Error - Photo doesn't exist!");
            return null;
        }

        byte[] bytes = File.ReadAllBytes(filePath);

        return bytes;
    }

    //TODO: Fix false positive when overwriting existing files
    // Maybe a check between the files date metadata and the current time?
    /// <summary>
    /// This method takes in a AudioClip, file name, and file directory to create a 
    /// byte array and write it to the proper directory. Returns true if the file 
    /// is successfully saved.
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="fileName"></param>
    /// <param name="fileDir"></param>
    /// <returns>bool</returns>
    private static bool SaveAudioToDir(AudioClip clip, string fileName, string fileDir)
    {
        //Check if the word audio directory exists, and if not then create it. This should only be needed when playing for first time.
        if (!Directory.Exists(Application.persistentDataPath + "/WordAudio"))
        {
            Debug.Log("Creating the /WordAudio directory");
            Directory.CreateDirectory(Application.persistentDataPath + "/WordAudio");
        }

        //Check if
        if (!Directory.Exists(fileDir))
        {
            Debug.Log("Creating new directory for " + fileName + " at: " + fileDir + " for audioclip:" + clip.name);
            Directory.CreateDirectory(fileDir);
        }
        else
        {
            Debug.Log("Error: That directory already exists!");
        }

        // Convert audio clip to wav encoded bytes
        byte[] bytes = WavEncoder.FromAudioClip(clip);

        // Get the file path and write bytes to the file
        string filePath = fileDir + "/" + fileName.ToLower() + ".wav";
        File.WriteAllBytes(filePath, bytes);

        return File.Exists(filePath);
    }

    private static bool SaveDLCAudioToDir(byte[] audiofile, string fileName, string fileDir)
    {
        if (!Directory.Exists(Application.persistentDataPath + "/WordAudio"))
        {
            Debug.Log("Creating the /WordAudio directory");
            Directory.CreateDirectory(Application.persistentDataPath + "/WordAudio");
        }

        if (!Directory.Exists(fileDir))
        {
            Directory.CreateDirectory(fileDir);
        }

        // Get the file path and write bytes to the file
        string filePath = fileDir + "/" + fileName.ToLower() + ".wav";
        File.WriteAllBytes(filePath, audiofile);

        return File.Exists(filePath);

    }

    /// <summary>
    /// Loads an audio clip from a given directory. Returns null if the audio clip 
    /// could not be loaded.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="fileDir"></param>
    /// <returns>AudioClip</returns>
    private static AudioClip LoadAudioByDir(string fileName, string fileDir)
    {
        AudioClip clip = null;
        string filePath = string.Format("{0}/{1}", fileDir, fileName.ToLower());
        
        if (File.Exists(filePath))
        {
            clip = WavEncoder.ToAudioClip(filePath);
        }
        else
        {
            return null;
        }

        return clip;
    }

    /// <summary>
    /// Deletes a file from the given directory.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="fileDir"></param>
    private static void DeleteAudioFromDir(string fileName, string fileDir)
    {
        string filePath = string.Format("{0}/{1}.wav", fileDir, fileName.ToLower());

        if (File.Exists(filePath)) File.Delete(filePath);
    }

    /// <summary>
    /// Deletes an image from the given directory.
    /// </summary>
    /// <param name="fileName"></param>
    private static void DeleteImageFromDir(string fileName, string fileDir)
    {
        string filePath = string.Format("{0}/{1}.png", fileDir, fileName.ToLower());

        if (File.Exists(filePath)) File.Delete(filePath);
    }

    private static int CreateUniqueSuffix(int x) { return (currentTime.Month * x) + (currentTime.Day * x) + (currentTime.Hour * x) + (currentTime.Minute * x) + (currentTime.Second * x); }

    public static List<string> GetAndroidGalleryPaths()
    {
        Debug.Log("In FILE ACCESS");
        List<string> results = new List<string>();
        HashSet<string> allowedExtesions = new HashSet<string>() { ".png", ".jpg", ".jpeg" };
        
        Debug.Log("Moving into try!");

        try
        {
            Debug.Log("Instantiating mediaClass!");
            AndroidJavaClass mediaClass = new AndroidJavaClass("android.provider.MediaStore$Images$Media");

            // Set the tags for the data we want about each image.  This should really be done by calling; 
            //string dataTag = mediaClass.GetStatic<string>("DATA");
            // but I couldn't get that to work...

            const string dataTag = "_data";

            Debug.Log("Instantiating the UnityPlayer!");
            string[] projection = new string[] { dataTag };
            AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            Debug.Log("Getting currentactivity!");
            AndroidJavaObject currentActivity = player.GetStatic<AndroidJavaObject>("currentActivity");
            
            string[] urisToSearch = new string[] { "EXTERNAL_CONTENT_URI","INTERNAL_CONTENT_URI" };
            Debug.Log("Entering for loop!");
            foreach (string uriToSearch in urisToSearch)
            {
                AndroidJavaObject externalUri = mediaClass.GetStatic<AndroidJavaObject>(uriToSearch);
                AndroidJavaObject finder = currentActivity.Call<AndroidJavaObject>("managedQuery", externalUri, projection, null, null, null);
                bool foundOne = finder.Call<bool>("moveToFirst");
                while (foundOne)
                {
                    int dataIndex = finder.Call<int>("getColumnIndex", dataTag);
                    string data = finder.Call<string>("getString", dataIndex);
                    if (allowedExtesions.Contains(Path.GetExtension(data).ToLower()))
                    {
                        string path = @"file:///" + data;
                        results.Add(path);
                    }

                    foundOne = finder.Call<bool>("moveToNext");
                }
            }
        }
        catch
        {
            Debug.Log("Error - Not on Android");
        }

        if (results == null)
        {
        }
        else if (results.Count <= 0)
        {
        }
        else
        {
        }
        return results;
    }
}