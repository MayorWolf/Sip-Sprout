using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Android;
using Unity.Notifications.Android;

public class MainBehaviour : MonoBehaviour
{
    /**
     * TODO:
     * - Daily Reset
     * - Better Notifications
     * 
     */




    [SerializeField]
    private TMP_Text _consumedString; //TextComponent that displays daily ammount of water consumption
    [SerializeField]
    private Slider _slider; //Slider displaying percentage of daily goal
    [SerializeField]
    private TMP_InputField _customAmmountInput;
    [SerializeField]
    Sprite[] _plantStatus;
    [SerializeField]
    private Image _plantDisplay;

    string[] _notifTitles = { "I am thirsty!", "Hydration Time!", "We've been trying to reach you about your extended car warranty." };
    
    private float _consumedWater = 0f;
    private float _consumptionGoal = 2.2f;


    // Start is called before the first frame update
    void Start()
    {
        //Load saved data
        _consumedWater = PlayerPrefs.GetFloat("ConsumedWater");
        _consumptionGoal = (PlayerPrefs.GetFloat("WaterGoal") == 0) ? 2.2f : PlayerPrefs.GetFloat("WaterGoal");

        //Update UI without changeing values
        AddHydration(0);

        //Setup Notifications
        _NotificationSetup();


    }

    // Update is called once per frame
    void Update()
    {

    }

    //Increase ammount of water the user drank today
    public void AddHydration(float addedWater)
    {
        //checked for negative inputs
        _consumedWater = (_consumedWater + addedWater >= 0) ? _consumedWater += addedWater : 0;

        //Update text, slider & image
        _consumedString.text = "Today's Goal: " + _consumedWater + "/" + _consumptionGoal + "l";
        _slider.value = (_consumedWater == 0) ? 0 : (_consumedWater / _consumptionGoal); //without dividing by 0 (because that would break the universe or something...)
        _UpdateImage();

        //Save new data
        PlayerPrefs.SetFloat("ConsumedWater", _consumedWater);
        PlayerPrefs.Save();
    }

    //Parses Input string to float for custom Hydration values
    public void AddCustomHydration()
    {
        AddHydration(float.Parse(_customAmmountInput.text));
    }

    //Reset to 0 (DEBUG ONLY)
    public void ResetHydration()
    {
        _consumedWater = 0;
        AddHydration(0);
    }

    //Basic notification setup for Android
    private void _NotificationSetup()
    {
        //Setup Notification channel
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        //Get Notification Permissions
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }

        //Throw notification
        var notification = new AndroidNotification();
        notification.Title = _notifTitles[Random.Range(0,_notifTitles.Length)];
        notification.Text = "Remember to drink Water from time to time. You and your plants need it.";
        notification.FireTime = System.DateTime.Now.AddSeconds(15);

        AndroidNotificationCenter.SendNotification(notification, "channel_id");
    }

    //Update plant image based on curren water percentage
    private void _UpdateImage()
    {
        var temp = _plantDisplay.GetComponent<Image>();
        if (_consumedWater / _consumptionGoal <= 0.33f)
        {
            temp.sprite = _plantStatus[2];
        }
        else if (_consumedWater / _consumptionGoal <= 0.66f)
        {
            temp.sprite = _plantStatus[1];
        }
        else
        {
            temp.sprite = _plantStatus[0];
        }

    }
}