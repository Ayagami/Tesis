using UnityEngine;
using System.Collections;

public class SpellManager : MonoBehaviour {

    public static SpellManager instance = null;

    void Awake()
    {
        instance = this;
    }
    /*
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}*/
}
/*
public class SpellCombination {   
    
    private Spell spell1;
    private Spell spell2;
    private Spell Result;

    public SpellCombination(Spell Origin, Spell Comb, Spell Result)
    {
        this.spell1 = Origin;
        this.spell2 = Comb;
        this.Result = Result;
    }

    public  bool Combines()
    {
        return false;
    }
}*/

public enum SpellTypes
{
    FIRE = 0,
    WATER,
    ROCK,
    DARK,
    LIGHT
}

public class Spell {

    private SpellTypes _type;


    public Spell(SpellTypes type)
    {
        _type = type;
    }

    public void AddCombination(Spell withWho, Spell Result)
    {

    }
}