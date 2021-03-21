using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MockedResponse
{
    string _chars = "bcdfghijklmpqrstvwxyz";
    string _vowels = "aeiou";
    char[,] _grid;
    int _width, _height;
    List<string> _rowList;
    List<string> v_list = new List<string>();
    public APIData Create(int width, int height)
    {
        _grid = new char[height, width];
        _rowList = new List<string>();
        _width = width;
        _height = height;
        CreateWords();
        return new APIData()
        {
            status = 200,
            message = "Mocked Response",
            errors = null,
            data = new Data()
            {
                rows = _rowList.ToArray(),
                definitions = CreateDefinitions()
            }
        };
    }

    void CreateWords()
    {
        PopulateGrid();
        GenerateRandomBlackSquares();
        FreeIsolatedLetters();

        v_list = _grid.GenerateDirtyWordList(WordUtils.Direction.Vertical, _width, _height);

        CleanVerticalWords(v_list);

        _rowList = _grid.GenerateDirtyWordList(WordUtils.Direction.Horizontal, _width, _height);
    }

    void PopulateGrid()
    {
        for (int i = 0; i < _height; i++)
        {
            bool _isVowel = i % 2 == 0;
            var word = "";
            for (int j = 0; j < _width; j++)
            {
                _grid[i, j] = (_isVowel ? _vowels[UnityEngine.Random.Range(0, _vowels.Length)] : _chars[UnityEngine.Random.Range(0, _chars.Length)]);
                word += _grid[i, j];
                _isVowel = !_isVowel;
            }
        }
    }

    void GenerateRandomBlackSquares()
    {
        for (int i = 0; i < _height; i++)
        {
            var blocked = UnityEngine.Random.Range(2, 5);
            for (int j = 0; j < blocked; j++)
            {
                _grid[i, UnityEngine.Random.Range(0, _width)] = '-';
            }
        }
    }

    void FreeIsolatedLetters()
    {
        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                bool l_blocked = (j - 1 < 0) || (_grid[i, j - 1] == '-'),
                     r_blockd = (j + 1 > _width - 1) || (_grid[i, j + 1] == '-'),
                     d_blocked = (i - 1 < 0) || (_grid[i - 1, j] == '-'),
                     u_blocked = (i + 1 > _height - 1) || (_grid[i + 1, j] == '-');

                if (_grid[i, j] != '-' && l_blocked && r_blockd && d_blocked && u_blocked)
                {
                    if (j - 1 > 0 && _grid[i, j - 1] == '-')
                        _grid[i, j - 1] = _vowels[UnityEngine.Random.Range(0, _vowels.Length)];
                    else if (j + 1 < _width - 1 && _grid[i, j + 1] == '-')
                        _grid[i, j + 1] = _vowels[UnityEngine.Random.Range(0, _vowels.Length)];
                }

            }
        }
    }

    void CleanVerticalWords(List<string> v_list)
    {
        for (int i = 0; i < v_list.Count; i++)
        {
            while (v_list[i].BlockedChars() > 5)
            {
                var rnd = Mathf.FloorToInt(UnityEngine.Random.Range(0, v_list[i].Length - 1));
                var letter = _vowels[UnityEngine.Random.Range(0, _vowels.Length)];
                v_list[i] = v_list[i].ReplaceLetterAtIndex(rnd, letter);
                _grid[rnd, i] = letter;
            }
        }
    }

    Definitions CreateDefinitions()
    {
        return new Definitions()
        {
            across = _rowList.CreateDefinition(),
            down = v_list.CreateDefinition()
        };
    }
}

