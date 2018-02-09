using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using symbolicCoordinates;

public class symbolicCoordinatesScript : MonoBehaviour
{
	public KMBombInfo Bomb;
	public KMSelectable submitBut;
	public KMSelectable lettersUp;
	public KMSelectable lettersDown;
	public KMSelectable digitsUp;
	public KMSelectable digitsDown;
	public KMAudio Audio;
	public TextMesh symbolText;
	public TextMesh lettersText;
	public TextMesh digitsText;
	public Renderer light1;
	public Renderer light2;
	public Renderer light3;
	public Renderer pass1rend;
	public Renderer pass2rend;
	public Renderer pass3rend;
	public Texture offMat;
	public Texture onMat;
	public Texture aqua;
	public Texture green;
	public Texture purple;
	public Texture yellow;


	private int lettersIndex = 0;
	private char[] lettersEntries = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '*' };
	private int digitsIndex = 0;
	private char[] digitsEntries = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '*' };

	string correctLetter1;
	string correctLetter2;
	string correctLetter3;
	string correctDigit1;
	string correctDigit2;
	string correctDigit3;

	int stage = 1;
	static int moduleIdCounter = 1;
	int moduleId;
	string letter1 = "";
	string letter2 = "";
	string letter3 = "";
	string digit1 = "";
	string digit2 = "";
	string digit3 = "";
	string digit1Colours;
	string digit2Colours;
	string digit3Colours;
	private string TwitchHelpMessage = "Type '!{0} submit [letter][digit]' (e.g. !{0} submit Z3)";

	public KMSelectable[] ProcessTwitchCommand(string command)
	        {
	            string[] split = command.ToUpperInvariant().Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

	            if (split.Length < 2)
	                return null;

	            switch (split[0])
	            {
	                case "SUBMIT":
	                    string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ*";
	                    string digits = "0123456789*";
	                    if (split[1].Length < 2 || !letters.Contains(split[1].Substring(0, 1)) || !digits.Contains(split[1].Substring(1, 1)))
	                        return null;
	                    int desiredLetter = letters.IndexOf(split[1].Substring(0, 1), StringComparison.Ordinal);
	                    int desiredDigit = digits.IndexOf(split[1].Substring(1, 1), StringComparison.Ordinal);
	                    int currentLetter = letters.IndexOf(lettersText.text, StringComparison.Ordinal);
	                    int currentDigit = digits.IndexOf(digitsText.text, StringComparison.Ordinal);
	                    List <KMSelectable> list = new List<KMSelectable>();
	                    for(int i = currentLetter; i != desiredLetter; i = ((i + 1) % letters.Length))
	                        list.Add(lettersDown);
	                    for(int i = currentDigit; i != desiredDigit; i = ((i + 1) % digits.Length))
	                        list.Add(digitsDown);
	                    list.Add(submitBut);
	                    return list.ToArray();
	                case "PRESS":
	                    switch (split[1])
	                    {
	                        case "LETTER":
	                            if (split.Length < 3) return null;
	                            if(split[2] == "UP")
	                                return new KMSelectable[] { lettersDown };
	                            if(split[2] == "DOWN")
	                                return new KMSelectable[] { lettersUp };
	                            return null;
	                        case "DIGIT":
	                            if (split.Length < 3) return null;
	                            if (split[2] == "UP")
	                                return new KMSelectable[] { digitsDown };
	                            if (split[2] == "DOWN")
	                                return new KMSelectable[] { digitsUp };
	                            return null;
	                        case "SUBMIT":
	                            return new KMSelectable[] { submitBut };
	                        default:
	                            return null;
	                    }
	                default:
	                    return null;
	            }
	        }

	void Awake()
	{
			moduleId = moduleIdCounter++;
			GetComponent<KMBombModule>().OnActivate += Begin;
			submitBut.OnInteract += delegate () { Onsubmit(); return false; };
			lettersUp.OnInteract += delegate () { OnlettersUp(); return false; };
			lettersDown.OnInteract += delegate () { OnlettersDown(); return false; };
			digitsUp.OnInteract += delegate () { OndigitsUp(); return false; };
			digitsDown.OnInteract += delegate () { OndigitsDown(); return false; };



			int pickedLetter1 = UnityEngine.Random.Range(0, 29);

					switch (pickedLetter1)
					{
							case 0:
									letter1 = "P L A"; //A
									break;
							case 1:
									letter1 = "C L A"; //B
									break;
							case 2:
									letter1 = "L A P"; //C
									break;
							case 3:
									letter1 = "A C E"; //D
									break;
							case 4:
									letter1 = "C L E"; //E
									break;
							case 5:
									letter1 = "C E L"; //F
									break;
							case 6:
									letter1 = "E L A"; //G
									break;
							case 7:
									letter1 = "P A L"; //H
									break;
							case 8:
									letter1 = "P C L"; //I
									break;
							case 9:
									letter1 = "A L P"; //J
									break;
							case 10:
									letter1 = "L E C"; //K
									break;
							case 11:
									letter1 = "L P E"; //L
									break;
							case 12:
									letter1 = "E A L"; //M
									break;
							case 13:
									letter1 = "P E C"; //N
									break;
							case 14:
									letter1 = "L C P"; //O
									break;
							case 15:
									letter1 = "A E L"; //P
									break;
							case 16:
									letter1 = "A L E"; //Q
									break;
							case 17:
									letter1 = "L E P"; //R
									break;
							case 18:
									letter1 = "E P C"; //S
									break;
							case 19:
									letter1 = "P C E"; //T
									break;
							case 20:
									letter1 = "C A L"; //U
									break;
							case 21:
									letter1 = "C P E"; //V
									break;
							case 22:
									letter1 = "E P L"; //W
									break;
							case 23:
									letter1 = "P A E"; //X
									break;
							case 24:
									letter1 = "A C L"; //Y
									break;
							case 25:
									letter1 = "E C P"; //Z
									break;
							case 26:
									letter1 = "L A E"; //*1
									break;
							case 27:
									letter1 = "A C P"; //*2
									break;
							case 28:
									letter1 = "C E P"; //*3
									break;
							case 29:
									letter1 = "E L C"; //*4
									break;
					}

			int pickedLetter2 = UnityEngine.Random.Range(0, 29);

					switch (pickedLetter2)
					{
						case 0:
								letter2 = "P L A"; //A
								break;
						case 1:
								letter2 = "C L A"; //B
								break;
						case 2:
								letter2 = "L A P"; //C
								break;
						case 3:
								letter2 = "A C E"; //D
								break;
						case 4:
								letter2 = "C L E"; //E
								break;
						case 5:
								letter2 = "C E L"; //F
								break;
						case 6:
								letter2 = "E L A"; //G
								break;
						case 7:
								letter2 = "P A L"; //H
								break;
						case 8:
								letter2 = "P C L"; //I
								break;
						case 9:
								letter2 = "A L P"; //J
								break;
						case 10:
								letter2 = "L E C"; //K
								break;
						case 11:
								letter2 = "L P E"; //L
								break;
						case 12:
								letter2 = "E A L"; //M
								break;
						case 13:
								letter2 = "P E C"; //N
								break;
						case 14:
								letter2 = "L C P"; //O
								break;
						case 15:
								letter2 = "A E L"; //P
								break;
						case 16:
								letter2 = "A L E"; //Q
								break;
						case 17:
								letter2 = "L E P"; //R
								break;
						case 18:
								letter2 = "E P C"; //S
								break;
						case 19:
								letter2 = "P C E"; //T
								break;
						case 20:
								letter2 = "C A L"; //U
								break;
						case 21:
								letter2 = "C P E"; //V
								break;
						case 22:
								letter2 = "E P L"; //W
								break;
						case 23:
								letter2 = "P A E"; //X
								break;
						case 24:
								letter2 = "A C L"; //Y
								break;
						case 25:
								letter2 = "E C P"; //Z
								break;
						case 26:
								letter2 = "L A E"; //*1
								break;
						case 27:
								letter2 = "A C P"; //*2
								break;
						case 28:
								letter2 = "C E P"; //*3
								break;
						case 29:
								letter2 = "E L C"; //*4
								break;
					}

					int pickedLetter3 = UnityEngine.Random.Range(0, 29);

							switch (pickedLetter3)
							{
								case 0:
										letter3 = "P L A"; //A
										break;
								case 1:
										letter3 = "C L A"; //B
										break;
								case 2:
										letter3 = "L A P"; //C
										break;
								case 3:
										letter3 = "A C E"; //D
										break;
								case 4:
										letter3 = "C L E"; //E
										break;
								case 5:
										letter3 = "C E L"; //F
										break;
								case 6:
										letter3 = "E L A"; //G
										break;
								case 7:
										letter3 = "P A L"; //H
										break;
								case 8:
										letter3 = "P C L"; //I
										break;
								case 9:
										letter3 = "A L P"; //J
										break;
								case 10:
										letter3 = "L E C"; //K
										break;
								case 11:
										letter3 = "L P E"; //L
										break;
								case 12:
										letter3 = "E A L"; //M
										break;
								case 13:
										letter3 = "P E C"; //N
										break;
								case 14:
										letter3 = "L C P"; //O
										break;
								case 15:
										letter3 = "A E L"; //P
										break;
								case 16:
										letter3 = "A L E"; //Q
										break;
								case 17:
										letter3 = "L E P"; //R
										break;
								case 18:
										letter3 = "E P C"; //S
										break;
								case 19:
										letter3 = "P C E"; //T
										break;
								case 20:
										letter3 = "C A L"; //U
										break;
								case 21:
										letter3 = "C P E"; //V
										break;
								case 22:
										letter3 = "E P L"; //W
										break;
								case 23:
										letter3 = "P A E"; //X
										break;
								case 24:
										letter3 = "A C L"; //Y
										break;
								case 25:
										letter3 = "E C P"; //Z
										break;
								case 26:
										letter3 = "L A E"; //*1
										break;
								case 27:
										letter3 = "A C P"; //*2
										break;
								case 28:
										letter3 = "C E P"; //*3
										break;
								case 29:
										letter3 = "E L C"; //*4
										break;
							}

						int pickedDigit1 = UnityEngine.Random.Range(0, 11);

								switch (pickedDigit1)
								{
										case 0:
												digit1 = "0";
												break;
										case 1:
												digit1 = "1";
												break;
										case 2:
												digit1 = "2";
												break;
										case 3:
												digit1 = "3";
												break;
										case 4:
												digit1 = "4";
												break;
										case 5:
												digit1 = "5";
												break;
										case 6:
												digit1 = "6";
												break;
										case 7:
												digit1 = "7";
												break;
										case 8:
												digit1 = "8";
												break;
										case 9:
												digit1 = "9";
												break;
										case 10:
												digit1 = "*1";
												break;
										case 11:
												digit1 = "*2";
												break;
								}

						int pickedDigit2 = UnityEngine.Random.Range(0, 11);

								switch (pickedDigit2)
								{
										case 0:
												digit2 = "0";
												break;
										case 1:
												digit2 = "1";
												break;
										case 2:
												digit2 = "2";
												break;
										case 3:
												digit2 = "3";
												break;
										case 4:
												digit2 = "4";
												break;
										case 5:
												digit2 = "5";
												break;
										case 6:
												digit2 = "6";
												break;
										case 7:
												digit2 = "7";
												break;
										case 8:
												digit2 = "8";
												break;
										case 9:
												digit2 = "9";
												break;
										case 10:
												digit2 = "*1";
												break;
										case 11:
												digit2 = "*2";
												break;
										}

								int pickedDigit3 = UnityEngine.Random.Range(0, 11);

										switch (pickedDigit3)
										{
												case 0:
														digit3 = "0";
														break;
												case 1:
														digit3 = "1";
														break;
												case 2:
														digit3 = "2";
														break;
												case 3:
														digit3 = "3";
														break;
												case 4:
														digit3 = "4";
														break;
												case 5:
														digit3 = "5";
														break;
												case 6:
														digit3 = "6";
														break;
												case 7:
														digit3 = "7";
														break;
												case 8:
														digit3 = "8";
														break;
												case 9:
														digit3 = "9";
														break;
												case 10:
														digit3 = "*1";
														break;
												case 11:
														digit3 = "*2";
														break;
												}

							symbolText.text = letter1;

														if (digit1 == "0")
														{
																light1.material.mainTexture = aqua;
																light2.material.mainTexture = green;
																light3.material.mainTexture = yellow;
																correctDigit1 = "0";
																digit1Colours = "aqua, green & yellow";
														}
														else if (digit1 == "1")
														{
																light1.material.mainTexture = green;
																light2.material.mainTexture = yellow;
																light3.material.mainTexture = purple;
																correctDigit1 = "1";
																digit1Colours = "green, yellow & purple";
														}
														else if (digit1 == "2")
														{
															light1.material.mainTexture = green;
															light2.material.mainTexture = purple;
															light3.material.mainTexture = yellow;
															correctDigit1 = "2";
															digit1Colours = "green, purple & yellow";
														}
														else if (digit1 == "3")
														{
															light1.material.mainTexture = yellow;
															light2.material.mainTexture = aqua;
															light3.material.mainTexture = purple;
															correctDigit1 = "3";
															digit1Colours = "yellow, aqua & purple";
														}
														else if (digit1 == "4")
														{
															light1.material.mainTexture = aqua;
															light2.material.mainTexture = green;
															light3.material.mainTexture = purple;
															correctDigit1 = "4";
															digit1Colours = "aqua, green & purple";
														}
														else if (digit1 == "5")
														{
															light1.material.mainTexture = purple;
															light2.material.mainTexture = aqua;
															light3.material.mainTexture = green;
															correctDigit1 = "5";
															digit1Colours = "purple, aqua & green";
														}
														else if (digit1 == "6")
														{
															light1.material.mainTexture = yellow;
															light2.material.mainTexture = green;
															light3.material.mainTexture = purple;
															correctDigit1 = "6";
															digit1Colours = "yellow, green & purple";
														}
														else if (digit1 == "7")
														{
															light1.material.mainTexture = green;
															light2.material.mainTexture = aqua;
															light3.material.mainTexture = purple;
															correctDigit1 = "7";
															digit1Colours = "green, aqua & purple";
														}
														else if (digit1 == "8")
														{
															light1.material.mainTexture = aqua;
															light2.material.mainTexture = purple;
															light3.material.mainTexture = yellow;
															correctDigit1 = "8";
															digit1Colours = "aqua, purple & yellow";
														}
														else if (digit1 == "9")
														{
															light1.material.mainTexture = purple;
															light2.material.mainTexture = green;
															light3.material.mainTexture = aqua;
															correctDigit1 = "9";
															digit1Colours = "purple, green & aqua";
														}
														else if (digit1 == "*1")
														{
															light1.material.mainTexture = yellow;
															light2.material.mainTexture = green;
															light3.material.mainTexture = aqua;
															correctDigit1 = "*";
															digit1Colours = "yellow, green & aqua";
														}
														else
														{
															light1.material.mainTexture = green;
															light2.material.mainTexture = yellow;
															light3.material.mainTexture = aqua;
															correctDigit1 = "*";
															digit1Colours = "green, yellow & aqua";
														}

						}


						void Begin()
						{


							if (letter1 == "P L A")
							{
									correctLetter1 = "A";
							}
							else
							{
									if (letter1 == "C L A")
									{
											correctLetter1 = "B";
									}
									else
									{
											if (letter1 == "L A P")
											{
													correctLetter1 = "C";
											}
											else
											{
													if (letter1 == "A C E")
													{
															correctLetter1 = "D";
													}
													else
													{
															if (letter1 == "C L E")
															{
																	correctLetter1 = "E";
															}
															else
															{
																	if (letter1 == "C E L")
																	{
																			correctLetter1 = "F";
																	}
																	else
																	{
																			if (letter1 == "E L A")
																			{
																					correctLetter1 = "G";
																			}
																			else
																			{
																					if (letter1 == "P A L")
																					{
																							correctLetter1 = "H";
																					}
																					else
																					{
																							if (letter1 == "P C L")
																							{
																									correctLetter1 = "I";
																							}
																							else
																							{
																									if (letter1 == "A L P")
																									{
																											correctLetter1 = "J";
																									}
																									else
																									{
																											if (letter1 == "L E C")
																											{
																													correctLetter1 = "K";
																											}
																											else
																											{
																													if (letter1 == "L P E")
																													{
																															correctLetter1 = "L";
																													}
																													else
																													{
																															if (letter1 == "E A L")
																															{
																																	correctLetter1 = "M";
																															}
																															else
																															{
																																	if (letter1 == "P E C")
																																	{
																																			correctLetter1 = "N";
																																	}
																																	else
																																	{
																																			if (letter1 == "L C P")
																																			{
																																					correctLetter1 = "O";
																																			}
																																			else
																																			{
																																					if (letter1 == "A E L")
																																					{
																																							correctLetter1 = "P";
																																					}
																																					else
																																					{
																																							if (letter1 == "A L E")
																																							{
																																									correctLetter1 = "Q";
																																							}
																																							else
																																							{
																																									if (letter1 == "L E P")
																																									{
																																											correctLetter1 = "R";
																																									}
																																									else
																																									{
																																											if (letter1 == "E P C")
																																											{
																																													correctLetter1 = "S";
																																											}
																																											else
																																											{
																																													if (letter1 == "P C E")
																																													{
																																															correctLetter1 = "T";
																																													}
																																													else
																																													{
																																															if (letter1 == "C A L")
																																															{
																																																	correctLetter1 = "U";
																																															}
																																															else
																																															{
																																																	if (letter1 == "C P E")
																																																	{
																																																			correctLetter1 = "V";
																																																	}
																																																	else
																																																	{
																																																			if (letter1 == "E P L")
																																																			{
																																																					correctLetter1 = "W";
																																																			}
																																																			else
																																																			{
																																																					if (letter1 == "P A E")
																																																					{
																																																							correctLetter1 = "X";
																																																					}
																																																					else
																																																					{
																																																							if (letter1 == "A C L")
																																																							{
																																																									correctLetter1 = "Y";
																																																							}
																																																							else
																																																							{
																																																									if (letter1 == "E C P")
																																																									{
																																																											correctLetter1 = "Z";
																																																									}
																																																									else
																																																									{
																																																											if (letter1 == "L A E")
																																																											{
																																																													correctLetter1 = "*";
																																																											}
																																																											else
																																																											{
																																																													if (letter1 == "A C P")
																																																													{
																																																															correctLetter1 = "*";
																																																													}
																																																													else
																																																													{
																																																															if (letter1 == "C E P")
																																																															{
																																																																	correctLetter1 = "*";
																																																															}
																																																															else
																																																															{
																																																																	symbolText.text = "E L C";
																																																																	correctLetter1 = "*";
																																																															}

																																																													}
																																																											}
																																																									}
																																																							}
																																																					}
																																																			}
																																																	}
																																															}
																																													}
																																											}
																																									}
																																							}
																																					}
																																			}
																																	}
																															}
																													}
																											}
																									}
																							}
																					}
																			}
																	}
															}
													}
											}
									}
							}

							if (letter2 == "P L A")
							{
									correctLetter2 = "A";
							}
							else
							{
									if (letter2 == "C L A")
									{
											correctLetter2 = "B";
									}
									else
									{
											if (letter2 == "L A P")
											{
													correctLetter2 = "C";
											}
											else
											{
													if (letter2 == "A C E")
													{
															correctLetter2 = "D";
													}
													else
													{
															if (letter2 == "C L E")
															{
																	correctLetter2 = "E";
															}
															else
															{
																	if (letter2 == "C E L")
																	{
																			correctLetter2 = "F";
																	}
																	else
																	{
																			if (letter2 == "E L A")
																			{
																					correctLetter2 = "G";
																			}
																			else
																			{
																					if (letter2 == "P A L")
																					{
																							correctLetter2 = "H";
																					}
																					else
																					{
																							if (letter2 == "P C L")
																							{
																									correctLetter2 = "I";
																							}
																							else
																							{
																									if (letter2 == "A L P")
																									{
																											correctLetter2 = "J";
																									}
																									else
																									{
																											if (letter2 == "L E C")
																											{
																													correctLetter2 = "K";
																											}
																											else
																											{
																													if (letter2 == "L P E")
																													{
																															correctLetter2 = "L";
																													}
																													else
																													{
																															if (letter2 == "E A L")
																															{
																																	correctLetter2 = "M";
																															}
																															else
																															{
																																	if (letter2 == "P E C")
																																	{
																																			correctLetter2 = "N";
																																	}
																																	else
																																	{
																																			if (letter2 == "L C P")
																																			{
																																					correctLetter2 = "O";
																																			}
																																			else
																																			{
																																					if (letter2 == "A E L")
																																					{
																																							correctLetter2 = "P";
																																					}
																																					else
																																					{
																																							if (letter2 == "A L E")
																																							{
																																									correctLetter2 = "Q";
																																							}
																																							else
																																							{
																																									if (letter2 == "L E P")
																																									{
																																											correctLetter2 = "R";
																																									}
																																									else
																																									{
																																											if (letter2 == "E P C")
																																											{
																																													correctLetter2 = "S";
																																											}
																																											else
																																											{
																																													if (letter2 == "P C E")
																																													{
																																															correctLetter2 = "T";
																																													}
																																													else
																																													{
																																															if (letter2 == "C A L")
																																															{
																																																	correctLetter2 = "U";
																																															}
																																															else
																																															{
																																																	if (letter2 == "C P E")
																																																	{
																																																			correctLetter2 = "V";
																																																	}
																																																	else
																																																	{
																																																			if (letter2 == "E P L")
																																																			{
																																																					correctLetter2 = "W";
																																																			}
																																																			else
																																																			{
																																																					if (letter2 == "P A E")
																																																					{
																																																							correctLetter2 = "X";
																																																					}
																																																					else
																																																					{
																																																							if (letter2 == "A C L")
																																																							{
																																																									correctLetter2 = "Y";
																																																							}
																																																							else
																																																							{
																																																									if (letter2 == "E C P")
																																																									{
																																																											correctLetter2 = "Z";
																																																									}
																																																									else
																																																									{
																																																											if (letter2 == "L A E")
																																																											{
																																																													correctLetter2 = "*";
																																																											}
																																																											else
																																																											{
																																																													if (letter2 == "A C P")
																																																													{
																																																															correctLetter2 = "*";
																																																													}
																																																													else
																																																													{
																																																															if (letter2 == "C E P")
																																																															{
																																																																	correctLetter2 = "*";
																																																															}
																																																															else
																																																															{
																																																																	symbolText.text = "E L C";
																																																																	correctLetter2 = "*";
																																																															}

																																																													}
																																																											}
																																																									}
																																																							}
																																																					}
																																																			}
																																																	}
																																															}
																																													}
																																											}
																																									}
																																							}
																																					}
																																			}
																																	}
																															}
																													}
																											}
																									}
																							}
																					}
																			}
																	}
															}
													}
											}
									}
							}

							if (letter3 == "P L A")
							{
									correctLetter3 = "A";
							}
							else
							{
									if (letter3 == "C L A")
									{
											correctLetter3 = "B";
									}
									else
									{
											if (letter3 == "L A P")
											{
													correctLetter3 = "C";
											}
											else
											{
													if (letter3 == "A C E")
													{
															correctLetter3 = "D";
													}
													else
													{
															if (letter3 == "C L E")
															{
																	correctLetter3 = "E";
															}
															else
															{
																	if (letter3 == "C E L")
																	{
																			correctLetter3 = "F";
																	}
																	else
																	{
																			if (letter3 == "E L A")
																			{
																					correctLetter3 = "G";
																			}
																			else
																			{
																					if (letter3 == "P A L")
																					{
																							correctLetter3 = "H";
																					}
																					else
																					{
																							if (letter3 == "P C L")
																							{
																									correctLetter3 = "I";
																							}
																							else
																							{
																									if (letter3 == "A L P")
																									{
																											correctLetter3 = "J";
																									}
																									else
																									{
																											if (letter3 == "L E C")
																											{
																													correctLetter3 = "K";
																											}
																											else
																											{
																													if (letter3 == "L P E")
																													{
																															correctLetter3 = "L";
																													}
																													else
																													{
																															if (letter3 == "E A L")
																															{
																																	correctLetter3 = "M";
																															}
																															else
																															{
																																	if (letter3 == "P E C")
																																	{
																																			correctLetter3 = "N";
																																	}
																																	else
																																	{
																																			if (letter3 == "L C P")
																																			{
																																					correctLetter3 = "O";
																																			}
																																			else
																																			{
																																					if (letter3 == "A E L")
																																					{
																																							correctLetter3 = "P";
																																					}
																																					else
																																					{
																																							if (letter3 == "A L E")
																																							{
																																									correctLetter3 = "Q";
																																							}
																																							else
																																							{
																																									if (letter3 == "L E P")
																																									{
																																											correctLetter3 = "R";
																																									}
																																									else
																																									{
																																											if (letter3 == "E P C")
																																											{
																																													correctLetter3 = "S";
																																											}
																																											else
																																											{
																																													if (letter3 == "P C E")
																																													{
																																															correctLetter3 = "T";
																																													}
																																													else
																																													{
																																															if (letter3 == "C A L")
																																															{
																																																	correctLetter3 = "U";
																																															}
																																															else
																																															{
																																																	if (letter3 == "C P E")
																																																	{
																																																			correctLetter3 = "V";
																																																	}
																																																	else
																																																	{
																																																			if (letter3 == "E P L")
																																																			{
																																																					correctLetter3 = "W";
																																																			}
																																																			else
																																																			{
																																																					if (letter3 == "P A E")
																																																					{
																																																							correctLetter3 = "X";
																																																					}
																																																					else
																																																					{
																																																							if (letter3 == "A C L")
																																																							{
																																																									correctLetter3 = "Y";
																																																							}
																																																							else
																																																							{
																																																									if (letter3 == "E C P")
																																																									{
																																																											correctLetter3 = "Z";
																																																									}
																																																									else
																																																									{
																																																											if (letter3 == "L A E")
																																																											{
																																																													correctLetter3 = "*";
																																																											}
																																																											else
																																																											{
																																																													if (letter3 == "A C P")
																																																													{
																																																															correctLetter3 = "*";
																																																													}
																																																													else
																																																													{
																																																															if (letter3 == "C E P")
																																																															{
																																																																	correctLetter3 = "*";
																																																															}
																																																															else
																																																															{
																																																																	symbolText.text = "E L C";
																																																																	correctLetter3 = "*";
																																																															}

																																																													}
																																																											}
																																																									}
																																																							}
																																																					}
																																																			}
																																																	}
																																															}
																																													}
																																											}
																																									}
																																							}
																																					}
																																			}
																																	}
																															}
																													}
																											}
																									}
																							}
																					}
																			}
																	}
															}
													}
											}
									}
							}
							Debug.LogFormat("[Symbolic Coordinates #{0}] The first display is {1}. The LEDs are {2}.", moduleId, letter1, digit1Colours);
							Debug.LogFormat("[Symbolic Coordinates #{0}] The correct input for stage 1 is {1}.", moduleId, correctLetter1 + correctDigit1);
		}





		public void OnlettersUp()
		{
				GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
				lettersUp.AddInteractionPunch(.5f);
				lettersIndex = ((lettersIndex + lettersEntries.Length) - 1) % lettersEntries.Length;
		    lettersText.text = lettersEntries[lettersIndex].ToString();
		}

		public void OnlettersDown()
		{
				GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
				lettersDown.AddInteractionPunch(.5f);
		    lettersIndex = (lettersIndex + 1) % lettersEntries.Length;
		    lettersText.text = lettersEntries[lettersIndex].ToString();
		}

		public void OndigitsUp()
		{
				GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
				digitsUp.AddInteractionPunch(.5f);
				digitsIndex = ((digitsIndex + digitsEntries.Length) - 1) % digitsEntries.Length;
		    digitsText.text = digitsEntries[digitsIndex].ToString();
		}

		public void OndigitsDown()
		{
				GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
				digitsDown.AddInteractionPunch(.5f);
		    digitsIndex = (digitsIndex + 1) % digitsEntries.Length;
		    digitsText.text = digitsEntries[digitsIndex].ToString();
		}

		public void Onsubmit()
		{
				GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
				GetComponent<KMSelectable>().AddInteractionPunch();

				switch (stage)
				{
				case 1:
				if (lettersText.text == correctLetter1 && digitsText.text == correctDigit1)
				{
						Debug.LogFormat("[Symbolic Coordinates #{0}] You submitted {1}. That is correct.", moduleId, lettersText.text + digitsText.text);
						pass1rend.GetComponent<Renderer>().material.mainTexture = onMat;
						GetComponent<KMSelectable>().AddInteractionPunch();
						symbolText.text = letter2;
						Audio.PlaySoundAtTransform("beep1", transform);
						if (digit2 == "0")
						{
								light1.material.mainTexture = aqua;
								light2.material.mainTexture = green;
								light3.material.mainTexture = yellow;
								correctDigit2 = "0";
								digit2Colours = "aqua, green & yellow";
						}
						else if (digit2 == "1")
						{
								light1.material.mainTexture = green;
								light2.material.mainTexture = yellow;
								light3.material.mainTexture = purple;
								correctDigit2 = "1";
								digit2Colours = "green, yellow & purple";
						}
						else if (digit2 == "2")
						{
							light1.material.mainTexture = green;
							light2.material.mainTexture = purple;
							light3.material.mainTexture = yellow;
							correctDigit2 = "2";
							digit2Colours = "green, purple & yellow";
						}
						else if (digit2 == "3")
						{
							light1.material.mainTexture = yellow;
							light2.material.mainTexture = aqua;
							light3.material.mainTexture = purple;
							correctDigit2 = "3";
							digit2Colours = "yellow, aqua & purple";
						}
						else if (digit2 == "4")
						{
							light1.material.mainTexture = aqua;
							light2.material.mainTexture = green;
							light3.material.mainTexture = purple;
							correctDigit2 = "4";
							digit2Colours = "aqua, green & purple";
						}
						else if (digit2 == "5")
						{
							light1.material.mainTexture = purple;
							light2.material.mainTexture = aqua;
							light3.material.mainTexture = green;
							correctDigit2 = "5";
							digit2Colours = "purple, aqua & green";
						}
						else if (digit2 == "6")
						{
							light1.material.mainTexture = yellow;
							light2.material.mainTexture = green;
							light3.material.mainTexture = purple;
							correctDigit2 = "6";
							digit2Colours = "yellow, green & purple";
						}
						else if (digit2 == "7")
						{
							light1.material.mainTexture = green;
							light2.material.mainTexture = aqua;
							light3.material.mainTexture = purple;
							correctDigit2 = "7";
							digit2Colours = "green, aqua & purple";
						}
						else if (digit2 == "8")
						{
							light1.material.mainTexture = aqua;
							light2.material.mainTexture = purple;
							light3.material.mainTexture = yellow;
							correctDigit2 = "8";
							digit2Colours = "aqua, purple & yellow";
						}
						else if (digit2 == "9")
						{
							light1.material.mainTexture = purple;
							light2.material.mainTexture = green;
							light3.material.mainTexture = aqua;
							correctDigit2 = "9";
							digit2Colours = "purple, green & aqua";
						}
						else if (digit2 == "*1")
						{
							light1.material.mainTexture = yellow;
							light2.material.mainTexture = green;
							light3.material.mainTexture = aqua;
							correctDigit2 = "*";
							digit2Colours = "yellow, green & aqua";
						}
						else
						{
							light1.material.mainTexture = green;
							light2.material.mainTexture = yellow;
							light3.material.mainTexture = aqua;
							correctDigit2 = "*";
							digit2Colours = "green, yellow & aqua";
						}
						Debug.LogFormat("[Symbolic Coordinates #{0}] The second display is {1}. The LEDs are {2}.", moduleId, letter2, digit2Colours);
						Debug.LogFormat("[Symbolic Coordinates #{0}] The correct input for stage 2 is {1}.", moduleId, correctLetter2 + correctDigit2);
						stage++;
				}
				else
				{
						GetComponent<KMBombModule>().HandleStrike();
						Debug.LogFormat("[Symbolic Coordinates #{0}] Strike! You submitted {1}. I was expecting {2}.", moduleId, lettersText.text + digitsText.text, correctLetter1 + correctDigit1);

				}
				break;

				case 2:
				if (lettersText.text == correctLetter2 && digitsText.text == correctDigit2)
				{
						Debug.LogFormat("[Symbolic Coordinates #{0}] You submitted {1}. That is correct.", moduleId, lettersText.text + digitsText.text);
						pass2rend.GetComponent<Renderer>().material.mainTexture = onMat;
						GetComponent<KMSelectable>().AddInteractionPunch();
						symbolText.text = letter3;
						Audio.PlaySoundAtTransform("beep2", transform);
						if (digit3 == "0")
						{
								light1.material.mainTexture = aqua;
								light2.material.mainTexture = green;
								light3.material.mainTexture = yellow;
								correctDigit3 = "0";
								digit3Colours = "aqua, green & yellow";

						}
						else if (digit3 == "1")
						{
								light1.material.mainTexture = green;
								light2.material.mainTexture = yellow;
								light3.material.mainTexture = purple;
								correctDigit3 = "1";
								digit3Colours = "green, yellow & purple";
						}
						else if (digit3 == "2")
						{
							light1.material.mainTexture = green;
							light2.material.mainTexture = purple;
							light3.material.mainTexture = yellow;
							correctDigit3 = "2";
							digit3Colours = "green, purple & yellow";
						}
						else if (digit3 == "3")
						{
							light1.material.mainTexture = yellow;
							light2.material.mainTexture = aqua;
							light3.material.mainTexture = purple;
							correctDigit3 = "3";
							digit3Colours = "yellow, aqua & purple";
						}
						else if (digit3 == "4")
						{
							light1.material.mainTexture = aqua;
							light2.material.mainTexture = green;
							light3.material.mainTexture = purple;
							correctDigit3 = "4";
							digit3Colours = "aqua, green & purple";
						}
						else if (digit3 == "5")
						{
							light1.material.mainTexture = purple;
							light2.material.mainTexture = aqua;
							light3.material.mainTexture = green;
							correctDigit3 = "5";
							digit3Colours = "purple, aqua & green";
						}
						else if (digit3 == "6")
						{
							light1.material.mainTexture = yellow;
							light2.material.mainTexture = green;
							light3.material.mainTexture = purple;
							correctDigit3 = "6";
							digit3Colours = "yellow, green & purple";
						}
						else if (digit3 == "7")
						{
							light1.material.mainTexture = green;
							light2.material.mainTexture = aqua;
							light3.material.mainTexture = purple;
							correctDigit3 = "7";
							digit3Colours = "green, aqua & purple";
						}
						else if (digit3 == "8")
						{
							light1.material.mainTexture = aqua;
							light2.material.mainTexture = purple;
							light3.material.mainTexture = yellow;
							correctDigit3 = "8";
							digit3Colours = "aqua, purple & yellow";
						}
						else if (digit3 == "9")
						{
							light1.material.mainTexture = purple;
							light2.material.mainTexture = green;
							light3.material.mainTexture = aqua;
							correctDigit3 = "9";
							digit3Colours = "purple, green & aqua";
						}
						else if (digit3 == "*1")
						{
							light1.material.mainTexture = yellow;
							light2.material.mainTexture = green;
							light3.material.mainTexture = aqua;
							correctDigit3 = "*";
							digit3Colours = "yellow, green & aqua";
						}
						else
						{
							light1.material.mainTexture = green;
							light2.material.mainTexture = yellow;
							light3.material.mainTexture = aqua;
							correctDigit3 = "*";
							digit3Colours = "green, yellow & aqua";
						}
						Debug.LogFormat("[Symbolic Coordinates #{0}] The third display is {1}. The LEDs are {2}.", moduleId, letter3, digit3Colours);
						Debug.LogFormat("[Symbolic Coordinates #{0}] The correct input for stage 3 is {1}.", moduleId, correctLetter3 + correctDigit3);
						stage++;
				}
				else
				{
						GetComponent<KMBombModule>().HandleStrike();
						Debug.LogFormat("[Symbolic Coordinates #{0}] Strike! You submitted {1}. I was expecting {2}.", moduleId, lettersText.text + digitsText.text, correctLetter2 + correctDigit2);
				}
				break;

				case 3:
				if (lettersText.text == correctLetter3 && digitsText.text == correctDigit3)
				{
						Debug.LogFormat("[Symbolic Coordinates #{0}] You submitted {1}. That is correct. Module disarmed.", moduleId, lettersText.text + digitsText.text);
						pass3rend.GetComponent<Renderer>().material.mainTexture = onMat;
						GetComponent<KMSelectable>().AddInteractionPunch();
						Audio.PlaySoundAtTransform("beep3", transform);
						symbolText.text = "PLACE";
						light1.material.mainTexture = offMat;
						light2.material.mainTexture = offMat;
						light3.material.mainTexture = offMat;

						GetComponent<KMBombModule>().HandlePass();
						stage++;
				}
				else
				{
						GetComponent<KMBombModule>().HandleStrike();
						Debug.LogFormat("[Symbolic Coordinates #{0}] Strike! You submitted {1}. I was expecting {2}.", moduleId, lettersText.text + digitsText.text, correctLetter3 + correctDigit3);

				}
				break;

				default:
				GetComponent<KMBombModule>().HandleStrike();
				Debug.LogFormat("[Symbolic Coordinates #{0}] Strike! The mothership has been contacted. Please do not re-submit coordinates.", moduleId);
				break;
				}


		}

}
