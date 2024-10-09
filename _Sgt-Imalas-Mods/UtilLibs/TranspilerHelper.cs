﻿using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace UtilLibs
{
	public class TranspilerHelper
	{


		public static int FindIndexOfNextLocalIndex(List<CodeInstruction> codeInstructions, int insertionIndex, bool goingBackwards = true) => FindIndicesOfLocalsByIndex(codeInstructions, insertionIndex, 1, goingBackwards)[0];

		public static int[] FindIndicesOfLocalsByIndex(List<CodeInstruction> codeInstructions, int insertionIndex, int numberOfVarsToFind = 1, bool goingBackwards = true)
		{
			var indices = new List<int>();

			if (insertionIndex != -1)
			{
				int direction = goingBackwards ? -1 : 1;
				for (int i = insertionIndex - 1; i >= 0 && i < codeInstructions.Count && indices.Count < numberOfVarsToFind; i += direction)
				{
					if (CodeInstructionExtensions.IsLdloc(codeInstructions[i]))
					{
						int locIndex = GiveOpCodeIndexFromLocalBuilder(codeInstructions[i]);
						if (!indices.Contains(locIndex))
							indices.Insert(0, locIndex);
						break;
					};
				}


				//SgtLogger.l(codeInstructions[i].operand.GetType().ToString(), "transpilertest");
				//SgtLogger.l(codeInstructions[i].operand.ToString(), "transpilertest");
				//SgtLogger.l(((LocalBuilder)codeInstructions[i].operand).LocalIndex.ToString(), "transpilertest");
			}
			else
			{
				indices.Add(-1);
			}
			return indices.ToArray();
		}

		public static Tuple<int, int> FindIndexOfNextLocalIndexWithPosition(List<CodeInstruction> codeInstructions, int insertionIndex, bool goingBackwards = true)
		{
			var array = FindIndicesOfLocalsByIndexWithPositions(codeInstructions, insertionIndex, 1, goingBackwards);
			return new Tuple<int, int>(array.first[0], array.second[0]);
		}
		public static Tuple<int[], int[]> FindIndicesOfLocalsByIndexWithPositions(List<CodeInstruction> codeInstructions, int insertionIndex, int numberOfVarsToFind = 1, bool goingBackwards = true)
		{
			var indices = new List<int>();
			var positions = new List<int>();

			if (insertionIndex != -1)
			{
				int direction = goingBackwards ? -1 : 1;
				for (int i = insertionIndex - 1; i >= 0 && i < codeInstructions.Count && indices.Count < numberOfVarsToFind; i += direction)
				{
					if (CodeInstructionExtensions.IsLdloc(codeInstructions[i]))
					{
						int locIndex = GiveOpCodeIndexFromLocalBuilder(codeInstructions[i]);
						if (!indices.Contains(locIndex))
						{
							indices.Insert(0, locIndex);
							positions.Insert(0, i); break;
						}
						break;
					};
				}


				//SgtLogger.l(codeInstructions[i].operand.GetType().ToString(), "transpilertest");
				//SgtLogger.l(codeInstructions[i].operand.ToString(), "transpilertest");
				//SgtLogger.l(((LocalBuilder)codeInstructions[i].operand).LocalIndex.ToString(), "transpilertest");
			}
			else
			{
				indices.Add(-1);
			}
			return new Tuple<int[], int[]>(indices.ToArray(), positions.ToArray());
		}



		static int GiveOpCodeIndexFromLocalBuilder(CodeInstruction codeInstruction)
		{
			var CheckCode = codeInstruction.opcode;
			if (CheckCode == OpCodes.Ldloc_0)
			{
				return 0;
			}
			else if (CheckCode == OpCodes.Ldloc_1)
			{
				return 1;
			}
			else if (CheckCode == OpCodes.Ldloc_2)
			{
				return 2;
			}
			else if (CheckCode == OpCodes.Ldloc_3)
			{
				return 3;
			}
			else
			{
				if (codeInstruction.operand == null)
					return -1;
				return ((LocalBuilder)codeInstruction.operand).LocalIndex;
			}
		}


		//public static int GetFirstIndexOfType(List<CodeInstruction> codeInstructions, Type ObjectType)
		//{
		//    SgtLogger.l(ObjectType.ToString(), "transpilertest");

		//    for (int i = 0; i < codeInstructions.Count; ++i)
		//    {
		//        if (CodeInstructionExtensions.IsLdloc(codeInstructions[i],))
		//        {
		//            //var locBuilder = codeInstructions[i].operand;
		//            SgtLogger.l(((LocalBuilder)codeInstructions[i].operand).LocalIndex.ToString(), "transpilertest");
		//            if (((LocalBuilder)locBuilder).LocalType == ObjectType)
		//            {
		//                return ((LocalBuilder)locBuilder).LocalIndex;
		//            }
		//        }
		//    }
		//    return -1;
		//}



		public static void PrintInstructions(List<HarmonyLib.CodeInstruction> codes, bool extendedInfo = false)
		{
			SgtLogger.l("\n", "IL-Dump Start:");
			for (int i = 0; i < codes.Count; i++)
			{
				var code = codes[i];
				//Debug.Log(code);
				//Debug.Log(code.opcode);
				//Debug.Log(code.operand);

				if (extendedInfo)
				{
					if (code.operand != null)
						SgtLogger.l(i + "=> OpCode: " + code.opcode + "::" + code.operand + "<> typeof (" + (code.operand.GetType()) + ")", "IL");
					else
						SgtLogger.l(i + "=> OpCode: " + code.opcode, "IL");
				}
				else
					SgtLogger.l(i + ": " + code, "IL");
			}
			SgtLogger.l("\n", "IL-Dump Finished");
		}
	}
}
