using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Crosswords : MonoBehaviour
{
    public GameObject go_slot;
    public Text definition;

    List<Word> _words;
    List<Slot> _slots, _selectedSlots;
    int _width, _height;
    APIData _apiData;
    char[,] _grid;
    Slot _slotTapped;

    public void CreateCrossword(APIData apiData)
    {
        InitializeData(apiData);
        StartCoroutine(CreateGrid());
    }

    void InitializeData(APIData apiData)
    {
        _slots?.ForEach(x => Destroy(x.gameObject));
        _slots = new List<Slot>();
        _words = new List<Word>();
        _selectedSlots = new List<Slot>();
        _apiData = apiData;

        for (int i = 0; i < apiData.data.rows.Length; i++)
           _apiData.data.rows[i] = apiData.data.rows[i].Replace(",", "");

        _width = apiData.data.rows[0].Length;
        _height = apiData.data.rows.Length;

        _grid = new char[_height, _width];
    }

    IEnumerator CreateGrid()
    {
        var panel = GameObject.Find("Slots").transform;
        panel.gameObject.GetComponent<GridLayoutGroup>().constraintCount = _width;
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                var slot = Instantiate(go_slot);
                slot.name = $"Slot [{j},{i}]";
                var sl = slot.AddComponent<Slot>();
                sl.row = j;
                sl.col = i;
                _slots.Add(sl);
                slot.transform.SetParent(panel);
                slot.GetComponent<RectTransform>().localPosition = new Vector3(-400 + i * 70, 400 - j * 70, 0);
                var letter = _apiData.data.rows[j][i];
                sl.letter = letter;
                _grid[j, i] = letter;
                var img = slot.transform.GetComponent<Image>();
                sl.image = img;
                if (letter == '-')
                    img.color = Color.black;
                else
                {
                    var txt = slot.transform.GetChild(0).GetComponent<Text>();
                    txt.text = letter.ToString().ToUpper();
                    sl.text = txt;
                    slot.GetComponent<Button>().onClick.AddListener(OnSlotClick);
                }
                //we could split the generation in frames for bigger grids, but is not needed at this moment
                //yield return new WaitForEndOfFrame();
            }
        }
        yield return new WaitForEndOfFrame();
        GenerateVerticalWords();
        GenerateHorizontalWords();
    }
    void GenerateVerticalWords()
    {
        var word_index = 0;
        for (int i = 0; i < _width; i++)
        {
            var word = new Word();
            for (int j = 0; j < _height; j++)
            {
                Action addWord = () =>
                {
                    word.definition = _apiData.data.definitions.down[word_index];
                    _words.Add(word);
                    word = new Word();
                    word_index++;
                };

                var slot = _slots.Where(x => x.row == j && x.col == i).First();

                if (slot.letter != '-')
                {
                    word.word += slot.letter;
                    word.slots.Add(slot);

                    if (j == _height - 1 && word.word.Length > 1)
                        addWord();
                }
                else if (word.word.Length > 1)
                    addWord();
                else
                    word = new Word();
            }

        }
    }

    void GenerateHorizontalWords()
    {
        var word_index = 0;
        for (int i = 0; i < _height; i++)
        {
            var word = new Word();
            for (int j = 0; j < _width; j++)
            {
                Action addWord = () =>
                {
                    word.definition = _apiData.data.definitions.across[word_index];
                    _words.Add(word);
                    word = new Word();
                    word_index++;
                };

                var slot = _slots.Where(x => x.row == i && x.col == j).First();

                if (slot.letter != '-')
                {
                    word.word += slot.letter;
                    word.slots.Add(slot);

                    if (j == _width - 1 && word.word.Length > 1)
                        addWord();
                }
                else if (word.word.Length > 1)
                    addWord();
                else
                    word = new Word();
            }

        }
    }
    public void OnSlotClick()
    {
        var slot = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Slot>();
        bool vertical = _slotTapped == slot;
        var words = _words.Where(x => x.slots.Contains(slot));

        Action switchTappedSlot = () =>
        {
            if (_slotTapped != slot)
                _slotTapped = slot;
            else
                _slotTapped = null;
        };

        if (_selectedSlots != null)
            _selectedSlots.ForEach(x => x.image.color = Color.white);


        if (words.Count() > 0)
        {
            var word = (vertical ? words.First() : words.Last());
            definition.text = word.definition;
            _selectedSlots = word.slots;
            word.slots.ForEach(x =>
            {
                x.text.color = new Color(0f, 0f, 0f, 1f);
                x.image.color = Color.yellow;
            });

            slot.image.color = new Color(1f, 0.5f, 0f, 1f);
            switchTappedSlot();
        }
        else
        {
            switchTappedSlot();
            OnSlotClick();
        }

    }
}
