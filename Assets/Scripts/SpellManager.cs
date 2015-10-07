﻿using UnityEngine;
using System.Collections;

public class SpellManager : MonoBehaviour {

    public static SpellManager instance = null;
	public FSkillMachine FSM;

	public GameObject[] Prefabs;	//Deben estar en ordeeen

    void Awake() {
        instance = this;
		FSM = new FSkillMachine ((int)SpellTypes.Count, (int)SpellTypes.Count);

		FSM.setCombination ((int)SpellTypes.FIRE, (int)SpellTypes.FIRE, (int)SpellTypes.FIRE1);
		FSM.setCombination ((int)SpellTypes.FIRE, (int)SpellTypes.FIRE1, (int)SpellTypes.FIRE2);
		FSM.setCombination ((int)SpellTypes.FIRE1, (int)SpellTypes.FIRE1, (int)SpellTypes.FIRE2);


    }

	public SpellTypes Combination(SpellTypes spell1, SpellTypes spell2){
		int result = FSM.getCombination ((int)spell1, (int)spell2);


		if (result == -1)	// No existe esa combinación.
			return SpellTypes.NULL;

		SpellTypes parsedResult = (SpellTypes) result;
		return parsedResult;
	}

	public GameObject getGraphic(SpellTypes Spell, SpellTypes Type){
		 switch (Spell) {
		
			case SpellTypes.FIRE : {
				if(Type == SpellTypes.SHIELD)
					return Prefabs[3];

				if(Type == SpellTypes.THROW)
					return Prefabs[0];
				break;
			}

			case SpellTypes.FIRE1 : {
				if(Type == SpellTypes.SHIELD)
					return Prefabs[3];
					
				if(Type == SpellTypes.THROW)
					return Prefabs[1];
				break;
			}

			case SpellTypes.FIRE2 : {
				if(Type == SpellTypes.SHIELD)
					return Prefabs[3];
					
				if(Type == SpellTypes.THROW)
					return Prefabs[2];
				break;
			}
		}

		return null;

	}


}


public enum SpellTypes
{
    FIRE = 0,
	FIRE1,
	FIRE2,
    WATER,
	WATER1,
	WATER2,
    ROCK,
	ROCK1,
	ROCK2,
    DARK,
	DARK1,
	DARK2,
	SHIELD,
	THROW,
	Count,
	NULL
}

public class FSkillMachine{

	private int[,] fsm;

	public FSkillMachine(int q_skillX, int q_SkillY){
		fsm = new int[q_skillX, q_SkillY];
		for(short j = 0; j < q_skillX; j++){
			for(short i = 0; i < q_SkillY; i++){
				fsm[j, i] = -1;
			}
		}
	}

	public void setCombination(int Source, int Combinable, int Result){
		fsm [Source, Combinable] = Result;
		fsm [Combinable, Source] = Result;
	}

	public int getCombination (int Source, int Combinable){
		return fsm [Source, Combinable];
	}
}