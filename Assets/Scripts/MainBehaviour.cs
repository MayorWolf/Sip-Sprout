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
     * - Test Notifications and reset
     * 
     */

    [SerializeField]
    private TMP_Text _consumedString; //TextComponent that displays daily ammount of water consumption
    [SerializeField]
    private TMP_Text _notificationToggleString;//TextComponent displaying the current notification status
    [SerializeField]
    private Slider _slider; //Slider displaying percentage of daily goal
    [SerializeField]
    private TMP_InputField _customAmmountInput; 
    [SerializeField]
    private TMP_InputField _customGoalInput;
    [SerializeField]
    Sprite[] _plantStatus; //List of plant sprites to display healthieness
    [SerializeField]
    private Image _plantDisplay; //Image displaying plant using diefferent plant sprites

    string[] _notifTitles = { "I am thirsty!", "Hydration Time!", "We've been trying to reach you about your extended car warranty." };
    
    private float _consumedWater = 0f;
    private float _consumptionGoal = 2.2f;
    private bool _notificationActive = true;

    private System.DateTime _lastDrinkTime, _lastUsage = System.DateTime.Now;


    // Start is called before the first frame update
    void Start()
    {
        _loadSavedData();

        _CheckForReset();
        _customGoalInput.text = _consumptionGoal.ToString();

        //Update UI without changeing values
        AddHydration(0);

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
        _consumedString.text = "Today's Goal: " + _consumedWater.ToString("G3") + "/" + _consumptionGoal + "l";
        _slider.value = (_consumedWater == 0) ? 0 : (_consumedWater / _consumptionGoal); //without dividing by 0 (because that would break the universe or something...)
        _UpdateImage();

        //Save new data
        PlayerPrefs.SetFloat("ConsumedWater", _consumedWater);

        if(addedWater > 0)
        {

            _ResetHourlyNotif();

        }

        PlayerPrefs.Save();
    }

    //Parses Input string to float for custom Hydration values
    public void AddCustomHydration()
    {
        AddHydration(float.Parse(_customAmmountInput.text));
    }

    //Set a custom daily goal
    public void SetCustomGoal()
    {
        if(float.Parse(_customGoalInput.text) > 0)
        {
            _consumptionGoal = float.Parse(_customGoalInput.text);

            //Save new data
            PlayerPrefs.SetFloat("WaterGoal", _consumptionGoal);
            PlayerPrefs.Save();

            AddHydration(0);
        }
    }

    //Reset )
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

        
    }

    private void _ResetHourlyNotif()
    {
        _lastDrinkTime = System.DateTime.Now;
        PlayerPrefs.SetString("LastDrink", _lastDrinkTime.ToString());
        PlayerPrefs.Save();

        //Remove scheduled notifications
        AndroidNotificationCenter.CancelAllScheduledNotifications();

        if (_notificationActive)
        {
            //Throw notification
            var notification = new AndroidNotification();
            notification.Title = _notifTitles[Random.Range(0, _notifTitles.Length)];
            notification.Text = "Remember to drink Water from time to time. You and your plants need it.";
            if (System.DateTime.Now.Hour + 1 >= 8 && System.DateTime.Now.Hour + 1 <= 20)
            {
                notification.FireTime = System.DateTime.Now.AddHours(1);
                Debug.Log("Timer set for " + System.DateTime.Now.AddHours(1));
            }
            else
            {
                notification.FireTime = _GetNext8AM();
                Debug.Log("QUIET TIME! Timer set for " + _GetNext8AM());
            }

            AndroidNotificationCenter.SendNotification(notification, "channel_id");

        }
    }

    public void ToggleNotifications()
    {
        _notificationActive = !_notificationActive;

        _notificationToggleString.text = (_notificationActive) ? "Notifications: ON" : "Notifications: OFF";

        int boolToInt = (_notificationActive) ? 1 : 0;
        PlayerPrefs.SetInt("NotificationsActive", boolToInt);
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

    private System.DateTime _GetNext8AM()
    {
        System.DateTime now = System.DateTime.Now;
        if (now.Hour >= 8)
        {
            now = now.AddDays(1);
        }
        return new System.DateTime(now.Year, now.Month, now.Day, 8, 0, 0);
    }

    private void _CheckForReset()
    {
        if (PlayerPrefs.GetString("LastUsage") != "") { _lastUsage = System.DateTime.Parse(PlayerPrefs.GetString("LastUsage")); }

        if (_lastUsage.Day < System.DateTime.Now.Day)
        {
            ResetHydration();
        }
        PlayerPrefs.SetString("LastUsage", System.DateTime.Now.ToString());
        PlayerPrefs.Save();
    }

    private void _loadSavedData()
    {
        //Load saved data
        _consumedWater = PlayerPrefs.GetFloat("ConsumedWater");
        _consumptionGoal = (PlayerPrefs.GetFloat("WaterGoal") == 0) ? 2.2f : PlayerPrefs.GetFloat("WaterGoal");
        _notificationActive = (PlayerPrefs.GetInt("NotificationsActive") == 1) ? true : false;
        _notificationToggleString.text = (_notificationActive) ? "Notifications: ON" : "Notifications: OFF"; //update Text accordingly

        if (PlayerPrefs.GetString("LastDrink") != "") { _lastDrinkTime = System.DateTime.Parse(PlayerPrefs.GetString("LastDrink")); }
        Debug.Log(System.DateTime.Parse(PlayerPrefs.GetString("LastDrink")));
    }
}
