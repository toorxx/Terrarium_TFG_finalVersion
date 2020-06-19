using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Area : MonoBehaviour
{
    public int _initial_plants;
    public int _initial_herbivores;
    public int _initial_carnivores;
    public int _min_plants;
    public int _time_before_herb;
    public int _time_before_carn;
    public int _time_before_agents_deactivation;
    public int _time_between_fileWrites;
    private bool agentsDeactivated;
    public bool autoReinstanceLast;
    public List<GameObject> Plants;
    public List<GameObject> Herbivores;
    public List<GameObject> Carnivores;
    public Plant plantPrefab;
    public Herbivore herbivorePrefab;
    public Carnivore carnivorePrefab;

    private static Area InstanceArea;
    public GameObject controlledGO;

    public List<int> carnivoreCount;
    public List<int> herbivoreCount;

    private int updateCounter;


    private void Awake() 
    {
        InstanceArea = this;
        Plants = new List<GameObject>();
        Herbivores = new List<GameObject>();
        Carnivores = new List<GameObject>();
        controlledGO = new GameObject();
        agentsDeactivated = false;

        carnivoreCount = new List<int>();
        herbivoreCount = new List<int>();

        updateCounter = 0;

        for(int i = 0; i < _initial_plants; i++)
        {
            Instantiate(plantPrefab, GetRandomPos(), Quaternion.identity, transform);
        }
        StartCoroutine(WaitToInstantiate(_time_before_herb, "herbivore"));
        StartCoroutine(WaitToInstantiate(_time_before_carn, "carnivore"));
        StartCoroutine(SetDeactivation(_time_before_agents_deactivation));
        StartCoroutine(writeAgentsCount(_time_between_fileWrites));
        StartCoroutine(manualInstantiation());
    }

    private void OnApplicationQuit(){
        Debug.Log("DESTROY");
        writeListToFile();
    }

    // Update is called once per frame
    void Update()
    {
        
        //Debug.Log(Time.time);
        // add plants randomly at x steps of time
       /*  if (Time.frameCount % 1 == 0)
        {
            if(Plants.Count < _min_plants)
                Instantiate(plantPrefab, GetRandomPos(), Quaternion.identity, transform);
        } */
   
        if(Plants.Count < _min_plants){
            for (int i = 0; i < _min_plants - Plants.Count; i++){
                Instantiate(plantPrefab, GetRandomPos(), Quaternion.identity, transform);
            }            
        }


        if (autoReinstanceLast && Carnivores.Count <= 0)
        {
            Instantiate(carnivorePrefab, GetRandomPos(), Quaternion.identity, transform);
        }

        // if(updateCounter == _time_between_fileWrites){
        //     writeAgentsCount_old();
        //     updateCounter = 0;
        // }
        // updateCounter++;

        // if(Input.GetKey(KeyCode.H))
        // {
        //     Instantiate(herbivorePrefab, GetRandomPos(), Quaternion.identity, transform);
        // }
        // if(Input.GetKey(KeyCode.C))
        // {
        //     Instantiate(carnivorePrefab, GetRandomPos(), Quaternion.identity, transform);
        // }
        
    }

    public static Area Instance { get { return InstanceArea; }}
    // bear in mind this is a workaround that works because its a rectangular plane
    public Vector2 GetBounds()
    {
        var x = transform.localScale.x * GetComponent<MeshFilter>().mesh.bounds.extents.x;
        var z = transform.localScale.x * GetComponent<MeshFilter>().mesh.bounds.extents.z;
        return(new Vector2(x, z));
    }

    public bool InBounds(float x, float z)
    {
        var localX = transform.localScale.x * GetComponent<MeshFilter>().mesh.bounds.extents.x;
        var localZ = transform.localScale.z * GetComponent<MeshFilter>().mesh.bounds.extents.x;
        return(x < localX && x > -localX && z < localZ && z > -localZ);
    }

    public void AddGameObject(GameObject go)
    {
        if (go.tag == "herbivore")
            Herbivores.Add(go);
        else if (go.tag == "carnivore")
            Carnivores.Add(go);
    }

    public void RemoveGameObject(GameObject go)
    {   
        if (go.tag == "herbivore"){
            Herbivores.Remove(go);
            Debug.Log("Herb removed");
        }
        else if (go.tag == "carnivore")
            Carnivores.Remove(go);
    }

    public Vector3 GetRandomPos()
    {
        var bounds = GetBounds();
        var x = Random.Range(-bounds.x, bounds.x);
        var z = Random.Range(-bounds.y, bounds.y);
        var rand2d = Random.insideUnitCircle * bounds.x;
        return( new Vector3(rand2d.x, 0, rand2d.y) );
    }

    public void MonitorLog()
    {
        MLAgents.Monitor.Log("Num Plants", Plants.Count, transform);
        MLAgents.Monitor.Log("Num herbivores", Herbivores.Count, transform);
        MLAgents.Monitor.Log("Num carnivores", Carnivores.Count, transform);
    }

    // set the agents to deactivate. We use this in order to not instantiate agents when episode is finished
    IEnumerator SetDeactivation(int time)
    {
        yield return new WaitForSeconds(time);
        foreach(var herbivore in Herbivores)
            herbivore.GetComponent<Herbivore>().canDisappear = true;
        foreach(var carnivore in Carnivores)
            carnivore.GetComponent<Carnivore>().canDisappear = true;
    }

    private void setAgentsList(string kind)
    {
        if(kind == "herbivore")
        {
            for (int i = 0; i < _initial_herbivores; i++)
                 Instantiate(herbivorePrefab, GetRandomPos(), Quaternion.identity, transform);
        }
        else if(kind == "carnivore")
        {
            for (int i = 0; i < _initial_carnivores; i++)
                Instantiate(carnivorePrefab, GetRandomPos(), Quaternion.identity, transform);
        }
    }

    IEnumerator WaitToInstantiate(int time, string kind)
    {
        // wait for the numbers of seconds specified by the user before instantiate agernts
        
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSecondsRealtime(time);
        setAgentsList(kind);
    }

    private IEnumerator writeAgentsCount(int time){
        
        while(true){
            yield return new WaitForSeconds(time);
            Debug.Log("Write " + herbivoreCount.Count);
            herbivoreCount.Add(Herbivores.Count);
            carnivoreCount.Add(Carnivores.Count);
        }
    }

    private void writeAgentsCount_old(){
        Debug.Log("Write " + herbivoreCount.Count);
        herbivoreCount.Add(Herbivores.Count);
        carnivoreCount.Add(Carnivores.Count);
    
    }

    private void writeListToFile(){
        //StopCoroutine("writeAgentsCount");
        string filePath = Application.dataPath + "/results/" + "last.csv";
        StreamWriter writer = new StreamWriter(filePath);
        writer.AutoFlush = true;

        writer.WriteLine("# Herbivores"+","+"# Carnivores");

        for( int i = 0; i < herbivoreCount.Count; i++){
            writer.WriteLine(herbivoreCount[i]+","+carnivoreCount[i]);
        }

    }

    private IEnumerator manualInstantiation(){
        while(true){
            yield return new WaitForSeconds(0.2f);
            if(Input.GetKey(KeyCode.H))
            {
                Instantiate(herbivorePrefab, GetRandomPos(), Quaternion.identity, transform);
            }
            if(Input.GetKey(KeyCode.C))
            {
                Instantiate(carnivorePrefab, GetRandomPos(), Quaternion.identity, transform);
            }
            if(Input.GetKey(KeyCode.Z))
            {
                autoReinstanceLast = false;
            }

        }
    }

    


}
