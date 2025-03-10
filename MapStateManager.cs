﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Haoran 
using System.IO;


/// <summary>
/// MapStateManager is the place to keep a succession of events or "states" when building 
/// a multi-step AI demo. Note that this is a way to manage 
/// 
/// State changes could happen for one of two reasons:
///     when the user has pressed a number key 0..9, desiring a new phase
///     when something happening in the game forces a transition to the next phase
/// 
/// One use will be for AI demos that are switched up based on keyboard input. For that, 
/// the number keys 0..9 will be used to dial in whichever phase the user wants to see.
/// </summary>

enum Behaviors
{
    wander = 1,
    Evade = 2,
    Pursue = 3,
    CheckDistance = 4,
    Arrive = 5,
    PathFollow = 6,
    seekWithArrive = 8,
    // Saturday =5,
    // Sunday = 6
}

public class MapStateManager : MonoBehaviour {
    // Set prefabs
    public GameObject PlayerPrefab;     // You, the player
    public GameObject HunterPrefab;     // Agent doing chasing
    public GameObject WolfPrefab;       // Agent getting chased
    public GameObject RedPrefab;        // Red Riding Hood, or just "team red"
    public GameObject BluePrefab;       // "team blue"
    public GameObject TreePrefab;       // New for Assignment #2

    public NPCController house;         // for future use
    //public NPCController player;
    public GameObject player;

    // Set up to use spawn points. Can add more here, and also add them to the 
    // Unity project. This won't be a good idea later on when you want to spawn
    // a lot of agents dynamically, as with Flocking and Formation movement.

    public GameObject spawner1;
    public Text SpawnText1;
    public GameObject spawner2;
    public Text SpawnText2;
    public GameObject spawner3;
    public Text SpawnText3;

    public int TreeCount;
 
    private List<GameObject> spawnedNPCs;   // When you need to iterate over a number of agents.
    private List<GameObject> trees;

    private int currentPhase = 0;           // This stores where in the "phases" the game is.
    private int previousPhase = 0;          // The "phases" we were just in

    //public int Phase => currentPhase;

    LineRenderer line;                 
    public GameObject[] Path;
    public Text narrator;                   // 

    // Use this for initialization. Create any initial NPCs here and store them in the 
    // spawnedNPCs list. You can always add/remove NPCs later on.

    void Start() {
        narrator.text = "This is the place to mention major things going on during the demo, the \"narration.\"";

        //TreeCount = 20;    // TreeCount isn't showing up in Inspector

        trees = new List<GameObject>();
        //SpawnTrees(TreeCount);
        SpawnTreesFromFile("treePositions20.txt");
        spawnedNPCs = new List<GameObject>();
        //spawnedNPCs.Add(SpawnItem(spawner1, HunterPrefab, null, SpawnText1, 4));
        //Invoke("SpawnWolf", 1);
        //Invoke("Meeting1", 2);
        EnterMapStateZero();
        player = GameObject.FindGameObjectWithTag("Player");

        //CreatePath();
    }

    /// <summary>
    /// This is where you put the code that places the level in a particular phase.
    /// Unhide or spawn NPCs (agents) as needed, and give them things (like movements)
    /// to do. For each case you may well have more than one thing to do.
    /// </summary>
    private void Update()
    {
        int num;

        string inputstring = Input.inputString;
        if (inputstring.Length > 0)
        {
            Debug.Log(inputstring);

            if (inputstring[0] == 'R')
            {
                DestroyTrees();
                SpawnTrees(20);
            }

            // Look for a number key click
            if (inputstring.Length > 0)
            {
                if (Int32.TryParse(inputstring, out num))
                {
                    if (num != currentPhase)
                    {
                        previousPhase = currentPhase;
                        currentPhase = num;
                    }
                }
            }
        }
        // Check if a game event had caused a change of phase.
        if (currentPhase == previousPhase)
            return;


        /************* FRAMEWORK VERSION
       // If we get here, we've been given a new phase, from either source
       switch (currentPhase) {
           case 0:
               EnterMapStateZero();
               break;

           case 1:
               EnterMapStateOne();
               break;

           case 2:
               EnterMapStateTwo();
               break;

           case 3:
               break;
       }
       **************/

        switch (currentPhase)
        {
            case 0:
            //if (spawnedNPCs.Count > 1 && Vector3.Distance(spawnedNPCs[1].transform.position, spawnedNPCs[0].transform.position) < 12)
            //{
            //    narrator.text = "The Hunter spots the wolf and believes it is his target. The Wolf runs.";
            //    spawnedNPCs[0].GetComponent<SteeringBehavior>().target = spawnedNPCs[1].GetComponent<NPCController>();
            //    spawnedNPCs[1].GetComponent<SteeringBehavior>().target = spawnedNPCs[0].GetComponent<NPCController>();
            //    spawnedNPCs[0].GetComponent<NPCController>().phase = 1;
            //    spawnedNPCs[1].GetComponent<NPCController>().phase = 2;
            //    currentPhase++;
            //}
                EnterMapStateZero();
                break;
            case 1:
                EnterMapStateOne();
                //if (Vector3.Distance(spawnedNPCs[1].transform.position, spawnedNPCs[0].transform.position) < 2)
                //{
                //    narrator.text = "Both the Hunter and Wolf move to another area. Little Red arrives and moves to her house.";
                //    spawnedNPCs[0].GetComponent<NPCController>().label.enabled = false;
                //    spawnedNPCs[0].GetComponent<NPCController>().DestroyPoints();
                //    spawnedNPCs[0].SetActive(false);
                //    spawnedNPCs[1].GetComponent<NPCController>().label.enabled = false;
                //    spawnedNPCs[1].GetComponent<NPCController>().DestroyPoints();
                //    spawnedNPCs[1].SetActive(false);
                //    spawnedNPCs.Add(SpawnItem(spawner3, RedPrefab, null, SpawnText3, 5));
                //    CreatePath();
                //    Invoke("SpawnWolf2", 10);
                //    currentPhase++;
                //}
                break;
            case 2:
                EnterMapStateTwo();
                //if (spawnedNPCs.Count > 3 && Vector3.Distance(spawnedNPCs[2].transform.position, spawnedNPCs[3].transform.position) < 12)
                //{
                //    narrator.text = "Little Red notices the Wolf and moves toward it.";
                //    spawnedNPCs[2].GetComponent<SteeringBehavior>().target = spawnedNPCs[3].GetComponent<NPCController>();
                //    SetArrive(spawnedNPCs[2]);
                //    SetArrive(spawnedNPCs[3]);
                //    Invoke("Meeting2", 7);
                //    currentPhase++;
                //}
            break;

            case 3:
                EnterMapStateThree();
                //if (Vector3.Distance(spawnedNPCs[2].transform.position, house.transform.position) < 12)
                //{
                //    spawnedNPCs[2].GetComponent<SteeringBehavior>().target = house;
                //    SetArrive(spawnedNPCs[2]);
                //}
                //if (Vector3.Distance(spawnedNPCs[2].transform.position, house.transform.position) < 2)
                //{
                //    spawnedNPCs[2].GetComponent<NPCController>().DestroyPoints();
                //    spawnedNPCs[2].GetComponent<NPCController>().label.enabled = false;
                //    spawnedNPCs[2].SetActive(false);
                //}
                //if (Vector3.Distance(spawnedNPCs[3].transform.position, house.transform.position) < 12)
                //{
                //    SetArrive(spawnedNPCs[3]);
                //}
                //if (Vector3.Distance(spawnedNPCs[3].transform.position, house.transform.position) < 2)
                //{
                //    spawnedNPCs[3].GetComponent<NPCController>().DestroyPoints();
                //    spawnedNPCs[3].GetComponent<NPCController>().label.enabled = false;
                //    spawnedNPCs[3].SetActive(false);
                //}
                //if (spawnedNPCs.Count > 4 && Vector3.Distance(spawnedNPCs[4].transform.position, house.transform.position) < 12)
                //{
                //    SetArrive(spawnedNPCs[4]);
                //}
                //if (spawnedNPCs.Count > 4 && Vector3.Distance(spawnedNPCs[4].transform.position, house.transform.position) < 2)
                //{
                //    spawnedNPCs[4].GetComponent<NPCController>().DestroyPoints();
                //    spawnedNPCs[4].GetComponent<NPCController>().label.enabled = false;
                //    spawnedNPCs[4].SetActive(false);
                //    Invoke("End", 5);
                //}
                break;

            case 4:
                EnterMapStateFour();
                break;
            case 5:
                EnterMapStateFive();
                break;
            case 7:
                EnterMapStateSeven();
                break;
        }
    }


    private void EnterMapStateZero()
    {
        narrator.text = "Mapstate Zero\n20 Trees";

        currentPhase = 0; // or whatever. Won't necessarily advance the phase every time
        previousPhase = 0;

        spawnedNPCs.ForEach(Destroy);
      

    }

    private void EnterMapStateOne() {
        narrator.text = "MapState one\n Hunter appears and wander";

        GameObject hunter = SpawnItem(spawner1, HunterPrefab, null, SpawnText2, 0);
        spawnedNPCs.Add(hunter);

         //hunter
        spawnedNPCs[0].GetComponent<NPCController>().phase = (int)Behaviors.wander;

        hunter.GetComponent<SteeringBehavior>().house = this.house;

        currentPhase = 1;
        previousPhase = 1;

    
    }

    private void EnterMapStateTwo()
    {
        narrator.text = "MapState Two\n Wolf appears and wander";

        GameObject wolf = SpawnItem(spawner2, WolfPrefab, null, SpawnText3, 0);
        spawnedNPCs.Add(wolf);
        // wolf
        spawnedNPCs[1].GetComponent<NPCController>().phase = (int)Behaviors.wander;

        wolf.GetComponent<SteeringBehavior>().house = this.house;

        currentPhase = 2;
        previousPhase = 2;
       
    }

    private void EnterMapStateThree()
    {
        narrator.text = "MapState Three\n";
        
        narrator.text += "The hunter and the wolf wander until the hunter spots the wolf and believes it is his target. The Wolf runs.";
        spawnedNPCs[0].GetComponent<SteeringBehavior>().target = spawnedNPCs[1].GetComponent<NPCController>();
        spawnedNPCs[1].GetComponent<SteeringBehavior>().target = spawnedNPCs[0].GetComponent<NPCController>();
        
        //hunter
        spawnedNPCs[0].GetComponent<NPCController>().phase =(int)Behaviors.CheckDistance;
        //wolf
        spawnedNPCs[1].GetComponent<NPCController>().phase = (int)Behaviors.CheckDistance;
        
        currentPhase = 3;
        previousPhase = 3;

        
    }

    private void EnterMapStateFour()
    {

        narrator.text = "Mapstate Four\n";

        currentPhase = 4; // or whatever. Won't necessarily advance the phase every time
        previousPhase = 4;

        //spawnedNPCs[0].SetActive(true);
        //spawnedNPCs[1].SetActive(true);
        spawnedNPCs.ForEach(Destroy);
        spawnedNPCs.Clear();
        CreatePath();
        GameObject red = SpawnItem(spawner3, RedPrefab, null, SpawnText1, (int)Behaviors.PathFollow);

        spawnedNPCs.Add(red);
        red.GetComponent<SteeringBehavior>().target = this.house.GetComponent<NPCController>();
        red.GetComponent<SteeringBehavior>().house = this.house;

        //DestroyTrees();
    }

     private void EnterMapStateFive()
    {

        narrator.text = "Mapstate Five\n";

        currentPhase = 5; // or whatever. Won't necessarily advance the phase every time
        previousPhase = 5;

       GameObject wolf = SpawnItem(spawner2, WolfPrefab, null, SpawnText3, (int)Behaviors.Pursue);

        spawnedNPCs.Add(wolf);
        //add  spawnedNPCs[0]:red as target
        wolf.GetComponent<SteeringBehavior>().target = spawnedNPCs[0].GetComponent<NPCController>();
        wolf.GetComponent<SteeringBehavior>().house = this.house;
        //DestroyTrees();
    }

    private void EnterMapStateSeven()
    {

        narrator.text = "Mapstate Seven\n";

        currentPhase = 7; // or whatever. Won't necessarily advance the phase every time
        previousPhase = 7;

        GameObject hunter = SpawnItem(spawner1, HunterPrefab, this.house, SpawnText2, (int)Behaviors.seekWithArrive);
        spawnedNPCs.Add(hunter);//[2]
        hunter.GetComponent<SteeringBehavior>().house = this.house;


 
       
    }

    // ... Etc. Etc.

    /// <summary>
    /// SpawnItem placess an NPC of the desired type into the game and sets up the neighboring 
    /// floating text items nearby (diegetic UI elements), which will follow the movement of the NPC.
    /// </summary>
    /// <param name="spawner"></param>
    /// <param name="spawnPrefab"></param>
    /// <param name="target"></param>
    /// <param name="spawnText"></param>
    /// <param name="phase"></param>
    /// <returns></returns>
    private GameObject SpawnItem(GameObject spawner, GameObject spawnPrefab, NPCController target, Text spawnText, int phase)
    {
        Vector3 size = spawner.transform.localScale;
        Vector3 position = spawner.transform.position + new Vector3(UnityEngine.Random.Range(-size.x / 2, size.x / 2), 0, UnityEngine.Random.Range(-size.z / 2, size.z / 2));
        GameObject temp = Instantiate(spawnPrefab, position, Quaternion.identity);
        if (target)
        {
            temp.GetComponent<SteeringBehavior>().target = target;
        }
        temp.GetComponent<NPCController>().label = spawnText;
        temp.GetComponent<NPCController>().phase = phase;
        Camera.main.GetComponent<CameraController>().player = temp;
        return temp;
    }

   

    /// <summary>
    /// SpawnTrees will randomly place tree prefabs all over the map. The diameters
    /// of the trees are also varied randomly.
    /// 
    /// Note that it isn't particularly smart about this (yet): notably, it doesn't
    /// check first to see if there is something already there. This should get fixed.
    /// </summary>
    /// <param name="numTrees">desired number of trees</param>
    private void SpawnTrees(int numTrees)
    {
        float MAX_X = 20;  // Size of the map; ideally, these shouldn't be hard coded
        float MAX_Z = 25;
        float less_X = MAX_X - 1;
        float less_Z = MAX_Z - 1;

        float diameter;

        for (int i = 0; i < numTrees; i++)
        {
            //Vector3 size = spawner.transform.localScale;
            Vector3 position = new Vector3(UnityEngine.Random.Range(-less_X, less_X), 0, UnityEngine.Random.Range(-less_Z, less_Z));

            GameObject temp = Instantiate(TreePrefab, position, Quaternion.identity);

            // diameter will be somewhere between .2 and .7 for both X and Z:
            diameter = UnityEngine.Random.Range(0.5F, 0.7F);
            temp.transform.localScale = new Vector3(diameter, 1.0F, diameter);

            trees.Add(temp);
          
        }
    }

    //Haoran
    //Spawn trees from txt file
    private void SpawnTreesFromFile(String filePathWithName){
            //string path = "Assets/Scripts/treePositions20.txt";
            string path = "Assets/Scripts/"+filePathWithName;
            StreamReader reader = new StreamReader(path); 
            // Debug.Log(reader.ReadToEnd());

            while(!reader.EndOfStream)
            {
                string line = reader.ReadLine( );
                // Do Something with the input. 
                //Debug.Log(line);
                string[] coordinates = line.Split(',');
                //Debug.Log(coordinates[0] + "?" + coordinates[1]+"?"+coordinates[2]);

                Vector3 position = new Vector3(float.Parse(coordinates[0]),float.Parse(coordinates[1]),float.Parse(coordinates[2]));
                GameObject temp = Instantiate(TreePrefab, position, Quaternion.identity);

                float diameter = UnityEngine.Random.Range(0.5F, 0.7F);
                temp.transform.localScale = new Vector3(diameter, 1.0F, diameter);

                trees.Add(temp);
            }

            reader.Close( );  
    }

    private void DestroyTrees()
    {
        GameObject temp;
        for (int i = 0; i < trees.Count; i++)
        {
            temp = trees[i];
            Destroy(temp);
        }
        // Following this, write whatever methods you need that you can bolt together to 
        // create more complex movement behaviors.
    }
    private void SpawnWolf()
    {
        narrator.text = "The Wolf appears. Most wolves are ferocious, but this one is docile.";
        spawnedNPCs.Add(SpawnItem(spawner2, WolfPrefab, null, SpawnText2, 4));
    }

    private void Meeting1() {
        if (currentPhase == 0) {
            spawnedNPCs[0].GetComponent<SteeringBehavior>().target = spawnedNPCs[1].GetComponent<NPCController>();
            spawnedNPCs[1].GetComponent<SteeringBehavior>().target = spawnedNPCs[0].GetComponent<NPCController>();
            SetArrive(spawnedNPCs[0]);
            SetArrive(spawnedNPCs[1]);
        }
    }

    private void SpawnWolf2() {
        narrator.text = "The Wolf looks for shelter, and spots little Red.";
        spawnedNPCs.Add(SpawnItem(spawner3, WolfPrefab, spawnedNPCs[2].GetComponent<NPCController>(), SpawnText1, 1));
        spawnedNPCs[3].GetComponent<NPCController>().label.enabled = true;
    }

    private void Meeting2() {
        narrator.text = "The two converse, and little Red directs the Wolf to her house.";
        spawnedNPCs[2].GetComponent<NPCController>().DestroyPoints();
        spawnedNPCs[2].GetComponent<NPCController>().phase = 5;
        spawnedNPCs[3].GetComponent<SteeringBehavior>().target = house;
        spawnedNPCs[3].GetComponent<NPCController>().phase = 1; ;
        Invoke("SpawnHunter", 10);
    }

    private void SpawnHunter() {
        narrator.text = "The Hunter arrives, determined to catch the killer. He spots a house and moves accordingly.";
        spawnedNPCs.Add(SpawnItem(spawner3, HunterPrefab, house, SpawnText2, 1));
        spawnedNPCs[4].GetComponent<NPCController>().label.enabled = true;
    }

    private void End() {
        narrator.text = "Days later, reports come in. The killer is still at large, but police have found one clue on its identity. "
            +"A little red hood. END";
        currentPhase++;
    }

    private void SetArrive(GameObject character) {

        character.GetComponent<NPCController>().phase = 3;
        character.GetComponent<NPCController>().DrawConcentricCircle(character.GetComponent<SteeringBehavior>().slowRadiusL);
    }

    private void CreatePath()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = Path.Length;
        for (int i = 0; i < Path.Length; i++)
        {
            line.SetPosition(i, Path[i].transform.position);
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(spawner1.transform.position, spawner1.transform.localScale);
        Gizmos.DrawCube(spawner2.transform.position, spawner2.transform.localScale);
        Gizmos.DrawCube(spawner3.transform.position, spawner3.transform.localScale);
    }
}
