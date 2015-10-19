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

		FSM.setCombination ((int)SpellTypes.WATER, (int)SpellTypes.WATER, (int)SpellTypes.WATER1);
		FSM.setCombination ((int)SpellTypes.WATER, (int)SpellTypes.WATER1, (int)SpellTypes.WATER2);
		FSM.setCombination ((int)SpellTypes.WATER1, (int)SpellTypes.WATER1, (int)SpellTypes.WATER2);

		FSM.setCombination ((int)SpellTypes.ROCK, (int)SpellTypes.ROCK, (int)SpellTypes.ROCK1);
		FSM.setCombination ((int)SpellTypes.ROCK, (int)SpellTypes.ROCK1, (int)SpellTypes.ROCK2);
		FSM.setCombination ((int)SpellTypes.ROCK1, (int)SpellTypes.ROCK1, (int)SpellTypes.ROCK2);

		FSM.setCombination ((int)SpellTypes.AIR, (int)SpellTypes.AIR, (int)SpellTypes.AIR1);
		FSM.setCombination ((int)SpellTypes.AIR, (int)SpellTypes.AIR1, (int)SpellTypes.AIR2);
		FSM.setCombination ((int)SpellTypes.AIR1, (int)SpellTypes.AIR1, (int)SpellTypes.AIR2);

		FSM.setCombination ((int)SpellTypes.LIGHT, (int)SpellTypes.LIGHT, (int)SpellTypes.LIGHT1);
		FSM.setCombination ((int)SpellTypes.LIGHT, (int)SpellTypes.LIGHT1, (int)SpellTypes.LIGHT2);
		FSM.setCombination ((int)SpellTypes.LIGHT1, (int)SpellTypes.LIGHT1, (int)SpellTypes.LIGHT2);

		FSM.setCombination ((int)SpellTypes.SHADOW, (int)SpellTypes.SHADOW, (int)SpellTypes.SHADOW1);
		FSM.setCombination ((int)SpellTypes.SHADOW, (int)SpellTypes.SHADOW1, (int)SpellTypes.SHADOW2);
		FSM.setCombination ((int)SpellTypes.SHADOW1, (int)SpellTypes.SHADOW1, (int)SpellTypes.SHADOW2);

		FSM.setCombination ((int)SpellTypes.ARCANE, (int)SpellTypes.ARCANE, (int)SpellTypes.ARCANE1);
		FSM.setCombination ((int)SpellTypes.ARCANE, (int)SpellTypes.ARCANE1, (int)SpellTypes.ARCANE2);
		FSM.setCombination ((int)SpellTypes.ARCANE1, (int)SpellTypes.ARCANE1, (int)SpellTypes.ARCANE2);

		FSM.setCombination ((int)SpellTypes.FROST, (int)SpellTypes.FROST, (int)SpellTypes.FROST1);
		FSM.setCombination ((int)SpellTypes.FROST, (int)SpellTypes.FROST1, (int)SpellTypes.FROST2);
		FSM.setCombination ((int)SpellTypes.FROST1, (int)SpellTypes.FROST2, (int)SpellTypes.FROST2);

		FSM.setCombination ((int)SpellTypes.LIGHTNING, (int)SpellTypes.LIGHTNING, (int)SpellTypes.LIGHTNING1);
		FSM.setCombination ((int)SpellTypes.LIGHTNING, (int)SpellTypes.LIGHTNING1, (int)SpellTypes.LIGHTNING2);
		FSM.setCombination ((int)SpellTypes.LIGHTNING1, (int)SpellTypes.LIGHTNING1, (int)SpellTypes.LIGHTNING2);

		FSM.setCombination ((int)SpellTypes.LIFE, (int)SpellTypes.LIFE, (int)SpellTypes.LIFE1);
		FSM.setCombination ((int)SpellTypes.LIFE, (int)SpellTypes.LIFE1, (int)SpellTypes.LIFE2);
		FSM.setCombination ((int)SpellTypes.LIFE1, (int)SpellTypes.LIFE1, (int)SpellTypes.LIFE2);

		FSM.setCombination ((int)SpellTypes.AIR,  (int)SpellTypes.WATER,  (int)SpellTypes.FROST);
		FSM.setCombination ((int)SpellTypes.AIR1, (int)SpellTypes.WATER1, (int)SpellTypes.FROST1);
		FSM.setCombination ((int)SpellTypes.AIR2, (int)SpellTypes.WATER2, (int)SpellTypes.FROST2);

		FSM.setCombination ((int)SpellTypes.LIGHT,  (int)SpellTypes.WATER,  (int)SpellTypes.LIFE);
		FSM.setCombination ((int)SpellTypes.LIGHT1, (int)SpellTypes.WATER1, (int)SpellTypes.LIFE1);
		FSM.setCombination ((int)SpellTypes.LIGHT2, (int)SpellTypes.WATER2, (int)SpellTypes.LIFE2);

		FSM.setCombination ((int)SpellTypes.LIGHT,  (int)SpellTypes.ROCK,  (int)SpellTypes.SHADOW);
		FSM.setCombination ((int)SpellTypes.LIGHT1, (int)SpellTypes.ROCK1, (int)SpellTypes.SHADOW1);
		FSM.setCombination ((int)SpellTypes.LIGHT2, (int)SpellTypes.ROCK2, (int)SpellTypes.SHADOW2);

		FSM.setCombination ((int)SpellTypes.LIGHT,  (int)SpellTypes.AIR,  (int)SpellTypes.ARCANE);
		FSM.setCombination ((int)SpellTypes.LIGHT1, (int)SpellTypes.AIR1, (int)SpellTypes.ARCANE1);
		FSM.setCombination ((int)SpellTypes.LIGHT2, (int)SpellTypes.AIR2, (int)SpellTypes.ARCANE2);

		FSM.setCombination ((int)SpellTypes.ARCANE,  (int)SpellTypes.FIRE,  (int)SpellTypes.LIGHTNING);
		FSM.setCombination ((int)SpellTypes.ARCANE1, (int)SpellTypes.FIRE1, (int)SpellTypes.LIGHTNING1);
		FSM.setCombination ((int)SpellTypes.ARCANE2, (int)SpellTypes.FIRE2, (int)SpellTypes.LIGHTNING2);

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

			case SpellTypes.WATER : {
				if(Type == SpellTypes.SHIELD)
					return Prefabs[13];
				
				if(Type == SpellTypes.THROW)
					return Prefabs[5];
				break;
			}

			case SpellTypes.WATER2:

			case SpellTypes.WATER1 : {
				if(Type == SpellTypes.SHIELD)
					return Prefabs[13];
				
				if(Type == SpellTypes.THROW)
					return Prefabs[10];
				break;
			}


			case SpellTypes.ROCK : {
				if(Type == SpellTypes.SHIELD)
					return Prefabs[12];
				
				if(Type == SpellTypes.THROW)
					return Prefabs[6];
				break;
			}
				
			case SpellTypes.ROCK2:
				
			case SpellTypes.ROCK1 : {
				if(Type == SpellTypes.SHIELD)
					return Prefabs[12];
				
				if(Type == SpellTypes.THROW)
					return Prefabs[9];
				break;
			}

			case SpellTypes.AIR : {
				if(Type == SpellTypes.SHIELD)
					return Prefabs[11];
				
				if(Type == SpellTypes.THROW)
					return Prefabs[8];
				break;
			}
				
			case SpellTypes.AIR2:
				
			case SpellTypes.AIR1 : {
				if(Type == SpellTypes.SHIELD)
					return Prefabs[11];
				
				if(Type == SpellTypes.THROW)
					return Prefabs[7];
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

	AIR,
	AIR1,
	AIR2,

	LIGHT,
	LIGHT1,
	LIGHT2,

	SHADOW,
	SHADOW1,
	SHADOW2,

	ARCANE,
	ARCANE1,
	ARCANE2,

	FROST,
	FROST1,
	FROST2,

	LIGHTNING,
	LIGHTNING1,
	LIGHTNING2,

	LIFE,
	LIFE1,
	LIFE2,

	SHIELD,
	THROW,
	RAY,
	DROP,
	
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

	public void setCombination(int Source, int Combinable, int Result, int SResult = -1){
		fsm [Source, Combinable] = Result;
		if (SResult == -1)
			fsm [Combinable, Source] = Result;
		else
			fsm [Combinable, Source] = SResult;
	}

	public int getCombination (int Source, int Combinable){
		return fsm [Source, Combinable];
	}
}