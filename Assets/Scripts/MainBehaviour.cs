using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainBehaviour : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _consumedString; //TextComponent that displays daily ammount of water consumption
    [SerializeField]
    private Slider _slider; //Slider displaying percentage of daily goal
    [SerializeField]
    private TMP_InputField _customAmmountInput;

    private float _consumedWater = 0f;
    private float _consumptionGoal = 2.2f;
    // Start is called before the first frame update
    void Start()
    {
        AddHydration(0); //Update UI without changeing values
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void AddHydration(float addedWater)
    {
        _consumedWater += addedWater;
        _consumedString.text = "Today's Goal: " + _consumedWater + "/" + _consumptionGoal + "l"; //Update string
        _slider.value = (_consumedWater == 0) ? 0 : (_consumedWater / _consumptionGoal); //update slider without dividing by 0 (because that would break the universe or something...)
    }

    public void AddCustomHydration()
    {
        AddHydration(float.Parse(_customAmmountInput.text));
    }
}