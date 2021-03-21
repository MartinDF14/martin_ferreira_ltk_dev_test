using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public static class WordUtils
{
    public enum Direction
    {
        Horizontal,
        Vertical
    }

    public static string ReplaceLetterAtIndex(this string word, int index, char character = '-')
    {
        char[] ch = word.ToCharArray();
        ch[index] = character;
        return new string(ch);
    }

    public static int BlockedChars(this string word)
    {
        return word.Where(x => x == '-').Count();
    }

    public static List<string> GenerateDirtyWordList(this char[,] grid, Direction direction, int width, int height)
    {
        var list = new List<string>();
        if(direction == Direction.Vertical)
        {
            for (int i = 0; i < width; i++)
            {
                var word = "";
                for (int j = 0; j < height; j++)
                {
                    word += grid[j, i];
                }
                word.Remove(word.Length - 1);
                list.Add(word);
            }
        }
        else
        {
            for (int i = 0; i < height; i++)
            {
                var word = "";
                for (int j = 0; j < width; j++)
                {
                    word += grid[i, j] + ",";
                }
                word.Remove(word.Length - 1);
                list.Add(word);
            }
        }
        return list;
    }

    public static string[] CreateDefinition(this List<string> list)
    {
        return list.GenerateCleanWordList().Aggregate(new List<string>(), (x, y) => { x.Add("Definition of " + y); return x; }, (x) => x).ToArray();
    }

    public static List<string> GenerateCleanWordList(this List<string> dirtyList)
    {
        var words = new List<string>();
        var word = "";

        foreach (var completeWord in dirtyList)
        {
            for (int j = 0; j < completeWord.Length; j++)
            {
                if (completeWord[j] != '-' && completeWord[j] != ',')
                    word += completeWord[j];

                if (j == completeWord.Length - 1 || completeWord[j] == '-')
                {
                    if (word.Length > 1)
                        words.Add(word);
                    word = "";
                }
            }
        }
        return words;
    }

}