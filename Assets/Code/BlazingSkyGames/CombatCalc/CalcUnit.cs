/*
The MIT License (MIT)

https://opensource.org/licenses/MIT

Copyright (c) 2016 Matthew Draper

Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
and associated documentation files (the "Software"), 
to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included 
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", 
WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE 
AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using UnityEngine;
using System.Collections;

public class combatResult
{
	public int damage;
	public double critMult;
	public bool crit;
}

public class CalcUnit : MonoBehaviour
{
	public int HP;

	public int Strength;
	public int Intelligence;
	public int Stability;
	public int Insight;
	public int Vitality;
	public int Skill;

	//see this doc
	//https://docs.google.com/spreadsheets/d/138VGZZzNuFbEXh27yZ0QuRD780J2cALNZ6BIrzTwxO4/edit?usp=sharing
	public combatResult combatCalc (EffectType type, CalcUnit target, int abilityPower)
	{
		double B = type == EffectType.Physical ? Strength : Intelligence;
		double C = type == EffectType.Physical ? target.Stability : target.Insight;
		double D = Skill;
		double E = target.Vitality;
		double G = abilityPower;

		double MaxDMG = B / C * G * 0.91 + 1.0;
		double DMG = Random.Range (0.8f, 1.0f) * MaxDMG;

		double CritChance = B / E * 0.2;

		bool didCrit = Random.value < CritChance;

		double CritMult = 1 + (D / C / 2);

		combatResult result = new combatResult ();
		result.damage = (int)DMG;
		result.critMult = CritMult;
		result.crit = didCrit;

		return result;
	}

}
