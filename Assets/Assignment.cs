
/*
This RPG data streaming assignment was created by Fernando Restituto with 
pixel RPG characters created by Sean Browning.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

#region Assignment Instructions

/*  Hello!  Welcome to your first lab :)

Wax on, wax off.

    The development of saving and loading systems shares much in common with that of networked gameplay development.  
    Both involve developing around data which is packaged and passed into (or gotten from) a stream.  
    Thus, prior to attacking the problems of development for networked games, you will strengthen your abilities to develop solutions using the easier to work with HD saving/loading frameworks.

    Try to understand not just the framework tools, but also, 
    seek to familiarize yourself with how we are able to break data down, pass it into a stream and then rebuild it from another stream.


Lab Part 1

    Begin by exploring the UI elements that you are presented with upon hitting play.
    You can roll a new party, view party stats and hit a save and load button, both of which do nothing.
    You are challenged to create the functions that will save and load the party data which is being displayed on screen for you.

    Below, a SavePartyButtonPressed and a LoadPartyButtonPressed function are provided for you.
    Both are being called by the internal systems when the respective button is hit.
    You must code the save/load functionality.
    Access to Party Character data is provided via demo usage in the save and load functions.

    The PartyCharacter class members are defined as follows.  */

public partial class PartyCharacter
{

    public int classID;

    public int health;
    public int mana;

    public int strength;
    public int agility;
    public int wisdom;

    public LinkedList<int> equipment;

}


/*
    Access to the on screen party data can be achieved via …..

    Once you have loaded party data from the HD, you can have it loaded on screen via …...

    These are the stream reader/writer that I want you to use.
    https://docs.microsoft.com/en-us/dotnet/api/system.io.streamwriter
    https://docs.microsoft.com/en-us/dotnet/api/system.io.streamreader

    Alright, that’s all you need to get started on the first part of this assignment, here are your functions, good luck and journey well!
*/


#endregion


#region Assignment Part 1

static public class AssignmentPart1
{
    enum pcSignifier
    {
        StatsData = 0,
        EquipmentData,
    }

    static public void SavePartyButtonPressed()
    {
        foreach (PartyCharacter pc in GameContent.partyCharacters)
        {
            Debug.Log("PC class id == " + pc.classID);
        }

        // % get a ref of the pc on screen
        LinkedList<PartyCharacter> myPartyCharacters = GameContent.partyCharacters;

        // % Write each pc data on a txt
        using (StreamWriter sw = new StreamWriter(Application.dataPath + Path.DirectorySeparatorChar + "MyParty.txt"))
        {
            foreach (var pc in myPartyCharacters)
            {
                sw.WriteLine((int)pcSignifier.StatsData + "," +
                pc.classID + "," +
                pc.health + "," +
                pc.mana + "," +
                pc.strength + "," +
                pc.agility + "," +
                pc.wisdom);

                foreach (var equip in pc.equipment)
                {
                    sw.WriteLine((int)pcSignifier.EquipmentData + "," + equip);
                }
            }
        }
    }

    static public void LoadPartyButtonPressed()
    {
        // % get a ref of the pc on screen (to add last)
        LinkedList<PartyCharacter> myPartyCharacters = GameContent.partyCharacters;

        myPartyCharacters.Clear();

        using (StreamReader sr = new StreamReader(Application.dataPath + Path.DirectorySeparatorChar + "MyParty.txt"))
        {
            string line;

            while ((line = sr.ReadLine()) != null)
            {
                string[] arrData = line.Split(',');
                int signifier = int.Parse(arrData[0]);

                if (signifier == (int)pcSignifier.StatsData)
                {
                    PartyCharacter pc = new PartyCharacter(int.Parse(arrData[1]),
                    int.Parse(arrData[2]),
                    int.Parse(arrData[3]),
                    int.Parse(arrData[4]),
                    int.Parse(arrData[5]),
                    int.Parse(arrData[6]));
                    myPartyCharacters.AddLast(pc);
                }
                else if (signifier == (int)pcSignifier.EquipmentData)
                {
                    myPartyCharacters.Last.Value.equipment.AddLast(int.Parse(arrData[1]));
                }
            }
        }

        GameContent.RefreshUI();
    }
}


#endregion


#region Assignment Part 2

//  Before Proceeding!
//  To inform the internal systems that you are proceeding onto the second part of this assignment,
//  change the below value of AssignmentConfiguration.PartOfAssignmentInDevelopment from 1 to 2.
//  This will enable the needed UI/function calls for your to proceed with your assignment.
static public class AssignmentConfiguration
{
    public const int PartOfAssignmentThatIsInDevelopment = 2;
}

/*

In this part of the assignment you are challenged to expand on the functionality that you have already created.  
    You are being challenged to save, load and manage multiple parties.
    You are being challenged to identify each party via a string name (a member of the Party class).

To aid you in this challenge, the UI has been altered.  

    The load button has been replaced with a drop down list.  
    When this load party drop down list is changed, LoadPartyDropDownChanged(string selectedName) will be called.  
    When this drop down is created, it will be populated with the return value of GetListOfPartyNames().

    GameStart() is called when the program starts.

    For quality of life, a new SavePartyButtonPressed() has been provided to you below.

    An new/delete button has been added, you will also find below NewPartyButtonPressed() and DeletePartyButtonPressed()

Again, you are being challenged to develop the ability to save and load multiple parties.
    This challenge is different from the previous.
    In the above challenge, what you had to develop was much more directly named.
    With this challenge however, there is a much more predicate process required.
    Let me ask you,
        What do you need to program to produce the saving, loading and management of multiple parties?
        What are the variables that you will need to declare?
        What are the things that you will need to do?  
    So much of development is just breaking problems down into smaller parts.
    Take the time to name each part of what you will create and then, do it.

Good luck, journey well.

*/

static public class AssignmentPart2
{
    enum pcSignifier
    {
        StatsData = 0,
        EquipmentData,
        LastIndexUsed,
        IndexAndName,
    }
    static List<string> listOfPartyNames;
    static LinkedList<PartyIndexAndName> listIndexAndName;
    static string dirPath = Application.dataPath + Path.DirectorySeparatorChar;
    static string PartiesDataFile = "PartiesData.txt";
    static int lastIndexUsed;
    static string currentPartyName = "";

    static public void GameStart()
    {

        listOfPartyNames = new List<string>();
        listIndexAndName = new LinkedList<PartyIndexAndName>();
        if (File.Exists(dirPath + PartiesDataFile))
        {
            using (StreamReader sr = new StreamReader(dirPath + PartiesDataFile))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] arrData = line.Split(',');
                    int signifier = int.Parse(arrData[0]);

                    if (signifier == (int)pcSignifier.LastIndexUsed)
                    {
                        lastIndexUsed = int.Parse(arrData[1]);
                    }
                    else if (signifier == (int)pcSignifier.IndexAndName)
                    {
                        listIndexAndName.AddLast(new PartyIndexAndName(int.Parse(arrData[1]), arrData[2]));
                    }
                }
            }
        }
        //currentPartyName = listOfPartyNames[0];
        UpdateListOfPartyNames();
        GameContent.RefreshUI();
    }

    static public List<string> GetListOfPartyNames()
    {
        UpdateListOfPartyNames();
        return listOfPartyNames;
    }
    static public void UpdateListOfPartyNames()
    {
        listOfPartyNames.Clear();
        foreach (var indexAndName in listIndexAndName)
        {
            listOfPartyNames.Add(indexAndName.name);
        }
    }
    static public void LoadPartyDropDownChanged(string selectedName)
    {
        GameContent.partyCharacters.Clear();
        currentPartyName = selectedName;
        int indexToLoad = -1;

        foreach (var nameAndIndex in listIndexAndName)
        {
            if (nameAndIndex.name == selectedName)
                indexToLoad = nameAndIndex.index;
        }

        using (StreamReader sr = new StreamReader(dirPath + indexToLoad + ".txt"))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] arrData = line.Split(',');
                int signifier = int.Parse(arrData[0]);

                if (signifier == (int)pcSignifier.StatsData)
                {
                    PartyCharacter pc = new PartyCharacter(int.Parse(arrData[1]),
                    int.Parse(arrData[2]),
                    int.Parse(arrData[3]),
                    int.Parse(arrData[4]),
                    int.Parse(arrData[5]),
                    int.Parse(arrData[6]));
                    GameContent.partyCharacters.AddLast(pc);
                }
                else if (signifier == (int)pcSignifier.EquipmentData)
                {
                    GameContent.partyCharacters.Last.Value.equipment.AddLast(int.Parse(arrData[1]));
                }
            }
        }

        GameContent.RefreshUI();
    }

    static public void SavePartyButtonPressed()
    {
        bool bUniqueName = true;

        foreach (var nameAndIndex in listIndexAndName)
        {
            if (nameAndIndex.name == GameContent.GetPartyNameFromInput())
            {
                SaveParty(dirPath + nameAndIndex.index + ".txt");
                bUniqueName = false;
            }
        }

        if (bUniqueName)
        {
            lastIndexUsed++;
            SaveParty(dirPath + lastIndexUsed + ".txt");
            listIndexAndName.AddLast(new PartyIndexAndName(lastIndexUsed, GameContent.GetPartyNameFromInput()));
        }

        GameContent.RefreshUI();

        SaveIndexManagementFile();
    }

    static public void SaveParty(string fileName)
    {
        using (StreamWriter sw = new StreamWriter(fileName))
        {
            foreach (var pc in GameContent.partyCharacters)
            {
                sw.WriteLine((int)pcSignifier.StatsData + "," +
                pc.classID + "," +
                pc.health + "," +
                pc.mana + "," +
                pc.strength + "," +
                pc.agility + "," +
                pc.wisdom);

                foreach (var equip in pc.equipment)
                {
                    sw.WriteLine((int)pcSignifier.EquipmentData + "," + equip);
                }
            }
        }
    }

    static public void SaveIndexManagementFile()
    {
        using (StreamWriter sw = new StreamWriter(dirPath + PartiesDataFile))
        {
            sw.WriteLine((int)pcSignifier.LastIndexUsed + "," + lastIndexUsed);

            foreach (var indexAndName in listIndexAndName)
            {
                sw.WriteLine((int)pcSignifier.IndexAndName + "," + indexAndName.index + "," + indexAndName.name);
            }
        }
    }
    static public void DeletePartyButtonPressed()
    {
        LinkedList<PartyIndexAndName> tempList = new LinkedList<PartyIndexAndName>();
        foreach (var nameAndIndex in listIndexAndName)
        {
            if (!(nameAndIndex.name == currentPartyName))
                tempList.AddLast(nameAndIndex);
        }
        listIndexAndName = tempList;
        SaveIndexManagementFile();

        GameContent.partyCharacters.Clear();
        GameContent.RefreshUI();
    }


}

public class PartyIndexAndName
{
    public int index;
    public string name;
    public PartyIndexAndName(int index, string name)
    {
        this.index = index;
        this.name = name;
    }
}

#endregion


