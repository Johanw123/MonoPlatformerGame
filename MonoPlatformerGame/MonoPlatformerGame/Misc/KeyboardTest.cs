using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoPlatformerGame
{
	public class KeyboardTest
	{
		Dictionary<Keys, bool> dicKeys = new Dictionary<Keys, bool>();
		public string output = "";
		KeyboardState mainState;
		KeyboardState prevState;
		public KeyboardTest()
		{



		}
		Keys[] a;
		Keys prevKey;
		public void Update()
		{
			prevState = mainState;
			mainState = Keyboard.GetState();
			if(a != null && a.Length > 0)
				prevKey = a[0];

			a = mainState.GetPressedKeys();

			if(a.Length <= 0)
			{
				dicKeys[prevKey] = false;
				return;
			}
			Keys currKey = a[0];

			if(prevKey != currKey)
				dicKeys[prevKey] = false;

			if(dicKeys.ContainsKey(currKey))
			{
				//Key found
				if(dicKeys[currKey])
				{
					//Key is already pressed down
					//TODO
					//add delay and then start adding char to the output might need keys in struct with timer etc

				}
				else
				{
					//Key was not pressed down
					dicKeys[currKey] = true;
					output += Convert(a);
				}

			}
			else
			{
				//Key was not found, add it
				dicKeys.Add(currKey, true);
				output += Convert(a);
			}


		}


		public string Convert(Keys[] keys)
		{
			string output = "";
			bool usesShift = (keys.Contains(Keys.LeftShift) || keys.Contains(Keys.RightShift));

			foreach (Keys key in keys)
			{
				//thanks SixOfEleven @ DIC
				//if (prevState.IsKeyUp(key))
				//	continue;

				if(key >= Keys.A && key <= Keys.Z)
					output += key.ToString();
				else if(key >= Keys.NumPad0 && key <= Keys.NumPad9)
					output += ((int)(key - Keys.NumPad0)).ToString();
				else if(key >= Keys.D0 && key <= Keys.D9)
				{
					string num = ((int)(key - Keys.D0)).ToString();
					#region special num chars
					if(usesShift)
					{
						switch(num)
						{
							case "1":
								{
									num = "!";
								}
								break;
							case "2":
								{
									num = "@";
								}
								break;
							case "3":
								{
									num = "#";
								}
								break;
							case "4":
								{
									num = "$";
								}
								break;
							case "5":
								{
									num = "%";
								}
								break;
							case "6":
								{
									num = "^";
								}
								break;
							case "7":
								{
									num = "&";
								}
								break;
							case "8":
								{
									num = "*";
								}
								break;
							case "9":
								{
									num = "(";
								}
								break;
							case "0":
								{
									num = ")";
								}
								break;
							default:
								//wtf?
								break;
						}
					}
					#endregion
					output += num;
				}
				else if(key == Keys.OemPeriod)
					output += ".";
				else if(key == Keys.OemTilde)
					output += "'";
				else if(key == Keys.Space)
					output += " ";
				else if(key == Keys.OemMinus)
					output += "-";
				else if(key == Keys.OemPlus)
					output += "+";
				else if(key == Keys.OemQuestion && usesShift)
					output += "?";
				else if(key == Keys.Back) //backspace
				{
				
					if(output.Length >= 3)
					{
						output = output.Remove(output.Length - 1, 1);
					}

					

				}

				if (!usesShift) //shouldn't need to upper because it's automagically in upper case
					output = output.ToLower();
			}
			return output;
		}



	}
}

