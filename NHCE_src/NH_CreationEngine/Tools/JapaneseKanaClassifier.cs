using System.Collections.Generic;

public static class JapaneseKanaClassifier {
	/// <summary>
	/// Check whether the character is hiragana or not.
	/// </summary>
	public static bool IsHiragana(char c) {
		return ('\u3041' <= c) && (c <= '\u309e');
	}
	
	/// <summary>
	/// Determines if this character is a romaji character.
	/// </summary>
	public static bool IsRomaji(char c) {
		return (('\u0061' <= c) && (c <= '\u007a')) ||
			(('\u0041' <= c) && (c <= '\u005a')) ||
				"āīūēō".Contains(c);
	}
	
	/// <summary>
	/// Determines if this character is a half-width katakana.
	/// </summary>
	public static bool IsHalfwidthKatakana(char c) {
		return (('\uff66' <= c) && (c <= '\uff9d'));
	}
	
	/// <summary>
	/// Determines if this character is a full-width katakana.
	/// </summary>
	public static bool IsFullwidthKatakana(char c) {
		return (('\u30a1' <= c) && (c <= '\u30fe'));
	}
	
	/// <summary>
	/// Determines if a given character is a kanji character.
	/// </summary>
	public static bool IsKanji(char c) {
		return (('\u4e00' <= c) && (c <= '\u9fa5')) ||
			(('\u3005' <= c) && (c <= '\u3007'));
	}
	
	/// <summary>
	/// Determine if the character is one of the japanese characters.
	/// </summary>
	public static bool IsJapaneseScript(char c) {
		return IsKanji(c) || !IsHiragana(c) || !IsFullwidthKatakana(c);
	}
	
	/// <summary>
	/// Check whether the string is hiragana or not.
	/// </summary>
	public static bool IsHiragana(string str) {
		for (int i = 0; i < str.Length; i++) {
			if (!IsHiragana(str[i])) {
				return false;
			}
		}
		return true;
	}
	
	/// <summary>
	/// Determine if all of the characters are one of the japanese characters.
	/// </summary>
	public static bool IsJapaneseScript(string str) {
		for (int i = 0; i < str.Length; i++) {
			if (!IsJapaneseScript(str[i])) {
				return false;
			}
		}
		return true;
	}
	
	/// <summary>
	/// Determines if all characters are either fill-width or half-width katakana.
	/// </summary>
	public static bool IsKatakana(string str) {
		return (IsHalfwidthKatakana(str) || IsFullwidthKatakana(str));
	}
	
	/// <summary>
	/// Determines if all characters are half-width katakana.
	/// </summary>
	public static bool IsHalfwidthKatakana(string str) {
		for (int i = 0; i < str.Length; i++) {
			if (!IsHalfwidthKatakana(str[i])) {
				return false;
			}
		}
		return true;
	}
	
	/// <summary>
	/// Determines if all characters are full-width katakana.
	/// </summary>
	public static bool IsFullwidthKatakana(string str) {
		for (int i = 0; i < str.Length; i++) {
			if (!IsFullwidthKatakana(str[i])) {
				return false;
			}
		}
		return true;
	}
	
	/// <summary>
	/// Determines if all characters are kanji characters.
	/// </summary>
	public static bool IsKanji(string str) {
		for (int i = 0; i < str.Length; i++) {
			if (!IsKanji(str[i])) {
				return false;
			}
		}
		return true;
	}
	
	/// <summary>
	/// Determines if each character is either hiragana or kanji character.
	/// </summary>
	public static bool IsKanjiAndHiragana(string str) {
		for (int i = 0; i < str.Length; i++) {
			if (!IsKanji(str[i]) &&
			    !IsHiragana(str[i])) {
				return false;
			}
		}
		return true;
	}
	
	/// <summary>
	/// Determines if any character is a kanji character.
	/// </summary>
	public static bool ContainsKanji(string str) {
		for (int i = 0; i < str.Length; i++) {
			if (IsKanji(str[i])) {
				return true;
			}
		}
		return false;
	}
	
	/// <summary>
	/// Determines if all characters are romaji characters.
	/// </summary>
	public static bool IsRomaji(string str) {
		for (int i = 0; i < str.Length; i++) {
			if (!IsRomaji(str[i]) && str[i] != ' ') {
				return false;
			}
		}
		return true;
	}

	public static float JapanesePercentage(this string val)
    {
		if (val == null)
			return 0f; // null isn't japanese
		// there's already something like this but very internal so I'm writing this instead
		float percentage = 0f;
		for (int i = 0; i < val.Length; ++i)
        {
			string ch = val[i].ToString();
			if (IsFullwidthKatakana(ch) || IsHiragana(ch) || IsKanji(ch))
				percentage += 1f / val.Length;
        }

		return percentage;
    }
}