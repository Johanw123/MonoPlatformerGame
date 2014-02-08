using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlatformerGame
{
    public class ModManager
    {
        public static int DoubleJump = 0;
        public static int Gravity = 3000;
        public static bool JumpMania = false;
        public static bool CantStop = false;

        public static void SetupMods(Level level)
        {
            foreach (var pair in level.Map.Properties)
			{
				string key = pair.Key;
				string property = pair.Value;

				switch (key)
                {
				case "DoubleJump":
					DoubleJump = ParseInt(property) ?? DoubleJump;
					break;
				case "Gravity":
					Gravity = ParseInt(property) ?? Gravity;
					break;
				case "JumpMania":
					JumpMania = ParseBool(property) ?? JumpMania;
					break;
				case "CantStop":
					CantStop = ParseBool(property) ?? CantStop;
                    break;
                }
            }
        }

		private static bool? ParseBool(string s)
		{
			bool isBool;
			bool? returnValue = null;

			returnValue = bool.TryParse (s, out isBool);

			if (!isBool)
			{
				returnValue = IntToBool (ParseInt (s));
			}

			return returnValue;
		}

		private static int? ParseInt(string s)
		{
			int value;
			if (int.TryParse(s, out value))
			{
				// Parse successful. value can be any integer
			}
			else
			{
				return null;
			}

			return value;
		}

		private static float ParseFloat(string s)
		{
			float value;
			if (float.TryParse(s, out value))
			{
				// Parse successful. value can be any integer
			}
			else
			{
				// Parse failed. value will be 0.
			}

			return value;
		}

		private static bool IntToBool(int i)
		{
			if (i == 1)
				return true;
			else
				return false;
		}

		private static bool IntToBool(int? i)
		{
			if (i == 1)
				return true;
			else
				return false;
		}
    }
}
