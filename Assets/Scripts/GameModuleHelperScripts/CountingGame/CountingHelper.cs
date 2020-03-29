using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CountingHelper : AB_GameHelper {
    #region Member Variables
    
    // Serialized fields
    [SerializeField] Canvas parentCanvas;
    [SerializeField] GameObject spawnPanel;
    [SerializeField] GameObject basket;
    [SerializeField] Text objectiveText;
    [SerializeField] Text objectiveProgressText;
    [SerializeField] VW_GameLoop gameLoop;

    // Data objects
    private DO_PlayListObject playObj;
    private DO_CountingGame cg;

    // Game logic data
    private List<string> distractorItems;
    private List<string> spawnItems;
    private List<GameObject> itemObjects;
    private string collectItem;
    private int collectNum;
    private int itemMax;
    private int collectedItemNum;
    private int mistakeChain;
    public int mistakeCount;

    // Spawning logic data
    public bool isSpawnSet;
    public float parentWidth;
    public float parentHeight;
    public float largeOffsetX;
    public float largeOffsetY;
    public float smallOffsetY;

    // Feature enabling data
    private bool hints;
    private bool countAudio;
    private bool startAudio;
    private bool defaultImg;

    #endregion

    #region Messenger Registration 

    /// <summary>
    /// This function is used to setup messenger listeners.
    /// </summary>
    private void Start() {
        isSpawnSet = false;
        ClearItems();
        Resume();
    }

    #endregion

    #region One-Line Functions 

    /// <summary>
    /// This function returns true if all items have been collected.
    /// </summary>
    /// <returns>bool</returns>
    private bool IsGameOver() { return collectedItemNum >= collectNum; }

    #endregion

    #region Member Functions

    private void ClearItems() {
        foreach (GameObject item in itemObjects) {
            Destroy(item);
        }

        distractorItems.Clear();
        spawnItems.Clear();
        itemObjects.Clear();
    }

    /// <summary>
    /// The purpose of this function is to handle the game logic based on where an item has been dropped 
    /// on the screen. It is called when the "CountingItemDropped" listener receives a message from the 
    /// "OnEndDrag" function of a "DragHandlerCounting.cs" script. These scripts are attached
    /// to CountingItem prefabs, and will pass a reference to themselves when activated. 
    /// </summary>
    /// <param name="dropItem"></param>
    public void HandleItemDrop(GameObject dropItem) {
        // Get the dragHandlerCounting script and XY position of the dropItem
        DragHandlerCounting dh = dropItem.GetComponent<DragHandlerCounting>();
        float dropItemX = dropItem.transform.localPosition.x;
        float dropItemY = dropItem.transform.localPosition.y;

        // Check to see if the dropItem is out of bounds
        bool outOfBounds = false;
        if ((largeOffsetX * -1 <= dropItemX) && (dropItemX <= largeOffsetX)) { // dropItem is within spawnable X values
            if ((smallOffsetY * -1 <= dropItemY) && (dropItemY <= largeOffsetY)) { // dropItem is within spawnable Y values
                // Clean up the drop
                Debug.Log("Position Safe: " + dropItem.name);
                dh.Clean();
            } else { // dropItem is out of bounds: Y position
                Debug.Log("Out of bounds: " + dropItem.name + " (Y) " + dropItemY);
                outOfBounds = true;
            }
        } else { // dropItem is out of bounds: X position
            Debug.Log("Out of bounds: " + dropItem.name + " (X) " + dropItemX);
            outOfBounds = true;
        }

        if (outOfBounds) {
            // Check if the correct item was dropped in the basket
            bool correctDrop = false;
            if ((-150 <= dropItemX) && (dropItemX <= 150)) { // dropItem dropped within X values of basket
                if (dropItemY <= smallOffsetY * -1) { // dropItem dropped within Y values of basket
                    if (dropItem.name == collectItem) { // the dropItem is correct
                        correctDrop = true;
                    }
                }
            }

            if (correctDrop) { // Update the game, destroy the item, then check if the game is done
                collectedItemNum++;
                mistakeCount += mistakeChain;
                mistakeChain = 0;

                UpdateProgress(mistakeChain);

                if (countAudio) {
                    gameLoop.PlayAudio(collectedItemNum.ToString());
                }

                Destroy(dropItem);

                if (IsGameOver()) {
                    StartCoroutine(GameOver());
                }
            } else { // Put dropItem back where it was dragged from
                mistakeChain++;
                UpdateProgress(mistakeChain);
                dh.ResetPos();
            }
        }
    }

    /// <summary>
    /// This function uses the fullscreen spawnPanel canvas ready as part of the game's prefab to 
    /// determine the safe X/Y center offsets for spawning collectible items.
    /// </summary>
    private void SetSpawnArea() {
        if (!isSpawnSet) {
            // Get the width and height of the parent canvas
            RectTransform parentTransform = spawnPanel.transform.GetComponent<RectTransform>();
            parentWidth = parentTransform.rect.width;
            parentHeight = parentTransform.rect.height;

            // Determine X/Y offsets by subtracting a marginal amount
            largeOffsetX = (parentWidth / 2) - 50;
            largeOffsetY = (parentHeight / 2) - 50;
            smallOffsetY = (parentHeight / 2) - 225;

            isSpawnSet = true;
        }
    }

    /// <summary>
    /// This function sets up the remaining game data and objects after the SetupFromString(string json) 
    /// function is called.
    /// </summary>
    private void Setup() {
        // Setup non-JSON game data
        collectedItemNum = 0;
        mistakeChain = 0;
        mistakeCount = 0;

        // Setup the list of items to spawn
        spawnItems = new List<string>();
        for (int i = 0; i < itemMax; i++) {
            if (i < collectNum) {
                spawnItems.Add(collectItem);
            } else {
                spawnItems.Add(distractorItems[i % distractorItems.Count]);
            }
        }
        spawnItems.Shuffle();

        // Determine the spawnable area
        SetSpawnArea();

        // Spawn Items on the screen
        itemObjects = new List<GameObject>();
        SpawnItems(spawnItems);

        // Update objective information
        objectiveText.text = string.Format("Collect:\n{0} {1}s", collectNum, collectItem);
        UpdateProgress(0);

        if (startAudio) {
            // Play starting audio clips
        }

        Debug.Log("Counting Start...");
    }

    /// <summary>
    /// This function is used to setup the customizable game data once it receives JSON data 
    /// from the game loop. A following Setup() function is called to complete non-JSON setup
    /// and start the game.
    /// </summary>
    /// <param name="json"></param>
    public void SetupFromString(string json) {
        Debug.Log(json);

        cg = JsonUtility.FromJson<DO_CountingGame>(json);

        // Setup game data
        itemMax = cg.itemMax;
        collectNum = cg.collectNum;
        collectItem = gameLoop.controller.GetWordById(cg.collectWordId);

        distractorItems = new List<string>();
        foreach (int wordId in cg.wordIds) {
            if (wordId != cg.collectWordId) {
                distractorItems.Add(gameLoop.controller.GetWordById(wordId));
            }
        }
        distractorItems.Shuffle();

        // Setup extra features
        hints = cg.hints;
        countAudio = cg.countAudio;
        startAudio = cg.startAudio;
        defaultImg = cg.defaultImg;

        // Finish setup and start the game
        Setup();
    }

    /// <summary>
    /// This function takes the list of items passed in, and spawns them randomly within the safe
    /// X/Y offests of the screen.
    /// </summary>
    /// <param name="items"></param>
    private void SpawnItems(List<string> items) {
        foreach (string item in items) {
            // Create a randomized spawn position within the spawnable area
            Vector3 pos = new Vector3(UnityEngine.Random.Range(largeOffsetX * -1, largeOffsetX), UnityEngine.Random.Range(smallOffsetY * -1, largeOffsetY), 0.0f);

            // Instantiate a game object for the item
            GameObject obj = GameObject.Instantiate((GameObject)Resources.Load("CountingItem"), spawnPanel.transform);
            obj.transform.localScale = new Vector3(0.45f, 0.45f, 1.0f);
            obj.transform.localPosition = pos;
            obj.name = item;

            if (defaultImg) { // use the first object image
                gameLoop.SetManualImage(item, obj, 1);
            } else { // use a random image
                gameLoop.SetImage(item, obj);
            }
            
            itemObjects.Add(obj);
        }
    }

    /// <summary>
    /// This function is used to update the adaptive difficulty functionality and the objective
    /// progress whenever an item is dropped in the basket.
    /// </summary>
    /// <param name="mistakes"></param>
    private void UpdateProgress(int mistakes) {
        switch (mistakes) {
            case 0:  // A new point was just scored
                // Update the objectiveProgressText with the new collectedItemNum
                objectiveProgressText.text = string.Format("{0} / {1}", collectedItemNum, collectNum);

                if (hints) {
                    // Un-ghost all incorrect items
                    foreach (GameObject item in itemObjects) {
                        if (item != null) {
                            if (item.name != collectItem) {
                                RawImage itemImg = item.GetComponent<RawImage>();
                                itemImg.raycastTarget = true;
                                itemImg.color = new Color32(255, 255, 255, 255);
                            }
                        }
                    }
                }

                break;
            case 4: // 4 mistakes made consecutively
                if (hints) {
                    // Ghost all incorrect items
                    foreach (GameObject item in itemObjects) {
                        if (item != null) {
                            if (item.name != collectItem) {
                                RawImage itemImg = item.GetComponent<RawImage>();
                                itemImg.raycastTarget = false;
                                itemImg.color = new Color32(255, 255, 255, 127);
                            }
                        }
                    }
                }

                break;
            default:
                break;
        }
    }

    #endregion

    #region Function Overrides

    public override void Resume() {
        playObj = gameLoop.GetCurrentPlay();
        SetupFromString(playObj.json);
    }

    #endregion

    #region Coroutines

    /// <summary>
    /// This function is called when all correct items have been collected. It makes sure that any 
    /// remaining audio clips finish playing before signalling that teh game is done.
    /// </summary>
    /// <returns></returns>
    private IEnumerator GameOver() {
        yield return new WaitWhile(() => gameLoop.audioSource.isPlaying);
        ClearItems();
        gameLoop.PlayEntryCompleted(playObj.type_id);
    }

    #endregion
}
