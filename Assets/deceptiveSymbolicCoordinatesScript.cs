using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class deceptiveSymbolicCoordinatesScript : MonoBehaviour {

	public KMAudio mAudio;
	public KMBombModule modSelf;
	public TextMesh letterMesh, numberMesh, symbolMesh;
	public KMSelectable[] letterSelectables, numberSelectables;
	public KMSelectable submitBtn;
	public Material[] coloredLEDMats, stageLEDMats;
	public MeshRenderer[] coloredLEDRenderers, stageLEDRenderers;
	public string[] debugColors;
	readonly string possibleLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ*", possibleNumbers = "0123456789*";
	char selectedDigit, selectedLetter;
	readonly string[] possibleLetterCombinations = {
		"P L A", //A
		"C L A", //B
		"L A P", //C
		"A C E", //D
		"C L E", //E
		"C E L", //F
		"E L A", //G
		"P A L", //H
		"P C L", //I
		"A L P", //J
		"L E C", //K
		"L P E", //L
		"E A L", //M
		"P E C", //N
		"L C P", //O
		"A E L", //P
		"A L E", //Q
		"L E P", //R
		"E P C", //S
		"P C E", //T
		"C A L", //U
		"C P E", //V
		"E P L", //W
		"P A E", //X
		"A C L", //Y
		"E C P", //Z
		"L A E", //*1
		"A C P", //*2
		"C E P", //*3
		"E L C", //*4
	}, indexedStringsAll = {
		"RNBCDQAEZTVLSK*IOYWMPHUGXFJ",
		"JDY*BTVOHPIMSUWNLAXRQCKEGFZ",
		"NHVMJZYPOAELWRTKFSUIGQCBXD*",
		"YKZWIXDJ*LCFQSTAEPHURBVONMG",
		"TOERJBUSXQCG*AYZVMDFWHLNPIK",
		"FGOEJMB*URNWDPCAYXZKLQTHSIV",
		"GWSDJYAFO*XITBRPHMLQKZVCNUE",
		"EVZAQ*XFWYBKPSHJMDIRCOGULNT",
		"MWXH*TCQZGAVFROSDKPUEJBNYIL",
		"XZTDHBC*LUOSFMGINERQWYJPKAV",
		"CTLPNQHOMJXWFSIUABE*KYGDZRV",
	}, debugPositions = { "TL", "TM", "TR", "ML", "MM", "MR", "BL", "BM", "BR" };
	readonly int[][] possibleLEDCombinations = {
		new[] { 0, 1, 3 },
		new[] { 1, 3, 2 },
		new[] { 1, 2, 3 },
		new[] { 3, 0, 2 },
		new[] { 0, 1, 2 },
		new[] { 2, 0, 1 },
		new[] { 3, 1, 2 },
		new[] { 1, 0, 2 },
		new[] { 0, 2, 3 },
		new[] { 2, 1, 0 },
		new[] { 3, 1, 0 },
		new[] { 1, 3, 0 },
	};

	int[] symbolIdx, colorIdx;

	static int modIDCnt;
	int modID, curIdxNavigaion;
	bool verticalWrapping, horizontalWrapping, onFirstPress = true, holdingSubmit, holdingSubmitOnFirstPress, allowInteractions, moduleSolved;
	float timePassed;
	List<int> idxesOverwrite;
	// Use this for initialization
	void Start () {
		modID = ++modIDCnt;
		symbolMesh.text = possibleLetterCombinations.PickRandom();
		var selectedRandomLEDCombination = possibleLEDCombinations.PickRandom();
        for (var x = 0; x < coloredLEDRenderers.Length; x++)
        {
			coloredLEDRenderers[x].material = coloredLEDMats[selectedRandomLEDCombination[x]];
        }
		submitBtn.OnInteract += delegate {
			if (allowInteractions)
			{
				holdingSubmitOnFirstPress = onFirstPress;
				holdingSubmit = true;
				timePassed = 0;
				submitBtn.AddInteractionPunch();
				mAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, submitBtn.transform);
				if (onFirstPress)
				{
					HandleFirstPress();
					mAudio.PlaySoundAtTransform("beep1", submitBtn.transform);
				}
			}
			else
            {
				submitBtn.AddInteractionPunch();
				mAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, submitBtn.transform);
			}
			return false;
		};
		submitBtn.OnInteractEnded += delegate {
			if (allowInteractions)
			{
				if (!holdingSubmitOnFirstPress)
				{
					for (var x = 0; x < stageLEDRenderers.Length; x++)
					{
						stageLEDRenderers[x].material = stageLEDMats[0];
					}
					if (timePassed < 1f)
					{
						if (idxesOverwrite == null || !idxesOverwrite.Any())
						{
							QuickLog("You kept the current board, which is correct.");
							SolveModule();
						}
						else
                        {
							QuickLog("You kept the current board, which is not correct! That's a strike.");
							modSelf.HandleStrike();
						}
					}
					else if (timePassed >= 3f)
					{
						if (idxesOverwrite == null || !idxesOverwrite.Any())
						{
							QuickLog("You overwrote the {0} symbol/color pair, which is not correct! That's a strike.", debugPositions[curIdxNavigaion]);
							modSelf.HandleStrike();
						}
						else if (!idxesOverwrite.Contains(curIdxNavigaion))
						{
							QuickLog("You overwrote the {0} symbol/color pair, which is not the correct pairs to overwrite. That's a strike.", debugPositions[curIdxNavigaion]);
							modSelf.HandleStrike();
						}
						else
                        {
							QuickLog("You overwrote the {0} symbol/color pair, which is one the correct pairs to overwrite.", debugPositions[curIdxNavigaion]);
						}
						OverwriteSelectedCoordinate();
					}
				}
				holdingSubmit = false;
				mAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonRelease, submitBtn.transform);
			}
		};
        for (var x = 0; x < letterSelectables.Length; x++)
        {
			int y = x;
			letterSelectables[x].OnInteract += delegate {
				if (allowInteractions)
				{
					letterSelectables[y].AddInteractionPunch(0.5f);
					if (onFirstPress)
					{
						mAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, letterSelectables[y].transform);
						HandleFirstPress();
					}
					else
					{
						var lastCurIdxNav = curIdxNavigaion;
						HandleDelta(y + 1);
						mAudio.PlayGameSoundAtTransform(curIdxNavigaion == lastCurIdxNav ? KMSoundOverride.SoundEffect.ButtonRelease : KMSoundOverride.SoundEffect.ButtonPress, letterSelectables[y].transform);
						UpdateGridVisuals();
					}
				}
				else
				{
					letterSelectables[y].AddInteractionPunch(0.5f);
					mAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, letterSelectables[y].transform);
				}
				return false;
			};
        }
        for (var x = 0; x < numberSelectables.Length; x++)
        {
			int y = x;
			numberSelectables[x].OnInteract += delegate {
				if (allowInteractions)
				{
					numberSelectables[y].AddInteractionPunch(0.5f);
					if (onFirstPress)
					{
						mAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, numberSelectables[y].transform);
						HandleFirstPress();
					}
					else
					{
						var lastCurIdxNav = curIdxNavigaion;
						HandleDelta(y + 1, false);
						mAudio.PlayGameSoundAtTransform(curIdxNavigaion == lastCurIdxNav ? KMSoundOverride.SoundEffect.ButtonRelease : KMSoundOverride.SoundEffect.ButtonPress, numberSelectables[y].transform);
						UpdateGridVisuals();
					}
				}
				else
				{
					numberSelectables[y].AddInteractionPunch(0.5f);
					mAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, numberSelectables[y].transform);
				}
				return false;
			};
        }

		QuickLog("The display is {0}. The LEDs are {1}.", symbolMesh.text, selectedRandomLEDCombination.Select(a => debugColors[a]).Join(", "));
		QuickLog("Pressing any of the buttons here will cause it to turn into what you don't think it would become...");
		GenerateBoard();

	}

	void HandleDelta(int delta = 1, bool horizontalMovement = true)
    {
		var curRowIdx = curIdxNavigaion / 3;
		var curColIdx = curIdxNavigaion % 3;

		if (horizontalMovement)
		{
			var newColIdx = (curColIdx + delta) % 3;
			var forbiddenMovementsAll = new Dictionary<int, bool>()
			{
				{ 0, newColIdx == 2 && !horizontalWrapping },
				{ 2, newColIdx == 0 && !horizontalWrapping },
			};
			var allowMovement = true;
            foreach (KeyValuePair<int, bool> forbidMovement in forbiddenMovementsAll)
            {
				if (forbidMovement.Key == curColIdx && forbidMovement.Value)
					allowMovement = false;
			}
			if (allowMovement)
				curColIdx = newColIdx;
		}
		else
        {
			var newRowIdx = (curRowIdx + delta) % 3;
			var forbiddenMovementsAll = new Dictionary<int, bool>()
			{
				{ 0, newRowIdx == 2 && !verticalWrapping },
				{ 2, newRowIdx == 0 && !verticalWrapping }
			};
			var allowMovement = true;
			foreach (KeyValuePair<int, bool> forbidMovement in forbiddenMovementsAll)
			{
				if (forbidMovement.Key == curRowIdx && forbidMovement.Value)
					allowMovement = false;
			}
			if (allowMovement)
				curRowIdx = newRowIdx;
		}
		curIdxNavigaion = curRowIdx * 3 + curColIdx;
	}
	void SolveModule()
    {
		QuickLog("Module disarmed.");
		mAudio.PlaySoundAtTransform("beep3", transform);
		allowInteractions = false;
		modSelf.HandlePass();
		symbolMesh.text = "ACELP";
		numberMesh.text = "";
		letterMesh.text = "";
		for (var x = 0; x < stageLEDRenderers.Length; x++)
		{
			stageLEDRenderers[x].material = stageLEDMats[1];
		}
		for (var x = 0; x < coloredLEDRenderers.Length; x++)
		{
			coloredLEDRenderers[x].material = stageLEDMats[0];
		}
	}
	void OverwriteSelectedCoordinate()
    {
		var x = curIdxNavigaion;
		symbolIdx[x] = Random.Range(0, 5);
		colorIdx[x] = Random.Range(0, 4);
		selectedDigit = possibleNumbers.PickRandom();
		selectedLetter = possibleLetters.PickRandom();
		DetermineOverwrites();

	}

	void GenerateBoard()
    {
		symbolIdx = new int[9];
		colorIdx = new int[9];
        for (var x = 0; x < 9; x++)
        {
			symbolIdx[x] = Random.Range(0, 5);
			colorIdx[x] = Random.Range(0, 4);
        }
		selectedDigit = possibleNumbers.PickRandom();
		selectedLetter = possibleLetters.PickRandom();
		verticalWrapping = Random.value < 0.5f;
		horizontalWrapping = Random.value < 0.5f;
		curIdxNavigaion = Random.Range(0, 9);
		QuickLog("Initial Grid:", debugPositions[curIdxNavigaion]);
		LogGrid();
		QuickLog("Topology Generated: {0}", verticalWrapping && horizontalWrapping ? "Torus" : horizontalWrapping ? "Cylinder, left and right edges wrap" : verticalWrapping ? "Cylinder, top and bottom edges wrap" : "Plane");
		QuickLog("Starting on the {0} of the 3x3 grid.", debugPositions[curIdxNavigaion]);
		DetermineOverwrites();
	}
	void DetermineOverwrites()
    {
		idxesOverwrite = new List<int>();
		var stringDetermined = indexedStringsAll[possibleNumbers.IndexOf(selectedDigit)];
		QuickLog("Using string determined from digit '{1}': {0}", stringDetermined, selectedDigit);
		var indexDetermined = stringDetermined.IndexOf(selectedLetter);
		QuickLog("Index of letter '{1}' from determined string: {0}", indexDetermined, selectedLetter);
		var sectionsDetermined = new[] { indexDetermined / 9 % 3, indexDetermined / 3 % 3, indexDetermined % 3 };
		QuickLog("Performing rules in order of sections {0}.", sectionsDetermined.Select(a => a+1).Join(", ") );

		if (idxesOverwrite.Any())
			QuickLog("Allowed pairs to overwrite: {0}", idxesOverwrite.Select(a => debugPositions[a]).Join());
		else
			QuickLog("Expecting to keep the current board.");
		allowInteractions = true;
	}

	void LogGrid()
    {
		for (var x = 0; x < 3; x++)
        {
			var logIdxEnums = new[] { 3 * x, 3 * x + 1, 3 * x + 2 };
			QuickLog("[{0}]", logIdxEnums.Select(a => "ACELP"[symbolIdx[a]] + " " + debugColors[colorIdx[a]]).Join("],["));
        }
    }
	void HandleFirstPress()
    {
		StartCoroutine(HandleLetterFakePressAnim());
		StartCoroutine(HandleNumberFakePressAnim());
		UpdateGridVisuals();
		QuickLog("The true form has been activated.");
		onFirstPress = false;
    }

	IEnumerator HandleLetterFakePressAnim()
    {
		var goDown = Random.value < 0.5f;
		var curIdx = possibleLetters.IndexOf(letterMesh.text);
		do
		{
			letterSelectables[goDown ? 1 : 0].AddInteractionPunch(0.25f);
			mAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, letterSelectables[goDown ? 1 : 0].transform);
			curIdx += (goDown ? -1 : 1) + possibleLetters.Length;
			curIdx %= possibleLetters.Length;
			letterMesh.text = possibleLetters[curIdx].ToString();
			yield return new WaitForSeconds(0.1f);
		}
		while (letterMesh.text != selectedLetter.ToString() && !moduleSolved);
	}
	IEnumerator HandleNumberFakePressAnim()
    {
		var goDown = Random.value < 0.5f;
		var curIdx = possibleNumbers.IndexOf(numberMesh.text);
		do
		{
			numberSelectables[goDown ? 1 : 0].AddInteractionPunch(0.25f);
			mAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, numberSelectables[goDown ? 1 : 0].transform);
			curIdx += (goDown ? -1 : 1) + possibleNumbers.Length;
			curIdx %= possibleNumbers.Length;
			numberMesh.text = possibleNumbers[curIdx].ToString();
			yield return new WaitForSeconds(0.1f);
		}
		while (numberMesh.text != selectedDigit.ToString() && !moduleSolved);
	}

	void UpdateGridVisuals()
    {
		var curRowIdx = curIdxNavigaion / 3;
		var curColIdx = curIdxNavigaion % 3;
		//Debug.Log(curIdxNavigaion);
        var deltasAll = new[] { 2, 0, 1 };
		symbolMesh.text = deltasAll.Select(a => "ACELP"[symbolIdx[3 * curRowIdx + (curColIdx + a) % 3]]).Join();
        for (var x = 0; x < deltasAll.Length; x++)
        {
			coloredLEDRenderers[x].material = coloredLEDMats[colorIdx[(curRowIdx + deltasAll[x]) % 3 * 3 + curColIdx]];
        }
    }

	void QuickLog(string value, params object[] args)
    {
		Debug.LogFormat("[{0} #{1}]: {2}", modSelf.ModuleDisplayName, modID, string.Format(value, args));
    }
	void FixedUpdate()
    {
		if (holdingSubmit && !holdingSubmitOnFirstPress)
		{
			if (timePassed < 5f)
				timePassed += Time.fixedDeltaTime;
			for (var x = 0; x < stageLEDRenderers.Length; x++)
            {
				stageLEDRenderers[x].material = stageLEDMats[timePassed >= x + 1 ? 1 : 0];
            }

		}
    }
}
