using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlatformerGame
{
    public class ModManager
    {
        private const int DoubleJumpDefault = 0;
        private const int GravityDefault = 3000;
        private const bool JumpManiaDefault = false;
        private const bool CantStopDefault = false;

        public static int DoubleJump = DoubleJumpDefault;
        public static int Gravity = GravityDefault;
        public static bool JumpMania = JumpManiaDefault;
        public static bool CantStop = CantStopDefault;

        public static void SetupMods(Level level)
        {
            ResetMods();
            foreach (var pair in level.Map.Properties)
			{
				string key = pair.Key;
				string property = pair.Value;

				switch (key)
                {
				case "DoubleJump":
					DoubleJump = ParseInt(property) ?? DoubleJumpDefault;
					break;
				case "Gravity":
					Gravity = ParseInt(property) ?? GravityDefault;
					break;
				case "JumpMania":
					JumpMania = ParseBool(property) ?? JumpManiaDefault;
					break;
				case "CantStop":
					CantStop = ParseBool(property) ?? CantStopDefault;
                    break;
                }
            }
        }

        private static void ResetMods()
        {
            DoubleJump = DoubleJumpDefault;
            Gravity = GravityDefault;
            JumpMania = JumpManiaDefault;
            CantStop = CantStopDefault;
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

		private static float? ParseFloat(string s)
		{
			float value;
			if (float.TryParse(s, out value))
			{
				// Parse successful. value can be any integer
			}
			else
			{
				return null;
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
